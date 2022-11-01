namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Resource<Schema> : Resource<Schema, object>
            where Schema : class
    {
    }
}
