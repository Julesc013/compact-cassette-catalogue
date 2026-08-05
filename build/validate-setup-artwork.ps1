[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$forms = @(
    [PSCustomObject]@{ project = 'Compact Cassette Catalogue Installer'; form = 'frmMain'; namespace = 'Compact_Cassette_Catalogue_Installer' },
    [PSCustomObject]@{ project = 'Compact Cassette Catalogue Installer'; form = 'frmFailure'; namespace = 'Compact_Cassette_Catalogue_Installer' },
    [PSCustomObject]@{ project = 'Compact Cassette Catalogue Installer'; form = 'frmSuccess'; namespace = 'Compact_Cassette_Catalogue_Installer' },
    [PSCustomObject]@{ project = 'Compact Cassette Catalogue Uninstaller'; form = 'frmFailure'; namespace = 'Compact_Cassette_Catalogue_Uninstaller' },
    [PSCustomObject]@{ project = 'Compact Cassette Catalogue Uninstaller'; form = 'frmSuccess'; namespace = 'Compact_Cassette_Catalogue_Uninstaller' }
)
foreach ($form in $forms) {
    $resxPath = Join-Path $repositoryRoot "$($form.project)\$($form.form).resx"
    [xml]$resx = Get-Content -LiteralPath $resxPath -Raw
    if (@($resx.root.data | Where-Object { [string]$_.name -ceq 'picSideBanner.Image' }).Count -ne 0) {
        throw "$($form.project)/$($form.form).resx still serializes a duplicate side-banner bitmap."
    }
    $designerPath = Join-Path $repositoryRoot "$($form.project)\$($form.form).Designer.vb"
    $designer = [IO.File]::ReadAllText($designerPath)
    $expected = "Me.picSideBanner.Image = Global.$($form.namespace).My.Resources.Resources.cassette_tapes_transparent_jpg"
    if ([regex]::Matches($designer, [regex]::Escape($expected)).Count -ne 1 -or $designer.Contains('resources.GetObject("picSideBanner.Image")')) {
        throw "$($form.project)/$($form.form) does not use exactly one shared project banner resource."
    }
}

$resourceHashes = New-Object Collections.Generic.List[String]
foreach ($project in @('Compact Cassette Catalogue Installer', 'Compact Cassette Catalogue Uninstaller')) {
    $resourcePath = Join-Path $repositoryRoot "$project\My Project\Resources.resx"
    [xml]$resources = Get-Content -LiteralPath $resourcePath -Raw
    $banner = @($resources.root.data | Where-Object { [string]$_.name -ceq 'cassette-tapes-transparent.jpg' })
    if ($banner.Count -ne 1 -or -not ([string]$banner[0].value).StartsWith('..\Resources\cassette-tapes-transparent.jpg2.png;', [StringComparison]::Ordinal)) {
        throw "$project does not expose exactly one canonical project banner resource."
    }
    $resourceHashes.Add((Get-FileHash -LiteralPath (Join-Path $repositoryRoot "$project\Resources\cassette-tapes-transparent.jpg2.png") -Algorithm SHA256).Hash.ToLowerInvariant())
}
if ($resourceHashes[0] -cne $resourceHashes[1]) { throw 'Installer and uninstaller canonical banner artwork bytes differ.' }
Write-Host "Setup artwork verified: five forms reuse one canonical banner per executable; duplicate serialized banner nodes=0; artwork SHA-256=$($resourceHashes[0])."
