using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class pluginversion
{
    public decimal ID { get; set; }

    public string pluginname { get; set; }

    public string pluginkey { get; set; }

    public string pluginversion1 { get; set; }

    public DateTime? CREATED { get; set; }
}
