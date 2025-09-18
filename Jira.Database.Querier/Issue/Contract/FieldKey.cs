using lazyzu.Jira.Database.Querier.User.Contract;
using System;

namespace lazyzu.Jira.Database.Querier.Issue.Contract
{
    public class FieldKey : IEquatable<FieldKey>
    {
        public string Name { get; init; }

        public FieldKey() { }

        public FieldKey(string name)
        {
            Name = name?.ToString();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(FieldKey other)
        {
            return this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }

    public interface ICustomFieldKey
    {
        string Name { get; }
        decimal Id { get; }
        Type ProjectionType { get; }
    }

    public class CustomFieldKey<TProjectionType> : FieldKey, ICustomFieldKey, IEquatable<ICustomFieldKey>
    {
        public decimal Id { get; init; }
        public Type ProjectionType => typeof(TProjectionType);

        public CustomFieldKey() { }

        public CustomFieldKey(string name, decimal id)
        {
            this.Name = name;
            this.Id = id;
        }

        public override string ToString()
        {
            return $"{Id}:{Name}";
        }

        public bool Equals(ICustomFieldKey other)
        {
            var result = true;
            result &= this.Name.Equals(other.Name);
            result &= this.Id.Equals(other.Id);
            result &= this.ProjectionType.Equals(other.ProjectionType);
            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id, ProjectionType);
        }
    }

    public class UserCustomFieldKey<TProjectionType> : CustomFieldKey<TProjectionType>, IUserFieldKeyCollection
    {
        public UserCustomFieldKey() { }

        public UserCustomFieldKey(string name, decimal id) 
        {
            this.Name = name;
            this.Id = id;
        }

        public User.Contract.FieldKey[] Fields { get; init; }
    }

    public interface IQuerySpecificationSchema<TPredicateFieldType> { }

    // Field Default value:
    // select * from genericconfiguration g 
}
