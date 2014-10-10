@echo off
cls

if not exist packages\FAKE\tools\FAKE.exe  (
  .nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
  .nuget\nuget.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion
)
if not exist packages\GroupProgress  (
  .nuget\nuget.exe install GroupProgress -OutputDirectory packages -ExcludeVersion
)
if not exist packages\DotLiquid*  (
  .nuget\nuget.exe install DotLiquid -OutputDirectory packages
)

if not exist node_modules  (
  call npm install
)

packages\FAKE\tools\FAKE.exe build.fsx %*
pause
