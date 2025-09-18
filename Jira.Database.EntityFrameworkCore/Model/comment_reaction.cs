using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class comment_reaction
{
    public decimal ID { get; set; }

    public decimal? comment_id { get; set; }

    public string AUTHOR { get; set; }

    public string EMOTICON { get; set; }

    public DateTime? created_date { get; set; }
}
