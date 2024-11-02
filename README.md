# PackageReference Version to Attribute

This Visual Studio extension and dotnet tool converts PackageReference Version child elements to attributes across your projects.

It works with C# csproj Visual Studio project files.

## Getting started with the Visual Studio Extension

1. [Download and install the extension from the Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=RamiAbughazaleh.PackageReferenceVersionToAttributeExtension).  
2. Right-click on a project and select `Convert PackageReference Version elements to attributes...`.  

   ![Preview](Preview.png)

3. Wait until the process finishes.  
   Check the status bar or the `PackageReferences Version to Attribute Extension` pane in the `Output Window` for details.

## Getting started with the dotnet tool

1. Install the dotnet tool using the following command:  
   ```bash
   dotnet tool install PackageReferenceVersionToAttribute.Tool

2. To convert all csproj files in the current directory and its subdirectories, run the tool with the command:  
   ```bash
   PackageReferenceVersionToAttribute.Tool .\**\*.csproj

3. For additional options and usage, you can run:
   ```bash
   PackageReferenceVersionToAttribute.Tool --help


## Technical details

The extension will first create a backup of the project file.  
For example, `MyProject.csproj` will be copied to `MyProject.csproj.bak`.  

If the project file is source controlled, it will be checked out for modification.  

### General

The `Version` child elements of `PackageReference` will be converted to a `Version` attribute.  
For example, the following elements in the `csproj` project file:
```
  <PackageReference Include="Newtonsoft.Json">
    <Version>13.0.3</Version>
  </PackageReference>
```

will be converted to this:
```
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

## Rate and Review

Has this extension helped you at all?

If so, please rate and share it.

Thank you! :)

# Change Log

## v1.0.1102.19 (November 2<sup>nd</sup>, 2024)
- Fixed [#2](https://github.com/icnocop/PackageReferenceVersionToAttribute/issues/2) - Added support for converting all projects in a solution, and converting multiple selected projects.
- Fixed [#1](https://github.com/icnocop/PackageReferenceVersionToAttribute/issues/1) - Preserved the XML declaration if it exists.
- Fixed [#4](https://github.com/icnocop/PackageReferenceVersionToAttribute/issues/4) - Added support for installating on Visual Studio Arm64.
- Fixed [#3](https://github.com/icnocop/PackageReferenceVersionToAttribute/issues/3) - Added dotnet tool command line utility.

## v1.0.1101.15 (November 1<sup>st</sup>, 2024)
- Fixed [#1](https://github.com/icnocop/PackageReferenceVersionToAttribute/issues/1) - Fixed issue when converting a project with an XML namespace.

## v1.0.1026.3 (October 26<sup>th</sup>, 2024)
- First release
