Imports System.IO
Imports System.Net
Imports IWshRuntimeLibrary

Public Class frmMain

    ' Name: Compact Cassette Catalogue Installer
    ' Date: 21 April 2020
    ' Author: Jules Carboni
    ' Purpose: Downloads and installs the latest version of Compact Cassette Catalogue.

    Dim pageIndex As Integer = 0
    Const PAGECOUNT As Integer = 3

    'Dim directoryChanged As Boolean = False

    Private Function confirmCancel() As MsgBoxResult

        Return MsgBox("Are you sure you want to cancel Compact Cassette Catalogue installation?", MsgBoxStyle.YesNo, "Comfirm Cancel Installation")

    End Function

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Load defaults into objects.

        txtDirectory.Text = installDirectory

        chkDesktop.Checked = shortcutDesktop
        chkStartMenu.Checked = shortcutStartMenu

        dialogDirectory.SelectedPath = installDirectory

        ' Set focus on Next button.
        btnNext.Select()


    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        ' Confirm exit request, then fall onto failure form.

        Dim cancelConfirm As MsgBoxResult = confirmCancel()

        ' If responded yes to the prompt, close the installer.
        If cancelConfirm = MsgBoxResult.Yes Then

            frmFailure.Show()
            Me.Close()

        End If

    End Sub

    Private Sub btnInstall_Click(sender As Object, e As EventArgs) Handles btnInstall.Click

        ' Save selected options then run the install form.
        ' The install form will get the program version and download the files.


        ' Disable/show buttons.
        pageIndex += 1
        displayPage()


        ' Perform the installation.

        Try


            lblStatusProcess.Text = "Gathering information" ' Update progress bar label.


            '' Save directory if it was changed.
            'If directoryChanged = True Then
            '' CHECK IF PROVIDED IS A VALID DIRECTORY (ELSE BREAK) [MAYBE IMPLEMENT THIS]
            'installDirectory = txtDirectory.Text
            'Else
            'installDirectory = INSTALLDIRECTORYDEFAULT ' Load the default directory.
            'End If


            installDirectory = txtDirectory.Text '  NOTE: If the directory doesn't exist, it will be created.

            ' Ensure that the directory has a "\" at the end.
            If installDirectory(installDirectory.Length - 1) <> "\"c Then
                installDirectory &= "\"
            End If

            ' Define derived directories.
            uninstallPath = installDirectory & UNINSTALLDESTINATION
            iconPath = installDirectory & ICONDESTINATION


            ' Save shortcut options.
            shortcutDesktop = chkDesktop.Checked
            shortcutStartMenu = chkStartMenu.Checked


            'frmInstall.Show()
            'Me.Close()



            ' Define variables.

            Dim programVersion As String


            ' Make a new instance of a web client to be used throughout this installation process.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Finding latest version" ' Update progress bar label.

            Dim installClient As WebClient = New WebClient()


            ' Get latest program version.
            ' Get from a file located in this program's GitHub repository at the VERSIONLINK web address.

            Using versionReader As New StreamReader(installClient.OpenRead(VERSIONLINK))

                ' Assume there are only 3 lines (and in data is in this order).
                ' Assume the first line is the program version (in the form X.X.X)
                programVersion = versionReader.ReadLine()

            End Using

            Dim programVersionFormatted As String = "v" & programVersion

            ' Save link to the specific release.
            Dim releaseLink As String = DOWNLOADLINK & programVersionFormatted & "/"

            ' Save name of main file.
            Dim mainFile As String = programVersionFormatted & ".exe"
            sourceFiles(0) = PREFIXSOURCES & mainFile


            ' Download latest program files.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Downloading program files" ' Update progress bar label.

            'Try (TEMP)

            ' Get UNINSTALLER first.
            installClient.DownloadFile(releaseLink & UNINSTALLSOURCE, uninstallPath) 'FOR TIMEOUT: , False, 100000)

            ' Get application ICON next.
            installClient.DownloadFile(releaseLink & ICONSOURCE, iconPath)


            ' Now get the REST OF THE FILES (including the main application executable(s)).
            For fileNumber As Integer = 0 To sourceFiles.Length - 1

                ' Define the remote source and local destination of this file.
                Dim fileSource As String = releaseLink & sourceFiles(fileNumber)
                Dim fileDestination As String = installDirectory & destinationFiles(fileNumber)

                installClient.DownloadFile(fileSource, fileDestination) ' Download the file to its corresponding destination.

                ' Get the next file.
                fileNumber += 1

            Next


            'Catch ex As IOException

            'Catch ex As WebException

            'Catch ex As Exception

            'End Try (TEMP)



            '' Move files to their paths in the install directory.

            'lblStatusProcess.Text = "Moving program files" ' Update progress bar label.



            ' Move files to their paths in the install directory.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Modifying registry" ' Update progress bar label.

            'Try (TEMP)

            ' Get the size of all the installed files in bytes.
            Dim folderSize As Long
            For Each filePath In My.Computer.FileSystem.GetFiles(installDirectory)
                Dim file = My.Computer.FileSystem.GetFileInfo(filePath)

                folderSize += file.Length
            Next
            Dim installSize As Integer = CInt(folderSize / 1024) ' Convert from bytes to kibibytes.

            'Openeing the Uninstall RegistryKey (don't forget to set the writable flag to true)
            With My.Computer.Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Uninstall", True)

                'Creating my AppRegistryKey
                Dim appKey As Microsoft.Win32.RegistryKey = .CreateSubKey(PROGRAMNAME)

                'Adding my values to my AppRegistryKey
                appKey.SetValue("DisplayName", PROGRAMNAME, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("DisplayVersion", programVersion, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("DisplayIcon", iconPath, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("Publisher", PROGRAMAUTHOR, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("UninstallString", uninstallPath, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("UninstallPath", uninstallPath, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("InstallLocation", installDirectory, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("EstimatedSize", installSize, Microsoft.Win32.RegistryValueKind.DWord)
                appKey.SetValue("NoModify", 1, Microsoft.Win32.RegistryValueKind.DWord)
                appKey.SetValue("Readme", READMELINK, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("URLInfoAbout", HOMEPAGELINK, Microsoft.Win32.RegistryValueKind.String)
                appKey.SetValue("URLUpdateInfo", UPDATELINK, Microsoft.Win32.RegistryValueKind.String)

            End With


            'Catch ex As Exception

            'End Try (TEMP)



            ' Create desktop and start menu shortcuts.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Creating shortcuts" ' Update progress bar label.

            Dim startPath As String = installDirectory & startFile

            ' Add Start Menu shortcut.

            If shortcutStartMenu = True Then

                ' Build the directory strcuture in the start menu folders.
                Dim commonStartMenuPath As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) ' Get path to common Start Menu items.
                Dim appStartMenuPath As String = Path.Combine(commonStartMenuPath, "Programs", PROGRAMNAME) ' The path to this specific application's folder.

                ' If this program doesn't have a Start Menu folder yet, create it.
                If Not Directory.Exists(appStartMenuPath) Then
                    Directory.CreateDirectory(appStartMenuPath)
                End If

                Dim shortcutStartMenuLocation As String = Path.Combine(appStartMenuPath, PROGRAMNAME + ".lnk")

                Dim shellStartMenu As New WshShell
                Dim shortcutStartMenu As IWshShortcut = DirectCast(shellStartMenu.CreateShortcut(shortcutStartMenuLocation), IWshShortcut)

                ' Set the shortcut properties.
                With shortcutStartMenu
                    .TargetPath = startPath
                    .WindowStyle = 1I
                    .Description = PROGRAMDESCRIPTION
                    .WorkingDirectory = installDirectory
                    .IconLocation = iconPath
                    .Arguments = String.Empty
                    .Save() ' Save the shortcut file.
                End With

            End If


            ' Add Desktop shortcut.

            If shortcutDesktop = True Then

                Dim commonDesktop As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                Dim shortcutDesktopLocation As String = Path.Combine(commonDesktop, PROGRAMNAME + ".lnk")

                Dim shellDesktop As New WshShell
                Dim shortcutDesktop As IWshShortcut = DirectCast(shellDesktop.CreateShortcut(shortcutDesktopLocation), IWshShortcut)

                ' Set the shortcut properties.
                With shortcutDesktop
                    .TargetPath = startPath
                    .WindowStyle = 1I
                    .Description = PROGRAMDESCRIPTION
                    .WorkingDirectory = installDirectory
                    .IconLocation = iconPath
                    .Arguments = String.Empty
                    .Save() ' Save the shortcut file.
                End With

            End If



            ' Upon completion, show the success form.

            barInstallProgress.PerformStep() ' Advance progress bar.

            frmSuccess.Show()
            Me.Close()


            'Catch ex As WebException ' HANDLE WEB EXCEPTIONS SPECIFICALLY (MAYBE)

        Catch ex As Exception

            ' If expection occurred, jump out to the failure form.

            frmFailure.Show()
            Me.Close()

        End Try


    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click

        ' Navigate to the next page.

        If pageIndex <> PAGECOUNT Then ' Only if there are pages to proceed to.

            pageIndex += 1

        End If

        displayPage()

    End Sub
    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click

        ' Navigate to the previous page.

        If pageIndex <> 0 Then ' Only if there are pages to return to.

            pageIndex -= 1

        End If

        displayPage()

    End Sub

    Private Sub updateNavigation()

        ' Ensure users can't navigate out-of-bounds

        ' If first page or the install page, disable back button.
        If pageIndex = 0 Or pageIndex = 3 Then
            btnBack.Enabled = False
        Else
            btnBack.Enabled = True
        End If


        If pageIndex = 2 Then 'PAGECOUNT - 1 ' If last page before installation, disable and hide next button and show and enable install button.
            btnNext.Enabled = False
            btnNext.Visible = False
            btnInstall.Enabled = True
            btnInstall.Visible = True

        ElseIf pageIndex = 3 Then ' If installation page, disable ability to go back.
            btnNext.Visible = True
            btnNext.Enabled = False
            btnInstall.Visible = False
            btnInstall.Enabled = False

        Else
            btnNext.Enabled = True
            btnNext.Visible = True
            btnInstall.Enabled = False
            btnInstall.Visible = False

        End If


    End Sub

    Private Sub displayPage()

        ' Hide/show the necessary panels to make the correct page visible.

        Select Case pageIndex

            Case 0
                pnlIntroduction.Visible = True
                pnlIntroduction.Enabled = True
                pnlOptions.Visible = False
                pnlOptions.Enabled = False
                pnlReady.Visible = False
                pnlReady.Enabled = False
                pnlInstall.Visible = False
                pnlInstall.Enabled = False

                btnNext.Select()

            Case 1
                pnlIntroduction.Visible = False
                pnlIntroduction.Enabled = False
                pnlOptions.Visible = True
                pnlOptions.Enabled = True
                pnlReady.Visible = False
                pnlReady.Enabled = False
                pnlInstall.Visible = False
                pnlInstall.Enabled = False

                btnNext.Select()

            Case 2
                pnlIntroduction.Visible = False
                pnlIntroduction.Enabled = False
                pnlOptions.Visible = False
                pnlOptions.Enabled = False
                pnlReady.Visible = True
                pnlReady.Enabled = True
                pnlInstall.Visible = False
                pnlInstall.Enabled = False

                btnCancel.Select()

            Case 3
                pnlIntroduction.Visible = False
                pnlIntroduction.Enabled = False
                pnlOptions.Visible = False
                pnlOptions.Enabled = False
                pnlReady.Visible = False
                pnlReady.Enabled = False
                pnlInstall.Visible = True
                pnlInstall.Enabled = True

                btnCancel.Select()

        End Select

        updateNavigation() ' Enable the correct navigation buttons.

    End Sub

    Private Sub btnChangeDirectory_Click(sender As Object, e As EventArgs) Handles btnChangeDirectory.Click

        ' Open up directory select dialog.

        ' Set directory dialog state.

        'IF SUCCESSFULLY SELECTED OTHER DIR, directoryChanged = TRUE !!!

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If MessageBox.Show("Are you sur to close this application?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
        Else
            e.Cancel = True
        End If
    End Sub

End Class
