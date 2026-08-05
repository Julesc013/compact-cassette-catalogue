function Get-C3VisualStudioServicingVersion {
    param(
        [Parameter(Mandatory = $true)][string]$ProductVersion,
        [Parameter(Mandatory = $true)][string]$Context
    )

    $match = [regex]::Match($ProductVersion, '^\d+(?:\.\d+)+')
    if (-not $match.Success) {
        throw "$Context Visual Studio product version '$ProductVersion' is not a parseable servicing version."
    }
    return [version]$match.Value
}

function Assert-C3VisualStudioServicingFloor {
    param(
        [Parameter(Mandatory = $true)][string]$ProductVersion,
        [Parameter(Mandatory = $true)][string]$MinimumVersion,
        [Parameter(Mandatory = $true)][string]$Context
    )

    $installed = Get-C3VisualStudioServicingVersion `
        -ProductVersion $ProductVersion `
        -Context $Context
    $floor = Get-C3VisualStudioServicingVersion `
        -ProductVersion $MinimumVersion `
        -Context "$Context servicing floor"
    if ($installed -lt $floor) {
        throw "$Context Visual Studio '$ProductVersion' is older than decision-date servicing floor '$MinimumVersion'."
    }
    return $installed
}
