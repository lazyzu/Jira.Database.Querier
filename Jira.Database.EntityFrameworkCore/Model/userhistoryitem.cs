namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class userhistoryitem
{
    public decimal ID { get; set; }

    public string entitytype { get; set; }

    public string entityid { get; set; }

    public string USERNAME { get; set; }

    public decimal? lastviewed { get; set; }

    public string data { get; set; }
}
