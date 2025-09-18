namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class userbase
{
    public decimal ID { get; set; }

    public string username { get; set; }

    public string PASSWORD_HASH { get; set; }
}
