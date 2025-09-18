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
    internal class IssueStatusOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var options = LoadlIssueStatusOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently

            if (options != null && options.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameBuilder = new VariableNameBuilder();

                codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Fields;");
                codeBuilder.AppendLine($@"namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine($@"    public static class IssueStatusSelection");
                codeBuilder.AppendLine(@"    {");
                foreach (var option in options)
                {
                    var optionName = variableNameBuilder.BuildVariableName(option.Name, option.Id);
                    var optionCategoryCodeIndicator = LoadCategorySelectionCodeIndicator(option.Category);

                    codeBuilder.AppendLine($@"        public static IIssueStatus {optionName} = new IssueStatus");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            Id = ""{option.Id}"",");
                    codeBuilder.AppendLine($@"            Name = ""{option.Name}"",");
                    codeBuilder.AppendLine($@"            Description = ""{option.Description}"",");
                    codeBuilder.AppendLine($@"            Category = {optionCategoryCodeIndicator},");
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }

                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.Unicode);

                sourceProductionContext.AddSource($"SimpleFieldSelection.IssueStatusSelection.g.cs", sourceText);
            }
        }

        private string LoadCategorySelectionCodeIndicator(decimal category)
        {
            switch (category)
            {
                case 1:
                    return "IssueStatusCategorySelection.NoCategory";
                case 2:
                    return "IssueStatusCategorySelection.NoCategory";
                case 4:
                    return "IssueStatusCategorySelection.NoCategory";
                case 3:
                    return "IssueStatusCategorySelection.NoCategory";
                default:
                    return "null";

            }
        }

        private async Task<ImmutableArray<IssueStatus>> LoadlIssueStatusOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                        , pname
                        , DESCRIPTION
                        , STATUSCATEGORY 
                        from issuestatus";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueStatus()
                {
                    Id = option.ID,
                    Name = option.pname,
                    Description = option.DESCRIPTION,
                    Category = option.STATUSCATEGORY
                }).ToImmutableArray();
            }
        }

        internal class IssueStatus
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }
            public decimal Category { get; init; }
        }
    }
}
