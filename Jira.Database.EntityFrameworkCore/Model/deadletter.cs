namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class deadletter
{
    public decimal ID { get; set; }

    public string MESSAGE_ID { get; set; }

    public decimal? LAST_SEEN { get; set; }

    public decimal? MAIL_SERVER_ID { get; set; }

    public string FOLDER_NAME { get; set; }
}
