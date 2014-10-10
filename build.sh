#!/bin/bash

mono tools/NuGet/nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
mono tools/NuGet/nuget.exe install FSharp.Formatting.CommandTool -OutputDirectory packages -ExcludeVersion 
mono tools/NuGet/nuget.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion 
mono --runtime=v4.0 packages/FAKE/tools/FAKE.exe build.fsx $@
