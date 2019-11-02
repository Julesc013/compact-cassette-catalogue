Public Class frmTapes
    Private Sub frmViewTapes_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim otherItems As String() = {"Item 2", "Item 3", "Item 4"}
        lstTapes.Items.Add("Item 1").SubItems.AddRange(otherItems) ''temporary
        lstTapes.Items.Add("Item 1b").SubItems.AddRange(otherItems)
        lstTapes.Items.Add("Item 1c").SubItems.AddRange(otherItems)

        datRecordedMax.Value = Date.Today
        datRecordedMax.MaxDate = Date.Today
        datRecordedMin.MaxDate = Date.Today
        numYearMax.Value = Date.Today.Year
        numYearMax.Maximum = Date.Today.Year
        numYearMin.Maximum = Date.Today.Year

    End Sub

End Class