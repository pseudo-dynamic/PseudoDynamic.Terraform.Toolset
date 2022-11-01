using PseudoDynamic.Terraform.Plugin.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    /// <summary>
    /// See <see cref="IDesignTimeTerraformService{Schema}"/>.
    /// </summary>
    public class DesignTimeTerraformService
    {
        private static readonly Type GenericTypeDefinition = typeof(IDesignTimeTerraformService<>);

        internal static Type GetSchemaType(Type terraformServiceType)
        {
            if (!terraformServiceType.IsImplementingGenericTypeDefinition(GenericTypeDefinition, out _, out var genericTypeArguments)) {
                throw new InvalidOperationException($"Type {terraformServiceType} was expected to implement {GenericTypeDefinition}");
            }

            return genericTypeArguments.Single();
        }
    }
}
