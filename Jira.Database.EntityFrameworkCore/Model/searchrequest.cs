namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class searchrequest
{
    public decimal ID { get; set; }

    public string filtername { get; set; }

    public string authorname { get; set; }

    public string DESCRIPTION { get; set; }

    public string username { get; set; }

    public string groupname { get; set; }

    public decimal? projectid { get; set; }

    public string reqcontent { get; set; }

    public decimal? FAV_COUNT { get; set; }

    public string filtername_lower { get; set; }
}
