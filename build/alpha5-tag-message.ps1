$script:C3Alpha5TagMessage = @'
C3 1.3.0 Alpha 5 - retained, unpublished owner-test preview

Package source and six test distributions: verified and reproduced
Repository-side application, layout, startup, and classic setup regressions: passed
Native owner visual, keyboard, and accessibility testing: pending
Historical Gate 1 and exact native target qualification: pending
Native ARM64 execution: pending
Public Alpha publication: not authorized
Beta tag, Beta-labelled artifacts, and GitHub publication: require explicit human approval
Public feed, master, dev/2.x, and legacy/1.x: unchanged
'@

function Assert-C3Alpha5TagMessage {
    param([Parameter(Mandatory = $true)][string]$Text)

    foreach ($fragment in @(
            'C3 1.3.0 Alpha 5',
            'retained, unpublished owner-test preview',
            'Package source and six test distributions: verified and reproduced',
            'Repository-side application, layout, startup, and classic setup regressions: passed',
            'Native owner visual, keyboard, and accessibility testing: pending',
            'Historical Gate 1 and exact native target qualification: pending',
            'Native ARM64 execution: pending',
            'Public Alpha publication: not authorized',
            'Beta tag, Beta-labelled artifacts, and GitHub publication: require explicit human approval',
            'Public feed, master, dev/2.x, and legacy/1.x: unchanged')) {
        if (-not $Text.Contains($fragment)) { throw "Alpha 5 tag message is missing: $fragment" }
    }
}
