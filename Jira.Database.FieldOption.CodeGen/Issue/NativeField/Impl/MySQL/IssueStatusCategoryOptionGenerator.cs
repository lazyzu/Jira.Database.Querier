using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.NativeField.Impl.MySQL
{
    internal class IssueStatusCategoryOptionGenerator : INativeFieldSelectionGenerator
    {
        public void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig)
        {
            var sourceCode = $@"
using lazyzu.Jira.Database.Querier.Issue.Fields;
namespace {simpleFieldConfig.IssueFieldSelectionOutputNamespace}
{{
    public static class IssueStatusCategorySelection
    {{
        public static IIssueStatusCategory NoCategory = new IssueStatusCategory
        {{
            Id = 1,
            Name = ""No Category""
        }};

        public static IIssueStatusCategory ToDo = new IssueStatusCategory
        {{
            Id = 2,
            Name = ""To Do""
        }};

        public static IIssueStatusCategory InProgress = new IssueStatusCategory
        {{
            Id = 4,
            Name = ""In Progress""
        }};

        public static IIssueStatusCategory Done = new IssueStatusCategory
        {{
            Id = 3,
            Name = ""Done""
        }};
    }}
}}";

            var sourceText = SourceText.From(sourceCode, Encoding.UTF8);

            sourceProductionContext.AddSource($"SimpleFieldSelection.IssueStatusCategorySelection.g.cs", sourceText);
        }
    }
}
