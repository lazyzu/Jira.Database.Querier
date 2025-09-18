namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class sharepermissions
{
    public decimal ID { get; set; }

    public decimal? entityid { get; set; }

    public string entitytype { get; set; }

    public string sharetype { get; set; }

    public string PARAM1 { get; set; }

    public string PARAM2 { get; set; }

    public decimal? RIGHTS { get; set; }
}
