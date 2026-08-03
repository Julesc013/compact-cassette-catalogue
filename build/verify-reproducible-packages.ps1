[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packagesRoot = Join-Path $repositoryRoot 'artifacts\packages'
$packageScript = Join-Path $PSScriptRoot 'package.ps1'

function Get-PackageSnapshot {
    if (-not (Test-Path -LiteralPath $packagesRoot -PathType Container)) {
        throw "Package output directory is missing: $packagesRoot"
    }

    $files = @(Get-ChildItem -LiteralPath $packagesRoot -File | Sort-Object Name)
    if ($files.Count -ne 3) {
        throw "Expected two portable ZIPs and SHA256SUMS.txt, found $($files.Count) files."
    }
    if (@($files | Where-Object { $_.Extension -ceq '.zip' }).Count -ne 2) {
        throw 'Expected exactly two portable ZIPs.'
    }
    if (@($files | Where-Object { $_.Name -ceq 'SHA256SUMS.txt' }).Count -ne 1) {
        throw 'Expected exactly one SHA256SUMS.txt.'
    }

    return @($files | ForEach-Object {
        [PSCustomObject]@{
            Name = $_.Name
            Length = $_.Length
            Sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        }
    })
}

function Invoke-PackagePass {
    param([int]$PassNumber)

    Write-Host "Reproducibility pass ${PassNumber}/2: full Release rebuild and package"
    & $packageScript | Out-Host
    return @(Get-PackageSnapshot)
}

$first = @(Invoke-PackagePass -PassNumber 1)
$second = @(Invoke-PackagePass -PassNumber 2)
$mismatches = New-Object Collections.Generic.List[String]

if ($first.Count -ne $second.Count) {
    $mismatches.Add("file count changed from $($first.Count) to $($second.Count)")
}

$comparisonCount = [Math]::Min($first.Count, $second.Count)
for ($index = 0; $index -lt $comparisonCount; $index++) {
    $left = $first[$index]
    $right = $second[$index]
    if ($left.Name -cne $right.Name) {
        $mismatches.Add("file name changed from '$($left.Name)' to '$($right.Name)'")
        continue
    }
    if ($left.Length -ne $right.Length) {
        $mismatches.Add("$($left.Name) length changed from $($left.Length) to $($right.Length)")
    }
    if ($left.Sha256 -cne $right.Sha256) {
        $mismatches.Add("$($left.Name) SHA-256 changed from $($left.Sha256) to $($right.Sha256)")
    }
}

if ($mismatches.Count -gt 0) {
    throw "Packages are not reproducible across full rebuilds:`n- $($mismatches -join "`n- ")"
}

Write-Host 'Reproducible package evidence:'
foreach ($record in $second) {
    Write-Host ("{0}  {1,10}  {2}" -f $record.Sha256, $record.Length, $record.Name)
}
Write-Host 'Portable packages are byte-identical across two full Release rebuilds.'
