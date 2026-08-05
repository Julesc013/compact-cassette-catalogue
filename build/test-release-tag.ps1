[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$resolver = Join-Path $PSScriptRoot 'resolve-release-tag.ps1'
$passed = 0

function Assert-Tag {
    param(
        [string]$ReleaseLabel,
        [string]$ExpectedTag
    )

    $actual = [string](& $resolver -ReleaseLabel $ReleaseLabel)
    if ($actual -cne $ExpectedTag) {
        throw "Release label '$ReleaseLabel' resolved to '$actual'; expected '$ExpectedTag'."
    }
    $script:passed++
}

Assert-Tag '2.0.0-alpha.5' '2.0.0a5'
Assert-Tag '2.0.0-alpha.12' '2.0.0a12'
Assert-Tag '2.0.0-beta.1' '2.0.0b1'
Assert-Tag '2.0.0-rc.2' '2.0.0rc2'
Assert-Tag '2.0.0' '2.0.0'

$rejected = $false
try {
    & $resolver -ReleaseLabel '2.0.0-alpha.0' | Out-Null
}
catch {
    $rejected = $true
}
if (-not $rejected) {
    throw 'The compact tag resolver accepted an invalid zero prerelease sequence.'
}
$passed++

Write-Host "$passed compact release-tag scenarios passed."
