using lazyzu.Jira.Database.FieldOption.CodeGen.Shared.Config;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.NativeField
{
    [Generator]
    public class IssueFieldSelectionGenerator : IIncrementalGenerator
    {
        private readonly INativeFieldSelectionGenerator[] generators;

        public IssueFieldSelectionGenerator()
        {
            generators = new INativeFieldSelectionGenerator[]
            {
                new Impl.MySQL.IssuePriorityOptionGenerator(),
                new Impl.MySQL.IssueResolutionOptionGenerator(),
                new Impl.MySQL.IssueSecurityLevelOptionGenerator(),
                new Impl.MySQL.IssueStatusOptionGenerator(),
                new Impl.MySQL.IssueStatusCategoryOptionGenerator(),
                new Impl.MySQL.IssueTypeOptionGenerator(),
                new Impl.MySQL.IssueLinkTypeOptionGenerator()
            };
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var nativeFieldConfigProvider = NativeFieldSelectionConfig.LoadProviderFrom(context.AdditionalTextsProvider);
            var databaseConfigProvider = DatabaseConfig.LoadProviderFrom(context.AdditionalTextsProvider);

            var configProvider = nativeFieldConfigProvider.Combine(databaseConfigProvider);

            //Debugger.Launch();

            context.RegisterImplementationSourceOutput(configProvider, (sourceProductionContext, configs) =>
            {
                var nativeFieldConfigs = configs.Left;
                if (nativeFieldConfigs.IsDefaultOrEmpty)
                    return;

                var simpleFieldConfig = nativeFieldConfigs.FirstOrDefault();
                if (simpleFieldConfig == null) return;

                var databaseConfigs = configs.Right;
                if (databaseConfigs.IsDefaultOrEmpty)
                    return;

                var databaseConfig = databaseConfigs.FirstOrDefault();
                if (databaseConfig == null) return;

                if (generators != null)
                {
                    foreach (var generator in generators)
                    {
                        generator.Generate(sourceProductionContext, simpleFieldConfig, databaseConfig);
                    }
                }
            });
        }
    }
}
