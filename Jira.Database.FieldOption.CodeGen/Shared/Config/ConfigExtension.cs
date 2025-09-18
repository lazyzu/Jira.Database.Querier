using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Threading;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config
{
    internal static class ConfigExtension
    {
        public const string SourceFileName = "SourceGeneration.json";

        public static IncrementalValuesProvider<AdditionalText> LimitSourceFileName(this IncrementalValuesProvider<AdditionalText> additionalTextsProvider, string sourceFileName = SourceFileName)
        {
            return additionalTextsProvider
                .Where(additionalText =>
                {
                    return additionalText.Path.EndsWith(sourceFileName, StringComparison.OrdinalIgnoreCase);
                });
        }

        public static IncrementalValueProvider<ImmutableArray<TConfig>> LoadConfigProvider<TConfig>(this IncrementalValuesProvider<AdditionalText> additionalTextsProvider
            , Func<string, CancellationToken, TConfig> configLoader)
        {
            return additionalTextsProvider
                .LimitSourceFileName()
                .Select((additionalText, cancellationToken) =>
                {
                    var sourceJsonText = additionalText.GetText(cancellationToken).ToString();
                    return configLoader(sourceJsonText, cancellationToken);
                })
                .Where(databaseConfig => databaseConfig != null)
                .Collect();
        }
    }
}
