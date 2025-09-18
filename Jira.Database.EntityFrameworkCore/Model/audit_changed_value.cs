namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class audit_changed_value
{
    public decimal ID { get; set; }

    public decimal? LOG_ID { get; set; }

    public string NAME { get; set; }

    public string DELTA_FROM { get; set; }

    public string DELTA_TO { get; set; }
}
