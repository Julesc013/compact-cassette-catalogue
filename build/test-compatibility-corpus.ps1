[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$validator = Join-Path $PSScriptRoot 'validate-compatibility-corpus.ps1'
$schemaPath = Join-Path $repositoryRoot `
    'spec\compatibility-corpus\v1\corpus.schema.json'
$canonicalPath = Join-Path $repositoryRoot `
    'fixtures\compatibility\1x\corpus.v1.json'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) (
    'c3-compatibility-corpus-' + [Guid]::NewGuid().ToString('N'))
$fixturePath = Join-Path $testRoot 'corpus.v1.json'
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
$passed = 0

function Reset-Fixture {
    Copy-Item -LiteralPath $canonicalPath -Destination $fixturePath -Force
}

function Write-Fixture {
    param([object]$Document)
    [IO.File]::WriteAllText(
        $fixturePath,
        (($Document | ConvertTo-Json -Depth 30) + "`n"),
        $utf8WithoutBom)
}

function Invoke-Validator {
    $arguments = @(
        '-NoLogo', '-NoProfile', '-NonInteractive', '-ExecutionPolicy', 'Bypass',
        '-File', $validator,
        '-RepositoryRoot', $repositoryRoot,
        '-CorpusPath', $fixturePath,
        '-SchemaPath', $schemaPath,
        '-SkipGitEvidence')
    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        [void](& powershell.exe @arguments 2>&1)
        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Assert-Passes {
    param([string]$Scenario)
    if ((Invoke-Validator) -ne 0) {
        throw "$Scenario`: expected compatibility-corpus validation to pass."
    }
    $script:passed++
}

function Assert-Fails {
    param([string]$Scenario)
    if ((Invoke-Validator) -eq 0) {
        throw "$Scenario`: expected compatibility-corpus validation to fail."
    }
    $script:passed++
}

try {
    [IO.Directory]::CreateDirectory($testRoot) | Out-Null

    Reset-Fixture
    Assert-Passes 'canonical corpus'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.releases[1].tag = $document.releases[0].tag
    Write-Fixture $document
    Assert-Fails 'duplicate release tag'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.releases[0].artifacts[0].url = 'https://example.invalid/C3.exe'
    Write-Fixture $document
    Assert-Fails 'noncanonical artifact URL'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.releases[4].fixtures[0].sha256 = ('0' * 64)
    Write-Fixture $document
    Assert-Fails 'fixture hash drift'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.releases[4].fixtures = @()
    Write-Fixture $document
    Assert-Fails 'supported producer without fixture'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.supportPolicy.supportedProducerTags[0] = 'v1.0.0b1'
    Write-Fixture $document
    Assert-Fails 'overlapping support categories'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.formatProvenance[0].format = '1.1.0'
    Write-Fixture $document
    Assert-Fails 'missing observed format'

    Write-Host "$passed compatibility-corpus scenarios passed."
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
    $global:LASTEXITCODE = 0
}
