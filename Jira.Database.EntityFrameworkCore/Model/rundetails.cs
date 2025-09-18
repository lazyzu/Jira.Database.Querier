using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class rundetails
{
    public decimal ID { get; set; }

    public string JOB_ID { get; set; }

    public DateTime? START_TIME { get; set; }

    public decimal? RUN_DURATION { get; set; }

    public string RUN_OUTCOME { get; set; }

    public string INFO_MESSAGE { get; set; }
}
