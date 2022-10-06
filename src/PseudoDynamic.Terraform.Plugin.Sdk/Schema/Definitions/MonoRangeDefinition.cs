namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal abstract class MonoRangeDefinition : RangeDefinition
    {
        public TerraformDefinition Item { get; }

        protected MonoRangeDefinition(TerraformDefinition item)
        {
            Item = item;
        }
    }
}
