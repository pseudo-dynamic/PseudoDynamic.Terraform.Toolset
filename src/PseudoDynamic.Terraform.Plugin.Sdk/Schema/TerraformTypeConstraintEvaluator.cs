using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class TerraformTypeConstraintEvaluator
    {
        public static readonly TerraformTypeConstraintEvaluator Default = new TerraformTypeConstraintEvaluator();

        private TerraformTypeConstraint? EvaluateNonObjectTypeCode(TypeCode typeCode) => typeCode switch {
            TypeCode.SByte or TypeCode.Byte
            or TypeCode.Int16 or TypeCode.UInt16
            or TypeCode.Int32 or TypeCode.UInt32
            or TypeCode.Int64 or TypeCode.UInt64
            or TypeCode.Decimal
            or TypeCode.Single or TypeCode.Double => TerraformTypeConstraint.Number,
            TypeCode.String => TerraformTypeConstraint.String,
            TypeCode.Boolean => TerraformTypeConstraint.Bool,
            _ => default
        };

        private TerraformTypeConstraint? EvaluateObjectTypeCodedValueType(Type valueType) => valueType switch {
            var _ when valueType == typeof(BigInteger) => TerraformTypeConstraint.Number,
            _ => default
        };

        private TerraformTypeConstraint EvaluateClassType(Type classType)
        {
            if (classType == typeof(object)) {
                return TerraformTypeConstraint.Dynamic;
            }

            // We now say it is a "object" but upper context can change to "block" or "tuple"
            return TerraformTypeConstraint.Object;
        }

        private TerraformTypeConstraint? EvaluateGenericClassTypeDefinition(Type typeDefinition)
        {
            if (typeDefinition == typeof(List<>)) {
                return TerraformTypeConstraint.List;
            }

            if (typeDefinition == typeof(HashSet<>)) {
                return TerraformTypeConstraint.Set;
            }

            if (typeDefinition == typeof(Dictionary<,>)) {
                return TerraformTypeConstraint.Map;
            }

            return default;
        }

        private TerraformTypeConstraint? EvaluateGenericReferenceTypeDefinition(Type typeDefinition)
        {

            if (typeDefinition == typeof(IList<>)) {
                return TerraformTypeConstraint.List;
            }

            if (typeDefinition == typeof(ISet<>)) {
                return TerraformTypeConstraint.Set;
            }

            if (typeDefinition == typeof(IDictionary<,>)) {
                return TerraformTypeConstraint.Map;
            }

            return default;
        }

        public IReadOnlySet<TerraformTypeConstraint> Evaluate(Type type)
        {
            var typeConstraints = new HashSet<TerraformTypeConstraint>();
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
                // If we find predestinated generic classes ..
                var genericClassTypeDefinitionEvaluation = type.IsGenericType
                    ? EvaluateGenericClassTypeDefinition(type.GetGenericTypeDefinition())
                    : default;

                if (genericClassTypeDefinitionEvaluation.HasValue) {
                    // .. then we add it and skip class type evaluation
                    typeConstraints.Add(genericClassTypeDefinitionEvaluation.Value);
                } else {
                    // Classes except string
                    typeConstraints.Add(EvaluateClassType(type));
                }
            }

            return typeConstraints;

            void AddNonNull(TerraformTypeConstraint? typeConstraint)
            {
                if (!typeConstraint.HasValue) {
                    return;
                }

                typeConstraints.Add(typeConstraint.Value);
            }
        }
    }
}
