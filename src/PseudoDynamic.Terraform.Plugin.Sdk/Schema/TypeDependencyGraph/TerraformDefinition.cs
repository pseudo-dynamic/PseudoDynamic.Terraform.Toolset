using System.Runtime.CompilerServices;
using System.Text;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class TerraformDefinition
    {
        protected readonly static Type UncomputedSourceType = typeof(object);

        /// <summary>
        /// Prevents: "RCS1132:Remove redundant overriding member." from Roslynator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static T PreventRCS1036<T>(T input) => input;

        protected internal static StringBuilder PrintMembersFix(StringBuilder builder, object anonymousObject) =>
            builder.Append($"{anonymousObject.ToString()?.Trim('{', '}')}, ");

        /// <summary>
        /// Represents the source type, the type this definition is actually
        /// representing.
        /// </summary>
        public virtual Type SourceType { get; }

        protected TerraformDefinition(Type sourceType) => SourceType = sourceType;

        public abstract TerraformDefinitionType DefinitionType { get; }

        protected internal abstract void Visit(TerraformDefinitionVisitor visitor);
    }
}
