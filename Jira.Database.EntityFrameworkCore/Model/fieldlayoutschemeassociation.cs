namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fieldlayoutschemeassociation
{
    public decimal ID { get; set; }

    public string ISSUETYPE { get; set; }

    public decimal? PROJECT { get; set; }

    public decimal? FIELDLAYOUTSCHEME { get; set; }
}
