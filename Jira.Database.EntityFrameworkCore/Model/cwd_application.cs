using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_application
{
    public decimal ID { get; set; }

    public string application_name { get; set; }

    public string lower_application_name { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public decimal? active { get; set; }

    public string description { get; set; }

    public string application_type { get; set; }

    public string credential { get; set; }
}
