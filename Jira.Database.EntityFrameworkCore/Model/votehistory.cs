using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class votehistory
{
    public decimal ID { get; set; }

    public decimal? issueid { get; set; }

    public decimal? VOTES { get; set; }

    public DateTime? TIMESTAMP { get; set; }
}
