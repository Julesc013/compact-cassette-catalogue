[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$paths = @(
    'Compact Cassette Catalogue Installer\app.manifest',
    'Compact Cassette Catalogue Uninstaller\app.manifest'
)

foreach ($relativePath in $paths) {
    $path = Join-Path $repositoryRoot $relativePath
    [xml]$manifest = Get-Content -LiteralPath $path -Raw
    $levels = @($manifest.SelectNodes("//*[local-name()='requestedExecutionLevel']"))
    if ($levels.Count -ne 1) {
        throw "Setup manifest '$relativePath' must contain exactly one requestedExecutionLevel element."
    }
    $level = $levels[0]
    if ($level.Attributes.Count -ne 2 -or
            [string]$level.GetAttribute('level') -cne 'requireAdministrator' -or
            [string]$level.GetAttribute('uiAccess') -cne 'false') {
        throw "Setup manifest '$relativePath' must request requireAdministrator with uiAccess disabled and no additional attributes."
    }
}

Write-Host 'Installer and uninstaller manifests require an administrator token with UI access disabled.'
