namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class propertyentry
{
    public decimal ID { get; set; }

    public string ENTITY_NAME { get; set; }

    public decimal? ENTITY_ID { get; set; }

    public string PROPERTY_KEY { get; set; }

    public decimal? propertytype { get; set; }
}
