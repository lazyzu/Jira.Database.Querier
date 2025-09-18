using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class oauthspconsumer
{
    public decimal ID { get; set; }

    public DateTime? CREATED { get; set; }

    public string CONSUMER_KEY { get; set; }

    public string consumername { get; set; }

    public string PUBLIC_KEY { get; set; }

    public string DESCRIPTION { get; set; }

    public string CALLBACK { get; set; }

    public string TWO_L_O_ALLOWED { get; set; }

    public string EXECUTING_TWO_L_O_USER { get; set; }

    public string TWO_L_O_IMPERSONATION_ALLOWED { get; set; }

    public string THREE_L_O_ALLOWED { get; set; }
}
