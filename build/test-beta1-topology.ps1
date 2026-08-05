[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-tag-message.ps1')
. (Join-Path $PSScriptRoot 'beta1-topology.ps1')

$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("c3-beta1-topology-" + [Guid]::NewGuid().ToString('N'))
function Invoke-Git { param([string[]]$Arguments) & git -C $temporaryRoot @Arguments | Out-Null; if ($LASTEXITCODE -ne 0) { throw "git failed: $($Arguments -join ' ')" } }
function Assert-Rejected([string]$Name, [scriptblock]$Action, [string]$Pattern) {
    try { & $Action; throw "$Name unexpectedly passed." } catch { if ($_.Exception.Message -notmatch $Pattern) { throw } }
}
try {
    New-Item -ItemType Directory -Path (Join-Path $temporaryRoot 'release\validation') -Force | Out-Null
    Invoke-Git -Arguments @('init', '-q'); Invoke-Git -Arguments @('config', 'user.name', 'C3 topology test'); Invoke-Git -Arguments @('config', 'user.email', 'c3-topology@example.invalid')
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'source.txt') -Value 'C' -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-verdict.json') -Value 'template' -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-verdict.md') -Value 'template' -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-post-tag.json') -Value 'template' -Encoding UTF8
    Invoke-Git -Arguments @('add', '.'); Invoke-Git -Arguments @('commit', '-q', '-m', 'C-beta')
    $source = (& git -C $temporaryRoot rev-parse HEAD).Trim()
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-verdict.json') -Value 'go' -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-verdict.md') -Value 'GO' -Encoding UTF8
    Invoke-Git -Arguments @('add', '.'); Invoke-Git -Arguments @('commit', '-q', '-m', 'E-beta')
    $evidence = (& git -C $temporaryRoot rev-parse HEAD).Trim()
    Invoke-Git -Arguments @('tag', '-a', 'v1.3.0b1', '-m', (Get-C3Beta1TagMessage), $evidence)
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'release\validation\1.3.0-beta.1-post-tag.json') -Value 'pass' -Encoding UTF8
    Invoke-Git -Arguments @('add', '.'); Invoke-Git -Arguments @('commit', '-q', '-m', 'P-beta')
    $post = (& git -C $temporaryRoot rev-parse HEAD).Trim()
    Assert-C3Beta1CommitTopology -RepositoryRoot $temporaryRoot -SourceCommit $source -EvidenceCommit $evidence -PostTagCommit $post
    Assert-Rejected 'non-direct C-beta' { Assert-C3Beta1CommitTopology -RepositoryRoot $temporaryRoot -SourceCommit $evidence -EvidenceCommit $evidence } 'direct single-parent child'
    Invoke-Git -Arguments @('tag', '-a', 'invalid-beta', '-m', 'incomplete Beta message', $evidence)
    Assert-Rejected 'incomplete annotated message' { Assert-C3Beta1CommitTopology -RepositoryRoot $temporaryRoot -SourceCommit $source -EvidenceCommit $evidence -TagRef refs/tags/invalid-beta } 'missing required message fragment'
    Set-Content -LiteralPath (Join-Path $temporaryRoot 'source.txt') -Value 'unexpected' -Encoding UTF8
    Invoke-Git -Arguments @('add', '.'); Invoke-Git -Arguments @('commit', '-q', '-m', 'bad-P')
    $badPost = (& git -C $temporaryRoot rev-parse HEAD).Trim()
    Assert-Rejected 'non-direct or expanded P-beta' { Assert-C3Beta1CommitTopology -RepositoryRoot $temporaryRoot -SourceCommit $source -EvidenceCommit $evidence -PostTagCommit $badPost } 'direct single-parent child|outside the sole'
    Write-Host 'Beta topology controls accepted exact C/E/tag/P and rejected wrong ancestry, incomplete tag authority, and expanded P.'
}
finally { if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force } }
