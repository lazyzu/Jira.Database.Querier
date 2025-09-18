namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fieldlayout
{
    public decimal ID { get; set; }

    public string NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public string layout_type { get; set; }

    public decimal? LAYOUTSCHEME { get; set; }
}
