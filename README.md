# PseudoDynamic.Terraform.Plugin.Sdk

This library want to take charge and allow you to write a provider in C# with ease. Before I begin, yes, this library has a lot similarities with [terraform-plugin-framework](https://github.com/hashicorp/terraform-plugin-framework)!

This is the main goal that this library should fullfill:

- Write resources and data sources

Please keep in mind, that this library is in the very early development stage and is anything but finished.

By saying this, let me give you a short list of protocol features that are or are not currently implemented:

- Provider
  - :heavy_check_mark: GetProviderSchema
  - :heavy_check_mark: ValidateProviderConfig
  - :heavy_check_mark: ConfigureProvider
- Data Source
  - :heavy_check_mark: ValidateDataResourceConfig
  - :heavy_check_mark: ReadDataSource
- Resources
  - :x: UpgradeResourceState
  - :x: ImportResourceState
  - :heavy_check_mark: ReadResource
  - :heavy_check_mark: ValidateResourceConfig
  - :heavy_check_mark: PlanResourceChange
  - :heavy_check_mark: ApplyResourceChange
- Terraform Types
  - :x: Tuples
  - :heavy_check_mark: [All others](#supported-c-mappings) are supported

Still fine? Then continue with the usage examples.

## Usage

To make use of this library, you need to install the NuGet package.

[![Nuget][nuget-package-badge]][nuget-package]

After you installed the package, you have to decide for a protocol version. At the current time **only protocol v5 is available**. In this library you have access to these by creating a protocol specification:

```csharp
IPluginServerSpecification.NewProtocolV5()
```

You use it to create a plugin server:

```csharp
var webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5())
    .Build();
```

This is a minimal provider configuration with a provider, one resource and one data source:

```csharp
var webHost = new WebHostBuilder()
    .UseTerraformPluginServer(IPluginServerSpecification.NewProtocolV5()
        .UseProvider<ProviderMetaSchema>(providerName, provider =>
        {
            provider.SetProvider<ProviderImpl>();
            provider.AddResource<ResourceImpl>();
            provider.AddDataSource<DataSourceImpl>();
        }))
    .Build();

[Block]
class ProviderMetaSchema {}

[Block]
class ProviderSchema {}

/* If using constructor, the constructor parameters are resolved by service provider. */
class ProviderImpl : Provider<ProviderSchema>
{
    public override Task ValidateConfig(IValidateConfigContext<ProviderSchema> context) => base.ValidateConfig(context);
    public override Task Configure(IConfigureContext<ProviderSchema> context) => base.Configure(context);
}

[Block]
class ResourceSchema
{
}

/* If using constructor, the constructor parameters are resolved by service provider. */
internal class ResourceImpl : Resource<ResourceSchema, ProviderMetaSchema>
{
    public override string TypeName => "sum_a_b";
    // MigrateState and ImportState are omitted because not yet functional
    public override Task ReviseState(IReviseStateContext<ResourceSchema, ProviderMetaSchema> context) => base.ReviseState(context);
    public override Task ValidateConfig(IValidateConfigContext<ResourceSchema> context) => base.ValidateConfig(context);
    public override Task Apply(IApplyContext<ResourceSchema, ProviderMetaSchema> context) => base.Apply(context);
    public override Task Plan(IPlanContext<ResourceSchema, ProviderMetaSchema> context) => base.Plan(context);
}

[Block]
internal class DataSourceSchema : Object.WithRanges.WithNestedBlocks {}

/* If using constructor, the constructor parameters are resolved by service provider. */
internal class DataSourceImpl : DataSource<DataSourceSchema, ProviderMetaSchema>
{
    public override string TypeName => "sum_x_y";
    public override Task ValidateConfig(IValidateConfigContext<DataSourceSchema> context) => base.ValidateConfig(context);
    public override Task Read(IReadContext<DataSourceSchema, ProviderMetaSchema> context) => base.Read(context);
}
```

Feel free to take a look at the other overload methods.

### Supported C# Mappings

Property names are taken as attribute names and property names are automatically converted to kebab_case.

> :bulb: If you prefer using a custom attribute name, then use [NameAttribute](#supported-c-attributes).

```csharp

[Object]
class AnObject {
    /* Can contain everything like a block except nested blocks. */
}

[Block]
class AnotherSchema { }

[Block]
class Schema {
    // By using object, you can decode parts of it at runtime.
    // See section for Terraform dynamic.
    public object Dynamic { get; set; }

    // String decoded as UTF8.
    public string String { get; set; }

    /* Here a list of all supported primitives:
     * Byte, SByte, UInt16, Int16, UInt32, Int32, UInt64, Int64, Single, Double
     */
    public int Number { get; set; }
    // For numbers bigger than UInt64.
    public BigInteger BigNumber { get;set;}

    // A boolean.
    public bool Boolean { get; set; }

    // List of any supported type.
    public IList<> List { get; set; }

    // Unique set of any supported type.
    public ISet<> List { get; set; }

    // Map of any supported type. The key must be of type string.
    public IDictionary<string,> List { get; set; }

    public AnObject Object { get; set; }

    // A nested block where only a single block definition is allowed.
    [NestedBlock]
    public AnotherSchema List { get; set; }

    // A list of nested blocks.
    [NestedBlock]
    public IList<AnotherSchema> List { get; set; }

    // A unique set of nested blocks.
    [NestedBlock]
    public ISet<AnotherSchema> List { get; set; }

    // A map of nested blocks. The key must be of type string.
    [NestedBlock]
    public IDictionary<string, AnotherSchema> List { get; set; }
}
```

As you may have seen, only public properties with public getter and public setter are considered as attributes. You can omit the setter if you specify an equivalent as constructor parameter. The names may only differ in upper and lower case. Read more about [schema class constructor](#schema-class-constructor).

## Optional Terraform Attributes

Whether an attribute is treated as optional or not is dependent on the fact whether the property type is nullable or not.

> :exclamation: This is only true if nullability analysis is enabled, otherwise the attributes are optional by default.

> :bulb: To enable nullability analysis, please use `<Nullable>enable</Nullable>` in your .csproj-file or use `#nullable enable` inside the source code, primarily where the schema classes are defined.

> :bulb: If you make your property type non-nullable, but the attribute is still opional, then you can use `OptionalAttribute` to enforce the attribute being optional.

> :exclamation: Currently Nullable<> is **not** supported.

Assuming nullability analysis is enabled, the attribute equivalent of `public AnObject Object { get; set; }` is required and the attribute equivalent of `public AnObject? Object { get; set; }` is optional.

> :exclamation: Because the nullability analysis feature should be always enabled, I do not support `RequiredAttribute`.

### Supported C# Attributes

- Class Attribtues
  - TupleAttribute (no function yet)
  - ObjectAttribute => make class equivalent to Terraform object type
  - BlockAttribute => make class equivalent to Terraform block type
- Class Constructor Attributes
  - BlockConstructorAttribute => prefer a constructor in case of two
- Propertiy Attributes
  - AttributeIgnoreAttribute => do not treat property as attribute
  - ComputedAttribute => mark attribute as computed
  - DeprecatedAttribute => mark attribute as deprecated
  - DescriptionKindAttribute => description kind of XML comment
  - NameAttribute => use custom name
  - NestedBlockAttribute => mark attribute as nested block
  - OptionalAttribute => mark attribute as optional
    Why is there no RequiredAttribute? See [optional Terraform attributes](#optional-terraform-attributes)
  - SensitiveAttribute => mark attribute as sensitive
  - ValueAttribute => overwrite implicit Terraform type determination

### Unknown Terraform Values

What about **unknown values**? Just wrap any type with `ITerraformValue<>`. By doing so, you are able to differentiate between unknown and null values.

> :exclamation: The only types you cannot wrap with `ITerraformValue<>` are nested blocks.

### Dynamic Terraform Values

If you use `System.Object` as type, you can decode it by an instance of `ITerraformDynamicDecoder`, which you can access through any context that provides data. For example:

```csharp
public override Task Plan(IPlanContext<,> context)
{
    if (context.DynamicDecoder.TryDecode... // Use of dynamic decoder
}
```

### Schema Class Constructor

If you use an constructor for objects or blocks, your constructor parameter names must match the properties. During matching, property names and constructor parameter names are compared case insensitive. By using `AttributeIgnoreAttribute` you can ignore certain properties, so they won't be recognized as attributes anymore. Those constructor parameters that are not present as attributes, are requested from the service provider.

## Provider Debugging

Currently there exist two approaches to debug a C# Terraform provider. Both require you to have Visual Studio or a C# debugger aware editor (maybe VSCode?).

1. (preferred) Place a `Debugger.Launch()` inside the provider, then run Terraform and when Terraform uses the provider it may hit the debugger launch. Now depending on the editor, a window may pop-up and asks for creating a debug instance.
2. Start the provider with attached debugger and then run Terraform.

This is possible workflow for the **first approach**:

1. Prepare a file (in the following I use `terraform.tfrc`) with the content:

```
provider_installation {
	dev_overrides {
		"<provider-name>" = "<non-relative-forward-slashed-directory-containing-the-provider>"
	}

    direct {}
}
```

The `<provider-name>`-part must just be in sync with the source of the required_provider block inside the Terraform provider block, that may look like `<organization>/<provider-name>` or `registry.terraform.io/<organization>/<provider-name>`. The directory path must be absolute and could look like `C:/repos/My.Provider/src/My.Provider/bin/Debug/net6.0`. In case of the C# console project `My.Provider` the binary `My.Provider.exe` should be created. You must copy or rename it to `terraform-provider-<provider-name>.exe`. You must ensure to only have `terraform-provider-<provider-name>.exe`, but not for example `terraform-provider-<provider-name>.dll` or `terraform-provider-<provider-name>.pdb` next to it, otherwise Terraform gets confused and throws an error. A simple trick is to let it compile normal, so that you have  `My.Provider.exe`, and then copy or rename it to `terraform-provider-<provider-name>.exe`.

Here a simple MSBuild solution where you need to replace `<provider-name>` with the actual one:

```msbuild
<Target Name="_WriteTerraformConfig" AfterTargets="AfterBuild">
	<PropertyGroup>
		<_RelativeProvider>bin/$(Configuration)/$(TargetFramework)</_RelativeProvider>
		<_Provider>$(MSBuildProjectDirectory)/$(_RelativeProvider)</_Provider>
		<_Provider>$([System.String]::Copy('$(_Provider)').Replace('\','/'))</_Provider>
		<_Lines>
				<![CDATA[
provider_installation {
	dev_overrides {
		"<provider-name>" = "$(_Provider)"
	}

    direct {}
}
]]>
		</_Lines>
	</PropertyGroup>

	<WriteLinesToFile File="terraform.tfrc" Lines="$(_Lines)" Overwrite="true" />
	<Copy SourceFiles="$(_Provider)/$(AssemblyName).exe" DestinationFiles="$(_Provider)/terraform-provider-debug.exe" />
</Target>
```

1. Assuming you navigated to the directory where the `terraform.tfrc` lies, you need now to set a Terraform-specific environment variable: `TF_CLI_CONFIG_FILE=terraform.tfrc`. Because it is very rare that `terraform.tfrc` lies next to the `*.tf` files you must adjust the path or simply use an absolute path.
2. Start Terraform.

The problem with the **second approach** is, that you are forced to overwrite the environment variable every start. Here a possible workflow:

1. Start the provider and make sure using `IPluginServerSpecification.NewProtocolV5().Debuggable().UseProvider(providerName,...)` and using a provider name in form of `registry.terraform.io/<organization>/<provider-name>`.
2. Follow the instructions in console output which asks you to set the required environment variable
3. For now I take Bash and write `TF_REATTACH_PROVIDERS=$'{"registry.terraform.io/<organization>/<provider-name>":{"Protocol":"grpc","ProtocolVersion":5,"Pid":9400,"Test":true,"Addr":{"Network":"tcp","String":"127.0.0.1:52294"}}}'` (Some informations will change slightly on every start)
4. Start Terraform

[nuget-package]: https://www.nuget.org/packages/PseudoDynamic.Terraform.Plugin.Sdk
[nuget-package-badge]: https://img.shields.io/nuget/v/PseudoDynamic.Terraform.Plugin.Sdk

## Publish Provider

You got me. Did I told you that I do this in my free time? Currently there is no automation to publish a Terraform provider. Since now I have no idea what the Terraform provider registry is expecting and what not. Depending on this, it may take a while until a neat publish integration will exist.

An alternative is to use the provider locally by using any of the method, that is described here: https://developer.hashicorp.com/terraform/cli/config/config-file#provider-installation