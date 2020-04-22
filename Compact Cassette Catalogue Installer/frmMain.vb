Public Class frmMain

    ' Name: Compact Cassette Catalogue Installer
    ' Date: 21 April 2020
    ' Author: Jules Carboni
    ' Purpose: Downloads and installs the latest version of Compact Cassette Catalogue.

    Dim pageNumber As Integer = 0
    Const PAGECOUNT As Integer = 3

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

    Private Sub btnInstall_Click(sender As Object, e As EventArgs) Handles btnInstall.Click

        ' Selected options are already saved, so just run the install form.

        frmInstall.Show()
        Me.Close()

    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click

        ' Go to and display the next page.

        If pageNumber <> PAGECOUNT Then ' Only if they have a page ahead.
            pageNumber -= 1
        End If

        displayPage() ' Display the correct panels.

    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click

        ' Go to and display the previous page.

        If pageNumber <> 0 Then ' Only if they have a page behind.
            pageNumber -= 1
        End If

        displayPage() ' Display the correct panels.

    End Sub

    Private Sub updateNavigation()

        ' Ensure users can't navigate out-of-bounds

        ' If first page, disable back button.
        If pageNumber <> 0 Then
            btnBack.Enabled = True
        Else
            btnBack.Enabled = False
        End If

        ' If last page, disable and hide next button and show and enable install button.
        If pageNumber <> PAGECOUNT - 1 Then
            btnNext.Enabled = True
            btnNext.Visible = True
            btnInstall.Enabled = False
            btnInstall.Visible = False
        Else
            btnNext.Enabled = False
            btnNext.Visible = False
            btnInstall.Enabled = True
            btnInstall.Visible = True
        End If

    End Sub

    Private Sub displayPage()

        ' Hide/show the necessary panels to make the correct page visible.

        Select Case pageNumber

            Case 0
                pnlIntroduction.Visible = True
                pnlIntroduction.Enabled = True
                pnlOptions.Visible = False
                pnlOptions.Enabled = False
                pnlHeaderOptions.Visible = False
                pnlHeaderOptions.Enabled = False
                pnlReady.Visible = False
                pnlReady.Enabled = False
                pnlHeaderReady.Visible = False
                pnlHeaderReady.Enabled = False

            Case 1
                pnlIntroduction.Visible = False
                pnlIntroduction.Enabled = False
                pnlOptions.Visible = True
                pnlOptions.Enabled = True
                pnlHeaderOptions.Visible = True
                pnlHeaderOptions.Enabled = True
                pnlReady.Visible = False
                pnlReady.Enabled = False
                pnlHeaderReady.Visible = False
                pnlHeaderReady.Enabled = False

            Case 2
                pnlIntroduction.Visible = False
                pnlIntroduction.Enabled = False
                pnlOptions.Visible = False
                pnlOptions.Enabled = False
                pnlHeaderOptions.Visible = False
                pnlHeaderOptions.Enabled = False
                pnlReady.Visible = True
                pnlReady.Enabled = True
                pnlHeaderReady.Visible = True
                pnlHeaderReady.Enabled = True

        End Select

        updateNavigation() ' Enable the correct navigation buttons.

    End Sub

End Class
