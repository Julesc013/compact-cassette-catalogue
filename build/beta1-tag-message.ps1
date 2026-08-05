$script:C3Beta1TagMessageFragments = @(
    'C3 1.3.0 Beta 1',
    'qualified retained Candidate',
    'Historical Gate 1: passed',
    'APP-001 through APP-015: closed',
    'Setup crash consistency: qualified',
    'XP, Windows 7, and native ARM64 targets: qualified',
    'Portable Beta GO: true',
    'Classic setup Beta GO: true',
    'Overall Beta GO: true',
    'Public GitHub release: not authorized',
    'VERSION feed: unchanged',
    'master and dev/2.x: unchanged'
)

function Assert-C3Beta1TagMessage {
    param([Parameter(Mandatory = $true)][string]$Text)
    foreach ($fragment in $script:C3Beta1TagMessageFragments) {
        if (-not $Text.Contains($fragment)) { throw "Beta 1 annotated tag is missing required message fragment: $fragment" }
    }
}

function Get-C3Beta1TagMessage {
    return @'
C3 1.3.0 Beta 1

This is the qualified retained Candidate.
Historical Gate 1: passed
APP-001 through APP-015: closed
Setup crash consistency: qualified
XP, Windows 7, and native ARM64 targets: qualified
Portable Beta GO: true
Classic setup Beta GO: true
Overall Beta GO: true
Public GitHub release: not authorized
VERSION feed: unchanged
master and dev/2.x: unchanged
'@
}

