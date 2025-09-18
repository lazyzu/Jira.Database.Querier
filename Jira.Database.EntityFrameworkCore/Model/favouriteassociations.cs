namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class favouriteassociations
{
    public decimal ID { get; set; }

    public string USERNAME { get; set; }

    public string entitytype { get; set; }

    public decimal? entityid { get; set; }

    public decimal? SEQUENCE { get; set; }
}
