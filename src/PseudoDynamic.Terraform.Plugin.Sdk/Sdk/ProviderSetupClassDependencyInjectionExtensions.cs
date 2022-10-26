namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ProviderSetupClassDependencyInjectionExtensions
    {
        public static IProviderSetup AddResource<Resource, Schema>(this IProviderSetup provider)
            where Resource : IResource<Schema>
            where Schema : class
        {
            provider.AddResource(typeof(Resource), typeof(Schema));
            return provider;
        }
    }
}
