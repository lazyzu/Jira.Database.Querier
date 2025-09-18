namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issue_field_option_scope
{
    public decimal ID { get; set; }

    public decimal? OPTION_ID { get; set; }

    public string ENTITY_ID { get; set; }

    public string SCOPE_TYPE { get; set; }
}
