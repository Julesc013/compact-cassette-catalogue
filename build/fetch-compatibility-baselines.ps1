[CmdletBinding()]
param(
    [string]$DestinationRoot,
    [switch]$IncludeInventory,
    [switch]$Force
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DestinationRoot)) {
    $DestinationRoot = Join-Path $repositoryRoot 'artifacts\compatibility\official'
}
$resolvedDestination = [IO.Path]::GetFullPath($DestinationRoot)
$artifactRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts'))
if (-not $resolvedDestination.StartsWith(
        $artifactRoot + [IO.Path]::DirectorySeparatorChar,
        [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Compatibility baselines must be downloaded below the ignored artifacts directory.'
}

& (Join-Path $PSScriptRoot 'validate-compatibility-corpus.ps1')
$corpusPath = Join-Path $repositoryRoot 'fixtures\compatibility\1x\corpus.v1.json'
$corpus = Get-Content -LiteralPath $corpusPath -Raw | ConvertFrom-Json
$releases = @($corpus.releases | Where-Object {
        $IncludeInventory -or [string]$_.support -ceq 'supported'
    })

foreach ($release in $releases) {
    $releaseDirectory = Join-Path $resolvedDestination ([string]$release.tag)
    [IO.Directory]::CreateDirectory($releaseDirectory) | Out-Null
    foreach ($artifact in $release.artifacts) {
        $destination = Join-Path $releaseDirectory ([string]$artifact.name)
        if ((Test-Path -LiteralPath $destination) -and -not $Force) {
            $existing = Get-Item -LiteralPath $destination
            $existingHash = (Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash.ToLowerInvariant()
            if ($existing.Length -eq [long]$artifact.size -and
                    $existingHash -ceq [string]$artifact.sha256) {
                Write-Host "Verified cached baseline: $($artifact.name)"
                continue
            }
            throw "Cached baseline differs from the corpus: $destination"
        }

        $temporary = $destination + '.downloading'
        if (Test-Path -LiteralPath $temporary) {
            Remove-Item -LiteralPath $temporary -Force
        }
        try {
            Invoke-WebRequest -UseBasicParsing -Uri ([string]$artifact.url) -OutFile $temporary
            $download = Get-Item -LiteralPath $temporary
            $downloadHash = (Get-FileHash -LiteralPath $temporary -Algorithm SHA256).Hash.ToLowerInvariant()
            if ($download.Length -ne [long]$artifact.size -or
                    $downloadHash -cne [string]$artifact.sha256) {
                throw "Downloaded baseline failed size/SHA-256 verification: $($artifact.name)"
            }
            Move-Item -LiteralPath $temporary -Destination $destination -Force
            Write-Host "Fetched and verified baseline: $($artifact.name)"
        }
        finally {
            if (Test-Path -LiteralPath $temporary) {
                Remove-Item -LiteralPath $temporary -Force
            }
        }
    }
}
