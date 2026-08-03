Namespace Brands

    Public Enum BrandFailure
        None = 0
        NameRequired
        InvalidCode
        DuplicateCode
        NotFound
        ReferencedByModel
        StorageFailure
    End Enum

    Public NotInheritable Class BrandOperationResult

        Private Sub New()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Brand As Brand
        Public Property Failure As BrandFailure
        Public Property Message As String

        Public Shared Function Success(value As Brand) As BrandOperationResult
            Return New BrandOperationResult() With {
                .IsSuccess = True,
                .Brand = value,
                .Failure = BrandFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(failure As BrandFailure, message As String) As BrandOperationResult
            Return New BrandOperationResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

End Namespace
