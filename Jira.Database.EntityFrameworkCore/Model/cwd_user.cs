using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_user
{
    public decimal ID { get; set; }

    public decimal? directory_id { get; set; }

    public string user_name { get; set; }

    public string lower_user_name { get; set; }

    public decimal? active { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public string first_name { get; set; }

    public string lower_first_name { get; set; }

    public string last_name { get; set; }

    public string lower_last_name { get; set; }

    public string display_name { get; set; }

    public string lower_display_name { get; set; }

    public string email_address { get; set; }

    public string lower_email_address { get; set; }

    public string CREDENTIAL { get; set; }

    public decimal? deleted_externally { get; set; }

    public string EXTERNAL_ID { get; set; }
}
