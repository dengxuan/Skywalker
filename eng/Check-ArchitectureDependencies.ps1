#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Enforces Skywalker's architecture dependency invariants across src/ .csproj files.

.DESCRIPTION
  Skywalker is an umbrella framework split into top-level families. The DDD kernel
  (Skywalker.Ddd.*) must stay minimal, and EVERYTHING outside it must be installable
  and usable WITHOUT taking a dependency on Skywalker.Ddd.*. This guard fails the
  build on any ProjectReference that breaks the following invariants:

    INV-1 (standalone)        non-Ddd family must NOT depend on Skywalker.Ddd.*
    INV-2 (kernel minimality) Skywalker.Ddd.* must NOT depend on an enhancement family
    INV-2b (kernel purity)    Skywalker.Ddd.* must depend on EventBus.Abstractions (the
                              port), NOT on a concrete EventBus implementation
    INV-3 (extensions purity) Skywalker.Extensions.* must NOT depend on EventBus.* or an
                              enhancement family

  Edges that are real but already tracked by an issue are listed in $AllowList below
  and reported as warnings instead of failing. Remove each entry when its issue lands.

  Tracking: Epic #300. See docs / issues #293-#297.

.EXAMPLE
  pwsh eng/Check-ArchitectureDependencies.ps1
#>
[CmdletBinding()]
param(
  [string]$SrcRoot = (Join-Path (Join-Path $PSScriptRoot '..') 'src')
)

$ErrorActionPreference = 'Stop'

# Enhancement feature families (each must be independently installable).
$EnhancementPrefixes = @(
  'Caching', 'Permissions', 'Settings', 'Localization', 'Validation',
  'Template', 'Security', 'Sms', 'HealthChecks', 'Messaging', 'Transport'
)

# Temporary allow-list of real-but-tracked violations. Remove an entry when its issue is fixed.
$AllowList = @(
  'Skywalker.Caching.Redis -> Skywalker.Ddd.Abstractions',                        # #293 (ISkywalkerBuilder lives in Ddd.Abstractions)
  'Skywalker.EventBus.RabbitMQ -> Skywalker.Ddd.Abstractions',                    # #293 (ISkywalkerBuilder lives in Ddd.Abstractions)
  'Skywalker.Settings.EntityFrameworkCore -> Skywalker.Ddd.Domain',               # #294
  'Skywalker.Settings.EntityFrameworkCore -> Skywalker.Ddd.EntityFrameworkCore',  # #294
  'Skywalker.Localization.EntityFrameworkCore -> Skywalker.Ddd.EntityFrameworkCore', # #294
  'Skywalker.Ddd.Domain -> Skywalker.EventBus.Local',                             # #295
  'Skywalker.Extensions.Emailing.Template -> Skywalker.Template.Abstractions'     # #296
)

function Get-Family([string]$name) {
  if ($name -eq 'Skywalker.Ddd' -or $name -like 'Skywalker.Ddd.*') { return 'ddd' }
  if ($name -like 'Skywalker.Extensions.*') { return 'extensions' }
  if ($name -like 'Skywalker.EventBus.*') { return 'eventbus' }
  if ($name -like 'Skywalker.Exceptions.*') { return 'exceptions' }  # shared error-contract leaf; allowed dep of ddd
  return 'enhancement'
}

function Test-IsEnhancement([string]$name) {
  foreach ($p in $EnhancementPrefixes) {
    if ($name -eq "Skywalker.$p" -or $name -like "Skywalker.$p.*") { return $true }
  }
  return $false
}

$projects = Get-ChildItem -Path $SrcRoot -Recurse -Filter *.csproj |
  Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }

$violations = New-Object System.Collections.Generic.List[string]
$tracked = New-Object System.Collections.Generic.List[string]

foreach ($proj in $projects) {
  $name = [System.IO.Path]::GetFileNameWithoutExtension($proj.Name)
  $fromFamily = Get-Family $name

  $doc = New-Object System.Xml.XmlDocument
  $doc.Load($proj.FullName)
  $includes = $doc.SelectNodes('//ProjectReference/@Include') | ForEach-Object { $_.Value }

  foreach ($inc in $includes) {
    if (-not $inc) { continue }
    $leaf = ($inc -replace '\\', '/').Split('/')[-1]
    $refName = [System.IO.Path]::GetFileNameWithoutExtension($leaf)
    $toFamily = Get-Family $refName
    $edge = "$name -> $refName"

    $reason = $null
    if ($fromFamily -ne 'ddd' -and $fromFamily -ne 'exceptions' -and $toFamily -eq 'ddd') {
      $reason = "INV-1 standalone: non-Ddd '$name' depends on Ddd '$refName'"
    }
    elseif ($fromFamily -eq 'ddd' -and (Test-IsEnhancement $refName)) {
      $reason = "INV-2 kernel-minimality: Ddd '$name' depends on enhancement '$refName'"
    }
    elseif ($fromFamily -eq 'ddd' -and $toFamily -eq 'eventbus' -and $refName -ne 'Skywalker.EventBus.Abstractions') {
      $reason = "INV-2b kernel-purity: Ddd '$name' depends on concrete EventBus impl '$refName' (use Skywalker.EventBus.Abstractions)"
    }
    elseif ($fromFamily -eq 'extensions' -and ($toFamily -eq 'eventbus' -or (Test-IsEnhancement $refName))) {
      $reason = "INV-3 extensions-purity: Extensions '$name' depends on '$refName'"
    }

    if ($reason) {
      if ($AllowList -contains $edge) {
        $tracked.Add($reason)
        Write-Host "::warning::[tracked-debt] $reason"
      }
      else {
        $violations.Add($reason)
      }
    }
  }
}

Write-Host ""
Write-Host "Scanned $($projects.Count) src projects. Tracked-debt edges (allow-listed): $($tracked.Count)."

if ($violations.Count -gt 0) {
  Write-Host ""
  Write-Host "Architecture dependency invariant VIOLATIONS ($($violations.Count)):" -ForegroundColor Red
  foreach ($v in $violations) { Write-Host "  - $v" -ForegroundColor Red }
  Write-Host ""
  Write-Host "These ProjectReferences break the standalone/kernel invariants (Epic #300)."
  Write-Host "Fix the reference, or — if intentional and tracked by an issue — add the"
  Write-Host "'From -> To' edge to the allow-list in eng/Check-ArchitectureDependencies.ps1."
  exit 1
}

Write-Host "Architecture dependency invariants: OK" -ForegroundColor Green
exit 0
