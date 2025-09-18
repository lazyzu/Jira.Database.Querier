using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select.Loader;
using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select
{
    internal static class SelectFieldOptionSourceCodeGenerator
    {
        public static void GenerateSelectionFieldOptionCode(IncrementalGeneratorInitializationContext context
            , IncrementalValueProvider<ImmutableArray<SelectionCustomfieldDeclaration>> selectionCustomfieldDeclarationProvider
            , IncrementalValueProvider<ImmutableArray<DatabaseConfig>> databaseConfigProvider)
        {
            // https://github.com/dotnet/roslyn/issues/57963
            // RegisterImplementationSourceOutput works in the same way as
            // RegisterSourceOutput but declares that the source produced has no semantic
            // impact on user code from the point of view of code analysis.This allows a host
            // such as the IDE, to chose not to run these outputs as a performance
            // optimization.A host that produces executable code will always run these
            // outputs.

            var selectionCustomfieldDeclarationProviderWithConfig = selectionCustomfieldDeclarationProvider.Combine(databaseConfigProvider);

            //Debugger.Launch();

            context.RegisterImplementationSourceOutput(selectionCustomfieldDeclarationProviderWithConfig, (context, selectionCustomfieldDeclarationWithConfig) =>
            {
                var selectionCustomfieldDeclarations = selectionCustomfieldDeclarationWithConfig.Left;
                if (selectionCustomfieldDeclarations.IsDefaultOrEmpty)
                    return;

                var databaseConfig = selectionCustomfieldDeclarationWithConfig.Right.FirstOrDefault();
                var selectOptionLoader = SelectOptionLoader.LoadFrom(databaseConfig);

                foreach (var selectionCustomfieldDeclaration in selectionCustomfieldDeclarations)
                {
                    var options = selectOptionLoader.LoadFieldOptions(selectionCustomfieldDeclaration.FieldId, context.CancellationToken).Result;

                    var sourceText = BuildCode(selectionCustomfieldDeclaration, options);
                    context.AddSource($"CustomFieldSelection.{selectionCustomfieldDeclaration.DeclarationName}.g.cs", sourceText);
                }
            });
        }

        public static SourceText BuildCode(SelectionCustomfieldDeclaration customFieldDeclaration, ImmutableArray<SelectOption> options)
        {
            if (options != null && options.Any())
            {
                var codeBuilder = new StringBuilder();
                var variableNameBuilder = new VariableNameBuilder();

                codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;");
                codeBuilder.AppendLine($@"namespace {customFieldDeclaration.Namespace}.CustomFieldSelection");
                codeBuilder.AppendLine(@"{");
                codeBuilder.AppendLine($@"    public static class {customFieldDeclaration.DeclarationName}");
                codeBuilder.AppendLine(@"    {");

                foreach (var option in options)
                {
                    var optionName = variableNameBuilder.BuildVariableName(option.Value, option.Id);

                    codeBuilder.AppendLine($@"        public static ISelectOption {optionName} = new SelectOption");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine($@"            Id = {option.Id},");
                    codeBuilder.AppendLine($@"            Value = ""{option.Value}"",");
                    codeBuilder.AppendLine($@"            Disabled = {option.Disabled.ToString().ToLower()},");
                    codeBuilder.AppendLine(@"        };");
                    codeBuilder.AppendLine($@"");
                }

                codeBuilder.AppendLine(@"    }");
                codeBuilder.AppendLine(@"}");

                return SourceText.From(codeBuilder.ToString(), Encoding.Unicode);
            }
            else return null;
        }
    }
}
