using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class projectversion
{
    public decimal ID { get; set; }

    public decimal? PROJECT { get; set; }

    public string vname { get; set; }

    public string DESCRIPTION { get; set; }

    public decimal? SEQUENCE { get; set; }

    public string RELEASED { get; set; }

    public string ARCHIVED { get; set; }

    public string URL { get; set; }

    public DateTime? STARTDATE { get; set; }

    public DateTime? RELEASEDATE { get; set; }
}
