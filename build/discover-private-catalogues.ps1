[CmdletBinding()]
param(
    [string]$OutputPath,
    [string[]]$SearchRoots,
    [int]$MaximumXmlFiles = 10000,
    [long]$MaximumFileBytes = 67108864
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $repositoryRoot 'artifacts\evidence\historical-gate1\private-catalogues.json'
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts')).TrimEnd('\') + '\'
if (-not $OutputPath.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Private catalogue evidence must remain below ignored artifact root '$artifactsRoot'."
}
if ($null -eq $SearchRoots -or $SearchRoots.Count -eq 0) {
    $documents = [Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)
    $SearchRoots = @(
        $documents,
        (Join-Path $documents 'Compact Cassette Catalogue'),
        (Join-Path $env:APPDATA 'Compact Cassette Catalogue'),
        (Join-Path $env:LOCALAPPDATA 'Compact Cassette Catalogue')
    ) | Select-Object -Unique
}

function Test-C3CatalogueRoot {
    param([Parameter(Mandatory = $true)][string]$Path, [Parameter(Mandatory = $true)][long]$MaximumBytes)
    $item = Get-Item -LiteralPath $Path
    if ($item.Length -gt $MaximumBytes -or ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { return $false }
    $settings = New-Object Xml.XmlReaderSettings
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $settings.XmlResolver = $null
    $settings.CloseInput = $true
    $settings.MaxCharactersInDocument = $MaximumBytes
    $stream = $null
    $reader = $null
    try {
        $stream = New-Object IO.FileStream($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
        $reader = [Xml.XmlReader]::Create($stream, $settings)
        while ($reader.Read()) {
            if ($reader.NodeType -eq [Xml.XmlNodeType]::Element) {
                return ($reader.LocalName -ceq 'Catalogue' -and [string]::IsNullOrEmpty($reader.NamespaceURI))
            }
        }
        return $false
    }
    catch { return $false }
    finally {
        if ($null -ne $reader) { $reader.Close() }
        if ($null -ne $stream) { $stream.Dispose() }
    }
}

$rootRecords = New-Object Collections.Generic.List[Object]
$found = New-Object Collections.Generic.List[Object]
$examinedTotal = 0
foreach ($root in $SearchRoots) {
    $rootPath = [IO.Path]::GetFullPath($root)
    $examined = 0
    $matches = 0
    $status = 'absent'
    if (Test-Path -LiteralPath $rootPath -PathType Container) {
        $status = 'searched-once'
        $files = @(Get-ChildItem -LiteralPath $rootPath -Filter '*.xml' -File -Recurse -ErrorAction SilentlyContinue)
        foreach ($file in $files) {
            if ($examinedTotal -ge $MaximumXmlFiles) { $status = 'truncated'; break }
            $examined++
            $examinedTotal++
            if (Test-C3CatalogueRoot -Path $file.FullName -MaximumBytes $MaximumFileBytes) {
                $matches++
                $found.Add([PSCustomObject]@{
                    path = $file.FullName
                    length = [long]$file.Length
                    sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
                    lastWriteUtc = $file.LastWriteTimeUtc.ToString('o')
                })
            }
        }
    }
    $rootRecords.Add([PSCustomObject]@{ root = $rootPath; status = $status; xmlExamined = $examined; catalogueMatches = $matches })
    if ($examinedTotal -ge $MaximumXmlFiles) { break }
}

$unique = @($found | Group-Object sha256 | ForEach-Object { $_.Group | Select-Object -First 1 } | Sort-Object sha256)
$catalogues = New-Object Collections.Generic.List[Object]
for ($i = 0; $i -lt $unique.Count; $i++) {
    $catalogues.Add([PSCustomObject]@{
        scenarioId = ('PRIVATE-CATALOGUE-{0:D3}' -f ($i + 1))
        sha256 = $unique[$i].sha256
        length = $unique[$i].length
        path = $unique[$i].path
        lastWriteUtc = $unique[$i].lastWriteUtc
    })
}
$record = [ordered]@{
    schemaVersion = 1
    classification = 'private-catalogue-index-retained-outside-git'
    recordedAtUtc = [DateTime]::UtcNow.ToString('o')
    repositoryCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    policy = [ordered]@{
        contentCopiedToGit = $false
        dtdProhibited = $true
        externalResolutionDisabled = $true
        maximumXmlFiles = $MaximumXmlFiles
        maximumFileBytes = $MaximumFileBytes
    }
    roots = $rootRecords.ToArray()
    xmlFilesExamined = $examinedTotal
    uniqueCatalogueCount = $catalogues.Count
    catalogues = $catalogues.ToArray()
}
New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null
[IO.File]::WriteAllText($OutputPath, (($record | ConvertTo-Json -Depth 8) + "`n"), (New-Object Text.UTF8Encoding($false)))
Write-Host "Private Catalogue-root discovery retained outside Git: $OutputPath ($($catalogues.Count) unique hash(es))."
return $record
