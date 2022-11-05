using static PseudoDynamic.Terraform.Plugin.Sdk.Resource;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ResourceExtensions
    {
        /// <summary>
        /// Checks if resource is about to be created.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="planContext"></param>
        /// <param name="createContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.State"/> is <see langword="null"/>.
        /// </returns>
        public static bool IsCreating<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> planContext, out ICreateContext<Schema, ProviderMetaSchema> createContext)
        {
            createContext = planContext;
            return planContext.State == null;
        }

        /// <summary>
        /// Checks if resource is about to be updated.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="planContext"></param>
        /// <param name="updateContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.State"/> and <see cref="IPlanContext{Schema, ProviderMetaSchema}.Plan"/> is not <see langword="null"/>.
        /// </returns>
        public static bool IsUpdating<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> planContext, out IUpdateContext<Schema, ProviderMetaSchema> updateContext)
        {
            updateContext = planContext;
            return planContext.State != null && planContext.Plan != null;
        }

        /// <summary>
        /// Checks if resource is about to be deleted.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <typeparam name="ProviderMetaSchema"></typeparam>
        /// <param name="planContext"></param>
        /// <param name="deleteContext"></param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IPlanContext{Schema, ProviderMetaSchema}.Plan"/> is <see langword="null"/>.
        /// </returns>
        public static bool IsDeleting<Schema, ProviderMetaSchema>(this IPlanContext<Schema, ProviderMetaSchema> planContext, out IDeleteContext<Schema, ProviderMetaSchema> deleteContext)
        {
            deleteContext = planContext;
            return planContext.Plan == null;
        }
    }
}
