[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$BinaryPath,
    [Parameter(Mandatory = $true)]
    [string]$InputPath,
    [Parameter(Mandatory = $true)]
    [string]$OutputPath,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedProductVersion,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedStage,
    [Parameter(Mandatory = $true)]
    [string]$ExpectedCatalogueFormat
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

# This helper is deliberately invoked in a disposable child PowerShell process.
# The parent verifies the official release asset's SHA-256 before this process
# loads it. Do not use this script directly with an unverified executable.
$resolvedBinary = (Resolve-Path -LiteralPath $BinaryPath).Path
$resolvedInput = (Resolve-Path -LiteralPath $InputPath).Path
$resolvedOutput = [IO.Path]::GetFullPath($OutputPath)

$assembly = [Reflection.Assembly]::LoadFrom($resolvedBinary)
try {
    $types = $assembly.GetTypes()
}
catch [Reflection.ReflectionTypeLoadException] {
    $types = @($_.Exception.Types | Where-Object { $null -ne $_ })
}
$globals = @($types | Where-Object { $_.Name -ceq 'varGlobals' })
if ($globals.Count -ne 1) {
    throw "Expected one varGlobals module in '$resolvedBinary'."
}
$globalsType = $globals[0]
$flags = [Reflection.BindingFlags]'Public,NonPublic,Static'

function Get-LiteralValue {
    param([string]$Name)
    $field = $globalsType.GetField($Name, $flags)
    if ($null -eq $field -or -not $field.IsLiteral) {
        throw "Historical binary does not expose literal '$Name'."
    }
    return [string]$field.GetRawConstantValue()
}

if ((Get-LiteralValue 'VERSION') -cne $ExpectedProductVersion) {
    throw 'Historical binary product version does not match the corpus.'
}
if ((Get-LiteralValue 'VERSIONSTAGE') -cne $ExpectedStage) {
    throw 'Historical binary stage does not match the corpus.'
}
if ((Get-LiteralValue 'VERSIONFILE') -cne $ExpectedCatalogueFormat) {
    throw 'Historical binary catalogue format does not match the corpus.'
}

[Runtime.CompilerServices.RuntimeHelpers]::RunClassConstructor($globalsType.TypeHandle)
$supportedField = $globalsType.GetField('VERSIONFILESUPPORTED', $flags)
$supportedVersions = @($supportedField.GetValue($null) | ForEach-Object { [string]$_ })
if ($supportedVersions -cnotcontains $ExpectedCatalogueFormat) {
    throw 'Historical reader does not declare the expected catalogue format.'
}

$catalogue = New-Object Data.DataSet('Catalogue')
foreach ($fieldName in @('information', 'counters', 'decks', 'brands', 'models', 'tapes')) {
    $field = $globalsType.GetField($fieldName, $flags)
    if ($null -eq $field) {
        throw "Historical schema field is missing: $fieldName"
    }
    $table = [Data.DataTable]$field.GetValue($null)
    if ($null -eq $table -or [string]::IsNullOrWhiteSpace($table.TableName)) {
        throw "Historical schema table is invalid: $fieldName"
    }
    [void]$catalogue.Tables.Add($table)
}

$xmlSettings = New-Object Xml.XmlReaderSettings
$xmlSettings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
$xmlSettings.XmlResolver = $null
$inputDocument = New-Object Xml.XmlDocument
$inputDocument.XmlResolver = $null
$reader = [Xml.XmlReader]::Create($resolvedInput, $xmlSettings)
try {
    $inputDocument.Load($reader)
}
finally {
    $reader.Dispose()
}
$versionNode = $inputDocument.SelectSingleNode(
    "/Catalogue/Information[Information='File Version']/Value")
if ($null -eq $versionNode -or $versionNode.InnerText -cne $ExpectedCatalogueFormat) {
    throw 'Candidate does not contain the historical reader format identifier.'
}

$catalogue.EnforceConstraints = $false
$catalogue.Clear()
[void]$catalogue.ReadXml($resolvedInput)
foreach ($expectation in @{
        Brands = 1
        Models = 1
        Decks = 1
        Tapes = 1
    }.GetEnumerator()) {
    if ($catalogue.Tables[$expectation.Key].Rows.Count -ne $expectation.Value) {
        throw "Historical reader produced the wrong $($expectation.Key) row count."
    }
}

$outputDirectory = [IO.Path]::GetDirectoryName($resolvedOutput)
[IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
$catalogue.WriteXml($resolvedOutput)
Write-Host "LEGACY_BASELINE_PASS|$ExpectedProductVersion|$resolvedOutput"
