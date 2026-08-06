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
End Module
