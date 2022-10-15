using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class MapperExtensions
    {
        public static void AddMapperBase(this Profile profile)
        {
            profile.CreateMap(typeof(IReadOnlyList<>), typeof(RepeatedField<>)).ConvertUsing(typeof(RepeatedFieldFromEnumerableConverter<,>));
            //profile.CreateMap(typeof(IList<>), typeof(RepeatedField<>)).ConvertUsing(typeof(RepeatedFieldFromEnumerableConverter<,>));
            profile.CreateMap<IEnumerable<byte>, ByteString>().ConvertUsing(x => ByteString.FromStream(new ByteStream(x)));

            //protected ProviderMapperProfileExtensions()
            //{
            //    //this.Internal().ForAllPropertyMaps(
            //    //    map => map.SourceType.IsGenericType && map.SourceType.GetGenericTypeDefinition() == typeof(RepeatedField<>),
            //    //    (_, o) => o.UseDestinationValue());

            //    //this.Internal().ForAllPropertyMaps(
            //    //    map => map.SourceType.IsGenericType && map.SourceType.GetGenericTypeDefinition() == typeof(MapField<,>),
            //    //    (_, o) => o.UseDestinationValue());

            //    //this.Internal().ForAllPropertyMaps(
            //    //    map => map.SourceType == typeof(ByteString),
            //    //    (_, o) => o.UseDestinationValue());
            //}
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

        internal class ByteStringFromEnumerable : ITypeConverter<IEnumerable<byte>, ByteString>
        {
            public ByteString Convert(IEnumerable<byte> source, ByteString destination, ResolutionContext context) => throw new NotImplementedException();

        }

        public class ByteStream : Stream, IDisposable
        {
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => 0;
            public override long Position { get; set; } = 0;

            private readonly IEnumerator<byte> _input;
            private bool _disposed;

            public ByteStream(IEnumerable<byte> input)
            {
                _input = input.GetEnumerator();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int i = 0;

                for (; i < count && _input.MoveNext(); i++) {
                    buffer[i + offset] = _input.Current;
                }

                return i;
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException();
            public override void SetLength(long value) => throw new InvalidOperationException();
            public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
            public override void Flush() => throw new InvalidOperationException();

            void IDisposable.Dispose()
            {
                if (_disposed) {
                    return;
                }

                _input.Dispose();
                _disposed = true;
            }
        }
    }
}
