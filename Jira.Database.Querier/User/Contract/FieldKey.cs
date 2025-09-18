namespace lazyzu.Jira.Database.Querier.User.Contract
{
    public record FieldKey(string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }

    public interface IUserFieldKeyCollection
    {
        FieldKey[] Fields { get; }
    }
}
