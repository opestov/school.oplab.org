#!/bin/bash

mono .nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion
mono .nuget/NuGet.exe install FSharp.Formatting.CommandTool -OutputDirectory packages -ExcludeVersion 
mono .nuget/NuGet.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion 

mono .nuget/NuGet.exe install GroupProgress -OutputDirectory packages -ExcludeVersion 
mono .nuget/NuGet.exe install DotLiquid -OutputDirectory packages

npm install

mono --runtime=v4.0 packages/FAKE/tools/FAKE.exe build.fsx $@
