using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    public class ProviderMapperTests
    {
        private static IMapper Mapper = new MapperConfiguration(config => config.AddProfile<ProviderMapper>()).CreateMapper();

        [Fact]
        internal void Mapper_configuration_is_valid() =>
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
