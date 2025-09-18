using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.UserField
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var userCustomfieldDeclarationProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: LimitUserCustomFieldDeclaration,
                transform: LoadUserCustomFieldDeclaration)
            .SelectMany((userCustomfieldDeclaration, cancellationToken) => userCustomfieldDeclaration)
            .Collect();

            UserFieldSourceCodeGenerator.GenerateUserFieldCode(context, userCustomfieldDeclarationProvider);
        }

        private static bool LimitUserCustomFieldDeclaration(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
            {
                return UserFieldGeneratorUtility.IsValidUserCustomFieldDeclare(fieldDeclarationSyntax);
            }

            return false;
        }

        private static IEnumerable<UserCustomfieldDeclaration> LoadUserCustomFieldDeclaration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var fieldDeclarationSyntax = (FieldDeclarationSyntax)context.Node;
            return UserFieldGeneratorUtility.LoadUserFieldDeclaration(fieldDeclarationSyntax, context, checkValidSelectCustomFieldDeclare: false);
        }
    }
}
