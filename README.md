# Lykke.Bil2.Sdk
SDK for Lykke blockchains integration contract v2 SDK and clients

# Nuget packing

Bil2 SDK consists of many assemblies and is delivered as multiple nugets. It was decided to deliver only those nugets which are needed for applications, 
thus there is a necessity to pack multiple assemblies into one nuget. At the moment (March 2019) there is no straightforward way to do this in dotnet.
So workaround is used:

1.

Every csproj that is used as nuget packing root should containt:

```xml
  <PropertyGroup>
    <TargetsForTfmSpecificBuildOutput>
      $(TargetsForTfmSpecificBuildOutput);PackReferencedProjectOutputs
    </TargetsForTfmSpecificBuildOutput>
  </PropertyGroup>

  <Target Name="PackReferencedProjectOutputs" DependsOnTargets="BuildOnlySettings;ResolveReferences">
    <ItemGroup>
      <BuildOutputInPackage Include="@(ReferenceCopyLocalPaths-&gt;WithMetadataValue('ReferenceSourceTarget', 'ProjectReference'))" />
    </ItemGroup>
  </Target>
```

to include output of referenced projects into the package.

2.

Packing root csproj should reference another project as private assets:

```xlm
<ProjectReference Include="..\Lykke.Bil2.Contract\Lykke.Bil2.Contract.csproj" PrivateAssets="All" />
```

3.

Packing root csproj should reference all transitive dependencies explicitly:

```xlm
  <ItemGroup Label="RabbitMq dependencies">
    <PackageReference Include="JetBrains.Annotations" Version="2018.3.0" />
    <PackageReference Include="Lykke.Common" Version="7.3.1" />
    <PackageReference Include="RabbitMQ.Client" Version="5.1.0" />
  </ItemGroup>
  
  <ItemGroup Label="Contract dependencies">
    <PackageReference Include="JetBrains.Annotations" Version="2018.3.0" />
    <PackageReference Include="Newtonsoft.Json" Version="12.0.1" />
    <PackageReference Include="Multiformats.Base" Version="2.0.1" />
    <PackageReference Include="System.Memory" Version="4.5.2" />
    <PackageReference Include="Lykke.Numerics" Version="1.0.0" />
  </ItemGroup>

  <ItemGroup Label="WebClient dependencies">
    <PackageReference Include="Lykke.Common" Version="7.3.1" />
    <PackageReference Include="Lykke.HttpClientGenerator" Version="2.4.1" />
  </ItemGroup>
```

Thus keep in mind:

1. If you added new dependency to the existing not packing root project, you should add this dependency to corresponding packing root projects too.
2. If you added new project as dependency to the packing root project, then you should add ```PrivateAssets="All"``` for this reference.
3. If you need to update version of a dependency, do it for entire solution.

## Packing root projects

* Lykke.Bil2.Client.BlocksReader
* Lykke.Bil2.Client.SignService
* Lykke.Bil2.Client.TransactionsExecutor
* Lykke.Bil2.Sdk.BlocksReader
* Lykke.Bil2.Sdk.SignService
* Lykke.Bil2.Sdk.TransactionsExecutor