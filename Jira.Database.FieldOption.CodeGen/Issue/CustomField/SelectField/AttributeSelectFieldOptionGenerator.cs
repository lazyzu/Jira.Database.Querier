using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select
{
    //[Generator]
    public class AttributeSelectFieldOptionGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilation = context.CompilationProvider;

            GenerateAttributeCode(context);

            var selectionCustomfieldDeclarationProvider = context.SyntaxProvider.CreateSyntaxProvider(
                  predicate: LimitSelectionCustomFieldGenerationAttribute
                , transform: LoadSelectionCustomFieldDeclarationFromAttribute)
            .SelectMany((selectionOption, _) => selectionOption)
            .Collect();

            var databaseConfigProvider = DatabaseConfig.LoadProviderFrom(context.AdditionalTextsProvider);

            SelectFieldOptionSourceCodeGenerator.GenerateSelectionFieldOptionCode(context, selectionCustomfieldDeclarationProvider, databaseConfigProvider);

            // SelectFieldOptionGeneratorUtility.GenerateSelectionFieldOptionCode(context, selectionCustomfieldDeclarationProvider);
        }

        private static IEnumerable<SelectionCustomfieldDeclaration> LoadSelectionCustomFieldDeclarationFromAttribute(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;

            if (attributeSyntax.Parent?.Parent is FieldDeclarationSyntax fieldDeclaration)
            {
                return SelectFieldOptionGeneratorUtility.LoadSelectionOptionDeclaration(fieldDeclaration, context, checkValidSelectCustomFieldDeclare: true);
            }

            return [];
        }

        private static bool LimitSelectionCustomFieldGenerationAttribute(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute)
                return false;

            var name = ExtractAttributeName(attribute.Name);
            return name is "SelectionOptionGeneration" or "SelectionOptionGenerationAttribute";
        }

        private static void GenerateAttributeCode(IncrementalGeneratorInitializationContext context)
        {
            // https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md#outputting-values
            // https://github.com/dotnet/roslyn/issues/76584
            context.RegisterPostInitializationOutput(postInitializationContext =>
            {
                // postInitializationContext.AddEmbeddedAttributeDefinition();

                var selectionGenerationAttributeSourceCode = @$"
using System;
namespace lazyzu.Jira.Database.FieldOption.SourceGenerator
{{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), Embedded]
    sealed class SelectionOptionGenerationAttribute : Attribute
    {{
        internal sealed SelectionOptionGenerationAttribute()
        {{
        }}
    }}
}}
";
                var sourceText = SourceText.From(selectionGenerationAttributeSourceCode, Encoding.UTF8);

                postInitializationContext.AddSource("SelectionOptionGenerationAttribute.g.cs", sourceText);
            });
        }

        private static string ExtractAttributeName(NameSyntax name)
        {
            return name switch
            {
                SimpleNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };
        }
    }
}
