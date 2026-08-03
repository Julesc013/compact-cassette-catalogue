[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$expectations = @(
    @{
        Lane = 'win-x86-net40'
        Path = "artifacts\bin\win-x86-net40\$Configuration\Compact Cassette Catalogue.exe"
        Machine = 0x014c
        OptionalHeader = 0x010b
    },
    @{
        Lane = 'win-x64-net48'
        Path = "artifacts\bin\win-x64-net48\$Configuration\Compact Cassette Catalogue.exe"
        Machine = 0x8664
        OptionalHeader = 0x020b
    }
)

foreach ($expectation in $expectations) {
    $path = Join-Path $repositoryRoot $expectation.Path
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Missing executable for $($expectation.Lane): $path"
    }

    $stream = [IO.File]::Open($path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    $reader = New-Object IO.BinaryReader($stream)
    try {
        $stream.Position = 0x3c
        $peOffset = $reader.ReadInt32()
        $stream.Position = $peOffset
        $signature = $reader.ReadUInt32()
        $machine = $reader.ReadUInt16()
        $stream.Position = $peOffset + 24
        $optionalHeader = $reader.ReadUInt16()
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }

    if ($signature -ne 0x00004550) {
        throw "$($expectation.Lane) does not contain a valid PE signature."
    }
    if ($machine -ne $expectation.Machine -or $optionalHeader -ne $expectation.OptionalHeader) {
        throw ("{0} has unexpected PE headers: machine=0x{1:x4}, optional=0x{2:x4}." -f
            $expectation.Lane, $machine, $optionalHeader)
    }

    Write-Host ("PE verified: {0} machine=0x{1:x4}, optional=0x{2:x4}" -f
        $expectation.Lane, $machine, $optionalHeader)
}

