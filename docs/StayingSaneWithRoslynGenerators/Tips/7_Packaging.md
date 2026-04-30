# Packaging

Your generator will be part of your library package, or a package on it's own. DragonFruit2 project file, with some parts omitted for brevity is:


```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
        <!-- I want the underlying library to reach the broadest audience -->
		<TargetFramework>netstandard2.0</TargetFramework>
		<LangVersion>14.0</LangVersion>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<IsPackable>true</IsPackable>
		<PackageId>DragonFruit2</PackageId>
		<Version>0.1.0-beta-0012</Version>
		<Authors>Kathleen Dollard</Authors>
		<Description>Source generator and analyzers for DragonFruit2 including required runtime dependencies.</Description>
		<RepositoryUrl>https://github.com/your/repo</RepositoryUrl>

	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="System.CommandLine" Version="2.0.2" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\DragonFruit2.Common\DragonFruit2.Common.csproj" />
		<ProjectReference Include="..\DragonFruit2.Generators\DragonFruit2.Generators.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false"  >
			<PrivateAssets>all</PrivateAssets>
		</ProjectReference>
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\DragonFruit2.Polyfills\DragonFruit2.Polyfills.csproj" PrivateAssets="All" IncludeAssets="runtime" />
	</ItemGroup>

	<ItemGroup>
		<None Include="$(OutputPath)\DragonFruit2.Generators.dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
	</ItemGroup>
</Project>
```
