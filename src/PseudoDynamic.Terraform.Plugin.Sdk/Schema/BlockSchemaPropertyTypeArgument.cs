using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class BlockSchemaPropertyTypeArgument
    {
        public Type Type => _nullabilityInfo.Type;
        public NullabilityState ReadNullability => _nullabilityInfo.ReadState;

        public bool IsTerraformValue {
            get {
                var isTerraformValueRaw = _isTerraformValue;
                bool isTerraformValue;

                if (isTerraformValueRaw.HasValue) {
                    isTerraformValue = isTerraformValueRaw.Value;
                } else {
                    //isTerraformValue = Generics.IsTypeImplementingGenericTypeDefinition(
                    //    Type,
                    //    typeof(ITerraformValue<>),
                    //    out var implementerType,
                    //    out var _);

                    isTerraformValue = false; // ISSUE

                    //if (isTerraformValue) {
                    //    var implementerGenericTypeDefinition = implementerType!.GetGenericTypeDefinition();

                    //    if (implementerGenericTypeDefinition != typeof(TerraformValue<>)
                    //        && implementerGenericTypeDefinition != typeof(ITerraformValue<>)) {
                    //        throw new NotSupportedException($"Only {typeof(TerraformValue<>).Name} or {typeof(ITerraformValue<>).Name} is allowed but not {implementerType.Name}");
                    //    }
                    //}

                    _isTerraformValue = isTerraformValue;
                }

                return isTerraformValue;
            }
        }

        public BlockSchemaPropertyTypeArgument[] TypeArguments =>
            _typeArguments ??= _nullabilityInfo.GenericTypeArguments.Select(x => new BlockSchemaPropertyTypeArgument(x)).ToArray();

        public BlockSchemaPropertyTypeArgument UnwrappedTypeArgument => _unwrappedTypeArgument ??= IsTerraformValue ? TypeArguments[0] : this;
        //public Type UnwrappedType => UnwrappedTypeArgument.Type;
        //public NullabilityState IsUnwrappedTypeArgumentReadNullability => UnwrappedTypeArgument.ReadNullability;

        private bool? _isTerraformValue;
        private NullabilityInfo _nullabilityInfo;
        private BlockSchemaPropertyTypeArgument[]? _typeArguments;
        private BlockSchemaPropertyTypeArgument? _unwrappedTypeArgument;

        private BlockSchemaPropertyTypeArgument(NullabilityInfo typeArgumentNullabilityInfo) =>
            _nullabilityInfo = typeArgumentNullabilityInfo ?? throw new ArgumentNullException(nameof(typeArgumentNullabilityInfo));

        public BlockSchemaPropertyTypeArgument GetTypeArgument(TerraformType terraformType) => terraformType switch {
            TerraformType.List or TerraformType.Set => TypeArguments[0],
            TerraformType.Map => TypeArguments[1],
            _ => throw new InvalidOperationException()
        };


        public BlockSchemaPropertyTypeArgument(PropertyInfo propertyInfo)
            : this(new NullabilityInfoContext().Create(propertyInfo))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unwrappedType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlySet<TerraformType> EvaludateTerraformType()
        {
            var guessedTerraformTypes = TerraformTypeEvaluator.Default.Evaluate(Type);

            if (guessedTerraformTypes.Count == 0) {
                throw new InvalidOperationException($"The \"{Type}\" is incompatible and cannot be translated to a Terraform value type.");
            }

            return guessedTerraformTypes;
        }
    }
}
