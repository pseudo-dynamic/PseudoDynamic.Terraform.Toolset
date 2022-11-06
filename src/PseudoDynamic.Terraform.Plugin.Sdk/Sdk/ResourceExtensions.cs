using static PseudoDynamic.Terraform.Plugin.Sdk.Resource;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains extension methods for contexts of <see cref="IResource{Schema, ProviderMetaSchema}"/>.
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// Checks if resource has plan. This is only the case when creating or updating the resource.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <param name="context"></param>
        /// <param name="planContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.Plan"/> is not <see langword="null"/>.
        /// </returns>
        public static bool HasPlan<Schema>(this IPlanContext<Schema> context, [MaybeNullWhen(false)] out IPlanContext<Schema> planContext)
        {
            var result = context.Plan != null;
            planContext = result ? context : null;
            return result;
        }

        /// <summary>
        /// Checks if resource is about to change. This is only the case when creating or updating the resource.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="context"></param>
        /// <param name="changeContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.State"/> is <see langword="null"/>.
        /// </returns>
        public static bool IsChanging<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> context, [MaybeNullWhen(false)] out IChangeContext<Schema, ProviderMetaSchema> changeContext)
        {
            var result = context.State == null;
            changeContext = result ? context : null;
            return result;
        }

        /// <summary>
        /// Checks if resource is about to be created.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="context"></param>
        /// <param name="createContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.State"/> is <see langword="null"/>.
        /// </returns>
        public static bool IsCreating<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> context, [MaybeNullWhen(false)] out ICreateContext<Schema, ProviderMetaSchema> createContext)
        {
            var result = context.State == null;
            createContext = result ? context : null;
            return result;
        }

        /// <summary>
        /// Checks if resource is about to be updated.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="context"></param>
        /// <param name="updateContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.State"/> and <see cref="IPlanContext{Schema, ProviderMetaSchema}.Plan"/> is not <see langword="null"/>.
        /// </returns>
        public static bool IsUpdating<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> context, [MaybeNullWhen(false)] out IUpdateContext<Schema, ProviderMetaSchema> updateContext)
        {
            var result = context.State != null && context.Plan != null;
            updateContext = result ? context : null;
            return result;
        }

        /// <summary>
        /// Checks if resource is about to be deleted.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="context"></param>
        /// <param name="deleteContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.Plan"/> is <see langword="null"/>.
        /// </returns>
        public static bool IsDeleting<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> context, [MaybeNullWhen(false)] out IDeleteContext<Schema, ProviderMetaSchema> deleteContext)
        {
            var result = context.Plan == null;
            deleteContext = result ? context : null;
            return result;
        }
    }
}
