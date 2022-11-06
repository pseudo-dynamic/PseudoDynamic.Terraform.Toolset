using DotNext.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Sdk;
using static Assertions;

var providerName = "registry.terraform.io/pseudo-dynamic/debug";

var webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5()
        .UseProvider<ProviderMetaSchema>(providerName, provider => {
            provider.SetProvider<ProviderImpl>();
            provider.AddResource<ResourceImpl>();
            provider.AddDataSource<DataSourceImpl>();
        }))
    .Build();

await webHost.RunAsync();

[Object]
internal class Object
{
    public string String { get; set; } = null!;
    public int Number { get; set; }
    public bool Bool { get; set; }

    [Object]
    public class WithRanges : Object
    {
        public IList<Object> List { get; set; } = null!;
        public ISet<Object> Set { get; set; } = null!;
        public IDictionary<string, string> Map { get; set; } = null!;

        public class WithNestedBlocks : WithRanges
        {
            [NestedBlock]
            public WithRanges SingleNested { get; set; } = null!;

            [NestedBlock]
            public IList<WithRanges> ListNested { get; set; } = null!;

            [NestedBlock]
            public ISet<WithRanges> SetNested { get; set; } = null!;

            [NestedBlock]
            public IDictionary<string, WithRanges> MapNested { get; set; } = null!;
        }
    }
}

[Block]
internal class ProviderSchema : Object.WithRanges.WithNestedBlocks
{
}

internal class ProviderImpl : Provider<ProviderSchema>
{
    public override Task ValidateConfig(IValidateConfigContext<ProviderSchema> context)
    {
        context.Reports.Warning("[provider] validate config");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context.CompletedTask;
    }

    public override Task Configure(IConfigureContext<ProviderSchema> context)
    {
        context.Reports.Warning("[provider] configure");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context.CompletedTask;
    }
}

[Block]
internal class ProviderMetaSchema : Object
{
}

[Block]
internal class ResourceSchema : Object.WithRanges.WithNestedBlocks
{
}

internal class ResourceImpl : Resource<ResourceSchema, ProviderMetaSchema>
{
    public override string TypeName => "empty";

    public override Task ValidateConfig(IValidateConfigContext<ResourceSchema> context)
    {
        context.Reports.Warning("[resource] validate");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context.CompletedTask;
    }

    public override Task Plan(IPlanContext<ResourceSchema, ProviderMetaSchema> context)
    {
        context.Reports.Warning("[resource] plan");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        AssertObjectWithRangesAndNestedBlocks(context.Plan);
        return context.CompletedTask;
    }

    public override Task Apply(IApplyContext<ResourceSchema, ProviderMetaSchema> context)
    {
        context.Reports.Warning("[resource] plan");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        AssertObjectWithRangesAndNestedBlocks(context.Plan);
        return context.CompletedTask;
    }
}

[Block]
internal class DataSourceSchema : Object.WithRanges.WithNestedBlocks
{
}

internal class DataSourceImpl : DataSource<DataSourceSchema, ProviderMetaSchema>
{
    public override string TypeName => "empty";

    public override Task ValidateConfig(DataSource.IValidateConfigContext<DataSourceSchema> context)
    {
        context.Reports.Warning("[data source] validate");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context.CompletedTask;
    }

    public override Task Read(DataSource.IReadContext<DataSourceSchema, ProviderMetaSchema> context)
    {
        context.Reports.Warning("[data source] read");
        AssertObject(context.ProviderMeta);
        AssertObjectWithRangesAndNestedBlocks(context.State);
        return context.CompletedTask;
    }
}

internal static class Assertions
{
    public static void AssertObject(Object @object)
    {
        @object.String.Should().Be("string");
        @object.Number.Should().Be(1);
        @object.Bool.Should().Be(true);
    }

    public static void AssertObjectIncludingRanges(Object.WithRanges @object)
    {
        AssertObject(@object);

        @object.List.Should().HaveCount(2);
        @object.List.ForEach(AssertObject);

        @object.Set.Should().HaveCount(1);
        @object.Set.ForEach(AssertObject);

        @object.Map.Should().HaveCount(2);
        @object.Map.Should().ContainKeys("one", "two");
        @object.Map.Should().ContainValues("1", "2");
    }

    public static void AssertObjectWithRangesAndNestedBlocks(Object.WithRanges.WithNestedBlocks? schema)
    {
        if (schema is null) {
            return;
        }

        AssertObjectIncludingRanges(schema);
        AssertObjectIncludingRanges(schema.SingleNested);

        schema.ListNested.Should().HaveCount(2);
        schema.ListNested.ForEach(AssertObjectIncludingRanges);

        schema.SetNested.Should().HaveCount(1);
        schema.SetNested.ForEach(AssertObjectIncludingRanges);

        schema.MapNested.Should().HaveCount(2);
        schema.MapNested.Should().ContainKeys("one", "two");
        schema.MapNested.Values.ForEach(AssertObjectIncludingRanges);
    }
}
