﻿namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProvider
    {
        string FullyQualifiedProviderName { get; }
        string ProviderName { get; }
    }
}
