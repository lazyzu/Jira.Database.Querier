using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.UserField
{
    internal class UserFieldGeneratorUtility
    {
        internal static IEnumerable<UserCustomfieldDeclaration> LoadUserFieldDeclaration(FieldDeclarationSyntax fieldDeclaration, GeneratorSyntaxContext context, bool checkValidSelectCustomFieldDeclare)
        {
            var descendantNodes = fieldDeclaration.DescendantNodes().ToImmutableArray();

            if (checkValidSelectCustomFieldDeclare
            && IsValidUserCustomFieldDeclare(fieldDeclaration) == false) return [];

            var isArgumentLoaed = TryLoadFieldInfo(descendantNodes, out var fieldName, out var fieldId);
            if (isArgumentLoaed == false) return [];

            return fieldDeclaration.Declaration.Variables.Select(variable =>
            {
                var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable);

                return new UserCustomfieldDeclaration(Namespace: fieldSymbol.ContainingNamespace.ToDisplayString()
                    , ClassName: fieldSymbol.ContainingType.Name
                    , DeclarationName: fieldSymbol.Name
                    , SchemaName: LoadSchemaName(descendantNodes)
                    , fieldName
                    , fieldId);
            });
        }

        internal static bool IsValidUserCustomFieldDeclare(FieldDeclarationSyntax fieldDeclaration)
        {
            var descendantNodes = fieldDeclaration.DescendantNodes().ToImmutableArray();
            return IsValidUserCustomFieldDeclare(fieldDeclaration, descendantNodes);
        }

        internal static bool IsValidUserCustomFieldDeclare(FieldDeclarationSyntax fieldDeclaration, ImmutableArray<SyntaxNode> descendantNodes)
        {
            bool isPublicDeclare = IsPublicDeclare(fieldDeclaration);
            if (isPublicDeclare == false) return false;
            bool isStaticDeclare = IsStaticDeclare(fieldDeclaration);
            if (isStaticDeclare == false) return false;

            bool isSelectCustomField = IsUserCustomField(descendantNodes);
            if (isSelectCustomField == false) return false;

            return true;
        }

        internal static bool IsPublicDeclare(FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration.Modifiers.Any(modifier => "public".Equals(modifier.ValueText));
        }

        internal static bool IsStaticDeclare(FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration.Modifiers.Any(modifier => "static".Equals(modifier.ValueText));
        }

        internal static bool IsUserCustomField(ImmutableArray<SyntaxNode> descendantNodes)
        {
            var typeArgumentListSyntax = descendantNodes.FirstOrDefault(node => node is TypeArgumentListSyntax) as TypeArgumentListSyntax;

            var isSelectCustomField = typeArgumentListSyntax?.Arguments.Any(typeArg =>
            {
                if (typeArg is IdentifierNameSyntax typeArgIdentifierNameSyntax)
                {
                    if ("UserCustomFieldSchema".Equals(typeArgIdentifierNameSyntax.Identifier.Text)) return true;
                    if ("MultiUserCustomFieldSchema".Equals(typeArgIdentifierNameSyntax.Identifier.Text)) return true;
                    return false;
                }
                else return false;
            }) ?? false;
            return isSelectCustomField;
        }

        internal static string LoadSchemaName(ImmutableArray<SyntaxNode> descendantNodes)
        {
            var typeArgumentListSyntax = descendantNodes.FirstOrDefault(node => node is TypeArgumentListSyntax) as TypeArgumentListSyntax;

            if(typeArgumentListSyntax == null) return null;

            foreach (var typeArg in typeArgumentListSyntax.Arguments)
            {
                if (typeArg is IdentifierNameSyntax typeArgIdentifierNameSyntax) return typeArgIdentifierNameSyntax.Identifier.Text;
            }

            return null;
        }

        internal static bool TryLoadFieldInfo(ImmutableArray<SyntaxNode> declarationSyntaxNodes, out string name, out decimal id)
        {
            name = string.Empty;
            id = -1;

            var argumentListSyntax = declarationSyntaxNodes.FirstOrDefault(node => node is ArgumentListSyntax) as ArgumentListSyntax;
            if (argumentListSyntax == null) return false;

            var nameLoaded = false;
            var idLoaed = false;

            foreach (var argument in argumentListSyntax.Arguments)
            {
                if (argument.Expression is LiteralExpressionSyntax literalExpressionSyntax)
                {
                    if (literalExpressionSyntax.Token.Value is string nameArgumentValue)
                    {
                        name = nameArgumentValue;
                        nameLoaded = true;
                    }
                    else if (literalExpressionSyntax.Token.Value is int idArgumentValue)
                    {
                        id = idArgumentValue;
                        idLoaed = true;
                    }
                }
            }

            return nameLoaded && idLoaed;
        }
    }

    internal record UserCustomfieldDeclaration(string Namespace, string ClassName, string DeclarationName, string SchemaName, string FieldName, decimal FieldId);
}
