Imports System.Globalization

Namespace CassetteModels

    Public NotInheritable Class CassetteModelService

        Private ReadOnly _repository As ICassetteModelRepository

        Public Sub New(repository As ICassetteModelRepository)
            If repository Is Nothing Then
                Throw New ArgumentNullException("repository")
            End If
            _repository = repository
        End Sub

        Public Function GetAll() As IList(Of CassetteModel)
            Return _repository.GetAll().OrderBy(
                Function(value As CassetteModel) value.Identifier,
                StringComparer.OrdinalIgnoreCase).ToList()
        End Function

        Public Function Find(identifier As String) As CassetteModel
            Return _repository.FindByIdentifier(Normalize(identifier))
        End Function

        Public Function Create(
                draft As CassetteModelDraft,
                addedAt As DateTime) As CassetteModelOperationResult

            Dim validation As CassetteModelOperationResult = ValidateDraft(draft)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim brandCode As String = Normalize(draft.BrandCode).ToUpperInvariant()
            If Not _repository.BrandExists(brandCode) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.BrandNotFound,
                    "The selected brand no longer exists.")
            End If

            Dim code As String = Normalize(draft.Code).ToUpperInvariant()
            Dim identifier As String = brandCode &
                draft.TypeNumber.ToString(CultureInfo.InvariantCulture) & code
            If _repository.IdentifierExists(identifier) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.DuplicateIdentifier,
                    "Cassette model identifier '" & identifier & "' is already in use.")
            End If

            Dim value As New CassetteModel(
                brandCode,
                draft.TypeNumber,
                draft.ModelName.Trim(),
                code,
                identifier,
                Normalize(draft.DisplayName),
                0,
                addedAt,
                Normalize(draft.Notes))

            Try
                _repository.Add(value)
                Return CassetteModelOperationResult.Success(value)
            Catch ex As Exception
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    ex.Message)
            End Try
        End Function

        Public Function Update(
                identifier As String,
                draft As CassetteModelDraft) As CassetteModelOperationResult

            Dim normalizedIdentifier As String = Normalize(identifier)
            Dim existing As CassetteModel = _repository.FindByIdentifier(normalizedIdentifier)
            If existing Is Nothing Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.NotFound,
                    "The selected cassette model no longer exists.")
            End If
            If draft Is Nothing OrElse String.IsNullOrWhiteSpace(draft.ModelName) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ModelNameRequired,
                    "Model name is required.")
            End If

            Dim updated As New CassetteModel(
                existing.BrandCode,
                existing.TypeNumber,
                draft.ModelName.Trim(),
                existing.Code,
                existing.Identifier,
                Normalize(draft.DisplayName),
                existing.TapeCount,
                existing.AddedAt,
                Normalize(draft.Notes))

            Try
                _repository.Update(updated)
                Return CassetteModelOperationResult.Success(updated)
            Catch ex As Exception
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    ex.Message)
            End Try
        End Function

        Public Function Delete(identifier As String) As CassetteModelOperationResult
            Dim normalizedIdentifier As String = Normalize(identifier)
            Dim existing As CassetteModel = _repository.FindByIdentifier(normalizedIdentifier)
            If existing Is Nothing Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.NotFound,
                    "The selected cassette model no longer exists.")
            End If
            If _repository.IsReferencedByTape(normalizedIdentifier) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ReferencedByTape,
                    "Cassette model '" & normalizedIdentifier & "' cannot be deleted while tapes use it.")
            End If

            Try
                _repository.Delete(normalizedIdentifier)
                Return CassetteModelOperationResult.Success(existing)
            Catch ex As Exception
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.StorageFailure,
                    ex.Message)
            End Try
        End Function

        Private Shared Function ValidateDraft(
                draft As CassetteModelDraft) As CassetteModelOperationResult

            If draft Is Nothing OrElse String.IsNullOrWhiteSpace(draft.BrandCode) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.BrandRequired,
                    "A brand is required.")
            End If
            If draft.TypeNumber < 1 OrElse draft.TypeNumber > 4 Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.InvalidType,
                    "Cassette type must be between I and IV.")
            End If
            If String.IsNullOrWhiteSpace(draft.ModelName) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.ModelNameRequired,
                    "Model name is required.")
            End If

            Dim code As String = Normalize(draft.Code).ToUpperInvariant()
            If code.Length <> 2 OrElse Not code.All(
                    Function(character As Char) character >= "A"c AndAlso character <= "Z"c) Then
                Return CassetteModelOperationResult.Failed(
                    CassetteModelFailure.InvalidCode,
                    "Model code must contain exactly two letters (A-Z).")
            End If

            Return CassetteModelOperationResult.Success(Nothing)
        End Function

        Private Shared Function Normalize(value As String) As String
            Return If(value, String.Empty).Trim()
        End Function

    End Class

End Namespace
