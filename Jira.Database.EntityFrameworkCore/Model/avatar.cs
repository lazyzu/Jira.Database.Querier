namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class avatar
{
    public decimal ID { get; set; }

    public string filename { get; set; }

    public string contenttype { get; set; }

    public string avatartype { get; set; }

    public string owner { get; set; }

    public decimal? systemavatar { get; set; }
}
