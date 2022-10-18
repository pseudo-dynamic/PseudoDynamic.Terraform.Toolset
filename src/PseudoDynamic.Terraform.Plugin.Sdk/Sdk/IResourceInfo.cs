namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IResourceInfo
    {
        /// <summary>
        /// <para>
        /// This type name that is going to be appended to the provider name to comply with the resource name
        /// convention of Terraform. (e.g. <![CDATA["<provider-name>_<type-name>"]]>
        /// </para>
        /// <para>
        /// Do not prepend the provider name by yourself! The name remain unformatted, so please ensure snake_case.
        /// </para>
        /// </summary>
        string TypeName { get; }
    }
}
