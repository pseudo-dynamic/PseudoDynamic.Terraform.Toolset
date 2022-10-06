namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal sealed class TerraformVisitContextType : VisitContextType
    {
        public static readonly VisitContextType TerraformValue = New().Inherits(PropertyGenericArgument);
    }
}
