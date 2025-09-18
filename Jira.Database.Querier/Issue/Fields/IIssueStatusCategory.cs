using Generator.Equals;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueStatusCategory
    {
        decimal Id { get; }
        string Name { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueStatusCategory : IIssueStatusCategory
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public string Name { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name}";
        }
    }
}
