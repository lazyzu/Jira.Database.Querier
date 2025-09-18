using Dapper;
using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select.Loader.Impl
{
    internal class MySqlSelectOptionLoader : ISelectOptionLoader
    {
        private readonly string connectionString;

        public MySqlSelectOptionLoader(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<ImmutableArray<SelectOption>> LoadFieldOptions(decimal fieldId, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                return await LoadFieldOptions(fieldId, dbConnection, cancellationToken);
            }
        }

        public static async Task<ImmutableArray<SelectOption>> LoadFieldOptions(decimal fieldId, IDbConnection dbConnection, CancellationToken cancellationToken)
        {
            var sql = @"select ID
                        , customvalue
                        , PARENTOPTIONID 
                        , disabled 
                        from customfieldoption 
                        where CUSTOMFIELD = @fieldId";

            var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, new
            {
                fieldId
            }, cancellationToken: cancellationToken));

            return queryResult.Select(option => new SelectOption()
            {
                Id = option.ID,
                Value = option.customvalue,
                ParentId = option.PARENTOPTIONID,
                Disabled = ToDisableBoolean(option)
            }).ToImmutableArray();
        }

        public static async Task<ImmutableArray<SelectOption>> LoadOptions(IEnumerable<decimal> optionIds, IDbConnection dbConnection, CancellationToken cancellationToken)
        {
            var _optionIds = optionIds?.ToArray() ?? new decimal[0];

            if (_optionIds.Any() == false) return ImmutableArray<SelectOption>.Empty;

            var sql = @"select ID
                        , customvalue
                        , PARENTOPTIONID 
                        , disabled 
                        from customfieldoption 
                        where iD in @optionIds";

            var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, new
            {
                optionIds = _optionIds
            }, cancellationToken: cancellationToken));

            return queryResult.Select(option => new SelectOption()
            {
                Id = option.ID,
                Value = option.customvalue,
                ParentId = option.PARENTOPTIONID,
                Disabled = ToDisableBoolean(option)
            }).ToImmutableArray();
        }

        private static bool ToDisableBoolean(dynamic option)
        {
            return "Y".Equals(option.disabled);
        }
    }
}
