namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class customfieldoption
{
    public decimal ID { get; set; }

    public decimal? CUSTOMFIELD { get; set; }

    public decimal? CUSTOMFIELDCONFIG { get; set; }

    public decimal? PARENTOPTIONID { get; set; }

    public decimal? SEQUENCE { get; set; }

    public string customvalue { get; set; }

    public string optiontype { get; set; }

    public string disabled { get; set; }
}
