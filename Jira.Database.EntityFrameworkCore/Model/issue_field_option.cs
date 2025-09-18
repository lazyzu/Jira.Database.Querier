namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class issue_field_option
{
    public decimal ID { get; set; }

    public decimal? OPTION_ID { get; set; }

    public string FIELD_KEY { get; set; }

    public string option_value { get; set; }

    public string PROPERTIES { get; set; }
}
