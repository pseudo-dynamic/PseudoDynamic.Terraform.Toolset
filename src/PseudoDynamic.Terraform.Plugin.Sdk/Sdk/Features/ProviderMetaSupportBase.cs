using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Features
{
    public abstract class ProviderMetaSupportBase<ProviderMetaSchema>
    {
        internal static readonly Type ProviderMetaSchemaType = typeof(ProviderMetaSchema);

        internal abstract IProviderFeature ProviderFeature { get; }

        internal ProviderMetaSupportBase()
        {
        }

        #region Resource

        public void AddResource<Resource, Schema>()
            where Resource : class, IResource<Schema, object>
            where Schema : class =>
            ProviderFeature.AddResource(typeof(Resource), typeof(Schema), ProviderMetaSchemaType);

        public void AddResource<Resource, Schema>(Resource resource)
            where Resource : class, IResource<Schema, object>
            where Schema : class =>
            ProviderFeature.AddResource(typeof(Resource), typeof(Schema), ProviderMetaSchemaType, resource);

        public void AddResource<Resource>(Resource? resource)
            where Resource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IResource<ProviderMetaSchema>
        {
            Type resourceType = typeof(Resource);
            Type schematype = DesignTimeTerraformService.GetSchemaType(resourceType);
            ProviderFeature.AddResource(resourceType, schematype, ProviderMetaSchemaType, resource);
        }

        public void AddResource<Resource>()
            where Resource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IResource<ProviderMetaSchema> =>
            AddResource(default(Resource));

        #endregion

        #region DataSource

        public void AddDataSource<DataSource, Schema>()
            where DataSource : class, IDataSource<Schema, object>
            where Schema : class =>
            ProviderFeature.AddDataSource(typeof(DataSource), typeof(Schema), ProviderMetaSchemaType);

        public void AddDataSource<DataSource, Schema>(DataSource dataSource)
            where DataSource : class, IDataSource<Schema, object>
            where Schema : class =>
            ProviderFeature.AddDataSource(typeof(DataSource), typeof(Schema), ProviderMetaSchemaType, dataSource);

        public void AddDataSource<DataSource>(DataSource? dataSource)
            where DataSource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IDataSource<ProviderMetaSchema>
        {
            Type dataSourceType = typeof(DataSource);
            Type schematype = DesignTimeTerraformService.GetSchemaType(dataSourceType);
            ProviderFeature.AddDataSource(dataSourceType, schematype, ProviderMetaSchemaType, dataSource);
        }

        public void AddDataSource<DataSource>()
            where DataSource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IDataSource<ProviderMetaSchema> =>
            AddDataSource(default(DataSource));

        #endregion
    }
}
