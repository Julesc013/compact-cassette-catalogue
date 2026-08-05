. (Join-Path $PSScriptRoot 'beta1-tag-message.ps1')

function Assert-C3Beta1CommitTopology {
    param(
        [Parameter(Mandatory = $true)][string]$RepositoryRoot,
        [Parameter(Mandatory = $true)][string]$SourceCommit,
        [Parameter(Mandatory = $true)][string]$EvidenceCommit,
        [string]$PostTagCommit,
        [string]$TagRef = 'refs/tags/v1.3.0b1'
    )

    foreach ($commit in @($SourceCommit, $EvidenceCommit) + $(if ([string]::IsNullOrWhiteSpace($PostTagCommit)) { @() } else { @($PostTagCommit) })) {
        if ($commit -notmatch '^[0-9a-f]{40}$') { throw 'Beta topology requires full source/evidence/post-tag commit SHAs.' }
    }
    $evidenceParents = @((& git -C $RepositoryRoot rev-list --parents -n 1 $EvidenceCommit).Trim().Split(' '))
    if ($evidenceParents.Count -ne 2 -or $evidenceParents[1] -cne $SourceCommit) { throw 'E-beta must be the direct single-parent child of C-beta.' }
    $expectedEvidenceChanges = @('release/validation/1.3.0-beta.1-verdict.json', 'release/validation/1.3.0-beta.1-verdict.md') | Sort-Object
    $evidenceChanges = @(& git -C $RepositoryRoot diff --name-only $SourceCommit $EvidenceCommit | Sort-Object)
    if (($evidenceChanges -join "`n") -cne ($expectedEvidenceChanges -join "`n")) { throw 'E-beta changes outside the exact machine/human verdict pair.' }
    if ((& git -C $RepositoryRoot cat-file -t $TagRef).Trim() -cne 'tag') { throw 'Beta topology requires an annotated tag object.' }
    Assert-C3Beta1TagMessage -Text ((@(& git -C $RepositoryRoot cat-file tag $TagRef)) -join "`n")
    if ((& git -C $RepositoryRoot rev-parse "$TagRef^{commit}").Trim() -cne $EvidenceCommit) { throw 'Beta annotated tag does not target E-beta.' }
    if (-not [string]::IsNullOrWhiteSpace($PostTagCommit)) {
        $postParents = @((& git -C $RepositoryRoot rev-list --parents -n 1 $PostTagCommit).Trim().Split(' '))
        if ($postParents.Count -ne 2 -or $postParents[1] -cne $EvidenceCommit) { throw 'P-beta must be the direct single-parent child of E-beta.' }
        $postChanges = @(& git -C $RepositoryRoot diff --name-only $EvidenceCommit $PostTagCommit)
        if ($postChanges.Count -ne 1 -or $postChanges[0] -cne 'release/validation/1.3.0-beta.1-post-tag.json') { throw 'P-beta changes outside the sole post-tag attestation.' }
    }
}

