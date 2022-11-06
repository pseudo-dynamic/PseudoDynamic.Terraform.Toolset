using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk;

public static class TerraformServiceExtensions
{
    /// <summary>
    /// Checks if resource has config. This is only the case when creating or updating the resource.
    /// </summary>
    /// <typeparam name="Schema"></typeparam>
    /// <param name="context"></param>
    /// <param name="configContext"></param>
    /// <returns>
    /// <see langword="true"/> if <see cref="IConfigContext{Schema}.Config"/> is not <see langword="null"/>.
    /// </returns>
    public static bool HasConfig<Schema>(this IConfigContext<Schema> context, [MaybeNullWhen(false)] out IConfigContext<Schema> configContext)
    {
        var result = context.Config != null;
        configContext = result ? context : null;
        return result;
    }

    /// <summary>
    /// Checks if resource has state. This is only the case when updating or deleting the resource.
    /// </summary>
    /// <typeparam name="Schema"></typeparam>
    /// <param name="context"></param>
    /// <param name="stateContext"></param>
    /// <returns>
    /// <see langword="true"/> if <see cref="IStateContext{Schema}.State"/> is not <see langword="null"/>.
    /// </returns>
    public static bool HasState<Schema>(this IStateContext<Schema> context, [MaybeNullWhen(false)] out IStateContext<Schema> stateContext)
    {
        var result = context.State != null;
        stateContext = result ? context : null;
        return result;
    }
}
