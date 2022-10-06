using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.Conventions;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class BlockSchemaBuilder
    {
        public static BlockSchemaBuilder CreateDefault() => new BlockSchemaBuilder(SnakeCaseAttributeNameConvention.Default);

        private IAttributeNameConvention _attributeNameConvention;
        private BlockSchema _block = BlockSchema.Empty;

        public BlockSchemaBuilder(IAttributeNameConvention attributeNameConvention)
        {
            _attributeNameConvention = attributeNameConvention;
        }

        private BlockSchema CreateConcatenatedSchema(Type schemaType, Dictionary<string, AttributeSchema> attributes)
        {
            var schemaProperties = schemaType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetGetMethod(nonPublic: false) != null && x.GetSetMethod(nonPublic: false) != null);

            foreach (var schemaProperty in schemaProperties) {
                var attribute = GetAttributeSchema(schemaProperty);
                attributes.Add(attribute.AttributeName, attribute);
            }

            //void GetElementInfo(TerraformType terraformType, out TerraformType terraformBlockRangeType, out Type blockRangeType, out Type unwrappedType)
            //{

            //}

            //TerraformType GetGuessedTerraformType()
            //{

            //}

            //BlockSchemaPropertyInfo GetSchemaPropertyElementInfo(PropertyInfo schemaPropertyInfo)
            //{

            //}

            BlockSchemaPropertyInfo GetSchemaPropertyInfo(PropertyInfo schemaPropertyInfo)
            {
                var schemaProperty = new BlockSchemaPropertyTypeArgument(schemaPropertyInfo);
                var SchemaPropertyName = schemaPropertyInfo.Name;

                var terraformTypeAttribute = schemaPropertyInfo.GetCustomAttribute<TerraformTypeAttribute>(inherit: true);
                var hasTerraformTypeAttribute = terraformTypeAttribute is not null;

                var guessedTerraformTypes = schemaProperty.UnwrappedTypeArgument.EvaludateTerraformType();
                TerraformType terraformType;

                BlockSchemaPropertyInfo? elementInfo = null;

                if (hasTerraformTypeAttribute) {
                    var explicitTerraformType = terraformTypeAttribute!.Type;
                    var isExplicitTerraformBlockType = explicitTerraformType.IsBlockType();

                    if (isExplicitTerraformBlockType && guessedTerraformTypes.Contains(TerraformType.Object)
                        || guessedTerraformTypes.Contains(explicitTerraformType)) {
                        terraformType = explicitTerraformType;
                    } else if (isExplicitTerraformBlockType && guessedTerraformTypes.Count == 1
                          && guessedTerraformTypes.Single().IsRangeType()) {
                        terraformType = explicitTerraformType;
                    } else {
                        throw new InvalidOperationException($"The \"{nameof(TerraformTypeAttribute)}\" attribute on \"{schemaPropertyInfo.Name}\" property wants to be "
                            + $"a \"{explicitTerraformType}\" Terraform value type but the \"{schemaPropertyInfo.PropertyType.Name}\" property type does not support it");
                    }
                } else if (guessedTerraformTypes.Count != 1) {
                    throw new InvalidOperationException($"The \"{schemaPropertyInfo.Name}\" property cannot implement more than one Terraform value type. " +
                        "The following Terraform value types have been found: "
                        + string.Join(", ", guessedTerraformTypes.Select(x => x.ToString())));
                } else {
                    //var guessedTerraformType = guessedTerraformTypes.Single();

                    //if (guessedTerraformType.IsRangeType())
                    //{
                    //    var rangeTypeArgument = schemaProperty.GetTypeArgument(guessedTerraformType);

                    //    if (rangeTypeArgument.contains

                    //    if (terraformRangeType
                    //}

                    //terraformType = guessedTerraformType;
                    terraformType = default;
                }

                bool isNullable = schemaProperty.UnwrappedTypeArgument.ReadNullability == NullabilityState.Nullable;

                return new BlockSchemaPropertyInfo(
                    schemaProperty.Type,
                    terraformType,
                    schemaProperty.UnwrappedTypeArgument.Type,
                    isNullable,
                    schemaProperty.IsTerraformValue);
            }

            AttributeSchema GetBaseAttributeSchema(PropertyInfo schemaProperty)
            {
                var attributeTypeInfo = GetSchemaPropertyInfo(schemaProperty);
                string attributeName = _attributeNameConvention.Format(schemaProperty.Name);

                var isComputed = schemaProperty.GetCustomAttribute<ComputedAttribute>(inherit: true) is not null;
                var nullabilityContext = new NullabilityInfoContext();

                NullabilityInfo valueNullabilityInfo;
                if (attributeTypeInfo.IsTerraformValue) {
                    valueNullabilityInfo = nullabilityContext.Create(schemaProperty).GenericTypeArguments[0];
                } else {
                    valueNullabilityInfo = nullabilityContext.Create(schemaProperty);
                }

                var valueNullability = valueNullabilityInfo.ReadState;
                var isNullable = valueNullability == NullabilityState.Nullable;
                var isRequired = isComputed ? false : !isNullable;
                var IsOptional = isComputed ? false : isNullable;

                var isSensitive = schemaProperty.GetCustomAttribute<SensitiveAttribute>(inherit: true) is not null;

                return new AttributeSchema(attributeName, attributeTypeInfo) {
                    IsComputed = isComputed,
                    IsRequired = isRequired,
                    IsOptional = IsOptional,
                    IsSensitive = isSensitive,
                };
            }

            AttributeSchema GetAttributeSchema(PropertyInfo schemaProperty)
            {
                return GetBaseAttributeSchema(schemaProperty);
            }

            return new BlockSchema(attributes);
        }

        public void ConcatSchema(Type schemaType)
        {
            var attributes = new Dictionary<string, AttributeSchema>(_block.Attributes);
            _block = CreateConcatenatedSchema(schemaType, attributes);
        }

        public BlockSchema BuildSchema() => new BlockSchema(_block);
    }
}
