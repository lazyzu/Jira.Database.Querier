using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class replicatedindexoperation
{
    public decimal ID { get; set; }

    public DateTime? INDEX_TIME { get; set; }

    public string NODE_ID { get; set; }

    public string AFFECTED_INDEX { get; set; }

    public string ENTITY_TYPE { get; set; }

    public string AFFECTED_IDS { get; set; }

    public string OPERATION { get; set; }

    public string FILENAME { get; set; }

    public string VERSIONS { get; set; }
}
