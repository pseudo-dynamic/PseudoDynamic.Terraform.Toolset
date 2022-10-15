using AutoMapper;
using Google.Protobuf;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class SchemaMapper : SchemaMapperBase
    {
        public SchemaMapper()
        {
            var schema = CreateMap<BlockDefinition, Schema>(MemberList.None)
                .ForMember(x => x.Version, o => o.MapFrom(x => x.SchemaVersion))
                .ForMember(x => x.Block, o => o.MapFrom(x => x));

            var blockFromBlock = CreateMap<BlockDefinition, Schema.Types.Block>(MemberList.None)
                .ForMember(x => x.Attributes, o => o.MapFrom(x => x.Attributes))
                .ForMember(x => x.BlockTypes, o => o.MapFrom(x => x.Blocks))
                .ForMember(x => x.Deprecated, o => o.MapFrom(x => x.IsDeprecated))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description))
                .ForMember(x => x.DescriptionKind, o => o.MapFrom(x => x.DescriptionKind))
                .ForMember(x => x.Version, o => o.MapFrom(x => x.SchemaVersion));

            var blockAttribute = CreateMap<BlockAttributeDefinition, Schema.Types.Attribute>(MemberList.None)
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

            var nestedBlockAttribute = CreateMap<NestedBlockAttributeDefinition, Schema.Types.NestedBlock>(MemberList.None)
                .ForMember(x => x.TypeName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.Nesting, o => o.MapFrom(x => x.ValueWrapping))
                .ForMember(x => x.Block, o => o.MapFrom(x => x.Block));
        }
    }
}
