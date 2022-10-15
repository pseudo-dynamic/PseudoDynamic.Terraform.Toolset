using AutoMapper;
using Google.Protobuf.Collections;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal abstract class SchemaMapperBase : Profile
    {
        public SchemaMapperBase() => CreateMap(typeof(IReadOnlyList<>), typeof(RepeatedField<>)).ConvertUsing(typeof(RepeatedFieldFromEnumerableConverter<,>));

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
