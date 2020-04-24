Module varGlobals

    ' Global and Constant variables.
    ' All the things needed for the actual installation (like assembly information).
    ' All the settings and options for installing that needs to be passed to frmInstall are stored here.

    Public Const PROGRAMNAME As String = "Compact Cassette Catalogue"
    Public Const PROGRAMAUTHOR As String = "Jules Carboni"
    Public programVersion As String ' Get this from the online VERSION file.

    'Public Const INSTALLDIRECTORYDEFAULT As String = "%PROGRAMFILES%\" & PROGRAMNAME & "\" ' If directory is unchanged in options, just use this.
    Public installDirectory As String = "C:\Program Files (x86)\" & PROGRAMNAME & "\"
    Public uninstallPath As String '= installDirectory & "\UNINSTALL.exe" ' Set this after options.
    Public iconPath As String '= installDirectory & "\application-icon.ico" ' Set this after options.

    Public Const PREFIXSOURCES As String = "C3-"

    Public Const UNINSTALLSOURCE As String = PREFIXSOURCES & "Uninstaller.exe"
    Public Const UNINSTALLDESTINATION As String = "UNINSTALL.exe"
    Public Const ICONSOURCE As String = PREFIXSOURCES & "Icon.ico"
    Public Const ICONDESTINATION As String = "application-icon.ico"

    Public sourceFiles As String() = {Nothing, PREFIXSOURCES & "Updater.exe"} ' Set the name of the source file later (when we have the version number).
    Public destinationFiles As String() = {PROGRAMNAME & ".exe", "UPDATE.exe"}

    Public Const DOWNLOADLINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases/download/" 'v1.0.0/C3-v1.0.0.exe
    Public Const VERSIONLINK As String = "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/VERSION" ' Raw Github file.

    Public shortcutDesktop As Boolean = True
    Public shortcutStartMenu As Boolean = True

End Module
