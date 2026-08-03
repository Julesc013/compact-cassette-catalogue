Imports System.Globalization

Namespace Tapes

    Public NotInheritable Class TapeService

        Private ReadOnly _repository As ITapeRepository

        Public Sub New(repository As ITapeRepository)
            If repository Is Nothing Then
                Throw New ArgumentNullException("repository")
            End If
            _repository = repository
        End Sub

        Public Function GetAll() As IList(Of Tape)
            Return _repository.GetAll().OrderBy(
                Function(value As Tape) value.ShortIdentifier,
                StringComparer.OrdinalIgnoreCase).ToList()
        End Function

        Public Function Find(shortIdentifier As String) As Tape
            Return _repository.FindByShortIdentifier(Normalize(shortIdentifier))
        End Function

        Public Function CreateMany(
                draft As TapeDraft,
                count As Integer,
                addedAt As DateTime) As TapeOperationResult

            Dim validation As TapeOperationResult = Validate(draft, count)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim modelIdentifier As String = Normalize(draft.ModelIdentifier)
            If Not _repository.ModelExists(modelIdentifier) Then
                Return TapeOperationResult.Failed(TapeFailure.ModelNotFound, "The selected cassette model no longer exists.")
            End If

            Dim firstNumber As Integer = _repository.NextNumberForModel(modelIdentifier)
            If firstNumber < 0 OrElse firstNumber + count - 1 > 999 Then
                Return TapeOperationResult.Failed(
                    TapeFailure.IdentifierCapacityExceeded,
                    "This model has exhausted the three-digit tape identifier range.")
            End If

            Dim values As New List(Of Tape)()
            For offset As Integer = 0 To count - 1
                Dim number As Integer = firstNumber + offset
                Dim numberCode As String = number.ToString("000", CultureInfo.InvariantCulture)
                Dim shortIdentifier As String = modelIdentifier & numberCode
                Dim identifier As String = BuildIdentifier(
                    modelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    numberCode)
                If _repository.IdentifierExists(identifier, shortIdentifier) Then
                    Return TapeOperationResult.Failed(
                        TapeFailure.DuplicateIdentifier,
                        "Tape identifier '" & shortIdentifier & "' is already in use.")
                End If

                values.Add(New Tape(
                    modelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    Normalize(draft.Region),
                    number,
                    identifier,
                    shortIdentifier,
                    draft.Condition,
                    draft.Packaged,
                    NormalizeSide(draft.SideA, draft.Packaged),
                    NormalizeSide(draft.SideB, draft.Packaged),
                    addedAt,
                    Normalize(draft.Notes)))
            Next

            Try
                _repository.AddRange(values)
                Return TapeOperationResult.Success(values)
            Catch ex As Exception
                Return TapeOperationResult.Failed(TapeFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Update(shortIdentifier As String, draft As TapeDraft) As TapeOperationResult
            Dim existing As Tape = _repository.FindByShortIdentifier(Normalize(shortIdentifier))
            If existing Is Nothing Then
                Return TapeOperationResult.Failed(TapeFailure.NotFound, "The selected tape no longer exists.")
            End If
            Dim validation As TapeOperationResult = Validate(draft, 1)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim value As New Tape(
                existing.ModelIdentifier,
                draft.Year,
                draft.LengthMinutes,
                Normalize(draft.Region),
                existing.Number,
                BuildIdentifier(
                    existing.ModelIdentifier,
                    draft.Year,
                    draft.LengthMinutes,
                    existing.Number.ToString("000", CultureInfo.InvariantCulture)),
                existing.ShortIdentifier,
                draft.Condition,
                draft.Packaged,
                NormalizeSide(draft.SideA, draft.Packaged),
                NormalizeSide(draft.SideB, draft.Packaged),
                existing.AddedAt,
                Normalize(draft.Notes))
            Try
                _repository.Update(value)
                Return TapeOperationResult.Success(New List(Of Tape) From {value})
            Catch ex As Exception
                Return TapeOperationResult.Failed(TapeFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Delete(shortIdentifier As String) As TapeOperationResult
            Dim existing As Tape = _repository.FindByShortIdentifier(Normalize(shortIdentifier))
            If existing Is Nothing Then
                Return TapeOperationResult.Failed(TapeFailure.NotFound, "The selected tape no longer exists.")
            End If
            Try
                _repository.Delete(existing.ShortIdentifier)
                Return TapeOperationResult.Success(New List(Of Tape) From {existing})
            Catch ex As Exception
                Return TapeOperationResult.Failed(TapeFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Private Shared Function Validate(draft As TapeDraft, count As Integer) As TapeOperationResult
            If draft Is Nothing OrElse String.IsNullOrWhiteSpace(draft.ModelIdentifier) Then
                Return TapeOperationResult.Failed(TapeFailure.ModelRequired, "A cassette model is required.")
            End If
            If count < 1 Then
                Return TapeOperationResult.Failed(TapeFailure.InvalidBulkCount, "At least one tape must be added.")
            End If
            If Not draft.Packaged Then
                If draft.SideA IsNot Nothing AndAlso draft.SideA.IsRecorded AndAlso
                        String.IsNullOrWhiteSpace(draft.SideA.Name) Then
                    Return TapeOperationResult.Failed(TapeFailure.SideNameRequired, "A name is required for side A.")
                End If
                If draft.SideB IsNot Nothing AndAlso draft.SideB.IsRecorded AndAlso
                        String.IsNullOrWhiteSpace(draft.SideB.Name) Then
                    Return TapeOperationResult.Failed(TapeFailure.SideNameRequired, "A name is required for side B.")
                End If
            End If
            Return TapeOperationResult.Success(New List(Of Tape)())
        End Function

        Private Shared Function NormalizeSide(value As TapeSide, packaged As Boolean) As TapeSide
            If packaged OrElse value Is Nothing OrElse Not value.IsRecorded Then
                Return TapeSide.Empty()
            End If
            Return New TapeSide(
                True,
                Normalize(value.Name),
                value.RecordedAt,
                Normalize(value.DeckName),
                Normalize(value.InputName),
                value.PeakLevel,
                Normalize(value.NoiseReduction),
                value.Hx,
                value.Mpx,
                value.Dubbed,
                Normalize(value.Speed),
                value.Bias,
                value.BiasCalibration,
                Normalize(value.Equalization),
                value.Level,
                value.LevelCalibration,
                Normalize(value.Contents),
                Normalize(value.Artist),
                Normalize(value.Title))
        End Function

        Private Shared Function EncodeLength(value As Decimal) As String
            Dim rounded As Integer = CInt(value)
            Dim digits As String = Math.Abs(rounded).ToString(CultureInfo.InvariantCulture)
            If digits.Length > 2 Then
                Return "X" & digits.Substring(1, 1)
            End If
            Return rounded.ToString("00", CultureInfo.InvariantCulture)
        End Function

        Private Shared Function BuildIdentifier(
                modelIdentifier As String,
                year As Integer,
                lengthMinutes As Decimal,
                numberCode As String) As String

            Return modelIdentifier &
                (year Mod 100).ToString("00", CultureInfo.InvariantCulture) &
                EncodeLength(lengthMinutes) & numberCode
        End Function

        Private Shared Function Normalize(value As String) As String
            Return If(value, String.Empty).Trim()
        End Function

    End Class

End Namespace
