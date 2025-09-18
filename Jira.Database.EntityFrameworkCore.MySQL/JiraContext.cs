using Microsoft.EntityFrameworkCore;

namespace lazyzu.Jira.Database.EntityFrameworkCore.MySQL
{
    public class JiraContext : lazyzu.Jira.Database.EntityFrameworkCore.JiraContext
    {
        public JiraContext(string connectionString, string serverVersion = "8.0.32-mysql") : base(JiraContextExtension.UseMySql(connectionString, serverVersion).Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .UseCollation("utf8mb4_bin")
            .HasCharSet("utf8mb4");

            base.OnModelCreating(modelBuilder);
        }
    }

    public static class JiraContextExtension
    {
        internal static DbContextOptionsBuilder<JiraContext> UseMySql(string connectionString, string serverVersion)
        {
            return UseMySql(new DbContextOptionsBuilder<JiraContext>(), connectionString, serverVersion);
        }

        public static DbContextOptionsBuilder<JiraContext> UseMySql(this DbContextOptionsBuilder<JiraContext> builder, string connectionString, string serverVersion = "8.0.32-mysql")
        {
            builder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse(serverVersion));
            return builder;
        }
    }
}
