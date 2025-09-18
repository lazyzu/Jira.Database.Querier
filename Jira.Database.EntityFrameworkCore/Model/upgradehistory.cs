namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class upgradehistory
{
    public decimal? ID { get; set; }

    public string UPGRADECLASS { get; set; }

    public string TARGETBUILD { get; set; }

    public string STATUS { get; set; }

    public string DOWNGRADETASKREQUIRED { get; set; }
}
