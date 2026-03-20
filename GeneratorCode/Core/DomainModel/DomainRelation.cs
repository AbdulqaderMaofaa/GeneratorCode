namespace GeneratorCode.Core.DomainModel
{
    public class DomainRelation
    {
        public string SourceEntity { get; set; } = string.Empty;

        public string TargetEntity { get; set; } = string.Empty;

        public RelationType RelationType { get; set; } = RelationType.OneToMany;

        public string ForeignKeyName { get; set; } = string.Empty;

        public DeleteBehavior DeleteBehavior { get; set; } = DeleteBehavior.Cascade;

        public string NavigationPropertyName { get; set; } = string.Empty;

        public string InverseNavigationPropertyName { get; set; } = string.Empty;
    }
}
