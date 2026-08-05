$script:C3Alpha3TagMessageFragments = @(
    'C3 1.3.0 Alpha 3',
    'retained, unpublished engineering preview',
    'Historical Gate 1: passed',
    'Legacy reliability repairs: qualified',
    'Native x86/x64/ARM64 target execution: qualified',
    'Optional classic setup: qualified',
    'Public publication: not authorized',
    'Beta-labelled artifacts: require explicit human approval',
    'Public feed and legacy/1.x: unchanged'
)

function Assert-C3Alpha3TagMessage {
    param([Parameter(Mandatory = $true)][string]$Text)

    foreach ($requiredTagFragment in $script:C3Alpha3TagMessageFragments) {
        if (-not $Text.Contains($requiredTagFragment)) {
            throw "Alpha 3 annotated tag is missing required message fragment: $requiredTagFragment"
        }
    }
}
