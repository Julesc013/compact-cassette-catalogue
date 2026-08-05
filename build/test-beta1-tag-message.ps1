[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'beta1-tag-message.ps1')

$valid = Get-C3Beta1TagMessage
Assert-C3Beta1TagMessage -Text $valid
foreach ($fragment in $script:C3Beta1TagMessageFragments) {
    try {
        Assert-C3Beta1TagMessage -Text $valid.Replace($fragment, '[missing]')
        throw "Beta 1 tag-message test accepted a message missing '$fragment'."
    }
    catch { if ($_.Exception.Message -notmatch 'missing required message fragment') { throw } }
}
Write-Host 'Beta 1 tag-message contract rejected every incomplete qualification/authority statement.'

