Namespace Decks

    Public Enum DeckFailure
        None = 0
        ManufacturerRequired
        ModelRequired
        TapeTypeRequired
        SpeedRequired
        DuplicateName
        NotFound
        ReferencedByTape
        StorageFailure
    End Enum

    Public NotInheritable Class DeckOperationResult

        Private Sub New()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Deck As Deck
        Public Property Failure As DeckFailure
        Public Property Message As String

        Public Shared Function Success(value As Deck) As DeckOperationResult
            Return New DeckOperationResult() With {
                .IsSuccess = True,
                .Deck = value,
                .Failure = DeckFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(failure As DeckFailure, message As String) As DeckOperationResult
            Return New DeckOperationResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

End Namespace
