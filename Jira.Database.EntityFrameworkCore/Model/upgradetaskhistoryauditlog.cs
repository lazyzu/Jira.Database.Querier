using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class upgradetaskhistoryauditlog
{
    public decimal ID { get; set; }

    public string UPGRADE_TASK_FACTORY_KEY { get; set; }

    public decimal? BUILD_NUMBER { get; set; }

    public string STATUS { get; set; }

    public string UPGRADE_TYPE { get; set; }

    public DateTime? TIMEPERFORMED { get; set; }

    public string ACTION { get; set; }
}
