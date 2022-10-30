namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IDataSource<Schema> : INameProvider where Schema : class
    {
        /// <summary>
        /// <para>
        /// This type name that is going to be appended to the provider name to comply with the data source name
        /// convention of Terraform. (e.g. <![CDATA["<provider-name>_<type-name>"]]>
        /// </para>
        /// <para>
        /// Do not prepend the provider name by yourself! The name remain unformatted, so please ensure snake_case!
        /// </para>
        /// </summary>
        string TypeName { get; }

        string INameProvider.Name => TypeName;

        /// <summary>
        /// Validates the user-defined data source inputs that has been made in Terraform.
        /// </summary>
        Task ValidateConfig(DataSource.ValidateContext<Schema> context);

        /// <summary>
        /// Reads
        /// </summary>
        Task Read(DataSource.ReadContext<Schema> context);
    }
}
