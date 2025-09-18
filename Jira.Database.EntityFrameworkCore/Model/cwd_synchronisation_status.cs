namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class cwd_synchronisation_status
{
    public decimal id { get; set; }

    public decimal? directory_id { get; set; }

    public string node_id { get; set; }

    public decimal? sync_start { get; set; }

    public decimal? sync_end { get; set; }

    public string sync_status { get; set; }

    public string status_parameters { get; set; }
}
