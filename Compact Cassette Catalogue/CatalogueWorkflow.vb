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
End Module

