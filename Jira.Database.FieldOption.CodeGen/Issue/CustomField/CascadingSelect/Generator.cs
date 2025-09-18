using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var selectionCustomfieldDeclarationProvider = context.SyntaxProvider.CreateSyntaxProvider(
                  predicate: LimitCascadingSelectionCustomFieldDeclaration
                , transform: LoadCascadingSelectionCustomFieldDeclaration)
            .SelectMany((selectionCustomfieldDeclaration, cancellationToken) => selectionCustomfieldDeclaration)
            .Collect();

            var databaseConfigProvider = DatabaseConfig.LoadProviderFrom(context.AdditionalTextsProvider);

            CascadingSelectFieldOptionSourceCodeGenerator.GenerateSelectionFieldOptionCode(context, selectionCustomfieldDeclarationProvider, databaseConfigProvider);
        }

        private static bool LimitCascadingSelectionCustomFieldDeclaration(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
            {
                return CascadingSelectFieldOptionGeneratorUtility.IsValidSelectCustomFieldDeclare(fieldDeclarationSyntax);
            }

            return false;
        }

        private static IEnumerable<CascadingSelectionCustomfieldDeclaration> LoadCascadingSelectionCustomFieldDeclaration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var fieldDeclarationSyntax = (FieldDeclarationSyntax)context.Node;
            return CascadingSelectFieldOptionGeneratorUtility.LoadSelectionOptionDeclaration(fieldDeclarationSyntax, context, checkValidSelectCustomFieldDeclare: false);
        }
    }
}
