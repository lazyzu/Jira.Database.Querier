namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_membership
{
    public decimal ID { get; set; }

    public decimal? parent_id { get; set; }

    public decimal? child_id { get; set; }

    public string membership_type { get; set; }

    public string group_type { get; set; }

    public string parent_name { get; set; }

    public string lower_parent_name { get; set; }

    public string child_name { get; set; }

    public string lower_child_name { get; set; }

    public decimal? directory_id { get; set; }
}
