﻿using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class Provider
    {
        internal static readonly GenericTypeAccessor ConfigureContextAccessor = new(typeof(ConfigureContext<>));

        public class ConfigureContext<Schema> : TerraformService.ShapingContext
        {
            public Schema Config { get; }

            internal ConfigureContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder, Schema config)
                : base(reports, dynamicDecoder) =>
                Config = config;
        }
    }
}
