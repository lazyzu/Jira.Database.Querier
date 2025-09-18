using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class jiraaction
{
    public decimal ID { get; set; }

    public decimal? issueid { get; set; }

    public string AUTHOR { get; set; }

    public string actiontype { get; set; }

    public string actionlevel { get; set; }

    public decimal? rolelevel { get; set; }

    public string actionbody { get; set; }

    public DateTime? CREATED { get; set; }

    public string UPDATEAUTHOR { get; set; }

    public DateTime? UPDATED { get; set; }

    public decimal? actionnum { get; set; }
}
