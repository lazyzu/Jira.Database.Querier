namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class audit_item
{
    public decimal ID { get; set; }

    public decimal? LOG_ID { get; set; }

    public string OBJECT_TYPE { get; set; }

    public string OBJECT_ID { get; set; }

    public string OBJECT_NAME { get; set; }

    public string OBJECT_PARENT_ID { get; set; }

    public string OBJECT_PARENT_NAME { get; set; }
}
