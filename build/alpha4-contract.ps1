$script:C3Alpha4Contract = [ordered]@{
    releaseVersion = '1.3.0'
    releaseStage = 'Alpha 4'
    releaseLabel = '1.3.0a4'
    releaseTag = 'v1.3.0a4'
    releaseChannel = 'alpha'
    publicationStatus = 'retained-unpublished'
    assemblyVersion = '1.3.0.0'
    fileVersion = '1.3.0.0'
    assemblyProductVersion = '1.3.0a4'
}
$script:C3Alpha4Lanes = @('win-x86-net40', 'win-x64-net48', 'win-arm64-net481')
$script:C3Alpha4AssetNames = @(
    @($script:C3Alpha4Lanes | ForEach-Object { "C3-v1.3.0a4-$_-portable.zip" }) +
    @($script:C3Alpha4Lanes | ForEach-Object { "C3-v1.3.0a4-$_-setup.zip" })
)

function Assert-C3Alpha4Manifest {
    param([Parameter(Mandatory = $true)]$Manifest)
    foreach ($name in $script:C3Alpha4Contract.Keys) {
        if ([string]$Manifest.$name -cne [string]$script:C3Alpha4Contract[$name]) {
            throw "Alpha 4 manifest property '$name' is '$($Manifest.$name)', expected '$($script:C3Alpha4Contract[$name])'."
        }
    }
    $lanes = @($Manifest.lanes)
    if ($lanes.Count -ne 3) { throw 'Alpha 4 requires exactly three lanes.' }
    for ($index = 0; $index -lt $lanes.Count; $index++) {
        if ([string]$lanes[$index].id -cne $script:C3Alpha4Lanes[$index] -or
                [string]$lanes[$index].packageName -cne "C3-v1.3.0a4-$($script:C3Alpha4Lanes[$index])-portable.zip" -or
                [string]$lanes[$index].setupPackageName -cne "C3-v1.3.0a4-$($script:C3Alpha4Lanes[$index])-setup.zip") {
            throw "Alpha 4 lane $index has an invalid identity or package projection."
        }
    }
}
