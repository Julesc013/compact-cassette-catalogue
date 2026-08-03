[CmdletBinding()]
param(
    [object]$Identity,
    [string]$RepositoryRoot
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = Split-Path -Parent $PSScriptRoot
}

if ($null -eq $Identity) {
    $Identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
}

$resolvedRoot = [IO.Path]::GetFullPath($RepositoryRoot)
$releaseChannel = [string]$Identity.ReleaseChannel
if ($releaseChannel -ceq 'alpha') {
    $manifestDirectory = Join-Path $resolvedRoot 'release\feeds\alpha'
}
elseif ($releaseChannel -ceq 'beta' -or $releaseChannel -ceq 'stable') {
    $releaseLabel = [string]$Identity.ReleaseLabel
    $expectedLabelPattern = if ($releaseChannel -ceq 'beta') {
        '^(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)-(?:beta|rc)\.[1-9][0-9]*$'
    }
    else {
        '^(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)$'
    }
    if ($releaseLabel -cnotmatch $expectedLabelPattern) {
        throw "Release label '$releaseLabel' is invalid for the '$releaseChannel' update-manifest channel."
    }
    $manifestDirectory = Join-Path $resolvedRoot (
        'release\candidates\' + $releaseLabel)
}
else {
    throw "Unsupported update-manifest channel: '$releaseChannel'"
}

[IO.Path]::GetFullPath((Join-Path $manifestDirectory 'release.json'))
