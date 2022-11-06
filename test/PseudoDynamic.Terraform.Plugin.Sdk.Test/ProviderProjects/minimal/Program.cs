using Microsoft.AspNetCore.Hosting;
using Moq;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Sdk;

string providerName = "pseudo-dynamic/debug";

Mock<IResource<Schema>> resource = new();
resource.SetupGet(x => x.Name).Returns("empty");

IWebHost webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5()
        .UseProvider(providerName, provider => provider.AddResource<IResource<Schema>, Schema>(resource.Object)))
    .Build();

await webHost.RunAsync();

[Block]
public class Schema { }