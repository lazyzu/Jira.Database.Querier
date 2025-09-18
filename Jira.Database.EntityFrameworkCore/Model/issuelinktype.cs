namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issuelinktype
{
    public decimal ID { get; set; }

    public string LINKNAME { get; set; }

    public string INWARD { get; set; }

    public string OUTWARD { get; set; }

    public string pstyle { get; set; }
}
