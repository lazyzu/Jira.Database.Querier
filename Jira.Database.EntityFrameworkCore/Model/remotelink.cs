namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class remotelink
{
    public decimal ID { get; set; }

    public decimal? ISSUEID { get; set; }

    public string GLOBALID { get; set; }

    public string TITLE { get; set; }

    public string SUMMARY { get; set; }

    public string URL { get; set; }

    public string ICONURL { get; set; }

    public string ICONTITLE { get; set; }

    public string RELATIONSHIP { get; set; }

    public string RESOLVED { get; set; }

    public string STATUSNAME { get; set; }

    public string STATUSDESCRIPTION { get; set; }

    public string STATUSICONURL { get; set; }

    public string STATUSICONTITLE { get; set; }

    public string STATUSICONLINK { get; set; }

    public string STATUSCATEGORYKEY { get; set; }

    public string STATUSCATEGORYCOLORNAME { get; set; }

    public string APPLICATIONTYPE { get; set; }

    public string APPLICATIONNAME { get; set; }
}
