using PseudoDynamic.Terraform.Plugin.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    /// <summary>
    /// Describes a Terraform service, which can be a provider, a resource, a data source, or a provisioner.
    /// </summary>
    internal abstract record TerraformServiceDescriptor : ITerraformServiceDescriptor
    {
        public object? Implementation { get; init; }
        public Type ImplementationType { get; private set; }
        public Type SchemaType { get; private set; }
        public virtual Type? ProviderMetaSchemaType { get; }

        public TerraformServiceDescriptor(Type serviceType, Type schemaType) =>
            SetTypes(serviceType, schemaType);

        /// <summary>
        /// Checks
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="havingGenericTypeArguments"></param>
        /// <param name="subject">For example "resource" or "data source".</param>
        protected void EnsureServiceImplementsGenericTypeDefinition(Type typeDefinition, Type[] havingGenericTypeArguments, string subject)
        {
            if (!ImplementationType.IsImplementingGenericTypeDefinition(typeDefinition, out _, out var genericTypeArguments)) {
                throw new ArgumentException($"The {subject} type {ImplementationType.FullName} should implement {typeDefinition.FullName}", nameof(ImplementationType));
            }

            if (!havingGenericTypeArguments.SequenceEqual(genericTypeArguments)) {
                throw new ArgumentException($"The {subject} type {ImplementationType.FullName} implements {typeDefinition.FullName}, but its generic type arguments do not match the expected ones: " +
                    string.Join(", ", havingGenericTypeArguments.Select(x => x?.ToString() ?? "<null>")), nameof(ImplementationType));
            }
        }

        [MemberNotNull(nameof(ImplementationType), nameof(SchemaType))]
        private void SetTypes(Type dataSourceType, Type schemaType)
        {
            ImplementationType = dataSourceType ?? throw new ArgumentNullException(nameof(dataSourceType));
            SchemaType = schemaType ?? throw new ArgumentNullException(nameof(schemaType));
        }
    }
}
