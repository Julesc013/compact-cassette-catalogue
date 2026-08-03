Public Class frmTapes

    Private NotInheritable Class ModelChoice
        Public Sub New(value As CassetteModel)
            Me.Value = value
        End Sub

        Public ReadOnly Property Value As CassetteModel

        Public Overrides Function ToString() As String
            Return Value.ModelName
        End Function
    End Class

    Private ReadOnly _selectedIdentifiers As New List(Of String)()
    Private _brands As IList(Of Brand) = New List(Of Brand)()

    Private Sub frmViewTapes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        datRecordedMin.MaxDate = Date.Today
        datRecordedMax.MaxDate = Date.Today
        datRecordedMax.Value = Date.Today
        numYearMin.Maximum = Date.Today.Year
        numYearMax.Maximum = Date.Today.Year
        numYearMax.Value = Date.Today.Year

        cmbTypes.SelectedIndex = 0
        cmbCondition.SelectedIndex = 0
        cmbNR.SelectedIndex = 0
        cmbContents.SelectedIndex = 0
        LoadBrandChoices()
        LoadDeckChoices()
        cmbBrand.SelectedIndex = 0
        cmbDeck.SelectedIndex = 0
        loadList()
    End Sub

    Private Sub LoadBrandChoices()
        _brands = brandService.GetAll(Nothing)
        cmbBrand.Items.Clear()
        cmbBrand.Items.Add("All Brands")
        For Each value As Brand In _brands
            cmbBrand.Items.Add(value.Name)
        Next
    End Sub

    Private Sub LoadDeckChoices()
        cmbDeck.Items.Clear()
        cmbDeck.Items.Add("All Decks")
        For Each value As Deck In deckService.GetAll()
            cmbDeck.Items.Add(value.Name)
        Next
    End Sub

    Private Sub loadList()
        Dim selectedBrandCode As String = Nothing
        If cmbBrand.SelectedIndex > 0 AndAlso cmbBrand.SelectedIndex <= _brands.Count Then
            selectedBrandCode = _brands(cmbBrand.SelectedIndex - 1).Code
        End If
        Dim selectedModel As ModelChoice = TryCast(cmbModel.SelectedItem, ModelChoice)
        Dim typeFilter As Integer = cmbTypes.SelectedIndex
        Dim conditionFilter As Integer = getCondition(cmbCondition.SelectedIndex - 1)
        Dim results As New List(Of Tape)()

        For Each value As Tape In tapeService.GetAll()
            Dim model As CassetteModel = cassetteModelService.Find(value.ModelIdentifier)
            If model Is Nothing Then
                Continue For
            End If
            If selectedBrandCode IsNot Nothing AndAlso
                    Not String.Equals(model.BrandCode, selectedBrandCode, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            If selectedModel IsNot Nothing AndAlso
                    Not String.Equals(
                        model.Identifier,
                        selectedModel.Value.Identifier,
                        StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            If typeFilter > 0 Then
                If chkTypeBetter.Checked AndAlso model.TypeNumber < typeFilter Then Continue For
                If Not chkTypeBetter.Checked AndAlso model.TypeNumber <> typeFilter Then Continue For
            End If
            If value.LengthMinutes < numLengthMin.Value OrElse value.LengthMinutes > numLengthMax.Value Then
                Continue For
            End If
            If value.Year < CInt(numYearMin.Value) OrElse value.Year > CInt(numYearMax.Value) Then
                Continue For
            End If
            If Not ContainsEither(value.SideA.Name, value.SideB.Name, txtName.Text) Then
                Continue For
            End If
            If conditionFilter >= 0 Then
                If chkConditionBetter.Checked AndAlso value.Condition < conditionFilter Then Continue For
                If Not chkConditionBetter.Checked AndAlso value.Condition <> conditionFilter Then Continue For
            End If
            If cmbDeck.SelectedIndex > 0 AndAlso
                    Not EqualsEither(value.SideA.DeckName, value.SideB.DeckName, cmbDeck.Text) Then
                Continue For
            End If
            If chkRecorded.Checked AndAlso Not HasRecordingInRange(value) Then
                Continue For
            End If
            If cmbNR.SelectedIndex > 0 AndAlso
                    Not EqualsEither(value.SideA.NoiseReduction, value.SideB.NoiseReduction, cmbNR.Text) Then
                Continue For
            End If
            If chkPackaged.Checked AndAlso Not value.Packaged Then
                Continue For
            End If
            If Not ContainsText(value.Notes, txtNotes.Text) Then
                Continue For
            End If
            If cmbContents.SelectedIndex > 0 AndAlso
                    Not EqualsEither(value.SideA.Contents, value.SideB.Contents, cmbContents.Text) Then
                Continue For
            End If
            If Not ContainsEither(value.SideA.Artist, value.SideB.Artist, txtArtist.Text) OrElse
                    Not ContainsEither(value.SideA.Title, value.SideB.Title, txtTitle.Text) Then
                Continue For
            End If
            results.Add(value)
        Next

        lstTapes.BeginUpdate()
        Try
            lstTapes.Items.Clear()
            For Each value As Tape In results
                Dim model As CassetteModel = cassetteModelService.Find(value.ModelIdentifier)
                Dim brand As Brand = brandService.Find(model.BrandCode)
                Dim item As New ListViewItem(value.ShortIdentifier)
                item.SubItems.Add(value.Identifier)
                item.SubItems.Add(DisplayPair(value.SideA.Name, value.SideB.Name))
                item.SubItems.Add(If(brand Is Nothing, model.BrandCode, brand.Name))
                item.SubItems.Add(model.ModelName)
                item.SubItems.Add(getTypeNumeral(model.TypeNumber, True))
                item.SubItems.Add(value.Year.ToString())
                item.SubItems.Add(value.LengthMinutes.ToString())
                item.SubItems.Add(value.Region)
                item.SubItems.Add(getConditionWorded(value.Condition))
                item.SubItems.Add(value.Packaged.ToString())
                item.SubItems.Add(DisplayDatePair(value.SideA, value.SideB))
                item.SubItems.Add(DisplayPair(value.SideA.NoiseReduction, value.SideB.NoiseReduction))
                item.SubItems.Add(DisplayPair(value.SideA.Contents, value.SideB.Contents))
                item.SubItems.Add(DisplayPair(value.SideA.DeckName, value.SideB.DeckName))
                item.SubItems.Add(DisplayPair(value.SideA.Artist, value.SideB.Artist))
                item.SubItems.Add(DisplayPair(value.SideA.Title, value.SideB.Title))
                item.SubItems.Add(value.Notes)
                lstTapes.Items.Add(item)
            Next
        Finally
            lstTapes.EndUpdate()
        End Try
        txtResults.Text = results.Count.ToString()
    End Sub

    Private Function HasRecordingInRange(value As Tape) As Boolean
        Return SideInRange(value.SideA) OrElse SideInRange(value.SideB)
    End Function

    Private Function SideInRange(value As TapeSide) As Boolean
        Return value.IsRecorded AndAlso
            value.RecordedAt.Date >= datRecordedMin.Value.Date AndAlso
            value.RecordedAt.Date <= datRecordedMax.Value.Date
    End Function

    Private Shared Function ContainsText(value As String, filter As String) As Boolean
        Return String.IsNullOrWhiteSpace(filter) OrElse
            If(value, String.Empty).IndexOf(filter.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0
    End Function

    Private Shared Function ContainsEither(first As String, second As String, filter As String) As Boolean
        Return ContainsText(first, filter) OrElse ContainsText(second, filter)
    End Function

    Private Shared Function EqualsEither(first As String, second As String, expected As String) As Boolean
        Return String.Equals(first, expected, StringComparison.CurrentCultureIgnoreCase) OrElse
            String.Equals(second, expected, StringComparison.CurrentCultureIgnoreCase)
    End Function

    Private Shared Function DisplayPair(first As String, second As String) As String
        Return DisplayValue(first) & ", " & DisplayValue(second)
    End Function

    Private Shared Function DisplayValue(value As String) As String
        Return If(String.IsNullOrWhiteSpace(value), "–", value)
    End Function

    Private Shared Function DisplayDatePair(first As TapeSide, second As TapeSide) As String
        Dim firstDate As String = If(first.IsRecorded, first.RecordedAt.ToShortDateString(), "–")
        Dim secondDate As String = If(second.IsRecorded, second.RecordedAt.ToShortDateString(), "–")
        Return firstDate & ", " & secondDate
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        loadList()
    End Sub

    Private Sub cmbTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTypes.SelectedIndexChanged
        Dim selectedIndex As Integer = cmbTypes.SelectedIndex
        If selectedIndex > 0 Then
            chkTypeBetter.Text = "Type " & getTypeNumeral(selectedIndex, False) & " or better."
            chkTypeBetter.Enabled = True
        Else
            chkTypeBetter.Enabled = False
            chkTypeBetter.Text = "Type I or better."
            chkTypeBetter.Checked = False
        End If
    End Sub

    Private Sub cmbCondition_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCondition.SelectedIndexChanged
        If cmbCondition.SelectedIndex > 0 Then
            chkConditionBetter.Text = cmbCondition.Text & " or better."
            chkConditionBetter.Enabled = True
        Else
            chkConditionBetter.Enabled = False
            chkConditionBetter.Text = "Poor or better."
            chkConditionBetter.Checked = False
        End If
    End Sub

    Private Sub cmbBrand_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBrand.SelectedIndexChanged
        cmbModel.Items.Clear()
        cmbModel.Items.Add("All Models")
        Dim selectedBrandCode As String = Nothing
        If cmbBrand.SelectedIndex > 0 AndAlso cmbBrand.SelectedIndex <= _brands.Count Then
            selectedBrandCode = _brands(cmbBrand.SelectedIndex - 1).Code
        End If
        If selectedBrandCode IsNot Nothing Then
            For Each value As CassetteModel In cassetteModelService.GetAll()
                If String.Equals(value.BrandCode, selectedBrandCode, StringComparison.OrdinalIgnoreCase) Then
                    cmbModel.Items.Add(New ModelChoice(value))
                End If
            Next
        End If
        cmbModel.SelectedIndex = 0
        cmbModel.Enabled = selectedBrandCode IsNot Nothing
    End Sub

    Private Sub chkRecorded_CheckedChanged(sender As Object, e As EventArgs) Handles chkRecorded.CheckedChanged
        datRecordedMin.Enabled = chkRecorded.Checked
        datRecordedMax.Enabled = chkRecorded.Checked
    End Sub

    Private Sub lstTapes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstTapes.SelectedIndexChanged
        _selectedIdentifiers.Clear()
        For Each item As ListViewItem In lstTapes.SelectedItems
            _selectedIdentifiers.Add(item.SubItems(0).Text)
        Next
        btnDelete.Enabled = _selectedIdentifiers.Count > 0
        btnEdit.Enabled = _selectedIdentifiers.Count = 1
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If _selectedIdentifiers.Count = 0 Then
            Return
        End If
        Dim prompt As String = "Delete the selected " & _selectedIdentifiers.Count.ToString() &
            " tape(s)?" & vbNewLine & "This action cannot be undone."
        If MsgBox(prompt, MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirm Tape Deletion") <> vbYes Then
            Return
        End If

        Dim failures As New List(Of String)()
        Dim changed As Boolean = False
        For Each identifier As String In _selectedIdentifiers
            Dim result As TapeOperationResult = tapeService.Delete(identifier)
            If result.IsSuccess Then
                changed = True
                consoleAdd("Deleted tape " & identifier & " successfully.")
            Else
                failures.Add(identifier & ": " & result.Message)
            End If
        Next
        If changed Then
            tapeCount = tapes.Rows.Count
            CompleteCatalogueMutation(Me)
        End If
        If failures.Count > 0 Then
            MsgBox(String.Join(vbNewLine, failures.ToArray()), MsgBoxStyle.Exclamation, "Some Tapes Were Not Deleted")
        End If
        loadList()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If _selectedIdentifiers.Count <> 1 Then
            Return
        End If
        Dim mainWindow As frmMain = TryCast(Owner, frmMain)
        If mainWindow IsNot Nothing Then
            mainWindow.ScrollToTape(_selectedIdentifiers(0))
            mainWindow.BringToFront()
        End If
    End Sub

End Class
