namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class changeitem
{
    public decimal ID { get; set; }

    public decimal? groupid { get; set; }

    public string FIELDTYPE { get; set; }

    public string FIELD { get; set; }

    public string OLDVALUE { get; set; }

    public string OLDSTRING { get; set; }

    public string NEWVALUE { get; set; }

    public string NEWSTRING { get; set; }
}
