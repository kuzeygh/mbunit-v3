@echo off
rem This successfully builds all binaries until "setup MSI" generation phase, which was not needed for me.
rem It fails when trying to work with some of the packages I excluded from the build process because they were broken.
rem The binaries can be found at .\build\modules\Build Package
.\build /p:SkipInstaller=true /p:SkipSyncProjects=true /p:SkipSources=true /p:SkipSourceServer=true /p:SkipModules="DLR;RSpec;MbUnitTemplatesVS2010;MbUnitTemplatesVS2008;MbUnitTemplatesVS2005;VisualStudio.Common;VisualStudio100;VisualStudio90" ".\packages\Bundle Package.module"