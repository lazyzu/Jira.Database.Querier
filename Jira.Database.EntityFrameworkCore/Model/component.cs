namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class component
{
    public decimal ID { get; set; }

    public decimal? PROJECT { get; set; }

    public string cname { get; set; }

    public string description { get; set; }

    public string URL { get; set; }

    public string LEAD { get; set; }

    public decimal? ASSIGNEETYPE { get; set; }

    public string ARCHIVED { get; set; }

    public string DELETED { get; set; }
}
