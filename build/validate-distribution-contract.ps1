[CmdletBinding()]
param(
    [string]$ProfilesRoot,
    [string]$PayloadPath,
    [string]$LanesPath
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ProfilesRoot)) {
    $ProfilesRoot = Join-Path $repositoryRoot 'release\profiles'
}
if ([string]::IsNullOrWhiteSpace($PayloadPath)) {
    $PayloadPath = Join-Path $ProfilesRoot 'portable-payload.v1.json'
}
if ([string]::IsNullOrWhiteSpace($LanesPath)) {
    $LanesPath = Join-Path $PSScriptRoot 'lanes.json'
}

$profileSchema = Join-Path $repositoryRoot 'spec\distribution\v1\distribution-profile.schema.json'
$payloadSchema = Join-Path $repositoryRoot 'spec\distribution\v1\payload.schema.json'
$jsonValidator = Join-Path $PSScriptRoot 'validate-json-document.ps1'
& $jsonValidator -SchemaPath $payloadSchema -DocumentPath $PayloadPath -MaximumBytes 65536

$lanes = Get-Content -LiteralPath $LanesPath -Raw | ConvertFrom-Json
$portableLanes = @($lanes.lanes | Where-Object { [string]$_.distribution -ceq 'portable' })
$profileFiles = @(Get-ChildItem -LiteralPath $ProfilesRoot -File -Filter '*-portable.toml' | Sort-Object Name)
if ($profileFiles.Count -ne $portableLanes.Count) {
    throw "Expected $($portableLanes.Count) portable distribution profiles, found $($profileFiles.Count)."
}

$tempRoot = Join-Path ([IO.Path]::GetTempPath()) ('c3-distribution-' + [Guid]::NewGuid().ToString('N'))
$profiles = New-Object Collections.Generic.List[Object]
try {
    [IO.Directory]::CreateDirectory($tempRoot) | Out-Null
    foreach ($file in $profileFiles) {
        $profile = & (Join-Path $PSScriptRoot 'read-distribution-profile.ps1') -Path $file.FullName
        $projection = [ordered]@{
            schemaVersion = $profile.SchemaVersion
            id = $profile.Id
            product = $profile.Product
            lane = $profile.Lane
            delivery = $profile.Delivery
            status = $profile.Status
            payloadProfile = $profile.PayloadProfile
            archiveRoot = $profile.ArchiveRoot
        }
        $projectionPath = Join-Path $tempRoot ($file.BaseName + '.json')
        [IO.File]::WriteAllText(
            $projectionPath,
            (($projection | ConvertTo-Json -Depth 4) + "`n"),
            (New-Object Text.UTF8Encoding($false)))
        & $jsonValidator -SchemaPath $profileSchema -DocumentPath $projectionPath -MaximumBytes 16384
        if ($file.Name -cne ($profile.Id + '.toml')) {
            throw "Distribution profile filename does not match its ID: $($file.Name)"
        }
        $profiles.Add($profile)
    }
}
finally {
    if (Test-Path -LiteralPath $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}

$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$expectedStatus = switch ([string]$identity.ReleaseChannel) {
    'alpha' { 'internal' }
    'beta' { 'preview' }
    'stable' { 'supported' }
    default { throw "No distribution-status policy exists for channel '$($identity.ReleaseChannel)'." }
}
foreach ($lane in $portableLanes) {
    $matches = @($profiles | Where-Object { $_.Lane -ceq [string]$lane.id })
    if ($matches.Count -ne 1) {
        throw "Lane '$($lane.id)' requires exactly one portable distribution profile."
    }
    $profile = $matches[0]
    if ($profile.Id -cne ([string]$lane.id + '-portable')) {
        throw "Distribution profile ID does not derive from lane '$($lane.id)'."
    }
    if ($profile.Status -cne $expectedStatus) {
        throw "Distribution profile '$($profile.Id)' status must be '$expectedStatus' on channel '$($identity.ReleaseChannel)'."
    }
}

$payload = Get-Content -LiteralPath $PayloadPath -Raw | ConvertFrom-Json
$targets = New-Object Collections.Generic.HashSet[String]([StringComparer]::OrdinalIgnoreCase)
$roles = New-Object Collections.Generic.HashSet[String]([StringComparer]::OrdinalIgnoreCase)
foreach ($entry in @($payload.entries)) {
    if (-not $targets.Add([string]$entry.target)) {
        throw "Portable payload target is duplicated: $($entry.target)"
    }
    if (-not $roles.Add([string]$entry.role)) {
        throw "Portable payload role is duplicated: $($entry.role)"
    }
    foreach ($pathValue in @([string]$entry.source, [string]$entry.target)) {
        if ($pathValue.Contains('\') -or $pathValue.Contains('/') -or $pathValue.Contains('..')) {
            throw "Portable payload paths must be shallow canonical filenames: $pathValue"
        }
    }
}
$requiredRoles = @(
    'build-metadata',
    'catalogue-assembly',
    'domain-assembly',
    'infrastructure-assembly',
    'cli-executable',
    'desktop-executable',
    'desktop-configuration',
    'readme',
    'release-notes'
)
$difference = @(Compare-Object ($requiredRoles | Sort-Object) (@($roles) | Sort-Object) -CaseSensitive)
if ($difference.Count -gt 0) {
    throw "Portable payload roles do not match the C3 2.0 core contract."
}

Write-Host "Distribution contract verified: $($profiles.Count) lane profiles, $($payload.entries.Count) canonical payload entries, status $expectedStatus."
