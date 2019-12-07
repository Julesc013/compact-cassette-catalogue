Public Class frmTapes
    Private Sub frmViewTapes_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initialise boundary values.
        datRecordedMax.Value = Date.Today
        datRecordedMax.MaxDate = Date.Today
        datRecordedMin.MaxDate = Date.Today
        numYearMax.Value = Date.Today.Year
        numYearMax.Maximum = Date.Today.Year
        numYearMin.Maximum = Date.Today.Year

    End Sub

End Class