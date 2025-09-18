namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class nomenclature_entries
{
    public decimal ID { get; set; }

    public string ORIGINAL_NAME { get; set; }

    public string NEW_NAME { get; set; }

    public string NEW_NAME_PLURAL { get; set; }

    public decimal? TIMESTAMP { get; set; }
}
