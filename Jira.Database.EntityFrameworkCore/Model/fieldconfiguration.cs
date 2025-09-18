namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fieldconfiguration
{
    public decimal ID { get; set; }

    public string configname { get; set; }

    public string DESCRIPTION { get; set; }

    public string FIELDID { get; set; }

    public decimal? CUSTOMFIELD { get; set; }
}
