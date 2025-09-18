using System;

namespace lazyzu.Jira.Database.EntityFrameworkCore.Model;

public partial class entity_property_index_document
{
    public decimal ID { get; set; }

    public string PLUGIN_KEY { get; set; }

    public string MODULE_KEY { get; set; }

    public string ENTITY_KEY { get; set; }

    public DateTime? UPDATED { get; set; }

    public string DOCUMENT { get; set; }
}
