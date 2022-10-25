using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    public class ProviderMappingProfileTests
    {
        private static IMapper Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<MappingProfileBase>();
            config.AddProfile<BlockMappingProfile>();
            config.AddProfile<ProviderMappingProfile>();
        }).CreateMapper();

        [Fact]
        internal void Mapper_configuration_is_valid() =>
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
