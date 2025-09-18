namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class reindex_component
{
    public decimal ID { get; set; }

    public decimal? REQUEST_ID { get; set; }

    public string AFFECTED_INDEX { get; set; }

    public string ENTITY_TYPE { get; set; }
}
