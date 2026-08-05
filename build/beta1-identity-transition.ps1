. (Join-Path $PSScriptRoot 'beta1-contract.ps1')

function Assert-C3Beta1IdentityTransition {
    param(
        [Parameter(Mandatory = $true)][string]$RepositoryRoot,
        [Parameter(Mandatory = $true)][string]$IdentityCommit,
        [string]$HeadCommit = 'HEAD'
    )

    if ($IdentityCommit -notmatch '^[0-9a-f]{40}$') { throw 'Beta identity verification requires the explicit full 40-character IdentityCommit.' }
    $resolvedHead = (& git -C $RepositoryRoot rev-parse $HeadCommit).Trim()
    if ($LASTEXITCODE -ne 0 -or $resolvedHead -notmatch '^[0-9a-f]{40}$') { throw 'Could not resolve the Beta source head.' }
    & git -C $RepositoryRoot merge-base --is-ancestor $IdentityCommit $resolvedHead
    if ($LASTEXITCODE -ne 0) { throw "Beta identity commit '$IdentityCommit' is not an ancestor of source '$resolvedHead'." }
    $parents = @((& git -C $RepositoryRoot rev-list --parents -n 1 $IdentityCommit).Trim().Split(' '))
    if ($parents.Count -ne 2) { throw 'Beta identity projection must be a single-parent commit.' }
    $parentCommit = $parents[1]

    $alphaManifest = (@(& git -C $RepositoryRoot show "$parentCommit`:build/lanes.json") -join "`n") | ConvertFrom-Json
    foreach ($comparison in @(
            @('releaseVersion', [string]$alphaManifest.releaseVersion, '1.3.0'),
            @('releaseStage', [string]$alphaManifest.releaseStage, 'Alpha 3'),
            @('releaseLabel', [string]$alphaManifest.releaseLabel, '1.3.0a3'),
            @('releaseTag', [string]$alphaManifest.releaseTag, 'v1.3.0a3'),
            @('releaseChannel', [string]$alphaManifest.releaseChannel, 'alpha'),
            @('publicationStatus', [string]$alphaManifest.publicationStatus, 'retained-unpublished'))) {
        if ($comparison[1] -cne $comparison[2]) { throw "Beta identity parent is not exact Alpha 3: $($comparison[0])." }
    }
    $alphaLanes = @($alphaManifest.lanes)
    if ($alphaLanes.Count -ne 3) { throw 'Beta identity parent does not contain exactly three Alpha 3 lanes.' }
    for ($index = 0; $index -lt 3; $index++) {
        $id = $script:C3Beta1LaneIds[$index]
        if ([string]$alphaLanes[$index].id -cne $id -or
                [string]$alphaLanes[$index].packageName -cne "C3-v1.3.0a3-$id-portable.zip" -or
                [string]$alphaLanes[$index].setupPackageName -cne "C3-v1.3.0a3-$id-setup.zip") {
            throw "Beta identity parent Alpha lane '$index' is not exact."
        }
    }

    $betaManifest = (@(& git -C $RepositoryRoot show "$IdentityCommit`:build/lanes.json") -join "`n") | ConvertFrom-Json
    [void](Assert-C3Beta1Manifest -Manifest $betaManifest)
    $allowed = @(
        'CHANGELOG.md', 'RELEASE_NOTES.md',
        'Compact Cassette Catalogue/My Project/AssemblyInfo.vb', 'Compact Cassette Catalogue/varGlobals.vb',
        'Compact Cassette Catalogue Installer/My Project/AssemblyInfo.vb',
        'Compact Cassette Catalogue Uninstaller/My Project/AssemblyInfo.vb',
        'SetupShared/SetupBundleRuntime.vb', 'build/get-runtime-lanes.ps1', 'build/lanes.json',
        'build/package-content/README.txt', 'docs/setup/1.3.0-manifest-contracts.md',
        'tests/C3.Setup.Characterization/Program.vb'
    )
    $changes = @(& git -C $RepositoryRoot diff --name-only $parentCommit $IdentityCommit)
    if ((($changes | Sort-Object) -join "`n") -cne (($allowed | Sort-Object) -join "`n")) {
        throw "Beta identity transition is not the exact closed metadata projection: $($changes -join ', ')"
    }
    return $IdentityCommit
}
