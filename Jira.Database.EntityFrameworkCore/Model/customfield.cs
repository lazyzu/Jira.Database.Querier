using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class customfield
{
    public decimal ID { get; set; }

    public string cfkey { get; set; }

    public string CUSTOMFIELDTYPEKEY { get; set; }

    public string CUSTOMFIELDSEARCHERKEY { get; set; }

    public string cfname { get; set; }

    public string DESCRIPTION { get; set; }

    public string defaultvalue { get; set; }

    public decimal? FIELDTYPE { get; set; }

    public decimal? PROJECT { get; set; }

    public string ISSUETYPE { get; set; }

    public DateTime? lastvalueupdate { get; set; }

    public decimal? issueswithvalue { get; set; }
}
