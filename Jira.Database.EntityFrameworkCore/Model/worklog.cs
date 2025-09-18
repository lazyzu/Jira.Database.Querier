using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class worklog
{
    public decimal ID { get; set; }

    public decimal? issueid { get; set; }

    public string AUTHOR { get; set; }

    public string grouplevel { get; set; }

    public decimal? rolelevel { get; set; }

    public string worklogbody { get; set; }

    public DateTime? CREATED { get; set; }

    public string UPDATEAUTHOR { get; set; }

    public DateTime? UPDATED { get; set; }

    public DateTime? STARTDATE { get; set; }

    public decimal? timeworked { get; set; }
}
