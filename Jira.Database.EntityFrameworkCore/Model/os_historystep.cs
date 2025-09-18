using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class os_historystep
{
    public decimal ID { get; set; }

    public decimal? ENTRY_ID { get; set; }

    public decimal? STEP_ID { get; set; }

    public decimal? ACTION_ID { get; set; }

    public string OWNER { get; set; }

    public DateTime? START_DATE { get; set; }

    public DateTime? DUE_DATE { get; set; }

    public DateTime? FINISH_DATE { get; set; }

    public string STATUS { get; set; }

    public string CALLER { get; set; }
}
