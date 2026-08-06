Public Enum CatalogueCreationIntent
    AddBrand
    AddModel
    AddDeck
    AddTape
    RecordSide
End Enum

Public Enum CatalogueCreationStep
    Brand
    Model
    Deck
    Tape
    RecordSide
End Enum

Public NotInheritable Class CatalogueChoice
    Public Sub New(key As String, text As String)
        If String.IsNullOrWhiteSpace(key) Then
            Throw New ArgumentException("A catalogue choice requires a stable key.", "key")
        End If
        If text Is Nothing Then
            Throw New ArgumentNullException("text")
        End If
        Me.Key = key
        Me.Text = text
    End Sub

    Public ReadOnly Property Key As String
    Public ReadOnly Property Text As String

    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class

Public NotInheritable Class CatalogueCreationResult
    Public Sub New(key As String, displayName As String)
        Me.Key = key
        Me.DisplayName = displayName
    End Sub

    Public ReadOnly Property Key As String
    Public ReadOnly Property DisplayName As String
End Class

Public NotInheritable Class ValidationIssue
    Public Sub New(controlName As String, message As String)
        Me.ControlName = controlName
        Me.Message = message
    End Sub

    Public ReadOnly Property ControlName As String
    Public ReadOnly Property Message As String
End Class

Public Module CatalogueWorkflow
    Public Function Plan(
            intent As CatalogueCreationIntent,
            hasBrands As Boolean,
            hasModels As Boolean,
            hasDecks As Boolean) As IList(Of CatalogueCreationStep)
        Dim steps As New List(Of CatalogueCreationStep)()
        Select Case intent
            Case CatalogueCreationIntent.AddBrand
                steps.Add(CatalogueCreationStep.Brand)
            Case CatalogueCreationIntent.AddModel
                If Not hasBrands Then
                    steps.Add(CatalogueCreationStep.Brand)
                End If
                steps.Add(CatalogueCreationStep.Model)
            Case CatalogueCreationIntent.AddDeck
                steps.Add(CatalogueCreationStep.Deck)
            Case CatalogueCreationIntent.AddTape
                If Not hasModels Then
                    If Not hasBrands Then
                        steps.Add(CatalogueCreationStep.Brand)
                    End If
                    steps.Add(CatalogueCreationStep.Model)
                End If
                steps.Add(CatalogueCreationStep.Tape)
            Case CatalogueCreationIntent.RecordSide
                If Not hasDecks Then
                    steps.Add(CatalogueCreationStep.Deck)
                End If
                steps.Add(CatalogueCreationStep.RecordSide)
            Case Else
                Throw New ArgumentOutOfRangeException("intent")
        End Select
        Return steps.AsReadOnly()
    End Function

    Public Function SelectedChoiceKey(combo As ComboBox) As String
        Dim choice As CatalogueChoice = TryCast(combo.SelectedItem, CatalogueChoice)
        If choice Is Nothing Then
            Return Nothing
        End If
        Return choice.Key
    End Function

    Public Function SelectChoice(combo As ComboBox, key As String) As Boolean
        If String.IsNullOrWhiteSpace(key) Then
            combo.SelectedIndex = -1
            Return False
        End If
        For index As Integer = 0 To combo.Items.Count - 1
            Dim choice As CatalogueChoice = TryCast(combo.Items(index), CatalogueChoice)
            If choice IsNot Nothing AndAlso String.Equals(choice.Key, key, StringComparison.Ordinal) Then
                combo.SelectedIndex = index
                Return True
            End If
        Next
        combo.SelectedIndex = -1
        Return False
    End Function

    Public Function ShowValidationIssues(
            form As Form,
            provider As ErrorProvider,
            issues As IList(Of ValidationIssue),
            title As String) As Boolean
        provider.Clear()
        If issues.Count = 0 Then
            Return True
        End If

        Dim messages As New List(Of String)()
        Dim firstControl As Control = Nothing
        For Each issue As ValidationIssue In issues
            Dim matches As Control() = form.Controls.Find(issue.ControlName, True)
            If matches.Length > 0 Then
                provider.SetError(matches(0), issue.Message)
                If firstControl Is Nothing Then
                    firstControl = matches(0)
                End If
            End If
            messages.Add(issue.Message)
        Next
        MessageBox.Show(
            form,
            String.Join(Environment.NewLine, messages.ToArray()),
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation)
        If firstControl IsNot Nothing Then
            firstControl.Focus()
        End If
        Return False
    End Function

    Public Function AddBrand(owner As IWin32Window) As Boolean
        If Not PrepareMainForCreation("adding a brand") Then
            Return False
        End If
        Dim created As CatalogueCreationResult = CreateBrand(owner, False, Nothing)
        If created Is Nothing Then
            Return False
        End If
        RefreshMainAfterCreation()
        Return True
    End Function

    Public Function AddModel(owner As IWin32Window) As Boolean
        If Not PrepareMainForCreation("adding a model") Then
            Return False
        End If
        Dim entries As New List(Of String)()
        Dim neededBrand As Boolean = brands.Rows.Count = 0
        Dim created As CatalogueCreationResult = AddModelWithPrerequisites(owner, neededBrand, entries)
        If created Is Nothing Then
            If entries.Count > 0 Then
                RefreshMainAfterCreation()
                ShowJourneySummary(owner, entries)
            End If
            Return False
        End If
        RefreshMainAfterCreation()
        If neededBrand Then
            ShowJourneySummary(owner, entries)
        End If
        Return True
    End Function

    Public Function AddDeck(owner As IWin32Window) As Boolean
        If Not PrepareMainForCreation("adding a deck") Then
            Return False
        End If
        Dim created As CatalogueCreationResult = CreateDeck(owner, False, Nothing)
        If created Is Nothing Then
            Return False
        End If
        RefreshMainAfterCreation()
        Return True
    End Function

    Public Function AddTape(owner As IWin32Window) As Boolean
        If Not PrepareMainForCreation("adding a new tape") Then
            Return False
        End If
        Dim entries As New List(Of String)()
        Dim startingCounts As Integer() = {
            brands.Rows.Count, models.Rows.Count, decks.Rows.Count, tapes.Rows.Count}
        Dim preferredModelIdentifier As String = Nothing
        If models.Rows.Count = 0 Then
            Dim createdModel As CatalogueCreationResult = AddModelWithPrerequisites(owner, True, entries)
            If createdModel Is Nothing Then
                CompleteCancelledJourney(owner, entries, startingCounts)
                Return False
            End If
            preferredModelIdentifier = createdModel.Key
        End If

        Using dialog As New frmTapeNew()
            dialog.PreferredModelIdentifier = preferredModelIdentifier
            dialog.SuppressSuccessMessage = True
            dialog.StartPosition = FormStartPosition.CenterParent
            If dialog.ShowDialog(owner) <> DialogResult.OK Then
                CompleteCancelledJourney(owner, entries, startingCounts)
                Return False
            End If
            entries.Add("Tape: " & dialog.CreatedDisplayName)
        End Using

        RefreshMainAfterCreation()
        ShowJourneySummary(owner, entries)
        Return True
    End Function

    Public Function AddModelWithPrerequisites(
            owner As IWin32Window,
            guided As Boolean,
            entries As IList(Of String)) As CatalogueCreationResult
        Dim preferredBrandCode As String = Nothing
        If brands.Rows.Count = 0 Then
            Dim createdBrand As CatalogueCreationResult = CreateBrand(owner, True, entries)
            If createdBrand Is Nothing Then
                Return Nothing
            End If
            preferredBrandCode = createdBrand.Key
        End If

        Using dialog As New frmModelNew()
            dialog.PreferredBrandCode = preferredBrandCode
            dialog.SuppressSuccessMessage = guided
            dialog.StartPosition = FormStartPosition.CenterParent
            If dialog.ShowDialog(owner) <> DialogResult.OK Then
                Return Nothing
            End If
            If entries IsNot Nothing Then
                entries.Add("Model: " & dialog.CreatedDisplayName)
            End If
            Return New CatalogueCreationResult(dialog.CreatedKey, dialog.CreatedDisplayName)
        End Using
    End Function

    Public Function CreateModelForDetour(owner As IWin32Window) As CatalogueCreationResult
        Return AddModelWithPrerequisites(owner, True, New List(Of String)())
    End Function

    Public Function CreateBrandForDetour(owner As IWin32Window) As CatalogueCreationResult
        Return CreateBrand(owner, True, Nothing)
    End Function

    Public Function CreateDeckForDetour(owner As IWin32Window) As CatalogueCreationResult
        Return CreateDeck(owner, True, Nothing)
    End Function

    Private Function CreateBrand(
            owner As IWin32Window,
            suppressMessage As Boolean,
            entries As IList(Of String)) As CatalogueCreationResult
        Using dialog As New frmBrandNew()
            dialog.SuppressSuccessMessage = suppressMessage
            dialog.StartPosition = FormStartPosition.CenterParent
            If dialog.ShowDialog(owner) <> DialogResult.OK Then
                Return Nothing
            End If
            If entries IsNot Nothing Then
                entries.Add("Brand: " & dialog.CreatedDisplayName)
            End If
            Return New CatalogueCreationResult(dialog.CreatedKey, dialog.CreatedDisplayName)
        End Using
    End Function

    Private Function CreateDeck(
            owner As IWin32Window,
            suppressMessage As Boolean,
            entries As IList(Of String)) As CatalogueCreationResult
        Using dialog As New frmDeckNew()
            dialog.SuppressSuccessMessage = suppressMessage
            dialog.StartPosition = FormStartPosition.CenterParent
            If dialog.ShowDialog(owner) <> DialogResult.OK Then
                Return Nothing
            End If
            If entries IsNot Nothing Then
                entries.Add("Deck: " & dialog.CreatedDisplayName)
            End If
            Return New CatalogueCreationResult(dialog.CreatedKey, dialog.CreatedDisplayName)
        End Using
    End Function

    Private Sub RefreshMainAfterCreation()
        Dim main As frmMain = FindOpenMain()
        If main Is Nothing Then
            Return
        End If
        main.loadData()
        main.Text = fileName & "* - C3"
    End Sub

    Private Function PrepareMainForCreation(description As String) As Boolean
        Dim main As frmMain = FindOpenMain()
        Return main Is Nothing OrElse main.ResolvePendingTapeEditForCreation(description)
    End Function

    Private Function FindOpenMain() As frmMain
        For Each openForm As Form In Application.OpenForms
            If TypeOf openForm Is frmMain Then
                Return DirectCast(openForm, frmMain)
            End If
        Next
        Return Nothing
    End Function

    Private Sub CompleteCancelledJourney(
            owner As IWin32Window,
            entries As IList(Of String),
            startingCounts As Integer())
        Dim catalogueChanged As Boolean =
            startingCounts(0) <> brands.Rows.Count OrElse
            startingCounts(1) <> models.Rows.Count OrElse
            startingCounts(2) <> decks.Rows.Count OrElse
            startingCounts(3) <> tapes.Rows.Count
        If catalogueChanged Then
            RefreshMainAfterCreation()
            ShowJourneySummary(owner, entries)
        End If
    End Sub

    Private Sub ShowJourneySummary(owner As IWin32Window, entries As IList(Of String))
        If Not My.Settings.showMessages OrElse entries.Count = 0 Then
            Return
        End If
        MessageBox.Show(
            owner,
            "Added:" & Environment.NewLine & "  " & String.Join(
                Environment.NewLine & "  ", entries.ToArray()),
            "Catalogue Items Added",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)
    End Sub
End Module
