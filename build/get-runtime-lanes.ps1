# PowerShell 2-compatible target projection of the release manifest. The
# preparation validator mechanically compares this projection to lanes.json.
@(
    (New-Object PSObject -Property @{
        id = 'win-x86-net40'
        releaseVersion = '1.3.0'
        releaseStage = 'Alpha 4'
        releaseLabel = '1.3.0a4'
        releaseTag = 'v1.3.0a4'
        releaseChannel = 'alpha'
        publicationStatus = 'retained-unpublished'
        packageName = 'C3-v1.3.0a4-win-x86-net40-portable.zip'
        setupPackageName = 'C3-v1.3.0a4-win-x86-net40-setup.zip'
        targetFramework = 'v4.0'
        peMachine = '0x014c'
        runtimeEnvironmentId = 'xp-sp3-x86-net40'
        runtimeArchitecture = 'x86'
        runtimeClaim = 'Windows XP SP3 x86 with .NET Framework 4.0 Full'
    }),
    (New-Object PSObject -Property @{
        id = 'win-x64-net48'
        releaseVersion = '1.3.0'
        releaseStage = 'Alpha 4'
        releaseLabel = '1.3.0a4'
        releaseTag = 'v1.3.0a4'
        releaseChannel = 'alpha'
        publicationStatus = 'retained-unpublished'
        packageName = 'C3-v1.3.0a4-win-x64-net48-portable.zip'
        setupPackageName = 'C3-v1.3.0a4-win-x64-net48-setup.zip'
        targetFramework = 'v4.8'
        peMachine = '0x8664'
        runtimeEnvironmentId = 'windows-7-sp1-x64-net48'
        runtimeArchitecture = 'x64'
        runtimeClaim = 'Windows 7 SP1 x64 with .NET Framework 4.8'
    }),
    (New-Object PSObject -Property @{
        id = 'win-arm64-net481'
        releaseVersion = '1.3.0'
        releaseStage = 'Alpha 4'
        releaseLabel = '1.3.0a4'
        releaseTag = 'v1.3.0a4'
        releaseChannel = 'alpha'
        publicationStatus = 'retained-unpublished'
        packageName = 'C3-v1.3.0a4-win-arm64-net481-portable.zip'
        setupPackageName = 'C3-v1.3.0a4-win-arm64-net481-setup.zip'
        targetFramework = 'v4.8.1'
        peMachine = '0xaa64'
        runtimeEnvironmentId = 'windows-11-21h2-arm64-net481'
        runtimeArchitecture = 'ARM64'
        runtimeClaim = 'Windows 11 21H2/RTM ARM64 with separately installed .NET Framework 4.8.1'
    })
)
