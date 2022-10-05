namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    public enum VisitingContextType
    {
        /// <summary>
        /// Currently visiting context is about a schema.
        /// </summary>
        Complex,
        /// <summary>
        /// Currently visiting context is about a property.
        /// </summary>
        Property,
        /// <summary>
        /// Currently visiting context is about a property generic argument.
        /// </summary>
        PropertyGenericArgument,
        /// <summary>
        /// Currently visiting context represents the end of a visited chain.
        /// </summary>
        Unknown,
    }
}
