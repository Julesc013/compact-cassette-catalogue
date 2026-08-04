[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$validator = Join-Path $PSScriptRoot 'validate-branch-contract.ps1'
$schemaPath = Join-Path $repositoryRoot `
    'spec\branch-contract\v1\branches.schema.json'
$canonicalPath = Join-Path $PSScriptRoot 'branches.json'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) (
    'c3-branch-contract-' + [Guid]::NewGuid().ToString('N'))
$fixturePath = Join-Path $testRoot 'branches.json'
$utf8WithoutBom = New-Object Text.UTF8Encoding($false)
$passed = 0

function Reset-Fixture {
    Copy-Item -LiteralPath $canonicalPath -Destination $fixturePath -Force
}

function Write-Fixture {
    param([object]$Document)
    [IO.File]::WriteAllText(
        $fixturePath,
        (($Document | ConvertTo-Json -Depth 20) + "`n"),
        $utf8WithoutBom)
}

function Invoke-Validator {
    $arguments = @(
        '-NoLogo', '-NoProfile', '-NonInteractive', '-ExecutionPolicy', 'Bypass',
        '-File', $validator,
        '-RepositoryRoot', $repositoryRoot,
        '-ContractPath', $fixturePath,
        '-SchemaPath', $schemaPath)
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
        throw "$Scenario`: expected branch-contract validation to pass."
    }
    $script:passed++
}

function Assert-Fails {
    param([string]$Scenario)
    if ((Invoke-Validator) -eq 0) {
        throw "$Scenario`: expected branch-contract validation to fail."
    }
    $script:passed++
}

try {
    [IO.Directory]::CreateDirectory($testRoot) | Out-Null

    Reset-Fixture
    Assert-Passes 'canonical permanent branches'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.currentGeneration.integration = 'master'
    Write-Fixture $document
    Assert-Fails 'duplicate permanent role'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.legacyGeneration.qualified = 'dev'
    Write-Fixture $document
    Assert-Fails 'Git ref namespace collision'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.currentGeneration.integration = 'dev..2.x'
    Write-Fixture $document
    Assert-Fails 'invalid Git branch name'

    Reset-Fixture
    $document = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    $document.schemaVersion = 2
    Write-Fixture $document
    Assert-Fails 'unsupported schema version'

    Write-Host "$passed branch-contract scenarios passed."
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
    $global:LASTEXITCODE = 0
}
