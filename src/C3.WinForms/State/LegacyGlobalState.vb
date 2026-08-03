' Transitional compatibility facade for forms that have not yet moved to typed
' feature services. New code must use the owning session, store, or feature type.

Friend Module LegacyGlobalState

    Public Const COPYRIGHTAUTHOR As String = "Jules Carboni"
    Public Const COPYRIGHTYEAR As String = "2019-2026"

    Public Const CONTACTLABEL As String = "github.com/Julesc013"
    Public Const CONTACTLINK As String = "https://github.com/Julesc013"
    Public Const WEBSITEMAIN As String = "https://github.com/Julesc013/compact-cassette-catalogue"
    Public Const WEBSITEHELP As String = "https://github.com/Julesc013/compact-cassette-catalogue/wiki"
    Public Const UPDATELINKDOWNLOAD As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases"
    Public Const FEEDBACKLINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/issues/new/choose"

    Public ReadOnly catalogueSession As New CatalogueSession("New Catalogue")
    Public ReadOnly catalogueStore As New LegacyXmlCatalogueStore()
    Public ReadOnly preferences As UserPreferencesService =
        UserPreferencesFactory.CreateDefault()
    Public ReadOnly catalogueMetadata As New LegacyCatalogueMetadataWriter(Function() catalogue)

    Public catalogue As DataSet = CreateInitialCatalogue()
    Public ReadOnly brandService As New BrandService(New LegacyBrandRepository(Function() catalogue))
    Public ReadOnly cassetteModelService As New CassetteModelService(
        New LegacyCassetteModelRepository(Function() catalogue))
    Public ReadOnly deckService As New DeckService(New LegacyDeckRepository(Function() catalogue))
    Public ReadOnly tapeService As New TapeService(New LegacyTapeRepository(Function() catalogue))

    Private Function CreateInitialCatalogue() As DataSet
        Dim now As DateTime = DateTime.Now
        Dim metadata As New LegacyCatalogueMetadata() With {
            .FileVersion = VERSIONFILE,
            .ProductVersion = VERSION,
            .ProductStage = VERSIONSTAGE,
            .ProductDate = VERSIONDATE,
            .CreatedAt = now
        }
        Return LegacyCatalogueSchema.Create(metadata)
    End Function

    Public Sub replaceCatalogue(replacement As DataSet)
        If replacement Is Nothing Then
            Throw New ArgumentNullException("replacement")
        End If

        For Each tableName As String In {"Information", "Counters", "Decks", "Brands", "Models", "Tapes"}
            If replacement.Tables(tableName) Is Nothing Then
                Throw New InvalidOperationException("Catalogue is missing required table '" & tableName & "'.")
            End If
        Next

        catalogue = replacement
    End Sub

    Function getCondition(value As Integer) As Integer
        Dim values = New Dictionary(Of Integer, Integer) From {
            {0, 8}, {1, 7}, {2, 6}, {3, 5}, {4, 4}, {5, 3}, {6, 2}, {7, 1}, {8, 0}
        }
        If Not values.ContainsKey(value) Then
            Return -1
        End If
        Return values(value)
    End Function

    Function getConditionWorded(value As Integer) As String
        Dim values = New Dictionary(Of Integer, String) From {
            {0, "Broken"}, {1, "Poor"}, {2, "Fair"}, {3, "Good"}, {4, "Good Plus"},
            {5, "Very Good"}, {6, "Very Good Plus"}, {7, "Near Mint"}, {8, "Mint"}
        }
        Return values(value)
    End Function

    Function getTypeNumeral(value As Integer, worded As Boolean) As String
        Dim numerals = New Dictionary(Of Integer, String) From {
            {1, "I"}, {2, "II"}, {3, "III"}, {4, "IV"}
        }
        If Not worded Then
            Return numerals(value)
        End If

        Dim names = New Dictionary(Of Integer, String) From {
            {1, "Ferric"}, {2, "Chrome"}, {3, "Ferrichrome"}, {4, "Metal"}
        }
        Return numerals(value) & " - " & names(value)
    End Function

    Sub consoleAdd(message As String)
        BufferedLogger.Information(message)
        Dim stamp As String = "[" & consoleStamp(DateTime.Now) & "]"
        For Each window As Form In Application.OpenForms
            Dim consoleWindow As frmConsole = TryCast(window, frmConsole)
            If consoleWindow IsNot Nothing Then
                consoleWindow.AppendEntry(stamp & " " & message)
                Exit For
            End If
        Next
    End Sub

    Function consoleStamp(dateTime As DateTime) As String
        Return dateTime.ToString("dd/MM/yy HH:mm:ss")
    End Function

    Sub openWebLink(link As String)
        Try
            Process.Start(link)
        Catch ex As Exception
            Dim message As String = "Failed to open link."
            consoleAdd(message & " " & link & " Error: " & ex.Message)
            MsgBox(
                message & vbNewLine & vbNewLine & link & vbNewLine & vbNewLine & "Error: " & ex.Message,
                MsgBoxStyle.Exclamation,
                "Could Not Open Link")
        End Try
    End Sub

End Module
