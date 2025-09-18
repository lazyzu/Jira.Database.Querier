namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class schemeissuesecurities
{
    public decimal ID { get; set; }

    public decimal? SCHEME { get; set; }

    public decimal? SECURITY { get; set; }

    public string sec_type { get; set; }

    public string sec_parameter { get; set; }
}
