namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class portalpage
{
    public decimal ID { get; set; }

    public string USERNAME { get; set; }

    public string PAGENAME { get; set; }

    public string DESCRIPTION { get; set; }

    public decimal? SEQUENCE { get; set; }

    public decimal? FAV_COUNT { get; set; }

    public string LAYOUT { get; set; }

    public decimal? ppversion { get; set; }
}
