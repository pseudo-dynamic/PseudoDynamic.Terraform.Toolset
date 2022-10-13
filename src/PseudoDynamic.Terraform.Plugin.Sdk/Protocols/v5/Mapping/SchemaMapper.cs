using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols.v5.Mapping
{
    internal class SchemaMapper : Profile
    {
        protected SchemaMapper()
        {
            var map = CreateMap<BlockDefinition, Schema.Types.Block>(MemberList.None)
                .ForMember(x => x.Attributes, o => o.MapFrom(x => x.Attributes))
                .ForMember(x => x.Version, o => o.MapFrom(x => x.SchemaVersion))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description))
                .ForMember(x => x.DescriptionKind, o => o.MapFrom(x => x.DescriptionKind))
                .ForMember(x => x.Deprecated, o => o.MapFrom(x => x.IsDeprecated));

            //new Schema.Types.Block() {
            //    BlockTypes =
            //}
        }
    }
}
