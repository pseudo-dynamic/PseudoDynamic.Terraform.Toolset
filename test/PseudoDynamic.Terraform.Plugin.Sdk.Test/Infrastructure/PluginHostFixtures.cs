﻿using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    public static class PluginHostFixtures
    {
        public class ProtocolV5 : PluginHostFixtureBase
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;
        }

        public class ProtocolV6 : PluginHostFixtureBase
        {
            public override PluginProtocol Protocol => PluginProtocol.V6;
        }
    }
}
