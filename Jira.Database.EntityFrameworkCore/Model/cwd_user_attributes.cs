namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_user_attributes
{
    public decimal ID { get; set; }

    public decimal? user_id { get; set; }

    public decimal? directory_id { get; set; }

    public string attribute_name { get; set; }

    public string attribute_value { get; set; }

    public string lower_attribute_value { get; set; }
}
