namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class versioncontrol
{
    public decimal ID { get; set; }

    public string vcsname { get; set; }

    public string vcsdescription { get; set; }

    public string vcstype { get; set; }
}
