namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ValidateConfig
    {
        internal static readonly GenericTypeAccessor ContextAccessor = new(typeof(Context<>));

        public class Context<Schema>
        {
            public Schema Config { get; }
            public Reports Reports { get; }

            internal Context(Schema config, Reports reports)
            {
                Config = config;
                Reports = reports;
            }
        }
    }
}
