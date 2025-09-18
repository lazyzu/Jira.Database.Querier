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
    internal class IssueTypeOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var options = LoadIssueTypeOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently

            if (options != null && options.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameBuilder = new VariableNameBuilder();

                codeBuilder.AppendLine($"using lazyzu.Jira.Database.Querier.Issue.Fields;");
                codeBuilder.AppendLine($@"namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine(@"    public static class IssueTypeSelection");
                codeBuilder.AppendLine(@"    {");
                foreach (var option in options)
                {
                    var optionName = variableNameBuilder.BuildVariableName(option.Name, option.Id);

                    codeBuilder.AppendLine($@"        public static IIssueType {optionName} = new IssueType");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            Id = ""{option.Id}"",");
                    codeBuilder.AppendLine($@"            Name = ""{option.Name}"",");
                    codeBuilder.AppendLine($@"            Description = ""{option.Description}"",");
                    codeBuilder.AppendLine($@"            IsSubTask = {option.IsSubTask.ToString().ToLower()},");
                    codeBuilder.AppendLine($@"            pstyle = ""{option.pstyle}""");
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }
                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.Unicode);
                sourceProductionContext.AddSource($"SimpleFieldSelection.IssueTypeSelection.g.cs", sourceText);
            }
        }

        private async Task<ImmutableArray<IssueType>> LoadIssueTypeOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                            , pname
                            , DESCRIPTION 
                            , pstyle
                            from issuetype";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueType()
                {
                    Id = option.ID,
                    Name = option.pname,
                    Description = option.DESCRIPTION,
                    pstyle = option.pstyle,
                }).ToImmutableArray();
            }
        }

        internal class IssueType
        {
            public string Id { get; init; }

            public string Name { get; init; }

            public string Description { get; init; }

            public bool IsSubTask => "jira_subtask".Equals(pstyle);

            public string pstyle { get; init; }
        }
    }
}
