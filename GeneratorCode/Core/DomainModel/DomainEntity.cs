using System.Collections.Generic;

namespace GeneratorCode.Core.DomainModel
{
    public class DomainEntity
    {
        public string Name { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public string Schema { get; set; } = "dbo";

        public List<DomainProperty> Properties { get; set; } = new();

        public List<DomainRelation> Relations { get; set; } = new();

        public bool UseBaseEntity { get; set; }

        public bool EnableAuditFields { get; set; }

        public string Description { get; set; }

        public List<Dictionary<string, object>> SeedData { get; set; } = new();
    }
}
