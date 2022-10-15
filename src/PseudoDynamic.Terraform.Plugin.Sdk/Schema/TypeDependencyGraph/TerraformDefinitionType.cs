namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public enum TerraformDefinitionType
    {
        Primitive,
        MonoRange,
        Map,
        Object,
        ObjectAttribute,
        Tuple,
        Block,
        BlockAttribute,
        NestedBlockAttribute
    }
}
