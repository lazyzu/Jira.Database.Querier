using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class customfieldvalue
{
    public decimal ID { get; set; }

    public decimal? ISSUE { get; set; }

    public decimal? CUSTOMFIELD { get; set; }

    public string PARENTKEY { get; set; }

    public string STRINGVALUE { get; set; }

    public decimal? NUMBERVALUE { get; set; }

    public string TEXTVALUE { get; set; }

    public DateTime? DATEVALUE { get; set; }

    public string VALUETYPE { get; set; }

    public decimal? UPDATED { get; set; }
}
