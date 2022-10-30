using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Describes a Terraform service, which can be a provider, a resource, a data source, or a provisioner.
    /// </summary>
    internal abstract record TerraformServiceDescriptor : ITerraformServiceDescriptor
    {
        public object? Service { get; init; }
        public Type ServiceType { get; private set; }
        public Type SchemaType { get; private set; }

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
            if (!ServiceType.IsImplementingGenericTypeDefinition(typeDefinition, out _, out var genericTypeArguments)) {
                throw new ArgumentException($"The {subject} type {ServiceType.FullName} should implement {typeDefinition.FullName}", nameof(ServiceType));
            }

            if (!havingGenericTypeArguments.SequenceEqual(genericTypeArguments)) {
                throw new ArgumentException($"The {subject} type {ServiceType.FullName} implements {typeDefinition.FullName}, but its generic type arguments do not match the expected ones: " +
                    string.Join(", ", havingGenericTypeArguments.Select(x => x.ToString())), nameof(ServiceType));
            }

            if (genericTypeArguments.Single() != SchemaType) {

            }
        }

        protected virtual void Validate()
        {
        }

        [MemberNotNull(nameof(ServiceType), nameof(SchemaType))]
        private void SetTypes(Type dataSourceType, Type schemaType)
        {
            ServiceType = dataSourceType ?? throw new ArgumentNullException(nameof(dataSourceType));
            SchemaType = schemaType ?? throw new ArgumentNullException(nameof(schemaType));
            Validate();
        }
    }
}
