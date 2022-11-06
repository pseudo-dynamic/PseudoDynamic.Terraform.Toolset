using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal class MappingProfileBase : Profile
    {
        private static ByteString CreateByteString(ReadOnlyMemory<byte> x) => ByteString.CopyFrom(x.Span);

        public MappingProfileBase()
        {
            CreateMap(typeof(IReadOnlyList<>), typeof(RepeatedField<>)).ConvertUsing(typeof(RepeatedFieldFromEnumerableConverter<,>));

            CreateMap<ReadOnlyMemory<byte>, ByteString>().ConvertUsing(x => CreateByteString(x));
            CreateMap<ByteString, ReadOnlyMemory<byte>>().ConvertUsing(x => x.Memory);
        }

        internal class RepeatedFieldFromEnumerableConverter<TSource, TDestination> : ITypeConverter<IEnumerable<TSource>, RepeatedField<TDestination>?>
        {
            public RepeatedField<TDestination> Convert(IEnumerable<TSource> source, RepeatedField<TDestination>? destination, ResolutionContext context)
            {
                destination ??= new RepeatedField<TDestination>();

                foreach (var item in source) {
                    destination.Add(context.Mapper.Map<TDestination>(item));
                }

                return destination;
            }
        }
    }
}
