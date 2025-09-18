using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_group
{
    public decimal ID { get; set; }

    public string group_name { get; set; }

    public string lower_group_name { get; set; }

    public decimal? active { get; set; }

    public decimal? local { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public string description { get; set; }

    public string lower_description { get; set; }

    public string group_type { get; set; }

    public decimal? directory_id { get; set; }

    public string external_id { get; set; }
}
