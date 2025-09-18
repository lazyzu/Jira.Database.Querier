using Dapper;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.NativeField.Impl.MySQL
{
    internal class IssueLinkTypeOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var options = LoadIssueLinkTypeOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently

            if (options != null && options.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameBuilder = new VariableNameBuilder();

                codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Fields;");
                codeBuilder.AppendLine($@"namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine($@"    public static class IssueLinkTypeSelection");
                codeBuilder.AppendLine(@"    {");

                foreach (var option in options)
                {
                    var optionName = variableNameBuilder.BuildVariableName(option.Name, option.Id);

                    codeBuilder.AppendLine($@"        public static IIssueLinkType {optionName} = new IssueLinkType");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            Id = {option.Id},");
                    codeBuilder.AppendLine($@"            Name = ""{option.Name}"",");
                    codeBuilder.AppendLine($@"            Inward = ""{option.Inward}"",");
                    codeBuilder.AppendLine($@"            Outward = ""{option.Outward}"",");
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }

                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.Unicode);

                sourceProductionContext.AddSource($"SimpleFieldSelection.IssueLinkTypeSelection.g.cs", sourceText);
            }
        }

        private async Task<ImmutableArray<IssueLinkType>> LoadIssueLinkTypeOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                            , LINKNAME
                            , INWARD 
                            , OUTWARD
                            from issuelinktype";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueLinkType()
                {
                    Id = option.ID,
                    Name = option.LINKNAME,
                    Inward = option.INWARD,
                    Outward = option.OUTWARD
                }).ToImmutableArray();
            }
        }

        internal class IssueLinkType
        {
            public decimal Id { get; init; }

            public string Name { get; init; }

            public string Inward { get; init; }

            public string Outward { get; init; }
        }
    }
}
