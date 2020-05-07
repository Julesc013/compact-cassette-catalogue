Module varGlobals

    ' Global and Constant variables.
    ' All the things needed for the actual installation (like assembly information).
    ' All the settings and options for installing that needs to be passed to frmInstall are stored here.

    Public Const PROGRAMNAME As String = "Compact Cassette Catalogue"
    Public Const PROGRAMAUTHOR As String = "Jules Carboni"
    Public Const PROGRAMDESCRIPTION As String = "Catalogue your compact cassettes."
    Public programVersion As String ' Get this from the online VERSION file.

    'Public Const INSTALLDIRECTORYDEFAULT As String = "%PROGRAMFILES%\" & PROGRAMNAME & "\" ' If directory is unchanged in options, just use this.
    Public installDirectory As String = "C:\Program Files (x86)\" & PROGRAMNAME & "\"
    Public uninstallPath As String '= installDirectory & "\UNINSTALL.exe" ' Set this after options.
    Public iconPath As String '= installDirectory & "\application-icon.ico" ' Set this after options.

    Public Const PREFIXSOURCES As String = "C3-"

    'Public Const UNINSTALLSOURCE As String = PREFIXSOURCES & "Uninstaller.exe"
    Public Const UNINSTALLDESTINATION As String = "UNINSTALL.exe"
    'Public Const ICONSOURCE As String = PREFIXSOURCES & "Icon.ico"
    Public Const ICONDESTINATION As String = PROGRAMNAME & ".ico"

    'Public sourceFiles As String() = {Nothing} ', PREFIXSOURCES & "Updater.exe"} ' Set the name of the source file later (when we have the version number).
    'Public destinationFiles As String() = {PROGRAMNAME & ".exe"} ', "UPDATE.exe"}

    Public startFile As String = PROGRAMNAME & ".exe" 'destinationFiles(0) ' The executable that should be launched first when starting the program.
    Public startPath As String

    Public Const DOWNLOADLINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/" 'v1.0.0/C3-v1.0.0.exe
    Public Const VERSIONLINK As String = "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/VERSION" ' Raw Github file.

    Public Const HOMEPAGELINK As String = "https://julescarboni.wordpress.com/jules-carboni/software/compact-cassette-catalogue/" ' The homepage site for the program.
    Public Const READMELINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/blob/master/README.md" ' Readme file location (hosted remotely).
    Public Const UPDATELINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases" ' Site where updated can be downloaded.

    Public shortcutDesktop As Boolean = True
    Public shortcutStartMenu As Boolean = True

End Module
