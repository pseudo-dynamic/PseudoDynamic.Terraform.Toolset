namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <inheritdoc/>
    public interface IDataSource<Schema> : IDataSource<Schema, object>
        where Schema : class
    {
    }
}
