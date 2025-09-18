namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class portletconfiguration
{
    public decimal ID { get; set; }

    public decimal? PORTALPAGE { get; set; }

    public string PORTLET_ID { get; set; }

    public decimal? COLUMN_NUMBER { get; set; }

    public decimal? positionseq { get; set; }

    public string GADGET_XML { get; set; }

    public string COLOR { get; set; }

    public string DASHBOARD_MODULE_COMPLETE_KEY { get; set; }
}
