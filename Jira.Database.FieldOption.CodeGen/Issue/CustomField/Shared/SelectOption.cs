using lazyzu.Jira.Database.FieldOption.CodeGen.Shared;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared
{
    internal class SelectOption : VariableNameBuilder.IOption<decimal>
    {
        public decimal Id { get; init; }
        public string Value { get; init; }
        public bool Disabled { get; init; }
        public decimal? ParentId { get; init; }
    }
}
