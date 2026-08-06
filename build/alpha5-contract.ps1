$script:C3Alpha5Contract = [ordered]@{
    releaseVersion = '1.3.0'
    releaseStage = 'Alpha 5'
    releaseLabel = '1.3.0a5'
    releaseTag = 'v1.3.0a5'
    releaseChannel = 'alpha'
    publicationStatus = 'retained-unpublished'
    assemblyVersion = '1.3.0.0'
    fileVersion = '1.3.0.0'
    assemblyProductVersion = '1.3.0a5'
}
$script:C3Alpha5Lanes = @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')
$script:C3Alpha5AssetNames = @(
    @($script:C3Alpha5Lanes | ForEach-Object { "C3-v1.3.0a5-$_-portable.zip" }) +
    @($script:C3Alpha5Lanes | ForEach-Object { "C3-v1.3.0a5-$_-setup.zip" })
)

function Assert-C3Alpha5Manifest {
    param([Parameter(Mandatory = $true)]$Manifest)

    foreach ($name in $script:C3Alpha5Contract.Keys) {
        if ([string]$Manifest.$name -cne [string]$script:C3Alpha5Contract[$name]) {
            throw "Alpha 5 manifest property '$name' is '$($Manifest.$name)', expected '$($script:C3Alpha5Contract[$name])'."
        }
    }
    $lanes = @($Manifest.lanes)
    if ($lanes.Count -ne 3) { throw 'Alpha 5 requires exactly three lanes.' }
    for ($index = 0; $index -lt $lanes.Count; $index++) {
        $laneId = $script:C3Alpha5Lanes[$index]
        if ([string]$lanes[$index].id -cne $laneId -or
                [string]$lanes[$index].packageName -cne "C3-v1.3.0a5-$laneId-portable.zip" -or
                [string]$lanes[$index].setupPackageName -cne "C3-v1.3.0a5-$laneId-setup.zip") {
            throw "Alpha 5 lane $index has an invalid identity or package projection."
        }
    }
}

function Assert-C3Alpha5Distribution {
    param(
        [Parameter(Mandatory = $true)][string]$Directory,
        $Record
    )

    $expected = @($script:C3Alpha5AssetNames) + @('SHA256SUMS.txt')
    $actual = @(Get-ChildItem -LiteralPath $Directory -File | Sort-Object Name | ForEach-Object { $_.Name })
    if (($actual -join "`n") -cne (($expected | Sort-Object) -join "`n")) {
        throw "Alpha 5 distribution is not the exact six-ZIP/checksum set: $($actual -join ', ')"
    }
    $checksumLines = @(Get-Content -LiteralPath (Join-Path $Directory 'SHA256SUMS.txt'))
    if ($checksumLines.Count -ne 6) { throw 'Alpha 5 SHA256SUMS.txt must contain exactly six records.' }
    foreach ($name in $script:C3Alpha5AssetNames) {
        $path = Join-Path $Directory $name
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        if (@($checksumLines | Where-Object { $_ -ceq "$hash  $name" }).Count -ne 1) {
            throw "Alpha 5 checksum closure failed for '$name'."
        }
        if ($null -ne $Record) {
            $entry = @($Record.assets | Where-Object { [string]$_.name -ceq $name })
            if ($entry.Count -ne 1 -or [string]$entry[0].sha256 -cne $hash -or
                    [long]$entry[0].size -ne [long](Get-Item -LiteralPath $path).Length) {
                throw "Alpha 5 retained record does not match '$name'."
            }
        }
    }
}
