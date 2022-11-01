namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <inheritdoc/>
    public interface IResource<Schema> : IResource<Schema, object>
        where Schema : class
    {
    }
}
