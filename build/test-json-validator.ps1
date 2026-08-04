[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$validator = Join-Path $PSScriptRoot 'validate-json-document.ps1'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) (
    'c3-json-validator-' + [Guid]::NewGuid().ToString('N'))
$utf8WithoutBom = New-Object Text.UTF8Encoding($false, $true)
$passed = 0

function Write-StrictUtf8Text {
    param(
        [string]$Path,
        [string]$Text
    )

    [IO.File]::WriteAllText($Path, $Text, $utf8WithoutBom)
}

function Invoke-ValidatorProcess {
    param(
        [string]$Executable,
        [string]$SchemaPath,
        [string]$DocumentPath,
        [long]$MaximumBytes = 0
    )

    $arguments = @(
        '-NoLogo',
        '-NoProfile',
        '-NonInteractive',
        '-ExecutionPolicy', 'Bypass',
        '-File', $validator,
        '-SchemaPath', $SchemaPath,
        '-DocumentPath', $DocumentPath)
    if ($MaximumBytes -gt 0) {
        $arguments += @('-MaximumBytes', [string]$MaximumBytes)
    }

    $savedErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        [void](& $Executable @arguments 2>&1)
        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $savedErrorActionPreference
    }
}

function Assert-ValidatorPasses {
    param(
        [string]$Executable,
        [string]$SchemaPath,
        [string]$DocumentPath,
        [string]$Scenario
    )

    $exitCode = Invoke-ValidatorProcess $Executable $SchemaPath $DocumentPath
    if ($exitCode -ne 0) {
        throw "$Scenario`: expected validation to pass under $Executable."
    }
    $script:passed++
}

function Assert-ValidatorFails {
    param(
        [string]$Executable,
        [string]$SchemaPath,
        [string]$DocumentPath,
        [string]$Scenario,
        [long]$MaximumBytes = 0
    )

    $exitCode = Invoke-ValidatorProcess `
        $Executable `
        $SchemaPath `
        $DocumentPath `
        $MaximumBytes
    if ($exitCode -eq 0) {
        throw "$Scenario`: expected validation to fail under $Executable."
    }
    $script:passed++
}

try {
    [IO.Directory]::CreateDirectory($testRoot) | Out-Null
    $schemaPath = Join-Path $testRoot 'schema.json'
    $unknownKeywordSchemaPath = Join-Path $testRoot 'unknown-keyword.schema.json'
    $duplicateKeywordSchemaPath = Join-Path $testRoot 'duplicate-keyword.schema.json'
    $invalidReferenceSchemaPath = Join-Path $testRoot 'invalid-reference.schema.json'
    $validPath = Join-Path $testRoot 'valid.json'
    $escapedNamesPath = Join-Path $testRoot 'escaped-names.json'
    $duplicateEscapedNamePath = Join-Path $testRoot 'duplicate-escaped-name.json'
    $reorderedDuplicatePath = Join-Path $testRoot 'reordered-duplicate.json'
    $numericDuplicatePath = Join-Path $testRoot 'numeric-duplicate.json'
    $invalidUtf8Path = Join-Path $testRoot 'invalid-utf8.json'
    $oversizedPath = Join-Path $testRoot 'oversized.json'

    $schemaText = @'
{
  "type": "object",
  "additionalProperties": false,
  "required": ["meta", "values", "records"],
  "properties": {
    "meta": {
      "type": "object",
      "additionalProperties": false,
      "required": ["$schema", "$id"],
      "properties": {
        "$schema": { "type": "string" },
        "$id": { "type": "string" }
      }
    },
    "values": {
      "type": "array",
      "uniqueItems": true
    },
    "records": {
      "type": "array",
      "uniqueItems": true,
      "items": {
        "type": "object",
        "additionalProperties": { "type": "string" }
      }
    }
  }
}
'@
    Write-StrictUtf8Text $schemaPath $schemaText
    Write-StrictUtf8Text $unknownKeywordSchemaPath (
        $schemaText.Replace('"uniqueItems": true', '"uniqueItemz": true'))
    Write-StrictUtf8Text $duplicateKeywordSchemaPath @'
{"type":"object","type":"array"}
'@
    Write-StrictUtf8Text $invalidReferenceSchemaPath @'
{"$defs":{"rule":{"type":"string"}},"$ref":"#/$defs"}
'@
    Write-StrictUtf8Text $validPath @'
{"meta":{"$schema":"schema","$id":"id"},"values":["A","a"],"records":[{"left":"A","right":"B"}]}
'@
    Write-StrictUtf8Text $escapedNamesPath @'
{"meta":{"$schema":"schema","$id":"id"},"values":[],"records":[]}
'@
    Write-StrictUtf8Text $duplicateEscapedNamePath @'
{"meta":{"$schema":"schema","$schema":"other","$id":"id"},"values":[],"records":[]}
'@
    Write-StrictUtf8Text $reorderedDuplicatePath @'
{"meta":{"$schema":"schema","$id":"id"},"values":[],"records":[{"left":"A","right":"B"},{"right":"B","left":"A"}]}
'@
    Write-StrictUtf8Text $numericDuplicatePath @'
{"meta":{"$schema":"schema","$id":"id"},"values":[1,1.0],"records":[]}
'@
    [IO.File]::WriteAllBytes(
        $invalidUtf8Path,
        [byte[]]@(0x7b, 0x22, 0x78, 0x22, 0x3a, 0x22, 0xc3, 0x28, 0x22, 0x7d))
    Write-StrictUtf8Text $oversizedPath (([IO.File]::ReadAllText($validPath)) + (' ' * 128))

    $shells = @(
        (Get-Command 'powershell.exe' -ErrorAction Stop).Source,
        (Get-Command 'pwsh.exe' -ErrorAction Stop).Source)
    foreach ($shell in $shells) {
        Assert-ValidatorPasses $shell $schemaPath $validPath (
            'case-sensitive unique items and object validation')
        Assert-ValidatorPasses $shell $schemaPath $escapedNamesPath (
            'distinct escaped JSON property names')
        Assert-ValidatorFails $shell $schemaPath $duplicateEscapedNamePath (
            'duplicate escaped JSON property')
        Assert-ValidatorFails $shell $schemaPath $reorderedDuplicatePath (
            'order-independent object equality for uniqueItems')
        Assert-ValidatorFails $shell $schemaPath $numericDuplicatePath (
            'mathematically equal numeric values for uniqueItems')
        Assert-ValidatorFails $shell $unknownKeywordSchemaPath $validPath (
            'unsupported schema keyword fails closed')
        Assert-ValidatorFails $shell $duplicateKeywordSchemaPath $validPath (
            'duplicate schema keyword')
        Assert-ValidatorFails $shell $invalidReferenceSchemaPath $validPath (
            'reference to a definitions container')
        Assert-ValidatorFails $shell $schemaPath $invalidUtf8Path (
            'malformed UTF-8 transport')
        Assert-ValidatorFails `
            $shell `
            $schemaPath `
            $oversizedPath `
            'transport byte limit' `
            ([IO.File]::ReadAllBytes($validPath).Length)
    }
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        $resolvedTestRoot = [IO.Path]::GetFullPath($testRoot)
        $resolvedTempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
        if (-not $resolvedTestRoot.StartsWith(
                $resolvedTempRoot,
                [StringComparison]::OrdinalIgnoreCase) -or
            [IO.Path]::GetFileName($resolvedTestRoot) -cnotmatch
                '^c3-json-validator-[0-9a-f]{32}$') {
            throw "Refusing to remove unsafe JSON-validator test path: $resolvedTestRoot"
        }
        Remove-Item -LiteralPath $resolvedTestRoot -Recurse -Force
    }
}

# GitHub Actions appends `exit $LASTEXITCODE` to PowerShell steps. Expected
# negative child-process cases must not leak their exit code past a successful
# test harness.
$global:LASTEXITCODE = 0
Write-Host "JSON-validator tests passed: $passed scenarios."
