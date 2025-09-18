namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class serviceconfig
{
    public decimal ID { get; set; }

    public decimal? delaytime { get; set; }

    public string CLAZZ { get; set; }

    public string servicename { get; set; }

    public string CRON_EXPRESSION { get; set; }
}
