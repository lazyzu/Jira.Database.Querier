namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class jiraperms
{
    public decimal ID { get; set; }

    public decimal? permtype { get; set; }

    public decimal? projectid { get; set; }

    public string groupname { get; set; }
}
