using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class userassociation
{
    public string SOURCE_NAME { get; set; }

    public decimal SINK_NODE_ID { get; set; }

    public string SINK_NODE_ENTITY { get; set; }

    public string ASSOCIATION_TYPE { get; set; }

    public decimal? SEQUENCE { get; set; }

    public DateTime? CREATED { get; set; }
}
