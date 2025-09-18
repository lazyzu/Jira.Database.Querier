using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class entity_property
{
    public decimal ID { get; set; }

    public string ENTITY_NAME { get; set; }

    public decimal? ENTITY_ID { get; set; }

    public string PROPERTY_KEY { get; set; }

    public DateTime? CREATED { get; set; }

    public DateTime? UPDATED { get; set; }

    public string json_value { get; set; }
}
