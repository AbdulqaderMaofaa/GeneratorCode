using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorCode.Core.DomainModel;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    public interface IDomainModelService
    {
        DomainModel.DomainModel CurrentModel { get; }

        void CreateNewModel(string projectName, string defaultNamespace, DatabaseType databaseType, string targetFramework = "net8.0");

        void AddEntity(DomainEntity entity);
        void RemoveEntity(string entityName);
        void UpdateEntity(string oldName, DomainEntity entity);

        void AddProperty(string entityName, DomainProperty property);
        void RemoveProperty(string entityName, string propertyName);
        void UpdateProperty(string entityName, string oldPropertyName, DomainProperty property);

        void AddRelation(string entityName, DomainRelation relation);
        void RemoveRelation(string entityName, string targetEntity);

        ValidationResult ValidateModel();

        Task SaveModelAsync(string path);
        Task<DomainModel.DomainModel> LoadModelAsync(string path);

        string ExportToJson();
        void ImportFromJson(string json);

        DomainEntity ConvertFromTableInfo(TableInfo tableInfo, DatabaseType databaseType);
    }
}
