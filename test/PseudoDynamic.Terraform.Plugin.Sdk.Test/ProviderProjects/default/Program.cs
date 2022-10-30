﻿using DotNext.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Sdk;
using static Assertions;

var providerName = "pseudo-dynamic/debug";

var webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5()
        .ConfigureProvider(providerName, provider =>
        {
            provider.SetProvider<ProviderImpl>();
            provider.AddResource<ResourceImpl>();
            provider.AddDataSource<DataSourceImpl>();
        }))
    .Build();

await webHost.RunAsync();

[Object]
class Object
{
    public string String { get; set; }
    public int Number { get; set; }
    public bool Bool { get; set; }

    [Object]
    public class InlcudingRanges : Object
    {
        public IList<Object> List { get; set; }
        public ISet<Object> Set { get; set; }
        public IDictionary<string, string> Map { get; set; }

        public class IncludingNestedBlocks : InlcudingRanges
        {
            [NestedBlock]
            public InlcudingRanges SingleNested { get; set; }

            [NestedBlock]
            public IList<InlcudingRanges> ListNested { get; set; }

            [NestedBlock]
            public ISet<InlcudingRanges> SetNested { get; set; }

            [NestedBlock]
            public IDictionary<string, InlcudingRanges> MapNested { get; set; }
        }
    }
}

[Block]
class ProviderSchema : Object.InlcudingRanges.IncludingNestedBlocks
{
}

class ProviderImpl : Provider<ProviderSchema>
{
    public override Task Configure(Provider.ConfigureContext<ProviderSchema> context)
    {
        context.Reports.Warning("[provider] configure");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context;
    }
}

[Block]
class ResourceSchema : Object.InlcudingRanges.IncludingNestedBlocks
{
}

class ResourceImpl : Resource<ResourceSchema>
{
    public override string TypeName => "empty";

    public override Task ValidateConfig(Resource.ValidateContext<ResourceSchema> context)
    {
        context.Reports.Warning("[resource] validate");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context;
    }

    public override Task Plan(Resource.PlanContext<ResourceSchema> context)
    {
        context.Reports.Warning("[resource] plan");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        AssertObjectWithRangesAndNestedBlocks(context.Planned);
        return context;
    }
}

[Block]
class DataSourceSchema : Object.InlcudingRanges.IncludingNestedBlocks
{
}

class DataSourceImpl : DataSource<DataSourceSchema>
{
    public override string TypeName => "empty";

    public override Task ValidateConfig(DataSource.ValidateContext<DataSourceSchema> context)
    {
        context.Reports.Warning("[data source] validate");
        AssertObjectWithRangesAndNestedBlocks(context.Config);
        return context;
    }

    public override Task Read(DataSource.ReadContext<DataSourceSchema> context)
    {
        context.Reports.Warning("[data source] read");
        AssertObjectWithRangesAndNestedBlocks(context.Computed);
        return context;
    }
}

static class Assertions
{
    public static void AssertObject(Object @object)
    {
        @object.String.Should().Be("string");
        @object.Number.Should().Be(1);
        @object.Bool.Should().Be(true);
    }

    public static void AssertObjectIncludingRanges(Object.InlcudingRanges @object)
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

    public static void AssertObjectWithRangesAndNestedBlocks(Object.InlcudingRanges.IncludingNestedBlocks schema)
    {
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