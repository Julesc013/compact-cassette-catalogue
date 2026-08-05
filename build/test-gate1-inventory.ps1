[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ('c3-gate1-inventory-' + [Guid]::NewGuid().ToString('N'))
$artifactRoot = Join-Path (Split-Path -Parent $PSScriptRoot) 'artifacts\tests\gate1-inventory'
New-Item -ItemType Directory -Path $temporaryRoot -Force | Out-Null
try {
    [IO.File]::WriteAllText((Join-Path $temporaryRoot 'catalogue.xml'), '<Catalogue><Information /></Catalogue>', (New-Object Text.UTF8Encoding($false)))
    [IO.File]::WriteAllText((Join-Path $temporaryRoot 'other.xml'), '<Other />', (New-Object Text.UTF8Encoding($false)))
    [IO.File]::WriteAllText((Join-Path $temporaryRoot 'external.xml'), '<!DOCTYPE Catalogue SYSTEM "file:///does-not-exist"><Catalogue />', (New-Object Text.UTF8Encoding($false)))
    [IO.File]::WriteAllText((Join-Path $temporaryRoot 'machine.vhdx'), 'synthetic', (New-Object Text.UTF8Encoding($false)))
    [IO.File]::WriteAllText((Join-Path $temporaryRoot 'target.rdp'), 'full address:s:test.invalid', (New-Object Text.UTF8Encoding($false)))

    $catalogueOutput = Join-Path $artifactRoot 'catalogues.json'
    $catalogue = & (Join-Path $PSScriptRoot 'discover-private-catalogues.ps1') -OutputPath $catalogueOutput -SearchRoots $temporaryRoot
    if ([int]$catalogue.uniqueCatalogueCount -ne 1 -or [string]$catalogue.catalogues[0].scenarioId -cne 'PRIVATE-CATALOGUE-001') {
        throw 'Private-catalogue discovery did not close the root-element and scenario-ID contract.'
    }
    $environmentOutput = Join-Path $artifactRoot 'environment.json'
    $environment = & (Join-Path $PSScriptRoot 'inventory-gate1-environments.ps1') -OutputPath $environmentOutput `
        -FileSearchRoots $temporaryRoot -MaximumDirectoriesPerRoot 10 -MaximumMatchesPerRoot 10 -SkipPlatformQueries
    if ([int]$environment.files.imageOrMediaCount -ne 1 -or [int]$environment.files.rdpFileCount -ne 1 -or
            [bool]$environment.boundedPolicy.networkRangeScanPerformed -or [bool]$environment.boundedPolicy.mediaDownloaded) {
        throw 'Environment inventory did not preserve its bounded, read-only contract.'
    }
    Write-Host 'Gate 1 inventory controls passed bounded environment and secure private-catalogue discovery tests.'
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
    if (Test-Path -LiteralPath $artifactRoot) { Remove-Item -LiteralPath $artifactRoot -Recurse -Force }
}
