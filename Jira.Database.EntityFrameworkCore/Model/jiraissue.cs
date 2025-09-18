using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class jiraissue
{
    public decimal ID { get; set; }

    public string pkey { get; set; }

    public decimal? issuenum { get; set; }

    public decimal? PROJECT { get; set; }

    public string REPORTER { get; set; }

    public string ASSIGNEE { get; set; }

    public string CREATOR { get; set; }

    public string issuetype { get; set; }

    public string SUMMARY { get; set; }

    public string DESCRIPTION { get; set; }

    public string ENVIRONMENT { get; set; }

    public string PRIORITY { get; set; }

    public string RESOLUTION { get; set; }

    public string issuestatus { get; set; }

    public DateTime? CREATED { get; set; }

    public DateTime? UPDATED { get; set; }

    public DateTime? DUEDATE { get; set; }

    public DateTime? RESOLUTIONDATE { get; set; }

    public decimal? VOTES { get; set; }

    public decimal? WATCHES { get; set; }

    public decimal? TIMEORIGINALESTIMATE { get; set; }

    public decimal? TIMEESTIMATE { get; set; }

    public decimal? TIMESPENT { get; set; }

    public decimal? WORKFLOW_ID { get; set; }

    public decimal? SECURITY { get; set; }

    public decimal? FIXFOR { get; set; }

    public decimal? COMPONENT { get; set; }

    public string ARCHIVEDBY { get; set; }

    public DateTime? ARCHIVEDDATE { get; set; }

    public string ARCHIVED { get; set; }
}
