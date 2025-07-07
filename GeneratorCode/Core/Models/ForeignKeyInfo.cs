namespace GeneratorCode.Core.Models
{
    public class ForeignKeyInfo
    {
        public string Name { get; set; }
        public string LocalColumn { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public string DeleteAction { get; set; }
        public string UpdateAction { get; set; }
    }
} 