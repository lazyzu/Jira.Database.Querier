using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class draftworkflowscheme
{
    public decimal ID { get; set; }

    public string NAME { get; set; }

    public string DESCRIPTION { get; set; }

    public decimal? WORKFLOW_SCHEME_ID { get; set; }

    public DateTime? LAST_MODIFIED_DATE { get; set; }

    public string LAST_MODIFIED_USER { get; set; }
}
