#!/bin/bash

mono .nuget/nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
mono .nuget/nuget.exe install FSharp.Formatting.CommandTool -OutputDirectory packages -ExcludeVersion 
mono .nuget/nuget.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion 

mono .nuget/nuget.exe install GroupProgress -OutputDirectory packages -ExcludeVersion 
mono .nuget/nuget.exe install DotLiquid -OutputDirectory packages

npm install

mono --runtime=v4.0 packages/FAKE/tools/FAKE.exe build.fsx $@
