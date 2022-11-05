using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal static class AssertingTerraformDefinitionEqualityComparer
    {
        public static readonly TerraformDefinitionEqualityComparer Default = new(static (x, y) => {
            Assert.Equal(x, y);
            return true;
        });
    }
}
