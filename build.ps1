[CmdletBinding()]
Param(
	[string]$Script = "./build.cake",
	[string]$Target,
	[string]$Configuration,
	[ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
	[string]$Verbosity,
	[switch]$ShowDescription,
	[Alias("WhatIf", "Noop")]
	[switch]$DryRun,
	[switch]$SkipToolPackageRestore,
	[Parameter(Position = 0, Mandatory = $false, ValueFromRemainingArguments = $true)]
	[string[]]$ScriptArgs
)

Write-Host "Preparing to run build script..."

if (!$PSScriptRoot) {
	$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

# Restore all Dotnet tools
Write-Host "Restoring all dotnet tools..."
dotnet tool restore

Write-Host "Bootstrapping Cake..."
Invoke-Expression "dotnet cake $Script --bootstrap"

# Build Cake arguments
$cakeArguments = @("$Script");
if ($Target) { $cakeArguments += "-target=$Target" }
if ($Configuration) { $cakeArguments += "-configuration=$Configuration" }
if ($Verbosity) { $cakeArguments += "-verbosity=$Verbosity" }
if ($ShowDescription) { $cakeArguments += "-showdescription" }
if ($DryRun) { $cakeArguments += "-dryrun" }
$cakeArguments += $ScriptArgs

# Run Cake
Write-Host "Running build script..."
Invoke-Expression "dotnet cake $cakeArguments"
