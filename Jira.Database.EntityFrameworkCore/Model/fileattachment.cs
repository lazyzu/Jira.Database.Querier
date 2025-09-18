using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class fileattachment
{
    public decimal ID { get; set; }

    public decimal? issueid { get; set; }

    public string MIMETYPE { get; set; }

    public string FILENAME { get; set; }

    public DateTime? CREATED { get; set; }

    public decimal? FILESIZE { get; set; }

    public string AUTHOR { get; set; }

    public decimal? zip { get; set; }

    public decimal? thumbnailable { get; set; }
}
