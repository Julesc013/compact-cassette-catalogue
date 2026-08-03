[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $repositoryRoot 'artifacts'
$testParent = Join-Path $artifactsRoot 'doc-tests'
$testRoot = $null
$validatorSource = Join-Path $PSScriptRoot 'validate-docs.ps1'
$passed = 0

function Assert-SafeTestPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $artifactsFullPath = [IO.Path]::GetFullPath($artifactsRoot).TrimEnd('\', '/')
    $pathFullPath = [IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
    if (-not $pathFullPath.StartsWith($artifactsFullPath + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to manage documentation-test path outside artifacts: $pathFullPath"
    }
}

function New-OwnedTestRoot {
    Assert-SafeTestPath $testParent
    New-Item -ItemType Directory -Path $testParent -Force | Out-Null

    for ($attempt = 0; $attempt -lt 32; $attempt++) {
        # Keep the name compact because this test also runs in path-distinct
        # exports on toolchains that retain the legacy Windows MAX_PATH limit.
        $token = [Guid]::NewGuid().ToString('N').Substring(0, 12)
        $candidate = Join-Path $testParent ("p{0}-{1}" -f $PID, $token)
        Assert-SafeTestPath $candidate

        try {
            return (New-Item -ItemType Directory -Path $candidate -ErrorAction Stop).FullName
        }
        catch [IO.IOException] {
            if (Test-Path -LiteralPath $candidate -PathType Container) {
                continue
            }

            throw
        }
    }

    throw 'Could not allocate a unique documentation-validation test directory.'
}

function New-ValidatorFixture {
    param([Parameter(Mandatory = $true)][string]$Root)

    $buildDirectory = Join-Path $Root 'build'
    New-Item -ItemType Directory -Path $buildDirectory -Force | Out-Null
    Copy-Item -LiteralPath $validatorSource -Destination (Join-Path $buildDirectory 'validate-docs.ps1')
}

function Assert-ValidatorFailsWith {
    param(
        [Parameter(Mandatory = $true)][string]$ValidatorPath,
        [Parameter(Mandatory = $true)][string]$ExpectedMessage,
        [Parameter(Mandatory = $true)][string]$Scenario
    )

    $caught = $null
    try {
        & $ValidatorPath
    }
    catch {
        $caught = $_
    }

    if ($null -eq $caught) {
        throw "Expected documentation validator to fail: $Scenario"
    }

    if ($caught.Exception.Message -notlike "*$ExpectedMessage*") {
        throw "Unexpected documentation-validator failure for ${Scenario}: $($caught.Exception.Message)"
    }
}

try {
    $testRoot = New-OwnedTestRoot

    # The source root deliberately has an ancestor named artifacts. This is the
    # layout used by clean-source reproducibility verification.
    $linkedFixture = Join-Path $testRoot 'source-under-artifacts'
    New-ValidatorFixture $linkedFixture
    Set-Content -LiteralPath (Join-Path $linkedFixture 'README.md') `
        -Value '[Guide](docs/guide.md)' -Encoding UTF8
    $linkedValidator = Join-Path $linkedFixture 'build\validate-docs.ps1'

    Assert-ValidatorFailsWith $linkedValidator 'README.md -> docs/guide.md' `
        'a broken link below an artifacts ancestor remains discoverable'
    $passed++

    $docsDirectory = Join-Path $linkedFixture 'docs'
    New-Item -ItemType Directory -Path $docsDirectory -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $docsDirectory 'guide.md') -Value '# Guide' -Encoding UTF8
    $ignoredDirectory = Join-Path $linkedFixture 'artifacts'
    New-Item -ItemType Directory -Path $ignoredDirectory -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $ignoredDirectory 'generated.md') `
        -Value '[Generated broken link](missing.md)' -Encoding UTF8
    & $linkedValidator
    $passed++

    $emptyFixture = Join-Path $testRoot 'empty-source'
    New-ValidatorFixture $emptyFixture
    $emptyValidator = Join-Path $emptyFixture 'build\validate-docs.ps1'
    Assert-ValidatorFailsWith $emptyValidator 'found no Markdown files' `
        'an empty scan cannot report a vacuous success'
    $passed++
}
finally {
    if ($null -ne $testRoot) {
        Assert-SafeTestPath $testRoot
        if (Test-Path -LiteralPath $testRoot) {
            Remove-Item -LiteralPath $testRoot -Recurse -Force
        }
    }
}

Write-Host "Documentation-validation tests passed: $passed scenarios."
