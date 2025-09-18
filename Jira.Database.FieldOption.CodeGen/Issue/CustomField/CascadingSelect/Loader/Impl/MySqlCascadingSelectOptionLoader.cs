using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Select.Loader.Impl;
using lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Issue.CustomField.CascadingSelect.Loader.Impl
{
    internal class MySqlCascadingSelectOptionLoader : ICascadingSelectOptionLoader
    {
        private readonly string connectionString;

        public MySqlCascadingSelectOptionLoader(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<ImmutableDictionary<decimal, SelectOption[]>> LoadFieldOptions(decimal fieldId, CancellationToken cancellationToken)
        {
            using (var dbConnection = new MySqlConnector.MySqlConnection(connectionString))
            {
                var options = await MySqlSelectOptionLoader.LoadFieldOptions(fieldId, dbConnection, cancellationToken);
                return LoadCascadingOption(options);
            }
        }


        private ImmutableDictionary<decimal, SelectOption[]> LoadCascadingOption(ImmutableArray<SelectOption> optionIds)
        {
            if (optionIds.Any())
            {
                var queryContext = LoadParentOption(optionIds);

                while (queryContext.ParentChildMap.Any())
                {
                    queryContext = LoadParentOption(queryContext, optionIds);
                }

                return queryContext.ResultCache
                    .ToImmutableDictionary(x => x.Key
                                , x => x.Value.Reverse<SelectOption>().ToArray());
            }
            else return ImmutableDictionary<decimal, SelectOption[]>.Empty;
        }

        private class QueryContext
        {
            public Dictionary<decimal, List<SelectOption>> ResultCache { get; init; }
            public Dictionary<decimal, decimal[]> ParentChildMap { get; init; }
        }

        private QueryContext LoadParentOption(ImmutableArray<SelectOption> options)
        {
            return new QueryContext
            {
                ResultCache = options.ToDictionary(option => option.Id
                                                 , option => new List<SelectOption>
                                                 {
                                                     option
                                                 }),
                ParentChildMap = options.Where(option => option.ParentId.HasValue)
                                        .GroupBy(option => option.ParentId.Value)
                                        .ToDictionary(parentIdGroup => parentIdGroup.Key
                                                    , parentIdGroup => parentIdGroup.Select(option => option.Id).ToArray())
            };
        }

        private QueryContext LoadParentOption(QueryContext context, ImmutableArray<SelectOption> options)
        {
            if (context.ParentChildMap.Any())
            {
                var parentOptionIds = context.ParentChildMap.Keys;

                var parentOptions = options.Where(option => parentOptionIds.Contains(option.Id));

                if (parentOptions.Any())
                {
                    foreach (var parentOption in parentOptions)
                    {
                        if (context.ParentChildMap.TryGetValue(parentOption.Id, out var childIds))
                        {
                            foreach (var childId in childIds)
                            {
                                if (context.ResultCache.TryGetValue(childId, out var cache)) cache.Add(parentOption);
                            }
                        }
                    }

                    return new QueryContext
                    {
                        ResultCache = context.ResultCache,
                        ParentChildMap = parentOptions.Where(parentOption => parentOption.ParentId.HasValue)
                        .GroupBy(parentOption => parentOption.ParentId.Value)
                        .ToDictionary(parentIdGroup => parentIdGroup.Key
                                    , parentIdGroup =>
                                    {
                                        return parentIdGroup.SelectMany(parentOption =>
                                        {
                                            if (context.ParentChildMap.TryGetValue(parentOption.Id, out var childIds)) return childIds;
                                            else return new decimal[0];
                                        }).Distinct()
                                          .ToArray();
                                    })
                    };
                }
                else return new QueryContext
                {
                    ResultCache = context.ResultCache,
                    ParentChildMap = new Dictionary<decimal, decimal[]>()
                };
            }
            else return new QueryContext
            {
                ResultCache = context.ResultCache,
                ParentChildMap = new Dictionary<decimal, decimal[]>()
            };
        }
    }
}
