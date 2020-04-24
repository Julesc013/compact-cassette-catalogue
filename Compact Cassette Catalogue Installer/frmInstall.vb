Imports System.IO
Imports System.Net

Public Class frmInstall
    Private Sub frmInstall_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Define variables.

        Dim programVersion As String


        ' Perform the installation.

        Try

            ' Make a new instance of a web client to be used throughout this installation process.

            lblStatusProcess.Text = "Initialising installer" ' Update progress bar label.

            Dim installClient As WebClient = New WebClient()


            ' Get latest program version.
            ' Get from a file located in this program's GitHub repository at the VERSIONLINK web address.

            lblStatusProcess.Text = "Finding latest version" ' Update progress bar label.

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

            Next


            'Catch ex As IOException

            'Catch ex As WebException

            'Catch ex As Exception

            'End Try (TEMP)



            '' Move files to their paths in the install directory.

            'lblStatusProcess.Text = "Moving program files" ' Update progress bar label.



            ' Move files to their paths in the install directory.

            lblStatusProcess.Text = "Modifying registry" ' Update progress bar label.

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

            End With


            ' Create desktop and start menu shortcuts.

            lblStatusProcess.Text = "Creating shortcuts" ' Update progress bar label.



            ' Upon completion, show the success form.


            frmSuccess.Show()
            Me.Close()


            'Catch ex As WebException ' HANDLE WEB EXCEPTIONS SPECIFICALLY (MAYBE)

        Catch ex As Exception

            ' If expection occurred, jump out to the failure form.

            frmFailure.Show()
            Me.Close()

        End Try

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        ' Confirm exit request, then fall onto failure form.

        Dim cancelConfirm As MsgBoxResult = MsgBox("Are you sure you want to cancel Compact Cassette Catalogue installation?", MsgBoxStyle.YesNo, "Comfirm Cancel Installation")

        ' If responded yes to the prompt, close the installer.
        If cancelConfirm = MsgBoxResult.Yes Then

            frmFailure.Show()
            Me.Close()

        End If

    End Sub

End Class