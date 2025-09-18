using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Avatar;
using lazyzu.Jira.Database.Querier.User.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    public interface IUserAvatar
    {
        decimal Id { get; }
        IAvatarUrl Urls { get; }
    }

    [Equatable(Explicit = true)]
    public partial class UserAvatar : IUserAvatar
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public IAvatarUrl Urls { get; init; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public interface IUserAvatarUrlBuilder
    {
        IAvatarUrl BuildFrom(decimal avatarId);
    }

    public class UserAvatarUrlBuilder : IUserAvatarUrlBuilder
    {
        protected readonly Uri baseUrl;
        protected readonly Uri projectavatarUrl;

        public UserAvatarUrlBuilder(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
            if (Uri.TryCreate(baseUrl, "secure/useravatar", out projectavatarUrl) == false)
            {
                throw new ArgumentException($"Not able to build projectavatar url from {baseUrl}");
            }
        }

        public virtual IAvatarUrl BuildFrom(decimal avatarId)
        {
            var projectavatarUrlStr = projectavatarUrl.AbsoluteUri;
            var baseQuery = ToQueryString(new Dictionary<string, string>
            {
                { "avatarId", avatarId.ToString() }
            });

            return new AvatarUrl
            {
                XSmall = $"{projectavatarUrlStr}?size=xsmall&{baseQuery}",
                Small = $"{projectavatarUrlStr}?size=small&{baseQuery}",
                Medium = $"{projectavatarUrlStr}?size=medium&{baseQuery}",
                Large = $"{projectavatarUrlStr}?{baseQuery}"
            };
        }

        protected virtual string ToQueryString(Dictionary<string, string> queryCollection)
        {
            var queryParts = queryCollection.Select(query =>
            {
                var queryKey = HttpUtility.UrlEncode(query.Key);
                var queryValue = HttpUtility.UrlEncode(query.Value);
                return $"{queryKey}={queryValue}";
            });

            return string.Join("&", queryParts);
        }
    }

    public class UserAvatarProjection : IUserProjectionWithContextSpecification<app_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<app_user, object>>[] IncludeExpressions { get; protected set; }

        protected readonly JiraContext jiraContext;
        protected readonly IUserAvatarUrlBuilder userAvatarUrlBuilder;
        protected readonly ILogger logger;

        public UserAvatarProjection(JiraContext jiraContext, IUserAvatarUrlBuilder userAvatarUrlBuilder, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.userAvatarUrlBuilder = userAvatarUrlBuilder;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserAvatar
            };

            IncludeExpressions = new Expression<Func<app_user, object>>[]
            {
                appUser => appUser.ID
            };
        }

        public virtual async Task<object> PrepareContext(IEnumerable<app_user> enties, CancellationToken cancellationToken = default)
        {
            var _users = enties?.ToArray() ?? new app_user[0];

            var result = new Dictionary<decimal, decimal?>();
            if (_users.Any())
            {
                var userIds = _users.Select<app_user, decimal?>(user => user.ID).ToArray();

                var query = from propertyentry in jiraContext.propertyentry.AsNoTracking()
                            from propertynumber in jiraContext.propertynumber.AsNoTracking().Where(_propertynumber => _propertynumber.ID == propertyentry.ID)
                            where userIds.Contains(propertyentry.ENTITY_ID)
                               && propertyentry.ENTITY_NAME == "ApplicationUser"
                               && propertyentry.PROPERTY_KEY == "user.avatar.id"
                            select new
                            {
                                propertyentry.ENTITY_ID,
                                propertynumber.propertyvalue
                            };

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                result = queryResult.ToDictionary(dbModel => dbModel.ENTITY_ID.Value, dbModel => dbModel.propertyvalue);
            }
            return result;
        }

        public virtual Task Projection(app_user entity, JiraUser projection, object context, CancellationToken cancellationToken = default)
        {
            if (context is IDictionary<decimal, decimal?> userAvatarIdMap)
            {
                if (userAvatarIdMap.TryGetValue(entity.ID, out var userAvatarId))
                {
                    if (userAvatarId.HasValue)
                    {
                        var avatarId = userAvatarId.Value;

                        projection.Avatar = new UserAvatar
                        {
                            Id = avatarId,
                            Urls = userAvatarUrlBuilder.BuildFrom(avatarId)
                        };
                    }
                }
                return Task.CompletedTask;
            }
            else throw new NotSupportedException();
        }
    }
}
