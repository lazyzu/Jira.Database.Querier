namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class jiraeventtype
{
    public decimal ID { get; set; }

    public decimal? TEMPLATE_ID { get; set; }

    public string NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public string event_type { get; set; }
}
