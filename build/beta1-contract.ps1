$script:C3Beta1ReleaseIdentity = [ordered]@{
    schemaVersion = '3'
    product = 'Compact Cassette Catalogue'
    releaseVersion = '1.3.0'
    releaseStage = 'Beta 1'
    releaseLabel = '1.3.0b1'
    releaseTag = 'v1.3.0b1'
    releaseChannel = 'beta'
    publicationStatus = 'retained-unpublished'
    assemblyVersion = '1.3.0.0'
    fileVersion = '1.3.0.0'
    assemblyProductVersion = '1.3.0b1'
}

$script:C3Beta1LaneIds = @(
    'win-x86-net40',
    'win-x64-net48',
    'win-arm64-net481'
)

$script:C3Beta1PortableAssetNames = @($script:C3Beta1LaneIds | ForEach-Object {
        "C3-v1.3.0b1-$_-portable.zip"
    })
$script:C3Beta1SetupAssetNames = @($script:C3Beta1LaneIds | ForEach-Object {
        "C3-v1.3.0b1-$_-setup.zip"
    })
$script:C3Beta1AssetNames = @($script:C3Beta1PortableAssetNames + $script:C3Beta1SetupAssetNames)

function Assert-C3Beta1Manifest {
    param([Parameter(Mandatory = $true)]$Manifest)

    foreach ($propertyName in $script:C3Beta1ReleaseIdentity.Keys) {
        if ([string]$Manifest.$propertyName -cne [string]$script:C3Beta1ReleaseIdentity[$propertyName]) {
            throw "Beta 1 manifest property '$propertyName' is '$($Manifest.$propertyName)', expected '$($script:C3Beta1ReleaseIdentity[$propertyName])'."
        }
    }

    $lanes = @($Manifest.lanes)
    if ($lanes.Count -ne 3) {
        throw "Beta 1 requires exactly three lanes; found $($lanes.Count)."
    }
    for ($index = 0; $index -lt 3; $index++) {
        $lane = $lanes[$index]
        if ([string]$lane.id -cne $script:C3Beta1LaneIds[$index] -or
                [string]$lane.packageName -cne $script:C3Beta1PortableAssetNames[$index] -or
                [string]$lane.setupPackageName -cne $script:C3Beta1SetupAssetNames[$index] -or
                [string]$lane.status -cne 'required') {
            throw "Beta 1 lane '$index' does not have the exact ID, portable/setup asset names, and required status."
        }
    }

    return $Manifest
}

function Assert-C3Beta1ManifestPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $manifest = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    return Assert-C3Beta1Manifest -Manifest $manifest
}

