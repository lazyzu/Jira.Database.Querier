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
    internal class IssueResolutionOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var options = LoadIssueResolutionOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently

            if (options != null && options.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameBuilder = new VariableNameBuilder();

                codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Fields;");
                codeBuilder.AppendLine($@"namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine($@"    public static class IssueResolutionSelection");
                codeBuilder.AppendLine(@"    {");
                foreach (var option in options)
                {
                    var optionName = variableNameBuilder.BuildVariableName(option.Name, option.Id);

                    codeBuilder.AppendLine($@"        public static IIssueResolution {optionName} = new IssueResolution");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            Id = ""{option.Id}"",");
                    codeBuilder.AppendLine($@"            Name = ""{option.Name}"",");
                    codeBuilder.AppendLine($@"            Description = ""{option.Description}"",");
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }
                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.Unicode);

                sourceProductionContext.AddSource($"SimpleFieldSelection.IssueResolutionSelection.g.cs", sourceText);
            }
        }

        private async Task<ImmutableArray<IssueResolution>> LoadIssueResolutionOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                            , pname
                            , DESCRIPTION 
                            from resolution";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueResolution()
                {
                    Id = option.ID,
                    Name = option.pname,
                    Description = option.DESCRIPTION,
                }).ToImmutableArray();
            }
        }

        internal class IssueResolution
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public string Description { get; init; }
        }
    }
}
