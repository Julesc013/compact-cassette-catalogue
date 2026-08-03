Namespace CassetteModels

    Public Enum CassetteModelFailure
        None = 0
        BrandRequired
        BrandNotFound
        InvalidType
        ModelNameRequired
        InvalidCode
        DuplicateIdentifier
        NotFound
        ReferencedByTape
        StorageFailure
    End Enum

    Public NotInheritable Class CassetteModelOperationResult

        Private Sub New()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Model As CassetteModel
        Public Property Failure As CassetteModelFailure
        Public Property Message As String

        Public Shared Function Success(value As CassetteModel) As CassetteModelOperationResult
            Return New CassetteModelOperationResult() With {
                .IsSuccess = True,
                .Model = value,
                .Failure = CassetteModelFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(
                failure As CassetteModelFailure,
                message As String) As CassetteModelOperationResult

            Return New CassetteModelOperationResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

End Namespace
