Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha5-tag-message.ps1')

Assert-C3Alpha5TagMessage -Text $script:C3Alpha5TagMessage
foreach ($line in @($script:C3Alpha5TagMessage -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })) {
    try {
        Assert-C3Alpha5TagMessage -Text $script:C3Alpha5TagMessage.Replace($line, '')
        throw "Altered Alpha 5 tag message was accepted after removing '$line'."
    }
    catch {
        if ($_.Exception.Message -like 'Altered Alpha 5*') { throw }
    }
}

$tagScript = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'create-alpha5-tag.ps1') -Raw
$collectionSafeDiffClosure = '@(Compare-Object ($allowed | Sort-Object) $changes).Count'
if ($tagScript.IndexOf($collectionSafeDiffClosure, [StringComparison]::Ordinal) -lt 0) {
    throw 'Alpha 5 tag creation must collection-wrap an empty Compare-Object result before reading Count under strict mode.'
}
$identicalDiffCount = @(Compare-Object @('qualified.json', 'qualified.md') @('qualified.json', 'qualified.md')).Count
if ($identicalDiffCount -ne 0) {
    throw 'Alpha 5 tag diff-closure regression fixture did not produce an empty collection.'
}
Write-Host 'Alpha 5 annotated-tag message contract passed.'
