namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fieldconfigschemeissuetype
{
    public decimal ID { get; set; }

    public string ISSUETYPE { get; set; }

    public decimal? FIELDCONFIGSCHEME { get; set; }

    public decimal? FIELDCONFIGURATION { get; set; }
}
