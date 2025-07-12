using System;
using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    public class TableInfo
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long RowCount { get; set; }
        public string Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool HasIndexes { get; set; }
        public bool HasTriggers { get; set; }
        public string Type { get; set; }
        public string Engine { get; set; }
        public string Collation { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public List<string> PrimaryKeys { get; set; }
        public List<ForeignKeyInfo> ForeignKeys { get; set; }
        public List<IndexInfo> Indexes { get; set; }
        public List<TriggerInfo> Triggers { get; set; }

        public TableInfo()
        {
            Columns = new List<ColumnInfo>();
            PrimaryKeys = new List<string>();
            ForeignKeys = new List<ForeignKeyInfo>();
            Indexes = new List<IndexInfo>();
            Triggers = new List<TriggerInfo>();
        }
    }
} 