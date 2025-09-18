namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issuestatus
{
    public string ID { get; set; }

    public decimal? SEQUENCE { get; set; }

    public string pname { get; set; }

    public string DESCRIPTION { get; set; }

    public string ICONURL { get; set; }

    public decimal? STATUSCATEGORY { get; set; }
}
