namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal static class TypeVisitorContextExtensions
    {
        /// <summary>
        /// The new instance of type <see cref="VisitingContext"/> inherits possible existing <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="walkingType"></param>
        /// <param name="contextType"></param>
        public static VisitingContext ToVisitingContext(
            this IVisitorContext? context,
            Type walkingType,
            VisitingContextType contextType)
        {
            if (context is null) {
                return new VisitingContext(walkingType) { ContextType = contextType };
            }

            return new VisitingContext(context, walkingType) { ContextType = contextType };
        }
    }
}
