using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text.Json;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config
{
    internal class DatabaseConfig
    {
        public string Type { get; init; }
        public string ConnectionString { get; init; }

        public static DatabaseConfig LoadFrom(string sourceJsonText)
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

        public static DatabaseConfig LoadFrom(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("Database", out var databaseElement))
            {
                var type = string.Empty;
                if (databaseElement.TryGetProperty("Type", out var typeElement)) type = typeElement.GetString();
                else return null;

                var connectionString = string.Empty;
                if (databaseElement.TryGetProperty("ConnectionString", out var connectionStringElement)) connectionString = connectionStringElement.GetString();
                else return null;

                return new DatabaseConfig
                {
                    Type = type,
                    ConnectionString = connectionString
                };
            }
            else return null;
        }

        public static IncrementalValueProvider<ImmutableArray<DatabaseConfig>> LoadProviderFrom(IncrementalValuesProvider<AdditionalText> additionalTextsProvider)
            => additionalTextsProvider.LoadConfigProvider((sourceJsonText, cancellationToken) => LoadFrom(sourceJsonText));
    }
}
