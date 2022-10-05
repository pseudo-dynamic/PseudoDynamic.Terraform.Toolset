namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal static class IVisitingContextExtensions
    {
        /// <summary>
        /// Creates a copy if this context with <see cref="VisitingContextType.Unknown"/> as new context type.
        /// </summary>
        public static VisitingContext AsUnknown(this IVisitingContext context) =>
            new VisitingContext(context) { ContextType = VisitingContextType.Unknown };
    }
}
