namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class notification
{
    public decimal ID { get; set; }

    public decimal? SCHEME { get; set; }

    public string EVENT { get; set; }

    public decimal? EVENT_TYPE_ID { get; set; }

    public decimal? TEMPLATE_ID { get; set; }

    public string notif_type { get; set; }

    public string notif_parameter { get; set; }
}
