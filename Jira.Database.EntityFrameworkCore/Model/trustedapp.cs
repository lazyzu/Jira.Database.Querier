using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class trustedapp
{
    public decimal ID { get; set; }

    public string APPLICATION_ID { get; set; }

    public string NAME { get; set; }

    public string PUBLIC_KEY { get; set; }

    public string IP_MATCH { get; set; }

    public string URL_MATCH { get; set; }

    public decimal? TIMEOUT { get; set; }

    public DateTime? CREATED { get; set; }

    public string CREATED_BY { get; set; }

    public DateTime? UPDATED { get; set; }

    public string UPDATED_BY { get; set; }
}
