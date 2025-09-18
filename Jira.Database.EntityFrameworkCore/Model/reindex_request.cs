using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class reindex_request
{
    public decimal ID { get; set; }

    public string TYPE { get; set; }

    public DateTime? REQUEST_TIME { get; set; }

    public DateTime? START_TIME { get; set; }

    public DateTime? COMPLETION_TIME { get; set; }

    public string STATUS { get; set; }

    public string EXECUTION_NODE_ID { get; set; }

    public string QUERY { get; set; }
}
