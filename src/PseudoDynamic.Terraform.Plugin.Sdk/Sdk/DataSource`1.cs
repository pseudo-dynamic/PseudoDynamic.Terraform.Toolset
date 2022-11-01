namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class DataSource<Schema> : DataSource<Schema, object>
        where Schema : class
    {
    }
}
