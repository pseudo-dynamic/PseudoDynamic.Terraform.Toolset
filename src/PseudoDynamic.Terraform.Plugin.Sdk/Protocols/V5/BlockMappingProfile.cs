using AutoMapper;
using Google.Protobuf;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class BlockMappingProfile : Profile
    {
        public BlockMappingProfile()
        {
            var schema = CreateMap<BlockDefinition, Schema>()
                .ForMember(x => x.Version, o => o.MapFrom(x => x.Version))
                .ForMember(x => x.Block, o => o.MapFrom(x => x));

            var blockFromBlock = CreateMap<BlockDefinition, Schema.Types.Block>()
                .ForMember(x => x.Attributes, o => o.MapFrom(x => x.Attributes))
                .ForMember(x => x.BlockTypes, o => o.MapFrom(x => x.Blocks))
                .ForMember(x => x.Deprecated, o => o.MapFrom(x => x.IsDeprecated))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description))
                .ForMember(x => x.DescriptionKind, o => o.MapFrom(x => x.DescriptionKind))
                .ForMember(x => x.Version, o => o.MapFrom(x => x.Version));

            var blockAttribute = CreateMap<BlockAttributeDefinition, Schema.Types.Attribute>()
                .ForMember(x => x.Computed, o => o.MapFrom(x => x.IsComputed))
                .ForMember(x => x.Deprecated, o => o.MapFrom(x => x.IsDeprecated))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description))
                .ForMember(x => x.DescriptionKind, o => o.MapFrom(x => x.DescriptionKind))
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.Optional, o => o.MapFrom(x => x.IsOptional))
                .ForMember(x => x.Required, o => o.MapFrom(x => x.IsRequired))
                .ForMember(x => x.Sensitive, o => o.MapFrom(x => x.IsSensitive));

            blockAttribute.ForMember(x => x.Type, o => o.MapFrom((destination, _) =>
                ByteString.CopyFromUtf8(BlockAttributeTypeBuilder.Default.BuildJsonType(destination.Value))));

            var nestedBlockAttribute = CreateMap<NestedBlockAttributeDefinition, Schema.Types.NestedBlock>()
                .ForMember(x => x.Block, o => o.MapFrom(x => x.Block))
                .ForMember(x => x.MaxItems, o => o.MapFrom(x => x.MaximumItems))
                .ForMember(x => x.MinItems, o => o.MapFrom(x => x.MinimumItems))
                .ForMember(x => x.Nesting, o => o.MapFrom(x => x.ValueWrapping))
                .ForMember(x => x.TypeName, o => o.MapFrom(x => x.Name));

            nestedBlockAttribute.ForMember(x => x.Nesting, o => o.MapFrom(x => ToNestedBlockNesting(x.ValueWrapping)));
        }

        Schema.Types.NestedBlock.Types.NestingMode ToNestedBlockNesting(ValueDefinitionWrapping? valueWrapping) => valueWrapping switch {
            ValueDefinitionWrapping.List => Schema.Types.NestedBlock.Types.NestingMode.List,
            ValueDefinitionWrapping.Set => Schema.Types.NestedBlock.Types.NestingMode.Set,
            ValueDefinitionWrapping.Map => Schema.Types.NestedBlock.Types.NestingMode.Map,
            _ => Schema.Types.NestedBlock.Types.NestingMode.Single
        };
    }
}
