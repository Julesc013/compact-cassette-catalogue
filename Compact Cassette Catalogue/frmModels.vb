Public Class frmModels
    Private Sub frmModels_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Set the table as the data-grid-view's data source so that it auto-populates. 
        dataModels.DataSource = models

    End Sub

End Class