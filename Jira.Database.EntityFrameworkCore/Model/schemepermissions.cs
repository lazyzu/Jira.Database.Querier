namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class schemepermissions
{
    public decimal ID { get; set; }

    public decimal? SCHEME { get; set; }

    public decimal? PERMISSION { get; set; }

    public string perm_type { get; set; }

    public string perm_parameter { get; set; }

    public string PERMISSION_KEY { get; set; }
}
