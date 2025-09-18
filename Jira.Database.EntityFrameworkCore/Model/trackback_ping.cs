using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class trackback_ping
{
    public decimal ID { get; set; }

    public decimal? ISSUE { get; set; }

    public string URL { get; set; }

    public string TITLE { get; set; }

    public string BLOGNAME { get; set; }

    public string EXCERPT { get; set; }

    public DateTime? CREATED { get; set; }
}
