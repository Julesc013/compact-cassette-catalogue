function Assert-C3NoPublicBetaRelease {
    param([Parameter(Mandatory = $true)][string]$RemoteUrl)

    if ($RemoteUrl -notmatch 'github\.com[/:](?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?$') {
        throw 'Beta public-release check requires a GitHub origin URL.'
    }
    $api = "https://api.github.com/repos/$($Matches.owner)/$($Matches.repo)/releases/tags/v1.3.0b1"
    try {
        $null = Invoke-WebRequest -Uri $api -Headers @{ 'User-Agent' = 'C3-Beta1-Control' } -UseBasicParsing
        throw 'A public GitHub release already exists for v1.3.0b1; this operation is not authorized.'
    }
    catch {
        $response = $_.Exception.Response
        if ($null -eq $response -or [int]$response.StatusCode -ne 404) { throw }
    }
    return $api
}

