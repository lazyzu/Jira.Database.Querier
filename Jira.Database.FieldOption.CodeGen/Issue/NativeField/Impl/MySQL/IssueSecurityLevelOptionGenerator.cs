using Dapper;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.NativeField.Impl.MySQL
{
    internal class IssueSecurityLevelOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var issueSecurityLevelSchemaOptions = LoadIssueSecurityLevelSchemaOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently
            var issueSecurityLevelOptions = LoadIssueSecurityLevelOption_FromMySql(databaseConfig.ConnectionString, sourceProductionContext.CancellationToken).Result;  // TODO: support MYSQL Only currently

            if (issueSecurityLevelOptions != null && issueSecurityLevelOptions.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameMap = LoadVariableNameMap(issueSecurityLevelSchemaOptions, issueSecurityLevelOptions);

                codeBuilder.AppendLine(@"using System.Collections.Immutable;");
                codeBuilder.AppendLine(@"using lazyzu.Jira.Database.Querier.Issue.Fields;");
                codeBuilder.AppendLine($@"namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine(@"    public static class IssueSecurityLevelSelection");
                codeBuilder.AppendLine(@"    {");

                foreach (var issueSecurityLevelSchemaOption in issueSecurityLevelSchemaOptions)
                {
                    var matchedIssueSecurityLevelOptions = issueSecurityLevelOptions.Where(issueSecurityLevelOption => issueSecurityLevelOption.Scheme == issueSecurityLevelSchemaOption.Id).ToArray();
                    var defaultIssueSecurityLevelOption = matchedIssueSecurityLevelOptions.FirstOrDefault(option => option.Id == issueSecurityLevelSchemaOption.DefaultValue);

                    var schemeVariableName = variableNameMap[issueSecurityLevelSchemaOption];
                    var defaultIssueSecurityLevelOptionVariableName = defaultIssueSecurityLevelOption == null ? "null" : variableNameMap[defaultIssueSecurityLevelOption];

                    codeBuilder.AppendLine($@"        public static class {schemeVariableName}");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            public static IIssueSecurityLevelScheme Scheme = new IssueSecurityLevelScheme()");
                    codeBuilder.AppendLine(@"            {");
                    codeBuilder.AppendLine($@"                Id = {issueSecurityLevelSchemaOption.Id},");
                    codeBuilder.AppendLine($@"                Name = ""{issueSecurityLevelSchemaOption.Name}"",");
                    codeBuilder.AppendLine($@"                Description = ""{issueSecurityLevelSchemaOption.Description}"",");
                    codeBuilder.AppendLine($@"                DefaultValue = {defaultIssueSecurityLevelOptionVariableName}");
                    codeBuilder.AppendLine(@"            };");
                    codeBuilder.AppendLine($@"");

                    foreach (var issueSecurityLevelOption in matchedIssueSecurityLevelOptions)
                    {
                        var issueSecurityLevelOptionVariableName = RemoveScheme(variableNameMap[issueSecurityLevelOption]);

                        codeBuilder.AppendLine($@"            public static IIssueSecurityLevel {issueSecurityLevelOptionVariableName} = new IssueSecurityLevel");
                        codeBuilder.AppendLine(@"            {");
                        codeBuilder.AppendLine($@"                Id = {issueSecurityLevelOption.Id},");
                        codeBuilder.AppendLine($@"                Name = ""{issueSecurityLevelOption.Name}"",");
                        codeBuilder.AppendLine($@"                Description = ""{issueSecurityLevelOption.Description}"",");
                        codeBuilder.AppendLine($@"                Scheme = {schemeVariableName}.Scheme");
                        codeBuilder.AppendLine(@"            };");
                        codeBuilder.AppendLine($@"");
                    }

                    codeBuilder.AppendLine(@"        }");
                    codeBuilder.AppendLine($@"");
                }

                var variableNameBuilder = new VariableNameBuilder();
                var index = 1;
                foreach (var issueSecurityLevelOptionNameGrouped in issueSecurityLevelOptions.GroupBy(issueSecurityLevelOption => issueSecurityLevelOption.Name))
                {
                    var name = issueSecurityLevelOptionNameGrouped.Key;
                    var variableName = variableNameBuilder.BuildVariableName(name, index++);

                    codeBuilder.AppendLine($@"        public static ImmutableArray<IIssueSecurityLevel> {variableName} = new ImmutableArray<IIssueSecurityLevel>");
                    codeBuilder.AppendLine(@"        {");

                    foreach (var issueSecurityLevelOption in issueSecurityLevelOptionNameGrouped)
                    {
                        if (variableNameMap.TryGetValue(issueSecurityLevelOption, out var issueSecurityLevelVariableName))
                        {
                            codeBuilder.AppendLine($@"            {issueSecurityLevelVariableName},");
                        }
                        else
                        {
                            codeBuilder.AppendLine($@"            new IssueSecurityLevel");
                            codeBuilder.AppendLine(@"            {");
                            codeBuilder.AppendLine($@"                Id = {issueSecurityLevelOption.Id},");
                            codeBuilder.AppendLine($@"                Name = ""{issueSecurityLevelOption.Name}"",");
                            codeBuilder.AppendLine($@"                Description = ""{issueSecurityLevelOption.Description}"",");
                            codeBuilder.AppendLine($@"                Scheme = null");
                            codeBuilder.AppendLine(@"            },");
                        }
                    }
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }

                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.Unicode);
                sourceProductionContext.AddSource($"SimpleFieldSelection.IssueSecurityLevelSelection.g.cs", sourceText);
            }
        }

        private Dictionary<object, string> LoadVariableNameMap(ImmutableArray<IssueSecurityLevelScheme> issueSecurityLevelSchemes, ImmutableArray<IssueSecurityLevel> issueSecurityLevels)
        {
            var result = new Dictionary<object, string>();

            var schemeVariableNameBuilder = new VariableNameBuilder();
            foreach (var issueSecurityLevelScheme in issueSecurityLevelSchemes)
            {
                result.Add(issueSecurityLevelScheme, $"Scheme{schemeVariableNameBuilder.BuildVariableName(issueSecurityLevelScheme.Name, issueSecurityLevelScheme.Id)}");
            }

            var schemeGroupedIssueSecurityLevel =
                from issueSecurityLevel in issueSecurityLevels
                from issueSecurityLevelScheme in issueSecurityLevelSchemes.Where(_issueSecurityLevelScheme => issueSecurityLevel.Scheme == _issueSecurityLevelScheme.Id)
                group issueSecurityLevel by issueSecurityLevelScheme into schemeGrouped
                select schemeGrouped;

            foreach (var issueSecurityLevelGroup in schemeGroupedIssueSecurityLevel)
            {
                var securityLevelVariableNameBuilder = new VariableNameBuilder();

                var scheme = issueSecurityLevelGroup.Key;
                if (scheme != null && result.TryGetValue(scheme, out var schemeVariableName))
                {
                    var securityLevels = issueSecurityLevelGroup;
                    foreach (var securityLevel in securityLevels)
                    {
                        result.Add(securityLevel, $"{schemeVariableName}.{securityLevelVariableNameBuilder.BuildVariableName(securityLevel.Name, securityLevel.Id)}");
                    }
                }
            }

            return result;
        }

        private string RemoveScheme(string variableName)
        {
            var realVariableNameStartIndex = variableName.IndexOf('.') + 1;
            return variableName.Substring(realVariableNameStartIndex);
        }


        private async Task<ImmutableArray<IssueSecurityLevelScheme>> LoadIssueSecurityLevelSchemaOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                            , NAME 
                            , DESCRIPTION
                            , DEFAULTLEVEL 
                            from issuesecurityscheme;";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueSecurityLevelScheme()
                {
                    Id = option.ID,
                    Name = option.NAME,
                    Description = option.DESCRIPTION,
                    DefaultValue = option.DEFAULTLEVEL
                }).ToImmutableArray();
            }
        }

        private async Task<ImmutableArray<IssueSecurityLevel>> LoadIssueSecurityLevelOption_FromMySql(string connectionString, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var sql = @"select ID
                            , NAME
                            , DESCRIPTION 
                            , SCHEME 
                            from schemeissuesecuritylevels";

                var queryResult = await dbConnection.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

                return queryResult.Select(option => new IssueSecurityLevel()
                {
                    Id = option.ID,
                    Name = option.NAME,
                    Description = option.DESCRIPTION,
                    Scheme = option.SCHEME
                }).ToImmutableArray();
            }
        }

        internal class IssueSecurityLevelScheme
        {
            public decimal Id { get; init; }

            public string Name { get; init; }

            public string Description { get; init; }

            public decimal? DefaultValue { get; init; }
        }

        internal class IssueSecurityLevel
        {
            public decimal Id { get; init; }

            public string Name { get; init; }

            public string Description { get; init; }

            public decimal? Scheme { get; init; }
        }
    }
}
