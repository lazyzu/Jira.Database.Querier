namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class priority
{
    public string ID { get; set; }

    public decimal? SEQUENCE { get; set; }

    public string pname { get; set; }

    public string DESCRIPTION { get; set; }

    public string ICONURL { get; set; }

    public string STATUS_COLOR { get; set; }
}
