Public Class frmDecks
    Private Sub FrmViewDecks_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Intialise objects
        numYearMax.Value = Date.Today.Year
        numYearMax.Maximum = Date.Today.Year
        numYearMin.Maximum = Date.Today.Year

    End Sub

End Class