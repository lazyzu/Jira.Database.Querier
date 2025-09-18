namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class draftworkflowschemeentity
{
    public decimal ID { get; set; }

    public decimal? SCHEME { get; set; }

    public string WORKFLOW { get; set; }

    public string issuetype { get; set; }
}
