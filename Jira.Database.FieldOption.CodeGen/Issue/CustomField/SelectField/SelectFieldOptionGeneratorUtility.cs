using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select
{
    internal static class SelectFieldOptionGeneratorUtility
    {
        internal static IEnumerable<SelectionCustomfieldDeclaration> LoadSelectionOptionDeclaration(FieldDeclarationSyntax fieldDeclaration, GeneratorSyntaxContext context, bool checkValidSelectCustomFieldDeclare)
        {
            var descendantNodes = fieldDeclaration.DescendantNodes().ToImmutableArray();

            if (checkValidSelectCustomFieldDeclare
            && IsValidSelectCustomFieldDeclare(fieldDeclaration) == false) return [];

            var isArgumentLoaed = TryLoadFieldInfo(descendantNodes, out var fieldName, out var fieldId);
            if (isArgumentLoaed == false) return [];

            return fieldDeclaration.Declaration.Variables.Select(variable =>
            {
                var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable);

                return new SelectionCustomfieldDeclaration(DeclarationName: fieldSymbol.Name
                    , Namespace: fieldSymbol.ContainingNamespace.ToDisplayString()
                    , fieldName
                    , fieldId);
            });
        }

        internal static bool IsValidSelectCustomFieldDeclare(FieldDeclarationSyntax fieldDeclaration)
        {
            var descendantNodes = fieldDeclaration.DescendantNodes().ToImmutableArray();
            return IsValidSelectCustomFieldDeclare(fieldDeclaration, descendantNodes);
        }

        internal static bool IsValidSelectCustomFieldDeclare(FieldDeclarationSyntax fieldDeclaration, ImmutableArray<SyntaxNode> descendantNodes)
        {
            bool isPublicDeclare = IsPublicDeclare(fieldDeclaration);
            if (isPublicDeclare == false) return false;
            bool isStaticDeclare = IsStaticDeclare(fieldDeclaration);
            if (isStaticDeclare == false) return false;

            bool isSelectCustomField = IsSelectCustomField(descendantNodes);
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

        internal static bool IsSelectCustomField(ImmutableArray<SyntaxNode> descendantNodes)
        {
            var typeArgumentListSyntax = descendantNodes.FirstOrDefault(node => node is TypeArgumentListSyntax) as TypeArgumentListSyntax;

            var isSelectCustomField = typeArgumentListSyntax?.Arguments.Any(typeArg =>
            {
                if (typeArg is IdentifierNameSyntax typeArgIdentifierNameSyntax)
                {
                    if ("SelectCustomFieldSchema".Equals(typeArgIdentifierNameSyntax.Identifier.Text)) return true;
                    if ("MultiSelectCustomFieldSchema".Equals(typeArgIdentifierNameSyntax.Identifier.Text)) return true;
                    return false;
                }
                else return false;
            }) ?? false;
            return isSelectCustomField;
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

    internal record SelectionCustomfieldDeclaration(string DeclarationName, string Namespace, string FieldName, decimal FieldId);
}
