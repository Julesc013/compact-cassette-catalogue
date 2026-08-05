[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPaths = @(
    'Compact Cassette Catalogue Installer/Compact Cassette Catalogue Installer.vbproj',
    'Compact Cassette Catalogue Uninstaller/Compact Cassette Catalogue Uninstaller.vbproj'
)
$expectedReferences = @('System', 'System.Core', 'System.Drawing', 'System.Windows.Forms', 'System.Xml')
$expectedSharedSources = @(
    'InstalledState.vb',
    'SetupBundleRuntime.vb',
    'SetupContracts.vb',
    'SetupEnvironment.vb',
    'SetupFileTransaction.vb',
    'SetupInstallOperation.vb',
    'SetupRegistry.vb',
    'SetupRemovalTransaction.vb',
    'SetupSelfRelocation.vb',
    'SetupShortcuts.vb',
    'SetupUninstallOperation.vb'
)

foreach ($relativeProjectPath in $projectPaths) {
    $projectPath = Join-Path $repositoryRoot $relativeProjectPath
    [xml]$project = Get-Content -LiteralPath $projectPath -Raw
    $namespace = New-Object Xml.XmlNamespaceManager($project.NameTable)
    $namespace.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')
    $references = @($project.SelectNodes('//msb:Reference', $namespace) | ForEach-Object { ([string]$_.Include).Split(',')[0] } | Sort-Object -Unique)
    if (($references -join "`n") -cne (($expectedReferences | Sort-Object) -join "`n")) {
        throw "$relativeProjectPath contains a reference outside the closed classic setup framework set: $($references -join ', ')"
    }
    $sharedSources = @($project.SelectNodes('//msb:Compile', $namespace) |
        ForEach-Object { [string]$_.Include } |
        Where-Object { $_ -like '..\SetupShared\*' } |
        ForEach-Object { [IO.Path]::GetFileName($_) } |
        Sort-Object -Unique)
    if (($sharedSources -join "`n") -cne (($expectedSharedSources | Sort-Object) -join "`n")) {
        throw "$relativeProjectPath does not compile the exact source-identical shared setup engine."
    }
    $webBootstrapper = @($project.SelectNodes('//msb:IsWebBootstrapper', $namespace))
    if ($webBootstrapper.Count -ne 1 -or [string]$webBootstrapper[0].InnerText -cne 'false') {
        throw "$relativeProjectPath must keep web bootstrapping disabled."
    }
}

$sourceRoots = @('SetupShared', 'Compact Cassette Catalogue Installer', 'Compact Cassette Catalogue Uninstaller')
$forbiddenPatterns = [ordered]@{
    'System.Net import' = '(?im)^\s*Imports\s+System\.Net(?:\.|\s*$)'
    'WebClient' = '(?i)\bWebClient\b'
    'HttpClient' = '(?i)\bHttpClient\b'
    'WebRequest' = '(?i)\b(?:HttpWebRequest|FtpWebRequest|WebRequest\.Create)\b'
    'download call' = '(?i)\.Download(?:File|Data|String|FileTaskAsync|DataTaskAsync|StringTaskAsync)\s*\('
    'archive extraction' = '(?i)\b(?:ZipFile|ZipArchive)\b'
    'ClickOnce runtime' = '(?i)\bApplicationDeployment\b'
}
$sourceFiles = @($sourceRoots | ForEach-Object {
        Get-ChildItem -LiteralPath (Join-Path $repositoryRoot $_) -Filter '*.vb' -File -Recurse
    })
foreach ($sourceFile in $sourceFiles) {
    $text = [IO.File]::ReadAllText($sourceFile.FullName)
    foreach ($name in $forbiddenPatterns.Keys) {
        if ($text -match [string]$forbiddenPatterns[$name]) {
            throw "Offline setup source contains prohibited $name in '$($sourceFile.FullName.Substring($repositoryRoot.Length + 1))'."
        }
    }
}

Write-Host "Offline setup authority verified: exact framework references/shared engine and no network, archive-extraction, or ClickOnce runtime path across $($sourceFiles.Count) VB sources."
