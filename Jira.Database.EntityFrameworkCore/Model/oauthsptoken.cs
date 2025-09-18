using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class oauthsptoken
{
    public decimal ID { get; set; }

    public DateTime? CREATED { get; set; }

    public string TOKEN { get; set; }

    public string TOKEN_SECRET { get; set; }

    public string TOKEN_TYPE { get; set; }

    public string CONSUMER_KEY { get; set; }

    public string USERNAME { get; set; }

    public decimal? TTL { get; set; }

    public string spauth { get; set; }

    public string CALLBACK { get; set; }

    public string spverifier { get; set; }

    public string spversion { get; set; }

    public string SESSION_HANDLE { get; set; }

    public DateTime? SESSION_CREATION_TIME { get; set; }

    public DateTime? SESSION_LAST_RENEWAL_TIME { get; set; }

    public DateTime? SESSION_TIME_TO_LIVE { get; set; }
}
