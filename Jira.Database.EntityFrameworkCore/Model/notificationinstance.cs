namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class notificationinstance
{
    public decimal ID { get; set; }

    public string notificationtype { get; set; }

    public decimal? SOURCE { get; set; }

    public string emailaddress { get; set; }

    public string MESSAGEID { get; set; }
}
