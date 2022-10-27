namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ProviderFeatureStructDependencyInjectionExtensions
    {
        public static IProviderFeature.IResourceFeature AddResource<Resource, Schema>(this IProviderFeature.IResourceFeature resourceFeature)
            where Resource : IResource<Schema>
            where Schema : struct
        {
            resourceFeature.ProviderFeature.AddResource(typeof(Resource), typeof(Schema));
            return resourceFeature;
        }

        public static IProviderFeature.IResourceFeature AddResource<Schema>(this IProviderFeature.IResourceFeature resourceFeature, IResource<Schema> resource)
            where Schema : struct
        {
            resourceFeature.ProviderFeature.AddResource(resource, typeof(Schema));
            return resourceFeature;
        }
    }
}
