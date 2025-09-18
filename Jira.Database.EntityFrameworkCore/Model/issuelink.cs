namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issuelink
{
    public decimal ID { get; set; }

    public decimal? LINKTYPE { get; set; }

    public decimal? SOURCE { get; set; }

    public decimal? DESTINATION { get; set; }

    public decimal? SEQUENCE { get; set; }
}
