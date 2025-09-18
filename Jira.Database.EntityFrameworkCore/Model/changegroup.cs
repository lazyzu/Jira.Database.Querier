using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class changegroup
{
    public decimal ID { get; set; }

    public decimal? issueid { get; set; }

    public string AUTHOR { get; set; }

    public DateTime? CREATED { get; set; }
}
