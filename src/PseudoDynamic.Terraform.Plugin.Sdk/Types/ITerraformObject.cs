namespace PseudoDynamic.Terraform.Plugin.Types
{
    /// <summary>
    /// Represents Terraform's "object({..})".
    /// The values of attributes of type of <typeparamref name="T"/>
    /// and their values of attributes (recursively) that contain
    /// attributes are treated as "object" and not as Terraform block.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface ITerraformObject<out T> : ITerraformValue<T>
        where T : class
    {
    }
}
