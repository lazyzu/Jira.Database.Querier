namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_application_address
{
    public decimal application_id { get; set; }

    public string remote_address { get; set; }

    public string encoded_address_binary { get; set; }

    public decimal? remote_address_mask { get; set; }
}
