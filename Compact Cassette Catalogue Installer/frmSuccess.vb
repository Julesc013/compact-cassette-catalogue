Public Class frmSuccess

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
