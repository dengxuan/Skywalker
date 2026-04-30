[CmdletBinding()]
param(
    [ValidateSet('x64', 'x86', 'arm64')]
    [string]$Arch = 'x64',

    [ValidateSet('x64', 'x86', 'arm64')]
    [string]$HostArch = 'x64'
)

$ErrorActionPreference = 'Stop'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    throw 'Visual Studio Installer vswhere.exe was not found. Install Visual Studio or Build Tools first.'
}

$installationPath = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if ([string]::IsNullOrWhiteSpace($installationPath)) {
    throw 'Visual Studio C++ x64/x86 build tools were not found. Install the Desktop development with C++ workload.'
}

$devShell = Join-Path $installationPath 'Common7\Tools\Launch-VsDevShell.ps1'
if (-not (Test-Path $devShell)) {
    throw "Visual Studio developer shell script was not found: $devShell"
}

& $devShell -Arch $Arch -HostArch $HostArch -SkipAutomaticLocation

$repositoryRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repositoryRoot

Write-Host "Skywalker developer shell ready ($Arch on $HostArch): $installationPath"