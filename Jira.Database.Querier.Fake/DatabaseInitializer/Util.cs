using System.Collections.Generic;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Fake.DatabaseInitializer
{
    internal static class Util
    {
        public static async Task AddMultiple<T>(IEnumerable<T> entities, EntityFrameworkCore.JiraContext jiraContext, AddSingleDelegate<T> addSingle, bool saveChange = true)
        {
            foreach (var entity in entities)
            {
                await addSingle(entity, jiraContext, saveChange: false);
            }

            if (saveChange) await jiraContext.SaveChangesAsync();
        }

        public delegate Task AddSingleDelegate<T>(T entity, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true);
    }
}
