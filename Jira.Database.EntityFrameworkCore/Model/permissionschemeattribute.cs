namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class permissionschemeattribute
{
    public decimal ID { get; set; }

    public decimal? SCHEME { get; set; }

    public string ATTRIBUTE_KEY { get; set; }

    public string ATTRIBUTE_VALUE { get; set; }
}
