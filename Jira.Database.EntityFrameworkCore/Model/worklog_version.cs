using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class worklog_version
{
    public decimal WORKLOG_ID { get; set; }

    public decimal? PARENT_ISSUE_ID { get; set; }

    public DateTime? UPDATE_TIME { get; set; }

    public decimal? INDEX_VERSION { get; set; }

    public string DELETED { get; set; }
}
