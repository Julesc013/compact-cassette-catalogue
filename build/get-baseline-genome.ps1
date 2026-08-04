[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$BaselineRef = '509c9ec29679e30dcdcb1f57d8874b850cee310c'
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot).TrimEnd('\', '/')
$productionRoot = Join-Path $RepositoryRoot 'Compact Cassette Catalogue'
$projectPath = Join-Path $productionRoot 'Compact Cassette Catalogue.vbproj'
$settingsPath = Join-Path $productionRoot 'My Project\Settings.settings'
$globalsPath = Join-Path $productionRoot 'varGlobals.vb'

foreach ($requiredPath in @($productionRoot, $projectPath, $settingsPath, $globalsPath)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Cannot inventory the baseline genome because '$requiredPath' is missing."
    }
}

function Get-RelativePath {
    param([string]$Path)
    $fullPath = [IO.Path]::GetFullPath($Path)
    return $fullPath.Substring($RepositoryRoot.Length).TrimStart('\', '/') -replace '\\', '/'
}

function Get-NormalizedTextHash {
    param([string]$Path)
    $text = [IO.File]::ReadAllText($Path) -replace "`r`n", "`n" -replace "`r", "`n"
    $bytes = (New-Object Text.UTF8Encoding($false)).GetBytes($text)
    $sha256 = [Security.Cryptography.SHA256]::Create()
    try {
        return ([BitConverter]::ToString($sha256.ComputeHash($bytes)) -replace '-', '').ToLowerInvariant()
    }
    finally {
        $sha256.Dispose()
    }
}

function Get-XmlValue {
    param(
        [xml]$Document,
        [string]$LocalName
    )
    $node = $Document.SelectSingleNode("//*[local-name()='$LocalName']")
    if ($null -eq $node) {
        return $null
    }
    return [string]$node.InnerText
}

$baselineCommit = (& git -C $RepositoryRoot rev-parse "$BaselineRef^{commit}").Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($baselineCommit)) {
    throw "Cannot resolve baseline ref '$BaselineRef'."
}

$productionFiles = @(Get-ChildItem -LiteralPath $productionRoot -Recurse -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    ForEach-Object { Get-RelativePath $_.FullName } |
    Sort-Object)

$typeNames = New-Object Collections.Generic.List[Object]
$controlNames = New-Object Collections.Generic.List[Object]
$conditionalCompilation = New-Object Collections.Generic.List[Object]
$vbFiles = @(Get-ChildItem -LiteralPath $productionRoot -Recurse -Filter '*.vb' -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    Sort-Object FullName)

foreach ($file in $vbFiles) {
    $relativePath = Get-RelativePath $file.FullName
    $text = [IO.File]::ReadAllText($file.FullName)

    foreach ($match in [regex]::Matches(
            $text,
            '(?im)^\s*(?:(?:Public|Friend|Private|Protected)\s+)?(?:(?:Partial|NotInheritable|MustInherit)\s+)*(?<kind>Class|Module)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)')) {
        $typeNames.Add([pscustomobject][ordered]@{
            path = $relativePath
            kind = $match.Groups['kind'].Value
            name = $match.Groups['name'].Value
        })
    }

    if ($file.Name.EndsWith('.Designer.vb', [StringComparison]::OrdinalIgnoreCase)) {
        foreach ($match in [regex]::Matches(
                $text,
                '(?im)^\s*(?:Friend|Private|Protected|Public)\s+WithEvents\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s+As\s+(?<type>[^\r\n]+)')) {
            $controlNames.Add([pscustomobject][ordered]@{
                path = $relativePath
                name = $match.Groups['name'].Value
                type = $match.Groups['type'].Value.Trim()
            })
        }
    }

    foreach ($match in [regex]::Matches($text, '(?im)^\s*#If\s+(?<expression>.+?)\s+Then\s*$')) {
        $conditionalCompilation.Add([pscustomobject][ordered]@{
            path = $relativePath
            expression = $match.Groups['expression'].Value.Trim()
        })
    }
}

$resourceKeys = New-Object Collections.Generic.List[Object]
$resxFiles = @(Get-ChildItem -LiteralPath $productionRoot -Recurse -Filter '*.resx' -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    Sort-Object FullName)
foreach ($file in $resxFiles) {
    [xml]$resource = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($node in @($resource.SelectNodes("//*[local-name()='data']"))) {
        $resourceKeys.Add([pscustomobject][ordered]@{
            path = Get-RelativePath $file.FullName
            key = [string]$node.GetAttribute('name')
        })
    }
}

$designerHashes = @($vbFiles |
    Where-Object { $_.Name.EndsWith('.Designer.vb', [StringComparison]::OrdinalIgnoreCase) } |
    ForEach-Object {
        [pscustomobject][ordered]@{
            path = Get-RelativePath $_.FullName
            sha256 = Get-NormalizedTextHash $_.FullName
        }
    })
$resxHashes = @($resxFiles | ForEach-Object {
    [pscustomobject][ordered]@{
        path = Get-RelativePath $_.FullName
        sha256 = Get-NormalizedTextHash $_.FullName
    }
})

[xml]$project = Get-Content -LiteralPath $projectPath -Raw
$references = @($project.SelectNodes("//*[local-name()='Reference']") |
    ForEach-Object { ([string]$_.Include -split ',')[0] } |
    Sort-Object -Unique)

[xml]$settingsDocument = Get-Content -LiteralPath $settingsPath -Raw
$settings = @($settingsDocument.SelectNodes("//*[local-name()='Setting']") | ForEach-Object {
    $valueNode = $_.SelectSingleNode("*[local-name()='Value']")
    [pscustomobject][ordered]@{
        name = [string]$_.Name
        type = [string]$_.Type
        scope = [string]$_.Scope
        default = if ($null -eq $valueNode) { $null } else { [string]$valueNode.InnerText }
    }
})

$globals = [IO.File]::ReadAllText($globalsPath)
$tables = New-Object Collections.Generic.List[Object]
foreach ($functionMatch in [regex]::Matches(
        $globals,
        '(?ims)^\s*Function\s+make(?<function>[A-Za-z_][A-Za-z0-9_]*)\(\)\s+As\s+DataTable(?<body>.*?)^\s*End Function')) {
    $body = $functionMatch.Groups['body'].Value
    $columns = New-Object Collections.Generic.List[Object]
    foreach ($columnMatch in [regex]::Matches(
            $body,
            '(?im)^\s*(?!'')table\.Columns\.Add\(New\s+DataColumn\("(?<name>[^"]+)",\s*GetType\((?<type>[^\)]+)\)\)\)')) {
        $columns.Add([pscustomobject][ordered]@{
            name = $columnMatch.Groups['name'].Value
            type = $columnMatch.Groups['type'].Value.Trim()
        })
    }

    $tableNameMatch = [regex]::Match($body, 'table\.TableName\s*=\s*"(?<name>[^"]+)"')
    $primaryKeyMatch = [regex]::Match($body, 'table\.PrimaryKey\s*=.*?table\.Columns\((?<index>\d+)\)')
    $primaryKey = $null
    if ($primaryKeyMatch.Success) {
        $primaryIndex = [int]$primaryKeyMatch.Groups['index'].Value
        if ($primaryIndex -ge 0 -and $primaryIndex -lt $columns.Count) {
            $primaryKey = $columns[$primaryIndex].name
        }
    }

    $tableName = $null
    if ($tableNameMatch.Success) {
        $tableName = $tableNameMatch.Groups['name'].Value
    }
    $tables.Add([pscustomobject][ordered]@{
        factory = 'make' + $functionMatch.Groups['function'].Value
        name = $tableName
        columns = $columns.ToArray()
        primaryKey = $primaryKey
    })
}

function Get-ConstantValue {
    param([string]$Name)
    $pattern = '(?im)^\s*Public\s+Const\s+' + [regex]::Escape($Name) +
        '(?:\s+As\s+String)?\s*=\s*"(?<value>[^"]+)"'
    $match = [regex]::Match($globals, $pattern)
    if (-not $match.Success) {
        throw "Could not find baseline constant '$Name'."
    }
    return $match.Groups['value'].Value
}

$supportedMatch = [regex]::Match(
    $globals,
    '(?im)^\s*Public\s+ReadOnly\s+VERSIONFILESUPPORTED\s+As\s+String\(\)\s*=\s*\{(?<values>[^\}]+)\}')
if (-not $supportedMatch.Success) {
    throw 'Could not find VERSIONFILESUPPORTED.'
}
$supportedFormats = @([regex]::Matches($supportedMatch.Groups['values'].Value, '"(?<value>[^"]+)"') |
    ForEach-Object { $_.Groups['value'].Value })

$assetPaths = @(
    'Compact Cassette Catalogue/icon-cassette.ico',
    'Compact Cassette Catalogue/Resources/banner-wide.png',
    'Compact Cassette Catalogue/Resources/cassette-icon.png'
)
$assets = @($assetPaths | ForEach-Object {
    $assetPath = Join-Path $RepositoryRoot ($_ -replace '/', '\')
    if (-not (Test-Path -LiteralPath $assetPath -PathType Leaf)) {
        throw "Principal baseline asset is missing: $_"
    }
    [pscustomobject][ordered]@{
        path = $_
        sha256 = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})

$projectFiles = @(Get-ChildItem -LiteralPath $productionRoot -Recurse -Filter '*.vbproj' -File |
    ForEach-Object { Get-RelativePath $_.FullName } |
    Sort-Object)
$csharpFiles = @(Get-ChildItem -LiteralPath $productionRoot -Recurse -Filter '*.cs' -File |
    ForEach-Object { Get-RelativePath $_.FullName } |
    Sort-Object)

[pscustomobject][ordered]@{
    schemaVersion = 1
    baseline = [pscustomobject][ordered]@{
        ref = $BaselineRef
        commit = $baselineCommit
        productionRoot = 'Compact Cassette Catalogue/'
    }
    productionFiles = $productionFiles
    typeNames = $typeNames.ToArray()
    controlNames = $controlNames.ToArray()
    resourceKeys = $resourceKeys.ToArray()
    designerHashes = $designerHashes
    resxHashes = $resxHashes
    identity = [pscustomobject][ordered]@{
        projectGuid = Get-XmlValue $project 'ProjectGuid'
        rootNamespace = Get-XmlValue $project 'RootNamespace'
        assemblyName = Get-XmlValue $project 'AssemblyName'
        startupObject = Get-XmlValue $project 'StartupObject'
        targetFramework = Get-XmlValue $project 'TargetFrameworkVersion'
    }
    frameworkReferences = $references
    settings = $settings
    dataSet = [pscustomobject][ordered]@{
        variable = 'catalogue'
        dataSetName = 'Catalogue'
        tables = $tables.ToArray()
    }
    catalogue = [pscustomobject][ordered]@{
        format = Get-ConstantValue 'VERSIONFILE'
        supportedFormats = $supportedFormats
        updateEndpoint = Get-ConstantValue 'UPDATELINKCHECK'
        defaultUpdatePolicy = [string]($settings | Where-Object { $_.name -ceq 'checkUpdates' } | Select-Object -First 1).default
    }
    principalAssets = $assets
    sourcePolicy = [pscustomobject][ordered]@{
        projectFiles = $projectFiles
        csharpFiles = $csharpFiles
        conditionalCompilation = $conditionalCompilation.ToArray()
    }
}
