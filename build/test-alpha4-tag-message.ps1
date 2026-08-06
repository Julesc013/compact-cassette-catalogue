Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha4-tag-message.ps1')
Assert-C3Alpha4TagMessage -Text $script:C3Alpha4TagMessage
foreach ($fragment in @('Human acceptance testing: pending', 'Public Alpha publication: not authorized')) {
    $altered = $script:C3Alpha4TagMessage.Replace($fragment, 'removed')
    try {
        Assert-C3Alpha4TagMessage -Text $altered
        throw "Altered Alpha 4 tag message was accepted after removing '$fragment'."
    } catch {
        if ($_.Exception.Message -like 'Altered Alpha 4*') { throw }
    }
}
Write-Host 'Alpha 4 annotated-tag message contract passed.'
