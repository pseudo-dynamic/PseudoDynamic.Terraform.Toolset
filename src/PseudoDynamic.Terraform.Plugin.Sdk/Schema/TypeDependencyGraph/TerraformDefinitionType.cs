namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public enum TerraformDefinitionType
    {
        Dynamic,
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
