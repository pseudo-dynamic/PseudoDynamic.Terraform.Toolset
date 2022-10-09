using System.Runtime.CompilerServices;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class VisitContextType
    {
        internal static Builder New([CallerMemberName] string? id = null) => new Builder(id);

        /// <summary>
        /// Currently visiting context is about a schema.
        /// </summary>
        public static readonly VisitContextType Complex = new();

        /// <summary>
        /// Currently visiting context is about a property.
        /// </summary>
        public static readonly VisitContextType PropertySegment = new();

        /// <summary>
        /// Currently visiting context is about a property.
        /// </summary>
        public static readonly VisitContextType Property = New().Inherits(PropertySegment);

        /// <summary>
        /// Currently visiting context is about a property generic argument.
        /// </summary>
        public static readonly VisitContextType PropertyGenericSegment = New().Inherits(PropertySegment);

        public string Id { get; }

        private readonly IReadOnlyList<VisitContextType> _inheritedTypes;

        internal VisitContextType([CallerMemberName] string? id = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _inheritedTypes = Array.Empty<VisitContextType>();
        }

        private VisitContextType(string id, IEnumerable<VisitContextType>? inheritedTypes)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));

            if (inheritedTypes is null) {
                _inheritedTypes = Array.Empty<VisitContextType>();
            } else {
                _inheritedTypes = new List<VisitContextType>(inheritedTypes);
            }
        }

        public VisitContextType Inherits(params VisitContextType[] types) =>
            New(Id).Inherits(types);

        public override string ToString() => _inheritedTypes.Count == 0
            ? Id
            : $"[{string.Join(",", _inheritedTypes.Select(x => x.ToString()).Prepend(Id))}]";

        public override bool Equals(object? obj)
        {
            if (obj is not VisitContextType otherType) {
                return false;
            }

            if (Equals(Id, otherType.Id)) {
                return true;
            }

            foreach (var inheritedType in _inheritedTypes) {
                if (inheritedType.Equals(otherType)) {
                    return true;
                }
            }

            return false;
        }

        public static bool operator ==(VisitContextType a, VisitContextType b) =>
            a.Equals(b);

        public static bool operator !=(VisitContextType a, VisitContextType b) =>
            !a.Equals(b);

        public sealed class Builder
        {
            private readonly string _id;
            private IEnumerable<VisitContextType>? _inheritedTypes;

            internal Builder([CallerMemberName] string? id = null) =>
                _id = id ?? throw new ArgumentNullException(nameof(id));

            public Builder Inherits(params VisitContextType[] types)
            {
                _inheritedTypes = types;
                return this;
            }

            public static implicit operator VisitContextType(Builder builder) =>
                new VisitContextType(builder._id, builder._inheritedTypes);
        }
    }
}
