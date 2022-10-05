using System.Numerics;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class TerraformTypeEvaluator
    {
        public static readonly TerraformTypeEvaluator Default = new TerraformTypeEvaluator();

        TerraformType? EvaluateNonObjectTypeCode(TypeCode typeCode) => typeCode switch {
            TypeCode.SByte or TypeCode.Byte
            or TypeCode.Int16 or TypeCode.UInt16
            or TypeCode.Int32 or TypeCode.UInt32
            or TypeCode.Int64 or TypeCode.UInt64 => TerraformType.Number,
            TypeCode.String => TerraformType.String,
            TypeCode.Boolean => TerraformType.Bool,
            _ => default
        };

        TerraformType? EvaluateObjectTypeCodedValueType(Type valueType) => valueType switch {
            var _ when valueType == typeof(BigInteger) => TerraformType.Number,
            _ => default
        };

        TerraformType EvaluateClassType(Type classType) => classType switch {
            var _ when classType == typeof(object) => TerraformType.Any,
            // We now say it is a "object" but upper context can change to "block" or "tuple"
            _ => TerraformType.Object
        };

        TerraformType? EvaluateGenericReferenceTypeDefinition(Type genericReferenceTypeDefinition) =>
            genericReferenceTypeDefinition switch {
                var _ when genericReferenceTypeDefinition == typeof(IList<>) => TerraformType.List,
                var _ when genericReferenceTypeDefinition == typeof(ISet<>) => TerraformType.Set,
                var _ when genericReferenceTypeDefinition == typeof(IDictionary<,>) => TerraformType.Map,
                _ => default
            };

        public IReadOnlySet<TerraformType> Evaluate(Type unwrappedPropertyType)
        {
            var terraformTypes = new HashSet<TerraformType>();
            var typeCode = Type.GetTypeCode(unwrappedPropertyType);

            if (typeCode != TypeCode.Object) {
                // Primitives including string
                AddNonNull(EvaluateNonObjectTypeCode(Type.GetTypeCode(unwrappedPropertyType)));
            }

            if (typeCode == TypeCode.Object && unwrappedPropertyType.IsValueType) {
                // Structs
                AddNonNull(EvaluateObjectTypeCodedValueType(unwrappedPropertyType));
            }

            if (unwrappedPropertyType.IsInterface && unwrappedPropertyType.IsGenericType) {
                // Generic interfaces
                AddNonNull(EvaluateGenericReferenceTypeDefinition(unwrappedPropertyType.GetGenericTypeDefinition()));
            } else if (typeCode == TypeCode.Object && unwrappedPropertyType.IsClass) {
                // Classes except string
                terraformTypes.Add(EvaluateClassType(unwrappedPropertyType));
            }

            return terraformTypes;

            void AddNonNull(TerraformType? terraformType)
            {
                if (!terraformType.HasValue) {
                    return;
                }

                terraformTypes.Add(terraformType.Value);
            }
        }
    }
}
