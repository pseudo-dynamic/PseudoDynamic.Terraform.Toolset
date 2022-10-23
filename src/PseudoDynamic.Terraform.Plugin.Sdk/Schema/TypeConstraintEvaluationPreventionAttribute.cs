namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Serves marking an implementation not being evaluated by <see cref="TerraformTypeConstraintEvaluator"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    internal class TypeConstraintEvaluationPreventionAttribute : Attribute
    {
    }
}
