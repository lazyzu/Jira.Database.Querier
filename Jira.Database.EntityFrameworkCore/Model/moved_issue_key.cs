namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class moved_issue_key
{
    public decimal ID { get; set; }

    public string OLD_ISSUE_KEY { get; set; }

    public decimal? ISSUE_ID { get; set; }
}
