using System;

namespace lazyzu.Jira.Database.Querier.Project.Contract
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

    public interface IProjectFieldKeyCollection
    {
        FieldKey[] Fields { get; }
    }
}
