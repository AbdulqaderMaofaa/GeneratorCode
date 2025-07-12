using System;

namespace GeneratorCode.Core.Models
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public int OrdinalPosition { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public string Description { get; set; }
        public string CSharpType { get; set; }
    }
} 