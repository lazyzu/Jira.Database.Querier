using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class audit_log
{
    public decimal ID { get; set; }

    public string REMOTE_ADDRESS { get; set; }

    public DateTime? CREATED { get; set; }

    public string AUTHOR_KEY { get; set; }

    public string SUMMARY { get; set; }

    public string CATEGORY { get; set; }

    public string OBJECT_TYPE { get; set; }

    public string OBJECT_ID { get; set; }

    public string OBJECT_NAME { get; set; }

    public string OBJECT_PARENT_ID { get; set; }

    public string OBJECT_PARENT_NAME { get; set; }

    public decimal? AUTHOR_TYPE { get; set; }

    public string EVENT_SOURCE_NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public string LONG_DESCRIPTION { get; set; }

    public string SEARCH_FIELD { get; set; }
}
