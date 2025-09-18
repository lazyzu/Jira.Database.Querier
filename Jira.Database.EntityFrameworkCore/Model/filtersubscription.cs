using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class filtersubscription
{
    public decimal ID { get; set; }

    public decimal? FILTER_I_D { get; set; }

    public string USERNAME { get; set; }

    public string groupname { get; set; }

    public DateTime? LAST_RUN { get; set; }

    public string EMAIL_ON_EMPTY { get; set; }
}
