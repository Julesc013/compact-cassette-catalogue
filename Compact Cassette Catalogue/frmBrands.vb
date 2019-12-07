Public Class frmViewBrands
    Private Sub frmViewBrands_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Set the table as the data-grid-view's data source so that it auto-populates. 
        dataBrands.DataSource = brands

    End Sub

End Class