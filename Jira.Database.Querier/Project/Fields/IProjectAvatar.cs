using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Avatar;
using lazyzu.Jira.Database.Querier.Project.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public interface IProjectAvatar
    {
        decimal Id { get; }
        IAvatarUrl Urls { get; }
    }

    [Equatable(Explicit = true)]
    public partial class ProjectAvatar : IProjectAvatar
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public IAvatarUrl Urls { get; init; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public interface IProjectAvatarUrlBuilder
    {
        IAvatarUrl BuildFrom(decimal projectId, decimal avatarId);
    }

    public class ProjectAvatarUrlBuilder : IProjectAvatarUrlBuilder
    {
        protected readonly Uri baseUrl;
        protected readonly Uri projectavatarUrl;

        public ProjectAvatarUrlBuilder(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
            if (Uri.TryCreate(baseUrl, "secure/projectavatar", out projectavatarUrl) == false)
            {
                throw new ArgumentException($"Not able to build projectavatar url from {baseUrl}");
            }
        }

        public virtual IAvatarUrl BuildFrom(decimal projectId, decimal avatarId)
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

        protected string ToQueryString(Dictionary<string, string> queryCollection)
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

    public class ProjectAvatarProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; protected init; }

        protected readonly IProjectAvatarUrlBuilder avatarUrlBuilder;

        public ProjectAvatarProjection(IProjectAvatarUrlBuilder avatarUrlBuilder)
        {
            this.avatarUrlBuilder = avatarUrlBuilder;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectAvatar
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.AVATAR
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            if (entity.AVATAR.HasValue)
            {
                projection.Avatar = new ProjectAvatar
                {
                    Id = entity.AVATAR.Value,
                    Urls = avatarUrlBuilder.BuildFrom(entity.ID, entity.AVATAR.Value)
                };
            }

            return Task.CompletedTask;
        }
    }
}
