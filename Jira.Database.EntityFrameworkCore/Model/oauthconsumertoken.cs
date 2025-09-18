using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class oauthconsumertoken
{
    public decimal ID { get; set; }

    public DateTime? CREATED { get; set; }

    public string TOKEN_KEY { get; set; }

    public string TOKEN { get; set; }

    public string TOKEN_SECRET { get; set; }

    public string TOKEN_TYPE { get; set; }

    public string CONSUMER_KEY { get; set; }
}
