namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class ResourceProxy
    {
        public static ResourceProxy<Schema> Of<Schema>(IResource<Schema> implementation) =>
            new ResourceProxy<Schema>(implementation);
    }

    internal class ResourceProxy<Schema> : IResource<Schema>
    {
        public string TypeName => _implementation.TypeName;

        private readonly IResource<Schema> _implementation;

        public ResourceProxy(IResource<Schema> implementation) =>
            _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));

        public Task MigrateState() => _implementation.MigrateState();
        public Task ImportState() => _implementation.ImportState();
        public Task ReviseState() => _implementation.ReviseState();
        public Task ValidateConfig(ValidateConfig.Context<Schema> context) => _implementation.ValidateConfig(context);
        public Task Plan() => _implementation.Plan();
        public Task Create() => _implementation.Create();
        public Task Update() => _implementation.Update();
        public Task Delete() => _implementation.Delete();
    }
}
