namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issuesecurityscheme
{
    public decimal ID { get; set; }

    public string NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public decimal? DEFAULTLEVEL { get; set; }
}
