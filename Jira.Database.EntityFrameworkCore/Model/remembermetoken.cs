using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class remembermetoken
{
    public decimal ID { get; set; }

    public DateTime? CREATED { get; set; }

    public string TOKEN { get; set; }

    public string USERNAME { get; set; }
}
