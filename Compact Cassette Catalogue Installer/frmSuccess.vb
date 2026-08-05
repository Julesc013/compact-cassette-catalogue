Public Class frmSuccess

    Private Sub frmSuccess_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AccessibleName = "Compact Cassette Catalogue setup complete"
        chkStartProgram.Text = "&Start Compact Cassette Catalogue after closing setup."
        chkStartProgram.AccessibleDescription = "Start the newly installed C3 program when Finish is selected."
        btnFinish.Text = "&Finish"
        btnFinish.AccessibleDescription = "Close setup and optionally start C3."
        btnBack.Visible = False
        btnCancel.Visible = False
        btnFinish.Select()
    End Sub

    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        If chkStartProgram.Checked Then
            Try
                Process.Start(startPath)
            Catch ex As Exception
                MessageBox.Show("C3 was installed, but Windows could not start it." & Environment.NewLine & Environment.NewLine & ex.Message,
                                "Could Not Start C3",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
            End Try
        End If
        Application.Exit()
    End Sub

End Class
