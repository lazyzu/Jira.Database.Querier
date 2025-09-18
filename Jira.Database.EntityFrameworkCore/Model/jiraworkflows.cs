namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class jiraworkflows
{
    public decimal ID { get; set; }

    public string workflowname { get; set; }

    public string creatorname { get; set; }

    public string DESCRIPTOR { get; set; }

    public string ISLOCKED { get; set; }
}
