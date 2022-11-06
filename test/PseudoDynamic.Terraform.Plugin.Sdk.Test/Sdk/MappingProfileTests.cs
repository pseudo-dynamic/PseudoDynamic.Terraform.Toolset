using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class MappingProfileTests
    {
        [Fact]
        internal void Mapper_configuration_is_valid()
        {
            var mapper = new MapperConfiguration(config => config.AddMaps(typeof(MappingProfile))).CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
