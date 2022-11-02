namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class Theory
    {
        // TODO: must be refactored once Xunit v3 is out with Explicit-feature.
        internal class TerraformAttribute : TheoryAttribute
        {
            public TerraformAttribute()
            {
#if !DEBUG
                Skip = "Terraform only runs in environment with active debug configuration";
#endif
            }
        }
    }
}
