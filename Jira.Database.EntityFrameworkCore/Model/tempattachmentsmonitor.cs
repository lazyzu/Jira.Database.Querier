namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class tempattachmentsmonitor
{
    public string TEMPORARY_ATTACHMENT_ID { get; set; }

    public string FORM_TOKEN { get; set; }

    public string FILE_NAME { get; set; }

    public string CONTENT_TYPE { get; set; }

    public decimal? FILE_SIZE { get; set; }

    public decimal? CREATED_TIME { get; set; }
}
