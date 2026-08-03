Namespace Brands

    Public NotInheritable Class BrandService

        Private ReadOnly _repository As IBrandRepository

        Public Sub New(repository As IBrandRepository)
            If repository Is Nothing Then
                Throw New ArgumentNullException("repository")
            End If
            _repository = repository
        End Sub

        Public Function GetAll(notesFilter As String) As IList(Of Brand)
            Dim values As IEnumerable(Of Brand) = _repository.GetAll()
            If Not String.IsNullOrWhiteSpace(notesFilter) Then
                Dim filter As String = notesFilter.Trim()
                values = values.Where(
                    Function(value As Brand) (If(value.Notes, String.Empty)).IndexOf(
                        filter,
                        StringComparison.CurrentCultureIgnoreCase) >= 0)
            End If
            Return values.OrderBy(Function(value As Brand) value.Code, StringComparer.OrdinalIgnoreCase).ToList()
        End Function

        Public Function Find(code As String) As Brand
            Return _repository.FindByCode(NormalizeCode(code))
        End Function

        Public Function Create(draft As BrandDraft, addedAt As DateTime) As BrandOperationResult
            Dim validation As BrandOperationResult = ValidateDraft(draft)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim code As String = NormalizeCode(draft.Code)
            If _repository.IsCodeInUse(code) Then
                Return BrandOperationResult.Failed(
                    BrandFailure.DuplicateCode,
                    "Brand code '" & code & "' is already in use.")
            End If

            Dim value As New Brand(draft.Name.Trim(), code, addedAt, NormalizeNotes(draft.Notes))
            Try
                _repository.Add(value)
                Return BrandOperationResult.Success(value)
            Catch ex As Exception
                Return BrandOperationResult.Failed(BrandFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Update(code As String, draft As BrandDraft) As BrandOperationResult
            Dim normalizedCode As String = NormalizeCode(code)
            Dim existing As Brand = _repository.FindByCode(normalizedCode)
            If existing Is Nothing Then
                Return BrandOperationResult.Failed(BrandFailure.NotFound, "The selected brand no longer exists.")
            End If

            Dim validation As BrandOperationResult = ValidateName(draft)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim updated As New Brand(
                draft.Name.Trim(),
                normalizedCode,
                existing.AddedAt,
                NormalizeNotes(draft.Notes))
            Try
                _repository.Update(updated)
                Return BrandOperationResult.Success(updated)
            Catch ex As Exception
                Return BrandOperationResult.Failed(BrandFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Delete(code As String) As BrandOperationResult
            Dim normalizedCode As String = NormalizeCode(code)
            Dim existing As Brand = _repository.FindByCode(normalizedCode)
            If existing Is Nothing Then
                Return BrandOperationResult.Failed(BrandFailure.NotFound, "The selected brand no longer exists.")
            End If
            If _repository.IsReferencedByModel(normalizedCode) Then
                Return BrandOperationResult.Failed(
                    BrandFailure.ReferencedByModel,
                    "Brand '" & normalizedCode & "' cannot be deleted while cassette models use it.")
            End If

            Try
                _repository.Delete(normalizedCode)
                Return BrandOperationResult.Success(existing)
            Catch ex As Exception
                Return BrandOperationResult.Failed(BrandFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Private Shared Function ValidateDraft(draft As BrandDraft) As BrandOperationResult
            Dim nameValidation As BrandOperationResult = ValidateName(draft)
            If Not nameValidation.IsSuccess Then
                Return nameValidation
            End If

            Dim code As String = NormalizeCode(draft.Code)
            If code.Length <> 2 OrElse Not code.All(Function(character As Char) character >= "A"c AndAlso character <= "Z"c) Then
                Return BrandOperationResult.Failed(
                    BrandFailure.InvalidCode,
                    "Brand code must contain exactly two letters (A-Z).")
            End If

            Return BrandOperationResult.Success(Nothing)
        End Function

        Private Shared Function ValidateName(draft As BrandDraft) As BrandOperationResult
            If draft Is Nothing OrElse String.IsNullOrWhiteSpace(draft.Name) Then
                Return BrandOperationResult.Failed(BrandFailure.NameRequired, "Brand name is required.")
            End If
            Return BrandOperationResult.Success(Nothing)
        End Function

        Private Shared Function NormalizeCode(value As String) As String
            Return If(value, String.Empty).Trim().ToUpperInvariant()
        End Function

        Private Shared Function NormalizeNotes(value As String) As String
            Return If(value, String.Empty)
        End Function

    End Class

End Namespace
