[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$ContractPath,
    [string]$SchemaPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($ContractPath)) {
    $ContractPath = Join-Path $RepositoryRoot 'build\branches.json'
}
if ([string]::IsNullOrWhiteSpace($SchemaPath)) {
    $SchemaPath = Join-Path $RepositoryRoot `
        'spec\branch-contract\v1\branches.schema.json'
}

& (Join-Path $PSScriptRoot 'validate-json-document.ps1') `
    -SchemaPath $SchemaPath `
    -DocumentPath $ContractPath `
    -MaximumBytes (16 * 1024) | Out-Null

$document = Get-Content -LiteralPath $ContractPath -Raw | ConvertFrom-Json
$names = [ordered]@{
    CurrentQualified = [string]$document.currentGeneration.qualified
    CurrentIntegration = [string]$document.currentGeneration.integration
    LegacyQualified = [string]$document.legacyGeneration.qualified
    LegacyIntegration = [string]$document.legacyGeneration.integration
}

foreach ($role in $names.Keys) {
    $name = [string]$names[$role]
    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        [void](& git check-ref-format --branch $name 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
    if ($exitCode -ne 0) {
        throw "Branch contract role '$role' has invalid Git branch name '$name'."
    }
}

$uniqueNames = @($names.Values | Sort-Object -Unique)
if ($uniqueNames.Count -ne $names.Count) {
    throw 'Every permanent branch role must have a distinct name.'
}
for ($left = 0; $left -lt $uniqueNames.Count; $left++) {
    for ($right = $left + 1; $right -lt $uniqueNames.Count; $right++) {
        $first = [string]$uniqueNames[$left]
        $second = [string]$uniqueNames[$right]
        if ($first.StartsWith($second + '/', [StringComparison]::Ordinal) -or
            $second.StartsWith($first + '/', [StringComparison]::Ordinal)) {
            throw "Permanent branches '$first' and '$second' collide in Git's ref namespace."
        }
    }
}

Write-Output ([PSCustomObject]@{
        SchemaVersion = [int]$document.schemaVersion
        CurrentQualified = [string]$names.CurrentQualified
        CurrentIntegration = [string]$names.CurrentIntegration
        LegacyQualified = [string]$names.LegacyQualified
        LegacyIntegration = [string]$names.LegacyIntegration
    })
