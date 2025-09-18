namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fieldlayoutitem
{
    public decimal ID { get; set; }

    public decimal? FIELDLAYOUT { get; set; }

    public string FIELDIDENTIFIER { get; set; }

    public string DESCRIPTION { get; set; }

    public decimal? VERTICALPOSITION { get; set; }

    public string ISHIDDEN { get; set; }

    public string ISREQUIRED { get; set; }

    public string RENDERERTYPE { get; set; }
}
