namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Annotates a class for being used as Terraform nested block.
    /// Prefer this over <see cref="BlockAttribute"/> to indicate a property being
    /// a nested block if you need to provide additional informations that can not
    /// be covered by <see cref="BlockAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NestedBlockAttribute : BlockAttribute
    {
        /// <summary>
        /// You must set this property if the type, that represents the nested block,
        /// is wrapped by a list, set or map and this wrapping type implicitly can
        /// represent not only a list, set or map but several at once.
        /// </summary>
        public ValueWrapping? WrappedBy { get; init; }
    }
}
