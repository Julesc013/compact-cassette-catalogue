[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ReleaseLabel
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$numericIdentifier = '(?:0|[1-9][0-9]*)'
$pattern =
    "^(?<version>$numericIdentifier\.$numericIdentifier\.$numericIdentifier)" +
    "(?:-(?<stage>alpha|beta|rc)\.(?<sequence>[1-9][0-9]*))?`$"

if ($ReleaseLabel -cnotmatch $pattern) {
    throw "ReleaseLabel must be a canonical C3 release label: $ReleaseLabel"
}

$tagName = [string]$Matches.version
if ($Matches.ContainsKey('stage') -and
    -not [string]::IsNullOrWhiteSpace([string]$Matches.stage)) {
    $stageToken = switch ([string]$Matches.stage) {
        'alpha' { 'a' }
        'beta' { 'b' }
        'rc' { 'rc' }
        default { throw "Unsupported release stage in label: $ReleaseLabel" }
    }
    $tagName += $stageToken + [string]$Matches.sequence
}

Write-Output $tagName
