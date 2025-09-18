using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text.Json;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config
{
    internal class NativeFieldSelectionConfig
    {
        public string IssueFieldSelectionOutputNamespace { get; init; }

        public static NativeFieldSelectionConfig LoadFrom(string sourceJsonText)
        {
            if (string.IsNullOrEmpty(sourceJsonText)) return null;
            else
            {
                try
                {
                    var jsonDocument = JsonDocument.Parse(sourceJsonText);
                    return LoadFrom(jsonDocument);
                }
                catch { return null; }
            }
        }

        public static NativeFieldSelectionConfig LoadFrom(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("NativeFieldSelection", out var nativeFieldElement))
            {
                var issueFieldSelectionOutputNamespace = string.Empty;
                if (nativeFieldElement.TryGetProperty("IssueFieldSelectionOutputNamespace", out var issueFieldSelectionOutputNamespaceElement)) issueFieldSelectionOutputNamespace = issueFieldSelectionOutputNamespaceElement.GetString();
                else return null;

                return new NativeFieldSelectionConfig
                {
                    IssueFieldSelectionOutputNamespace = issueFieldSelectionOutputNamespace
                };
            }
            else return null;
        }

        public static IncrementalValueProvider<ImmutableArray<NativeFieldSelectionConfig>> LoadProviderFrom(IncrementalValuesProvider<AdditionalText> additionalTextsProvider)
            => additionalTextsProvider.LoadConfigProvider((sourceJsonText, cancellationToken) => LoadFrom(sourceJsonText));
    }
}
