[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$fullPath = [IO.Path]::GetFullPath($Path)
if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
    throw "Distribution profile is missing: $fullPath"
}
$bytes = [IO.File]::ReadAllBytes($fullPath)
if ($bytes.Length -gt 8192) {
    throw "Distribution profile exceeds 8192 bytes: $fullPath"
}
$encoding = New-Object Text.UTF8Encoding($false, $true)
$text = $encoding.GetString($bytes).Replace("`r`n", "`n")
if ($text.Contains("`r")) {
    throw "Distribution profile must use canonical LF newlines: $fullPath"
}

$allowed = @(
    'schema-version',
    'id',
    'product',
    'lane',
    'delivery',
    'status',
    'payload-profile',
    'archive-root'
)
$values = @{}
foreach ($line in $text.Split("`n")) {
    $trimmed = $line.Trim()
    if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith('#')) {
        continue
    }
    if ($trimmed -notmatch '^([a-z][a-z0-9-]*) = (?:(\d+)|"([ -!#-\[\]-~]+)")$') {
        throw "Unsupported TOML syntax in distribution profile: $trimmed"
    }
    $key = $matches[1]
    if ($allowed -cnotcontains $key) {
        throw "Unknown distribution-profile key: $key"
    }
    if ($values.ContainsKey($key)) {
        throw "Duplicate distribution-profile key: $key"
    }
    $values[$key] = if ([string]::IsNullOrEmpty($matches[2])) {
        [string]$matches[3]
    }
    else {
        [int]$matches[2]
    }
}
foreach ($key in $allowed) {
    if (-not $values.ContainsKey($key)) {
        throw "Distribution profile is missing key: $key"
    }
}

[PSCustomObject]@{
    SchemaVersion = [int]$values['schema-version']
    Id = [string]$values.id
    Product = [string]$values.product
    Lane = [string]$values.lane
    Delivery = [string]$values.delivery
    Status = [string]$values.status
    PayloadProfile = [string]$values['payload-profile']
    ArchiveRoot = [string]$values['archive-root']
    Path = $fullPath
}
