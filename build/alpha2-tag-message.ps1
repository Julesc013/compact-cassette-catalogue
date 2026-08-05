$script:C3Alpha2TagMessageFragments = @(
    'C3 1.3.0 Alpha 2',
    'unpublished engineering preview',
    'Historical Gate 1: deferred',
    'Runtime repairs: deferred',
    'Native ARM64 execution: deferred',
    'Target-OS qualification: deferred',
    'Public publication: not authorized'
)

function Assert-C3Alpha2TagMessage {
    param([Parameter(Mandatory = $true)][string]$Text)

    foreach ($requiredTagFragment in $script:C3Alpha2TagMessageFragments) {
        if (-not $Text.Contains($requiredTagFragment)) {
            throw "Alpha 2 annotated tag is missing required message fragment: $requiredTagFragment"
        }
    }
}
