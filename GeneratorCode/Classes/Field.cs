using System;

namespace GeneratorCode.Classes
{
    public class Field
    {
        public Field(string name, Type type)
        {
            this.FieldName = name;
            this.FieldType = type;
        }

        public string FieldName;

        public Type FieldType;
    }
}
