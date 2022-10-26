namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ProviderSetupStructDependencyInjectionExtensions
    {
        public static IProviderSetup AddResource<Resource, Schema>(this IProviderSetup provider)
            where Resource : IResource<Schema>
            where Schema : struct
        {
            provider.AddResource(typeof(Resource), typeof(Schema));
            return provider;
        }
    }
}
