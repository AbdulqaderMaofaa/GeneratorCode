namespace GeneratorCode.Core.DomainModel
{
    public class DomainProperty
    {
        public string Name { get; set; } = string.Empty;

        public string ColumnName { get; set; } = string.Empty;

        public DomainPropertyType Type { get; set; } = DomainPropertyType.String;

        public bool IsRequired { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public int? MaxLength { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }
    }
}
