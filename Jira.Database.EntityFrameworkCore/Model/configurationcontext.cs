namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class configurationcontext
{
    public decimal ID { get; set; }

    public decimal? PROJECTCATEGORY { get; set; }

    public decimal? PROJECT { get; set; }

    public string customfield { get; set; }

    public decimal? FIELDCONFIGSCHEME { get; set; }
}
