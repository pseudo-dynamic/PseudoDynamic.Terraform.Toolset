using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceDescriptor
    {
        public object? Resource { get; }
        public Type ResourceType { get; private set; }
        public Type SchemaType { get; private set; }

        public ResourceDescriptor(Type resourceType, Type schemaType) =>
            SetTypes(resourceType, schemaType);

        public ResourceDescriptor(object resource, Type schemaType)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            SetTypes(resource.GetType(), schemaType);
        }

        [MemberNotNull(nameof(ResourceType), nameof(SchemaType))]
        private void SetTypes(Type resourceType, Type schemaType)
        {
            ResourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
            SchemaType = schemaType ?? throw new ArgumentNullException(nameof(schemaType));

            if (!resourceType.IsImplementingGenericTypeDefinition(typeof(IResource<>), out _, out var genericTypeArguments)) {
                throw new ArgumentException($"The resource type {resourceType.FullName} should implement {typeof(IResource<>).FullName}", nameof(resourceType));
            }

            if (genericTypeArguments.Single() != schemaType) {
                throw new ArgumentException($"The resource type {resourceType.FullName} implements {typeof(IResource<>).FullName} but its generic type must match with {schemaType.FullName}", nameof(resourceType));
            }
        }
    }
}
