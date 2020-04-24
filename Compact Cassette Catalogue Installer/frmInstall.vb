﻿Imports System.IO
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


            ' Download latest program files.

            lblStatusProcess.Text = "Downloading program files" ' Update progress bar label.

            'Try

            installClient.DownloadFile("http://www.cohowinery.com/downloads/WineList.txt",
    "C:\Documents and Settings\All Users\Documents\WineList.txt") 'FOR TIMEOUT: , False, 100000)

            'Catch ex As IOException

            'Catch ex As WebException

            'Catch ex As Exception

            'End Try



            ' Move files to their paths in the install directory.

            lblStatusProcess.Text = "Moving program files" ' Update progress bar label.



            ' Move files to their paths in the install directory.

            lblStatusProcess.Text = "Modifying registry" ' Update progress bar label.



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