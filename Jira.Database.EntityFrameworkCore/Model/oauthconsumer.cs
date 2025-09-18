using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class oauthconsumer
{
    public decimal ID { get; set; }

    public DateTime? CREATED { get; set; }

    public string consumername { get; set; }

    public string CONSUMER_KEY { get; set; }

    public string consumerservice { get; set; }

    public string PUBLIC_KEY { get; set; }

    public string PRIVATE_KEY { get; set; }

    public string DESCRIPTION { get; set; }

    public string CALLBACK { get; set; }

    public string SIGNATURE_METHOD { get; set; }

    public string SHARED_SECRET { get; set; }
}
