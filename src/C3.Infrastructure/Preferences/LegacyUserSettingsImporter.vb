Imports System.Collections.Generic
Imports System.IO

Namespace Preferences

    ''' <summary>
    ''' Selects the newest readable supported C3 1.x profile. Invalid content
    ''' is retained as diagnostic evidence before trying an older candidate.
    ''' Access and I/O failures stop import so a transient fault cannot cause a
    ''' stale profile to win or migration to be durably marked complete.
    ''' </summary>
    Public NotInheritable Class LegacyUserSettingsImporter

        Private ReadOnly _locator As LegacySettingsProfileLocator
        Private ReadOnly _reader As LegacySettingsProfileReader

        Public Sub New()
            Me.New(New LegacySettingsProfileLocator(), New LegacySettingsProfileReader())
        End Sub

        Public Sub New(
                locator As LegacySettingsProfileLocator,
                reader As LegacySettingsProfileReader)

            If locator Is Nothing Then
                Throw New ArgumentNullException("locator")
            End If
            If reader Is Nothing Then
                Throw New ArgumentNullException("reader")
            End If

            _locator = locator
            _reader = reader
        End Sub

        Public Function Import() As LegacyUserSettingsImportResult
            Return Import(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
        End Function

        Public Function Import(
                localApplicationDataDirectory As String) As LegacyUserSettingsImportResult

            Dim candidates As IList(Of LegacySettingsProfileCandidate) =
                _locator.Locate(localApplicationDataDirectory)
            If candidates.Count = 0 Then
                Return LegacyUserSettingsImportResult.NotFound()
            End If

            Dim rejected As New List(Of LegacySettingsProfileReadResult)()
            For Each candidate As LegacySettingsProfileCandidate In candidates
                Dim readResult As LegacySettingsProfileReadResult = _reader.Read(candidate)
                If readResult.IsSuccess Then
                    Return LegacyUserSettingsImportResult.Imported(
                        readResult.Profile,
                        rejected)
                End If
                rejected.Add(readResult)
                If readResult.Failure = LegacySettingsProfileReadFailure.Unavailable Then
                    If readResult.FailureException IsNot Nothing Then
                        Throw readResult.FailureException
                    End If
                    Throw New IOException(readResult.FailureMessage)
                End If
            Next

            Return LegacyUserSettingsImportResult.Failed(rejected)
        End Function

    End Class

End Namespace
