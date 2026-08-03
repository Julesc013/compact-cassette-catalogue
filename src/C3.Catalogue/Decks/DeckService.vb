Namespace Decks

    Public NotInheritable Class DeckService

        Private ReadOnly _repository As IDeckRepository

        Public Sub New(repository As IDeckRepository)
            If repository Is Nothing Then
                Throw New ArgumentNullException("repository")
            End If
            _repository = repository
        End Sub

        Public Function GetAll() As IList(Of Deck)
            Return _repository.GetAll().OrderBy(
                Function(value As Deck) value.Name,
                StringComparer.OrdinalIgnoreCase).ToList()
        End Function

        Public Function Find(name As String) As Deck
            Return _repository.FindByName(Normalize(name))
        End Function

        Public Function Create(details As DeckDetails, addedAt As DateTime) As DeckOperationResult
            Dim validation As DeckOperationResult = Validate(details)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim normalizedDetails As DeckDetails = Normalize(details)
            Dim name As String = normalizedDetails.Manufacturer & " " & normalizedDetails.Model
            If _repository.NameExists(name) Then
                Return DeckOperationResult.Failed(
                    DeckFailure.DuplicateName,
                    "Deck name '" & name & "' is already in use.")
            End If

            Dim value As New Deck(name, addedAt, normalizedDetails)
            Try
                _repository.Add(value)
                Return DeckOperationResult.Success(value)
            Catch ex As Exception
                Return DeckOperationResult.Failed(DeckFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Update(name As String, details As DeckDetails) As DeckOperationResult
            Dim normalizedName As String = Normalize(name)
            Dim existing As Deck = _repository.FindByName(normalizedName)
            If existing Is Nothing Then
                Return DeckOperationResult.Failed(DeckFailure.NotFound, "The selected deck no longer exists.")
            End If

            Dim validation As DeckOperationResult = Validate(details)
            If Not validation.IsSuccess Then
                Return validation
            End If

            Dim value As New Deck(existing.Name, existing.AddedAt, Normalize(details))
            Try
                _repository.Update(value)
                Return DeckOperationResult.Success(value)
            Catch ex As Exception
                Return DeckOperationResult.Failed(DeckFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Public Function Delete(name As String) As DeckOperationResult
            Dim normalizedName As String = Normalize(name)
            Dim existing As Deck = _repository.FindByName(normalizedName)
            If existing Is Nothing Then
                Return DeckOperationResult.Failed(DeckFailure.NotFound, "The selected deck no longer exists.")
            End If
            If _repository.IsReferencedByTape(normalizedName) Then
                Return DeckOperationResult.Failed(
                    DeckFailure.ReferencedByTape,
                    "Deck '" & normalizedName & "' cannot be deleted while recordings use it.")
            End If

            Try
                _repository.Delete(normalizedName)
                Return DeckOperationResult.Success(existing)
            Catch ex As Exception
                Return DeckOperationResult.Failed(DeckFailure.StorageFailure, ex.Message)
            End Try
        End Function

        Private Shared Function Validate(details As DeckDetails) As DeckOperationResult
            If details Is Nothing OrElse String.IsNullOrWhiteSpace(details.Manufacturer) Then
                Return DeckOperationResult.Failed(
                    DeckFailure.ManufacturerRequired,
                    "Manufacturer name is required.")
            End If
            If String.IsNullOrWhiteSpace(details.Model) Then
                Return DeckOperationResult.Failed(DeckFailure.ModelRequired, "Model name is required.")
            End If
            If Not (details.Type1 OrElse details.Type2 OrElse details.Type3 OrElse details.Type4) Then
                Return DeckOperationResult.Failed(
                    DeckFailure.TapeTypeRequired,
                    "At least one supported cassette type is required.")
            End If
            If Not (details.SpeedSlow OrElse details.SpeedNormal OrElse details.SpeedFast) Then
                Return DeckOperationResult.Failed(
                    DeckFailure.SpeedRequired,
                    "At least one supported tape speed is required.")
            End If
            Return DeckOperationResult.Success(Nothing)
        End Function

        Private Shared Function Normalize(details As DeckDetails) As DeckDetails
            Return New DeckDetails(
                Normalize(details.Manufacturer),
                Normalize(details.Model),
                details.Year,
                details.Condition,
                details.Type1,
                details.Type2,
                details.Type3,
                details.Type4,
                details.Hx,
                details.Mpx,
                details.DolbyB,
                details.DolbyC,
                details.DolbyS,
                details.Dbx1,
                details.Dbx2,
                details.Stereo,
                details.ProgramSearch,
                details.Reverse,
                details.Calibration,
                details.Azimuth,
                details.DubbingSlow,
                details.DubbingFast,
                details.FrequencyLow,
                details.FrequencyHigh,
                details.SignalRatio,
                Normalize(details.SignalRatioNoiseReduction),
                details.WowFlutter,
                details.Distortion,
                details.Heads,
                details.Wells,
                details.SpeedSlow,
                details.SpeedNormal,
                details.SpeedFast,
                Normalize(details.Notes))
        End Function

        Private Shared Function Normalize(value As String) As String
            Return If(value, String.Empty).Trim()
        End Function

    End Class

End Namespace
