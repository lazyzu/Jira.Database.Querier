namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class upgradetaskhistory
{
    public decimal ID { get; set; }

    public string UPGRADE_TASK_FACTORY_KEY { get; set; }

    public decimal? BUILD_NUMBER { get; set; }

    public string STATUS { get; set; }

    public string UPGRADE_TYPE { get; set; }
}
