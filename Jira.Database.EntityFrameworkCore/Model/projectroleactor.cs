namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class projectroleactor
{
    public decimal ID { get; set; }

    public decimal? PID { get; set; }

    public decimal? PROJECTROLEID { get; set; }

    public string ROLETYPE { get; set; }

    public string ROLETYPEPARAMETER { get; set; }
}
