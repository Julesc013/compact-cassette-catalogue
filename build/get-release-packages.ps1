[CmdletBinding()]
param(
    [object]$Identity,
    [string]$LanesPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($LanesPath)) {
    $LanesPath = Join-Path $PSScriptRoot 'lanes.json'
}

if ($null -eq $Identity) {
    $Identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
}
if (-not (Test-Path -LiteralPath $LanesPath -PathType Leaf)) {
    throw "Build-lane manifest is missing: $LanesPath"
}

$manifest = Get-Content -LiteralPath $LanesPath -Raw | ConvertFrom-Json
if ([string]$manifest.schemaVersion -cne '1') {
    throw "Unsupported build-lane manifest schema: $($manifest.schemaVersion)"
}

$definitions = New-Object Collections.Generic.List[Object]
$laneIds = New-Object Collections.Generic.List[String]
$fileNames = New-Object Collections.Generic.List[String]
foreach ($lane in @($manifest.lanes)) {
    $laneId = [string]$lane.id
    if ($laneId -cnotmatch '^[a-z0-9][a-z0-9.-]*$') {
        throw "Invalid lane identifier: '$laneId'"
    }
    if (@($laneIds | Where-Object {
            $_.Equals($laneId, [StringComparison]::OrdinalIgnoreCase)
        }).Count -gt 0) {
        throw "Duplicate lane identifier: '$laneId'"
    }
    $laneIds.Add($laneId)

    $distribution = [string]$lane.distribution
    if ($distribution -cne 'portable') {
        continue
    }

    $fileName = "C3-v$($Identity.ReleaseLabel)-$laneId-$distribution.zip"
    if (@($fileNames | Where-Object {
            $_.Equals($fileName, [StringComparison]::OrdinalIgnoreCase)
        }).Count -gt 0) {
        throw "Duplicate release package filename: '$fileName'"
    }
    $fileNames.Add($fileName)

    $definitions.Add([PSCustomObject]@{
        LaneId = $laneId
        Distribution = $distribution
        FileName = $fileName
        Project = [string]$lane.project
        Platform = [string]$lane.platform
        TargetFramework = [string]$lane.targetFramework
        OutputDirectory = [string]$lane.outputDirectory
        RuntimeClaim = [string]$lane.runtimeClaim
    })
}

if ($definitions.Count -eq 0) {
    throw 'The build-lane manifest defines no portable release packages.'
}

$definitions | Sort-Object FileName
