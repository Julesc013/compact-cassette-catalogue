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
Write-Host 'Alpha 5 annotated-tag message contract passed.'
