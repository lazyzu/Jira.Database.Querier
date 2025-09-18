using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueCustomFieldService
    {
        Task<ImmutableArray<ISelectOption>> GetOptionsAsync(CustomFieldKey<SelectCustomFieldSchema> field, CancellationToken cancellationToken = default);
        Task<ImmutableArray<ISelectOption>> GetOptionsAsync(CustomFieldKey<MultiSelectCustomFieldSchema> field, CancellationToken cancellationToken = default);
        Task<ImmutableDictionary<decimal, ICascadingSelection>> GetOptionsAsync(CustomFieldKey<CascadingSelectCustomFieldSchema> field, CancellationToken cancellationToken = default);
    }

    public class IssueCustomFieldService : IIssueCustomFieldService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        protected readonly CascadingSelectCustomFieldProjection cascadingSelectCustomFieldProjection;

        public IssueCustomFieldService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;

            this.cascadingSelectCustomFieldProjection = new CascadingSelectCustomFieldProjection(jiraContext, logger);
        }

        public virtual Task<ImmutableArray<ISelectOption>> GetOptionsAsync(CustomFieldKey<SelectCustomFieldSchema> field, CancellationToken cancellationToken = default)
            => getOptionsAsync(field.Id, cancellationToken);

        public virtual Task<ImmutableArray<ISelectOption>> GetOptionsAsync(CustomFieldKey<MultiSelectCustomFieldSchema> field, CancellationToken cancellationToken = default)
            => getOptionsAsync(field.Id, cancellationToken);

        protected virtual async Task<ImmutableArray<ISelectOption>> getOptionsAsync(decimal fieldId, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldoption.AsNoTracking()
                            .Where(option => option.CUSTOMFIELD == fieldId)
                            .OrderBy(option => option.SEQUENCE)
                            .Select(option => new
                            {
                                option.ID,
                                option.customvalue,
                                option.disabled,
                            });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.Select(dbModel => new SelectOption
            {
                Id = dbModel.ID,
                Value = dbModel.customvalue,
                Disabled = SelectCustomFieldExtension.IsDisabled(dbModel.disabled)
            } as ISelectOption).ToImmutableArray();
        }

        public virtual async Task<ImmutableDictionary<decimal, ICascadingSelection>> GetOptionsAsync(CustomFieldKey<CascadingSelectCustomFieldSchema> field, CancellationToken cancellationToken = default)
        {
            var optionIdQuery = jiraContext.customfieldoption.AsNoTracking()
                            .Where(option => option.CUSTOMFIELD == field.Id)
                            .OrderBy(option => option.SEQUENCE)
                            .Select(option => option.ID);

            var optionIds = await optionIdQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var cascadingOptions = await cascadingSelectCustomFieldProjection.LoadCascadingOption(optionIds, cancellationToken).ConfigureAwait(false);

            return cascadingOptions.ToImmutableDictionary(option => option.Key, option => new CascadingSelection
            {
                CascadingSelections = option.Value
            } as ICascadingSelection);
        }
    }
}
