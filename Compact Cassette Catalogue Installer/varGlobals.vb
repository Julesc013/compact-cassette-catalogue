Module varGlobals

    ' Global and Constant variables.
    ' All the things needed for the actual installation (like assembly information).
    ' All the settings and options for installing that needs to be passed to frmInstall are stored here.

    Public Const PROGRAMNAME As String = "Compact Cassette Catalogue"
    Public Const PROGRAMAUTHOR As String = "Jules Carboni"
    Public programVersion As String ' Get this from the online VERSION file.

    Public Const INSTALLDIRECTORYDEFAULT As String = "%PROGRAMFILES%\" & PROGRAMNAME & "\" ' If directory is unchnged in options, just use this.
    Public installDirectory As String = "C:\Program Files (x86)\" & PROGRAMNAME & "\"
    Public uninstallPath As String '= installDirectory & "\UNINSTALL.exe" ' Set this after options.
    Public iconPath As String '= installDirectory & "\application-icon.ico" ' Set this after options.

    Public shortcutDesktop As Boolean = True
    Public shortcutStartMenu As Boolean = True

    Public Const VERSIONLINK As String = "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/VERSION" ' Raw Github file.

End Module
