using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect.Loader;
using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared;
using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect
{
    internal record CascadingSelectionCustomfieldDeclaration(string DeclarationName, string Namespace, string FieldName, decimal FieldId);

    internal class CascadingSelectFieldOptionSourceCodeGenerator
    {
        public static void GenerateSelectionFieldOptionCode(IncrementalGeneratorInitializationContext context
            , IncrementalValueProvider<ImmutableArray<CascadingSelectionCustomfieldDeclaration>> selectionCustomfieldDeclarationProvider
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
                var selectOptionLoader = CascadingSelectOptionLoader.LoadFrom(databaseConfig);

                foreach (var selectionCustomfieldDeclaration in selectionCustomfieldDeclarations)
                {
                    var options = selectOptionLoader.LoadFieldOptions(selectionCustomfieldDeclaration.FieldId, context.CancellationToken).Result;

                    var sourceText = BuildCode(selectionCustomfieldDeclaration, options);
                    context.AddSource($"CustomFieldSelection.{selectionCustomfieldDeclaration.DeclarationName}.g.cs", sourceText);
                }
            });
        }

        public static SourceText BuildCode(CascadingSelectionCustomfieldDeclaration customFieldDeclaration, ImmutableDictionary<decimal, SelectOption[]> options)
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
                    var cascadingOptions = option.Value;
                    var cascadingVariableName = variableNameBuilder.BuildVariableName(cascadingOptions);

                    codeBuilder.AppendLine($@"        public static readonly CascadingSelection {cascadingVariableName} = new CascadingSelection");
                    codeBuilder.AppendLine(@"        {");
                    codeBuilder.AppendLine(@"            CascadingSelections = new ISelectOption[]");
                    codeBuilder.AppendLine(@"            {");
                    foreach (var x in option.Value)
                    {
                        codeBuilder.AppendLine($@"                new SelectOption");
                        codeBuilder.AppendLine(@"                {");
                        codeBuilder.AppendLine($@"                    Id = {x.Id},");
                        codeBuilder.AppendLine($@"                    Value = ""{x.Value}"",");
                        codeBuilder.AppendLine($@"                    Disabled = {x.Disabled.ToString().ToLower()}");
                        codeBuilder.AppendLine(@"                },");
                    }
                    codeBuilder.AppendLine(@"            }");
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
