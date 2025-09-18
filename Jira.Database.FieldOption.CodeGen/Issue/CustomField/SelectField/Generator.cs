using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var selectionCustomfieldDeclarationProvider = context.SyntaxProvider.CreateSyntaxProvider(
                  predicate: LimitSelectionCustomFieldDeclaration
                , transform: LoadSelectionCustomFieldDeclaration)
            .SelectMany((selectionCustomfieldDeclaration, cancellationToken) => selectionCustomfieldDeclaration)
            .Collect();

            var databaseConfigProvider = DatabaseConfig.LoadProviderFrom(context.AdditionalTextsProvider);

            SelectFieldOptionSourceCodeGenerator.GenerateSelectionFieldOptionCode(context, selectionCustomfieldDeclarationProvider, databaseConfigProvider);
        }

        private static bool LimitSelectionCustomFieldDeclaration(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
            {
                return SelectFieldOptionGeneratorUtility.IsValidSelectCustomFieldDeclare(fieldDeclarationSyntax);
            }

            return false;
        }

        private static IEnumerable<SelectionCustomfieldDeclaration> LoadSelectionCustomFieldDeclaration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var fieldDeclarationSyntax = (FieldDeclarationSyntax)context.Node;
            return SelectFieldOptionGeneratorUtility.LoadSelectionOptionDeclaration(fieldDeclarationSyntax, context, checkValidSelectCustomFieldDeclare: false);
        }
    }
}
