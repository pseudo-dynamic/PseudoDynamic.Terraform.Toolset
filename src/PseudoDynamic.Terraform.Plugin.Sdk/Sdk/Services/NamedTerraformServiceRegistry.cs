using System.Data;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class NamedTerraformServiceRegistry<TTerraformService>
        where TTerraformService : INameProvider
    {
        public IReadOnlyDictionary<string, TTerraformService> Services => _services;

        private readonly Dictionary<string, TTerraformService> _services = new();

        public bool ContainsKey(string name) =>
            _services.ContainsKey(name);

        protected void Add(TTerraformService terraformService)
        {
            string serviceName = terraformService.Name;

            if (ContainsKey(serviceName)) {
                throw new DuplicateNameException($"The resource type name \"{serviceName}\" is already taken");
            }

            _services.Add(serviceName, terraformService);
        }

        protected void Replace(TTerraformService terraformService) =>
            _services[terraformService.Name] = terraformService;
    }
}
