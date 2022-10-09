using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class TerraformTypeConstraintEvaluator
    {
        public static readonly TerraformTypeConstraintEvaluator Default = new TerraformTypeConstraintEvaluator();

        TerraformTypeConstraint? EvaluateNonObjectTypeCode(TypeCode typeCode) => typeCode switch {
            TypeCode.SByte or TypeCode.Byte
            or TypeCode.Int16 or TypeCode.UInt16
            or TypeCode.Int32 or TypeCode.UInt32
            or TypeCode.Int64 or TypeCode.UInt64 => TerraformTypeConstraint.Number,
            TypeCode.String => TerraformTypeConstraint.String,
            TypeCode.Boolean => TerraformTypeConstraint.Bool,
            _ => default
        };

        TerraformTypeConstraint? EvaluateObjectTypeCodedValueType(Type valueType) => valueType switch {
            var _ when valueType == typeof(BigInteger) => TerraformTypeConstraint.Number,
            _ => default
        };

        TerraformTypeConstraint EvaluateClassType(Type classType) => classType switch {
            var _ when classType == typeof(object) => TerraformTypeConstraint.Any,
            // We now say it is a "object" but upper context can change to "block" or "tuple"
            _ => TerraformTypeConstraint.Object
        };

        TerraformTypeConstraint? EvaluateGenericReferenceTypeDefinition(Type genericReferenceTypeDefinition) =>
            genericReferenceTypeDefinition switch {
                var _ when genericReferenceTypeDefinition == typeof(IList<>) => TerraformTypeConstraint.List,
                var _ when genericReferenceTypeDefinition == typeof(ISet<>) => TerraformTypeConstraint.Set,
                var _ when genericReferenceTypeDefinition == typeof(IDictionary<,>) => TerraformTypeConstraint.Map,
                _ => default
            };

        public IReadOnlySet<TerraformTypeConstraint> Evaluate(Type type)
        {
            var terraformTypes = new HashSet<TerraformTypeConstraint>();
            var typeCode = Type.GetTypeCode(type);

            if (typeCode != TypeCode.Object) {
                // Primitives including string
                AddNonNull(EvaluateNonObjectTypeCode(Type.GetTypeCode(type)));
            }

            if (typeCode == TypeCode.Object && type.IsValueType) {
                // Structs
                AddNonNull(EvaluateObjectTypeCodedValueType(type));
            }

            if (type.IsInterface && type.IsGenericType) {
                // Generic interfaces
                AddNonNull(EvaluateGenericReferenceTypeDefinition(type.GetGenericTypeDefinition()));
            } else if (typeCode == TypeCode.Object && type.IsClass) {
                // Classes except string
                terraformTypes.Add(EvaluateClassType(type));
            }

            return terraformTypes;

            void AddNonNull(TerraformTypeConstraint? terraformType)
            {
                if (!terraformType.HasValue) {
                    return;
                }

                terraformTypes.Add(terraformType.Value);
            }
        }
    }
}
