namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class Fact
    {
        // TODO: must be refactored once Xunit v3 is out with Explicit-feature.
        internal class TerraformAttribute : FactAttribute
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
