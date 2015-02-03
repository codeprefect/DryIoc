@echo off

set SLN="..\DryIoc.sln"
set OUTDIR="..\bin\Release"

echo:
echo:Building %SLN% into %OUTDIR% . . .

rem MSBuild 32-bit operating systems:
rem HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\12.0

for /f "tokens=2*" %%S in ('reg query HKLM\SOFTWARE\Wow6432Node\Microsoft\MSBuild\ToolsVersions\12.0 /v MSBuildToolsPath') do (

	if exist "%%T" (

		echo:
		echo:Using MSBuild from "%%T"

		"%%T\MSBuild.exe" %SLN% /t:Rebuild /m /fl /flp:Summary;Verbosity=minimal;LogFile=MSBuild.log ^
   			/p:OutDir=%OUTDIR% ^
   			/p:GenerateProjectSpecificOutputFolder=false ^
   			/p:Configuration=Release ^ 
   			/p:RestorePackages=false ^ 
			/p:BuildInParallel=true 
    )
)

if not "%1"=="-nopause" pause