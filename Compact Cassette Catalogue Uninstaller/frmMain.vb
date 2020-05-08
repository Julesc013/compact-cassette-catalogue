Imports System.IO
Imports System.Net
Imports IWshRuntimeLibrary

Public Class frmMain

    ' Name: Compact Cassette Catalogue Uninstaller
    ' Date: 8 May 2020
    ' Author: Jules Carboni
    ' Purpose: Uninstalls Compact Cassette Catalogue.


    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Set focus on Next button.
        btnUninstall.Select()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        '' Confirm exit request, then fall onto failure form.

        'Dim cancelConfirm As MsgBoxResult = confirmCancel()

        '' If responded yes to the prompt, close the installer.
        'If cancelConfirm = MsgBoxResult.Yes Then

        frmFailure.Show()
        Me.Close()

        'End If

    End Sub

    Private Sub btnUninstall_Click(sender As Object, e As EventArgs) Handles btnUninstall.Click

        ' Uninstall everything now.

        ' Disable/show elements.
        pnlReady.Visible = False
        pnlReady.Enabled = False
        pnlUninstall.Visible = True
        pnlUninstall.Enabled = True


        ' Initialise progress bar.
        barInstallProgress.Value = 0
        barInstallProgress.Step = 10
        barInstallProgress.Maximum = 10 * (3)


        ' Perform the uninstallation.

        Try


            ' Delete Windows Registry entry for this program.

            lblStatusProcess.Text = "Modifying registry" ' Update progress bar label.
            lblStatusProcess.Update() ' Update label so that it shows the new text.

            'Try (TEMP)


            '''''''''REMOVE REGISTRY ENTRIES


            'Catch ex As Exception
            ' Handle this specifically

            'End Try (TEMP)



            ' Delete files and installation folder.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Deleting program files" ' Update progress bar label.
            lblStatusProcess.Update() ' Update label so that it shows the new text.

            '''''''''DELETE FILES


            ' Delete desktop and start menu shortcuts.

            barInstallProgress.PerformStep() ' Advance progress bar.
            lblStatusProcess.Text = "Deleting shortcuts" ' Update progress bar label.
            lblStatusProcess.Update() ' Update label so that it shows the new text.

            '''''''''DELETE SHORTCUTS


            ' Upon completion, show the success form.

            barInstallProgress.PerformStep() ' Advance progress bar.

            frmSuccess.Show()
            Me.Close()


        Catch ex As Exception

            ' If expection occurred, jump out to the failure form.

            frmFailure.Show()
            Me.Close()

        End Try


    End Sub


End Class
