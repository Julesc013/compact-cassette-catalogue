[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$pathResolver = Join-Path $PSScriptRoot 'get-update-manifest-path.ps1'
$syncVersion = Join-Path $PSScriptRoot 'sync-version.ps1'
$schemaValidator = Join-Path $PSScriptRoot 'validate-json-document.ps1'
$feedSchema = Join-Path $repositoryRoot 'spec\update-feed\v1\release.schema.json'
$publishedExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\published-beta.example.json')
$invalidUnpublishedExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\invalid-unpublished-assets.example.json')
$invalidDuplicateRootExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\invalid-duplicate-root.example.json')
$invalidDuplicatePackageExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\invalid-duplicate-package-field.example.json')
$invalidPublishedAlphaExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\invalid-published-alpha.example.json')
$invalidEmptyBuildIdentifierExample = Join-Path $repositoryRoot (
    'spec\update-feed\v1\examples\invalid-empty-build-identifier.example.json')
$identity = & (Join-Path $PSScriptRoot 'get-release-identity.ps1')
$manifestPath = & $pathResolver -Identity $identity -RepositoryRoot $repositoryRoot
$expectedCurrentManifestPath = if ($identity.ReleaseChannel -ceq 'alpha') {
    [IO.Path]::GetFullPath(
        (Join-Path $repositoryRoot 'release\feeds\alpha\release.json'))
}
else {
    [IO.Path]::GetFullPath(
        (Join-Path $repositoryRoot (
                'release\candidates\' + $identity.ReleaseLabel + '\release.json')))
}
$buildInfoPath = Join-Path $repositoryRoot 'src\C3.WinForms\Generated\BuildInfo.g.vb'
$versionAssemblyInfoPath = Join-Path $repositoryRoot (
    'src\Shared\Generated\VersionAssemblyInfo.g.vb')
$updateBranchesPath = Join-Path $repositoryRoot (
    'src\Shared\Generated\UpdateBranches.g.vb')
$csharpVersionAssemblyInfoPath = Join-Path $repositoryRoot (
    'src\Shared\Generated\VersionAssemblyInfo.g.cs')
$csharpUpdateBranchesPath = Join-Path $repositoryRoot (
    'src\Shared\Generated\UpdateBranches.g.cs')
$generatedProjectionPaths = @(
    $manifestPath,
    $buildInfoPath,
    $versionAssemblyInfoPath,
    $csharpVersionAssemblyInfoPath,
    $updateBranchesPath,
    $csharpUpdateBranchesPath)
$generatedProjectionRelativePaths = @(
    $manifestPath.Substring($repositoryRoot.Length + 1).Replace('\', '/'),
    'src/C3.WinForms/Generated/BuildInfo.g.vb',
    'src/Shared/Generated/VersionAssemblyInfo.g.vb',
    'src/Shared/Generated/VersionAssemblyInfo.g.cs',
    'src/Shared/Generated/UpdateBranches.g.vb',
    'src/Shared/Generated/UpdateBranches.g.cs')
$passed = 0

function Assert-Equal {
    param(
        [string]$Expected,
        [string]$Actual,
        [string]$Scenario
    )

    if ($Expected -cne $Actual) {
        throw "$Scenario`: expected '$Expected', found '$Actual'."
    }
    $script:passed++
}

function Invoke-ScriptProcess {
    param(
        [string]$Executable,
        [string]$ScriptPath,
        [string[]]$Arguments = @()
    )

    $output = @(& $Executable -NoLogo -NoProfile -NonInteractive `
        -ExecutionPolicy Bypass -File $ScriptPath @Arguments 2>&1)
    if ($LASTEXITCODE -ne 0) {
        throw "'$Executable -File $ScriptPath' failed:`n$($output -join "`n")"
    }
    return $output
}

function Assert-ScriptFails {
    param(
        [string]$Executable,
        [string]$ScriptPath,
        [string[]]$Arguments,
        [string]$Scenario
    )

    $previousErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        [void](& $Executable -NoLogo -NoProfile -NonInteractive `
            -ExecutionPolicy Bypass -File $ScriptPath @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }
    if ($exitCode -eq 0) {
        throw "$Scenario`: expected the script to fail."
    }
    $script:passed++
}

Assert-Equal $expectedCurrentManifestPath $manifestPath 'current manifest path'

$betaIdentity = [PSCustomObject]@{
    ReleaseChannel = 'beta'
    ReleaseLabel = '2.0.0-beta.1'
    InformationalVersion = '2.0.0-beta.1+local.9'
}
$expectedBetaPath = [IO.Path]::GetFullPath(
    (Join-Path $repositoryRoot 'release\candidates\2.0.0-beta.1\release.json'))
Assert-Equal $expectedBetaPath (
    & $pathResolver -Identity $betaIdentity -RepositoryRoot $repositoryRoot) (
    'beta candidate uses the release label')

$releaseCandidateIdentity = [PSCustomObject]@{
    ReleaseChannel = 'beta'
    ReleaseLabel = '2.0.0-rc.1'
    InformationalVersion = '2.0.0-rc.1+local.9'
}
$expectedReleaseCandidatePath = [IO.Path]::GetFullPath(
    (Join-Path $repositoryRoot 'release\candidates\2.0.0-rc.1\release.json'))
Assert-Equal $expectedReleaseCandidatePath (
    & $pathResolver -Identity $releaseCandidateIdentity -RepositoryRoot $repositoryRoot) (
    'release-candidate staging uses the release label')

$stableIdentity = [PSCustomObject]@{
    ReleaseChannel = 'stable'
    ReleaseLabel = '2.0.0'
    InformationalVersion = '2.0.0+local.9'
}
$expectedStablePath = [IO.Path]::GetFullPath(
    (Join-Path $repositoryRoot 'release\candidates\2.0.0\release.json'))
Assert-Equal $expectedStablePath (
    & $pathResolver -Identity $stableIdentity -RepositoryRoot $repositoryRoot) (
    'stable candidate uses the release label')

$windowsPowerShell = Get-Command 'powershell.exe' -ErrorAction Stop
$powerShell = Get-Command 'pwsh.exe' -ErrorAction Stop
foreach ($shell in @($windowsPowerShell.Source, $powerShell.Source)) {
    $directPathOutput = @(Invoke-ScriptProcess `
        -Executable $shell `
        -ScriptPath $pathResolver)
    Assert-Equal $expectedCurrentManifestPath ([string]$directPathOutput[-1]) (
        "direct -File path resolution under $shell")

    [void](Invoke-ScriptProcess `
        -Executable $shell `
        -ScriptPath $schemaValidator `
        -Arguments @('-SchemaPath', $feedSchema, '-DocumentPath', $publishedExample))
    $passed++

    Assert-ScriptFails `
        -Executable $shell `
        -ScriptPath $schemaValidator `
        -Arguments @(
            '-SchemaPath', $feedSchema,
            '-DocumentPath', $invalidUnpublishedExample) `
        -Scenario "unpublished asset rejection under $shell"

    Assert-ScriptFails `
        -Executable $shell `
        -ScriptPath $schemaValidator `
        -Arguments @(
            '-SchemaPath', $feedSchema,
            '-DocumentPath', $invalidPublishedAlphaExample) `
        -Scenario "published alpha rejection under $shell"

    Assert-ScriptFails `
        -Executable $shell `
        -ScriptPath $schemaValidator `
        -Arguments @(
            '-SchemaPath', $feedSchema,
            '-DocumentPath', $invalidEmptyBuildIdentifierExample) `
        -Scenario "empty build identifier rejection under $shell"

    foreach ($duplicateFixture in @(
            $invalidDuplicateRootExample,
            $invalidDuplicatePackageExample)) {
        Assert-ScriptFails `
            -Executable $shell `
            -ScriptPath $schemaValidator `
            -Arguments @(
                '-SchemaPath', $feedSchema,
                '-DocumentPath', $duplicateFixture) `
            -Scenario "duplicate property rejection under $shell"
    }
}

[void](Invoke-ScriptProcess -Executable $windowsPowerShell.Source -ScriptPath $syncVersion)
$windowsProjectionBytes = @{}
foreach ($projectionPath in $generatedProjectionPaths) {
    $windowsProjectionBytes[$projectionPath] =
        [Convert]::ToBase64String([IO.File]::ReadAllBytes($projectionPath))
}
[void](Invoke-ScriptProcess -Executable $powerShell.Source -ScriptPath $syncVersion)
foreach ($projectionPath in $generatedProjectionPaths) {
    Assert-Equal $windowsProjectionBytes[$projectionPath] (
        [Convert]::ToBase64String([IO.File]::ReadAllBytes($projectionPath))) (
        "Windows PowerShell 5.1 and PowerShell 7 bytes for $projectionPath")
}

$releaseDateText = $identity.ReleaseDate.ToString(
    'yyyy-MM-dd',
    [Globalization.CultureInfo]::InvariantCulture)
$expectedJson = @(
    '{'
    '  "schemaVersion": 1,'
    '  "product": "Compact Cassette Catalogue",'
    '  "productId": "c3",'
    '  "channel": "' + $identity.ReleaseChannel + '",'
    '  "version": "' + $identity.ProductVersion + '",'
    '  "stage": "' + $identity.ReleaseStage + '",'
    '  "informationalVersion": "' + $identity.InformationalVersion + '",'
    '  "releaseDate": "' + $releaseDateText + '",'
    '  "catalogueWriteFormat": "' + $identity.CatalogueFormatVersion + '",'
    '  "published": false,'
    '  "releaseUrl": null,'
    '  "checksumManifest": null,'
    '  "packages": []'
    '}'
) -join "`n"
Assert-Equal ($expectedJson + "`n") (
    [IO.File]::ReadAllText($manifestPath, (New-Object Text.UTF8Encoding($false, $true)))) (
    'canonical unpublished manifest projection')

if (Test-Path -LiteralPath (Join-Path $repositoryRoot '.git')) {
    foreach ($relativePath in $generatedProjectionRelativePaths) {
        & git -C $repositoryRoot ls-files --error-unmatch -- $relativePath `
            1>$null 2>$null
        if ($LASTEXITCODE -ne 0) {
            throw "Generated projection is not tracked by Git: $relativePath"
        }
    }
    $projectionStatus = @(& git -C $repositoryRoot status `
            --porcelain=v1 `
            --untracked-files=all `
            -- `
            @generatedProjectionRelativePaths)
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not inspect generated update/version projection state.'
    }
    if ($projectionStatus.Count -ne 0) {
        throw "Generated update/version projections differ from committed canonical bytes:`n$($projectionStatus -join "`n")"
    }
    $passed++
}
else {
    Write-Host 'Git metadata is unavailable; skipped checked-in projection comparison.'
}

Write-Host "Update-feed contract tests passed: $passed scenarios."
