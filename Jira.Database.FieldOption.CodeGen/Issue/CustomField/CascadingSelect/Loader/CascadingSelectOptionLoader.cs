using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect.Loader.Impl;
using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect.Loader
{
    internal interface ICascadingSelectOptionLoader
    {
        Task<ImmutableDictionary<decimal, SelectOption[]>> LoadFieldOptions(decimal fieldId, CancellationToken cancellationToken);
    }

    internal static class CascadingSelectOptionLoader
    {
        public static ICascadingSelectOptionLoader LoadFrom(DatabaseConfig databaseConfig)
        {
            if (databaseConfig == null) throw new ArgumentException("Not found database config");
            if (string.IsNullOrEmpty(databaseConfig.Type)) throw new ArgumentException("Empty type value of database config");
            if (string.IsNullOrEmpty(databaseConfig.ConnectionString)) throw new ArgumentException("Empty connectionString value of database config");

            if ("mysql".Equals(databaseConfig.Type, StringComparison.OrdinalIgnoreCase)) return new MySqlCascadingSelectOptionLoader(databaseConfig.ConnectionString);

            throw new NotSupportedException($"Not supported database type: {databaseConfig.Type}");
        }
    }
}
