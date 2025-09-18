namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class project
{
    public decimal ID { get; set; }

    public string pname { get; set; }

    public string URL { get; set; }

    public string LEAD { get; set; }

    public string DESCRIPTION { get; set; }

    public string pkey { get; set; }

    public decimal? pcounter { get; set; }

    public decimal? ASSIGNEETYPE { get; set; }

    public decimal? AVATAR { get; set; }

    public string ORIGINALKEY { get; set; }

    public string PROJECTTYPE { get; set; }
}
