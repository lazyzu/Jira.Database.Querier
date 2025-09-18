using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class projectchangedtime
{
    public decimal PROJECT_ID { get; set; }

    public DateTime? ISSUE_CHANGED_TIME { get; set; }
}
