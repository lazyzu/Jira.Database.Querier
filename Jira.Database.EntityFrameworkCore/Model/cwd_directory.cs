using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_directory
{
    public decimal ID { get; set; }

    public string directory_name { get; set; }

    public string lower_directory_name { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public decimal? active { get; set; }

    public string description { get; set; }

    public string impl_class { get; set; }

    public string lower_impl_class { get; set; }

    public string directory_type { get; set; }

    public decimal? directory_position { get; set; }
}
