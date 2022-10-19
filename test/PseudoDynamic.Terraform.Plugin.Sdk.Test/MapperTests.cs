using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin
{
    public class MapperTests
    {
        [Fact]
        internal void Global_mapper_configuration_is_valid()
        {
            var mapper = new MapperConfiguration(config => config.AddMaps(typeof(Mapper))).CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
