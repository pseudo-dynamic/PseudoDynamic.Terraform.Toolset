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
        /// You must set this property if the type, that represents the nested block, is
        /// wrapped by a Terraform list, Terraform set or Terraform map and this wrapping
        /// type implicitly can represent not only a Terraform list, Terraform set or map
        /// but several at once.
        /// </summary>
        public ValueDefinitionWrapping? WrappedBy { get; init; }

        /// <summary>
        /// <see cref="MinimumItems"/> is the minimum number of instances of this block
        /// that a user must specify or Terraform will return an error.
        /// <br/>
        /// <br/><see cref="MinimumItems"/> can only be set for <see cref="ValueDefinitionWrapping.List"/>
        /// and <see cref="ValueDefinitionWrapping.Set"/>. If nested block is not wrapped you can also
        /// set <see cref="MinimumItems"/> and <see cref="MaximumItems"/> both to 1 to indicate
        /// that a single block is required to be set. Otherwise, <see cref="MinimumItems"/>
        /// must remain set to 0.
        /// </summary>
        public int MinimumItems { get; init; }

        /// <summary>
        /// <see cref="MaximumItems"/> is the minimum number of instances of this block
        /// that a user must specify or Terraform will return an error.
        /// <br/>
        /// <br/><see cref="MaximumItems"/> can only be set for <see cref="ValueDefinitionWrapping.List"/>
        /// and <see cref="ValueDefinitionWrapping.Set"/>. If nested block is not wrapped you can also
        /// set <see cref="MaximumItems"/> and <see cref="MinimumItems"/> both to 1 to indicate
        /// that a single block is required to be set. Otherwise, <see cref="MaximumItems"/>
        /// must remain set to 0.
        /// </summary>
        public int MaximumItems { get; init; }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        public NestedBlockAttribute()
        {
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="version">
        /// The schema version. The schema type this nested block describes may also be decorated with <see cref="BlockAttribute"/>, but its version is now overriden by <paramref name="version"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public NestedBlockAttribute(int version) : base(version)
        {
        }
    }
}
