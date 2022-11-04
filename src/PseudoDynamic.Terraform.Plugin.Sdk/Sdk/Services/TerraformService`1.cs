using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    /// <summary>
    /// Describes a Terraform service, which can be <see cref="ProviderService"/>, <see cref="ResourceService"/>, <see cref="DataSourceService"/>, or <see cref="ProvisionerService"/>.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    internal abstract record TerraformService<TImplementation> : ITerraformService<TImplementation>
    {
        public BlockDefinition Schema { get; }

        /// <summary>
        /// The implementation. Throws if <see langword="null"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        [NotNull]
        public virtual TImplementation Implementation => _implementation ?? throw new InvalidOperationException("Implementation is not set");

        TImplementation? _implementation;

        /// <summary>
        /// A constructor that does not initialize <see cref="Implementation"/>.
        /// </summary>
        /// <param name="schema"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected TerraformService(BlockDefinition schema) =>
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));

        protected TerraformService(BlockDefinition schema, TImplementation implementation)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        /// <summary>
        /// The implementation. Does not throw if <see langword="null"/>.
        /// </summary>
        protected TImplementation GetImplementation() => _implementation!;
    }
}
