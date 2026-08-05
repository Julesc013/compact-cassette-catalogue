[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha3-tag-message.ps1')

$validMessage = @'
C3 1.3.0 Alpha 3

This is a retained, unpublished engineering preview.
Historical Gate 1: passed
Legacy reliability repairs: qualified
Native x86/x64/ARM64 target execution: qualified
Optional classic setup: qualified
Public publication: not authorized
Beta-labelled artifacts: require explicit human approval
Public feed and legacy/1.x: unchanged
'@
Assert-C3Alpha3TagMessage -Text $validMessage

foreach ($fragment in $script:C3Alpha3TagMessageFragments) {
    $invalidMessage = $validMessage.Replace($fragment, '[missing]')
    try {
        Assert-C3Alpha3TagMessage -Text $invalidMessage
        throw "Alpha 3 tag-message test accepted a message missing '$fragment'."
    }
    catch {
        if ($_.Exception.Message -notmatch 'missing required message fragment') { throw }
    }
}

Write-Host 'Alpha 3 tag-message contract accepted the complete qualification/authority message and rejected every missing required fragment.'
