Namespace Tapes

    Public Enum TapeFailure
        None = 0
        ModelRequired
        ModelNotFound
        InvalidBulkCount
        IdentifierCapacityExceeded
        SideNameRequired
        DuplicateIdentifier
        NotFound
        StorageFailure
    End Enum

    Public NotInheritable Class TapeOperationResult

        Private Sub New()
            Tapes = New List(Of Tape)()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Tapes As IList(Of Tape)
        Public Property Failure As TapeFailure
        Public Property Message As String

        Public Shared Function Success(values As IList(Of Tape)) As TapeOperationResult
            Return New TapeOperationResult() With {
                .IsSuccess = True,
                .Tapes = values,
                .Failure = TapeFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(failure As TapeFailure, message As String) As TapeOperationResult
            Return New TapeOperationResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

End Namespace
