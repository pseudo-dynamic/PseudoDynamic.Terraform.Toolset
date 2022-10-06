namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal static class VisitContextExtensions
    {
        /// <summary>
        /// The new instance of type <see cref="VisitContext"/> inherits possible existing <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="walkingType"></param>
        /// <param name="contextType"></param>
        public static VisitContext ToVisitingContext(
            this IRootVisitContext? context,
            Type walkingType,
            VisitContextType contextType)
        {
            if (context is null) {
                return new VisitContext(walkingType) { ContextType = contextType };
            }

            return new VisitContext(context, walkingType) { ContextType = contextType };
        }
    }
}
