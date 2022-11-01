using Microsoft.AspNetCore.Hosting;
using Moq;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Sdk;

var providerName = "pseudo-dynamic/debug";

var resource = new Mock<IResource<Schema>>();
resource.SetupGet(x => x.Name).Returns("empty");

var webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5()
        .UseProvider(providerName, provider => provider.AddResource<IResource<Schema>, Schema>(resource.Object)))
    .Build();

await webHost.RunAsync();

[Block]
public class Schema { }