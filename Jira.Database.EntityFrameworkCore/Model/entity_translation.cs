namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class entity_translation
{
    public decimal ID { get; set; }

    public string ENTITY_NAME { get; set; }

    public decimal? ENTITY_ID { get; set; }

    public string LOCALE { get; set; }

    public string TRANS_NAME { get; set; }

    public string TRANS_DESC { get; set; }
}
