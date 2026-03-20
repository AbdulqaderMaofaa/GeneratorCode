using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeneratorCode.Core.DomainModel
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class DomainModelValidator
    {
        private static readonly Regex ValidIdentifier = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public ValidationResult Validate(DomainModel model)
        {
            var result = new ValidationResult();

            if (model == null)
            {
                result.Errors.Add("نموذج المجال مطلوب");
                return result;
            }

            if (string.IsNullOrWhiteSpace(model.ProjectName))
                result.Errors.Add("اسم المشروع مطلوب");

            if (string.IsNullOrWhiteSpace(model.DefaultNamespace))
                result.Errors.Add("مساحة الأسماء الافتراضية مطلوبة");

            if (model.Entities == null || model.Entities.Count == 0)
            {
                result.Errors.Add("يجب أن يحتوي النموذج على كيان واحد على الأقل");
                return result;
            }

            ValidateEntityNames(model, result);
            foreach (var entity in model.Entities)
            {
                ValidateEntity(entity, model, result);
            }

            return result;
        }

        private void ValidateEntityNames(DomainModel model, ValidationResult result)
        {
            var duplicates = model.Entities
                .GroupBy(e => e.Name, System.StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dup in duplicates)
            {
                result.Errors.Add($"اسم الكيان '{dup}' مكرر");
            }
        }

        private void ValidateEntity(DomainEntity entity, DomainModel model, ValidationResult result)
        {
            var prefix = $"الكيان '{entity.Name}'";

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                result.Errors.Add("يوجد كيان بدون اسم");
                return;
            }

            if (!ValidIdentifier.IsMatch(entity.Name))
                result.Errors.Add($"{prefix}: الاسم ليس معرّف C# صالح");

            if (entity.Properties == null || entity.Properties.Count == 0)
            {
                result.Errors.Add($"{prefix}: يجب أن يحتوي على خاصية واحدة على الأقل");
                return;
            }

            var hasPK = entity.Properties.Any(p => p.IsPrimaryKey);
            if (!hasPK)
                result.Errors.Add($"{prefix}: يجب أن يحتوي على مفتاح أساسي واحد على الأقل");

            ValidateProperties(entity, result, prefix);
            ValidateRelations(entity, model, result, prefix);
        }

        private void ValidateProperties(DomainEntity entity, ValidationResult result, string prefix)
        {
            var propDuplicates = entity.Properties
                .GroupBy(p => p.Name, System.StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dup in propDuplicates)
            {
                result.Errors.Add($"{prefix}: اسم الخاصية '{dup}' مكرر");
            }

            foreach (var prop in entity.Properties)
            {
                if (string.IsNullOrWhiteSpace(prop.Name))
                {
                    result.Errors.Add($"{prefix}: يوجد خاصية بدون اسم");
                    continue;
                }

                if (!ValidIdentifier.IsMatch(prop.Name))
                    result.Errors.Add($"{prefix}: الخاصية '{prop.Name}' ليست معرّف C# صالح");

                if (prop.Type == DomainPropertyType.String && prop.MaxLength.HasValue && prop.MaxLength.Value <= 0)
                    result.Warnings.Add($"{prefix}: الخاصية '{prop.Name}' لها حد أقصى للطول غير صالح");

                if (prop.Type == DomainPropertyType.Decimal)
                {
                    if (prop.Precision.HasValue && (prop.Precision.Value <= 0 || prop.Precision.Value > 38))
                        result.Warnings.Add($"{prefix}: الخاصية '{prop.Name}' لها دقة غير صالحة (يجب أن تكون 1-38)");

                    if (prop.Scale.HasValue && prop.Scale.Value < 0)
                        result.Warnings.Add($"{prefix}: الخاصية '{prop.Name}' لها مقياس غير صالح");
                }
            }
        }

        private void ValidateRelations(DomainEntity entity, DomainModel model, ValidationResult result, string prefix)
        {
            if (entity.Relations == null) return;

            foreach (var rel in entity.Relations)
            {
                if (string.IsNullOrWhiteSpace(rel.TargetEntity))
                {
                    result.Errors.Add($"{prefix}: يوجد علاقة بدون كيان هدف");
                    continue;
                }

                var targetExists = model.Entities.Any(e =>
                    string.Equals(e.Name, rel.TargetEntity, System.StringComparison.OrdinalIgnoreCase));

                if (!targetExists)
                    result.Errors.Add($"{prefix}: العلاقة تشير إلى كيان غير موجود '{rel.TargetEntity}'");
            }
        }
    }
}
