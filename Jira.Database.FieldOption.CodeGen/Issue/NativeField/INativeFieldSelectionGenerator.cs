using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.NativeField
{
    internal interface INativeFieldSelectionGenerator
    {
        void Generate(SourceProductionContext sourceProductionContext, NativeFieldSelectionConfig simpleFieldConfig, DatabaseConfig databaseConfig);
    }
}
