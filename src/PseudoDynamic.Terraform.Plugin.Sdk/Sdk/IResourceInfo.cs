namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IResourceInfo
    {
        /// <summary>
        /// <para>
        /// This type name that is going to be appended to the provider name to comply with the resource name
        /// convention of Terraform. (e.g. <![CDATA["<provider-name>_<type-suffix>"]]>
        /// </para>
        /// <para>
        /// Do not prepend the provider name by yourself.
        /// </para>
        /// </summary>
        string TypeName { get; }
    }
}
