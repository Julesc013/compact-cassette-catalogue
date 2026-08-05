[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'alpha2-tag-message.ps1')

$validMessage = @'
C3 1.3.0 Alpha 2

This is an unpublished engineering preview.
Historical Gate 1: deferred
Runtime repairs: deferred
Native ARM64 execution: deferred
Target-OS qualification: deferred
Public publication: not authorized
'@
Assert-C3Alpha2TagMessage -Text $validMessage

foreach ($fragment in $script:C3Alpha2TagMessageFragments) {
    $invalidMessage = $validMessage.Replace($fragment, '[missing]')
    try {
        Assert-C3Alpha2TagMessage -Text $invalidMessage
        throw "Alpha 2 tag-message test accepted a message missing '$fragment'."
    }
    catch {
        if ($_.Exception.Message -notmatch 'missing required message fragment') { throw }
    }
}

Write-Host 'Alpha 2 tag-message contract accepted the complete message and rejected every missing required fragment.'
