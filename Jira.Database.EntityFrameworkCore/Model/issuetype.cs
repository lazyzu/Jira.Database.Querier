namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issuetype
{
    public string ID { get; set; }

    public decimal? SEQUENCE { get; set; }

    public string pname { get; set; }

    public string pstyle { get; set; }

    public string DESCRIPTION { get; set; }

    public string ICONURL { get; set; }

    public decimal? AVATAR { get; set; }
}
