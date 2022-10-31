using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// This describes how <see cref="AttributeDefinition.Value"/> is wrapped by another <see cref="ValueDefinition"/>.
    /// Only used in <see cref="NestedBlockAttributeDefinition"/>.
    /// </summary>
    public enum ValueDefinitionWrapping
    {
        /// <summary>
        /// Block is wrapped by list.
        /// </summary>
        List,
        /// <summary>
        /// Block is wrapped by set.
        /// </summary>
        Set,
        /// <summary>
        /// Block is wrapped by map.
        /// </summary>
        Map,
        // TODO: The same as with SINGLE, except that if there is no block of that type Terraform will synthesize a
        // block value by pretending that all of the declared attributes are null and that there are zero blocks of
        // each declared block type.
        //Group
    }
}
