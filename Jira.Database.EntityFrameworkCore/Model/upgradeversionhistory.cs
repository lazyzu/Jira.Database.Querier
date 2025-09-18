using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class upgradeversionhistory
{
    public decimal? ID { get; set; }

    public DateTime? TIMEPERFORMED { get; set; }

    public string TARGETBUILD { get; set; }

    public string TARGETVERSION { get; set; }
}
