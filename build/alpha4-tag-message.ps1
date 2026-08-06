$script:C3Alpha4TagMessage = @'
C3 1.3.0 Alpha 4 - retained, unpublished owner-test preview

Package source and six test distributions: verified and reproduced
Repository-side application and classic setup regressions: passed
Human acceptance testing: pending
Historical Gate 1 and exact native target qualification: pending
Public Alpha publication: not authorized
Beta 1 tag, Beta-labelled release, and GitHub publication: require explicit human approval
Public feed, master, dev/2.x, and legacy/1.x: unchanged
'@

function Assert-C3Alpha4TagMessage {
    param([Parameter(Mandatory = $true)][string]$Text)
    foreach ($fragment in @(
            'C3 1.3.0 Alpha 4',
            'retained, unpublished owner-test preview',
            'Package source and six test distributions: verified and reproduced',
            'Human acceptance testing: pending',
            'Historical Gate 1 and exact native target qualification: pending',
            'Public Alpha publication: not authorized',
            'Beta 1 tag, Beta-labelled release, and GitHub publication: require explicit human approval',
            'Public feed, master, dev/2.x, and legacy/1.x: unchanged')) {
        if (-not $Text.Contains($fragment)) { throw "Alpha 4 tag message is missing: $fragment" }
    }
}
