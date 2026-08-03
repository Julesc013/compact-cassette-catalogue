Namespace Decks

    Public NotInheritable Class Deck

        Public Sub New(name As String, addedAt As DateTime, details As DeckDetails)
            If details Is Nothing Then
                Throw New ArgumentNullException("details")
            End If
            Me.Name = name
            Me.AddedAt = addedAt
            Me.Details = details
        End Sub

        Public ReadOnly Property Name As String
        Public ReadOnly Property AddedAt As DateTime
        Public ReadOnly Property Details As DeckDetails

    End Class

End Namespace
