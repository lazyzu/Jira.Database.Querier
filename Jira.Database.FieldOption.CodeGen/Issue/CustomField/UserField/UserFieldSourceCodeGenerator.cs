using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.UserField
{
    internal class UserFieldSourceCodeGenerator
    {
        public static void GenerateUserFieldCode(IncrementalGeneratorInitializationContext context
            , IncrementalValueProvider<ImmutableArray<UserCustomfieldDeclaration>> userCustomfieldDeclarationProvider)
        {
            context.RegisterImplementationSourceOutput(userCustomfieldDeclarationProvider, (context, userCustomfieldDeclarations) =>
            {
                foreach (var userCustomfieldDeclaration in userCustomfieldDeclarations)
                {
                    var sourceText = BuildCode(userCustomfieldDeclaration);
                    context.AddSource($"CustomFieldSelection.{userCustomfieldDeclaration.DeclarationName}.g.cs", sourceText);
                }
            });
        }

        public static SourceText BuildCode(UserCustomfieldDeclaration customFieldDeclaration)
        {
            var codeBuilder = new StringBuilder();

            codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Contract;");
            codeBuilder.AppendLine($@"using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;");
            codeBuilder.AppendLine($@"namespace {customFieldDeclaration.Namespace}");
            codeBuilder.AppendLine( @"{");
            codeBuilder.AppendLine($@"    public partial class {customFieldDeclaration.ClassName}");
            codeBuilder.AppendLine( @"    {");
            codeBuilder.AppendLine($@"        public static CustomFieldKey<UserCustomFieldSchema> {customFieldDeclaration.DeclarationName}_WithField(lazyzu.Jira.Database.Querier.User.Contract.FieldKey[] userFields)");
            codeBuilder.AppendLine( @"        {");
            codeBuilder.AppendLine($@"            return new UserCustomFieldKey<{customFieldDeclaration.SchemaName}>(""{customFieldDeclaration.FieldName}"", {customFieldDeclaration.FieldId})");
            codeBuilder.AppendLine( @"            {");
            codeBuilder.AppendLine( @"                Fields = userFields");
            codeBuilder.AppendLine( @"            };");
            codeBuilder.AppendLine( @"        }");
            codeBuilder.AppendLine( @"    }");
            codeBuilder.AppendLine( @"}");

            return SourceText.From(codeBuilder.ToString(), Encoding.Unicode);
        }
    }
}
