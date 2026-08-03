' Transitional compatibility facade for forms that have not yet moved to typed
' feature services. New code must use the owning session, store, or feature type.

Module varGlobals

    Public Const COPYRIGHTAUTHOR As String = "Jules Carboni"
    Public Const COPYRIGHTYEAR As String = "2019-2026"

    Public Const CONTACTLABEL As String = "github.com/Julesc013"
    Public Const CONTACTLINK As String = "https://github.com/Julesc013"
    Public Const WEBSITEMAIN As String = "https://github.com/Julesc013/compact-cassette-catalogue"
    Public Const WEBSITEHELP As String = "https://github.com/Julesc013/compact-cassette-catalogue/wiki"
    Public Const UPDATELINKDOWNLOAD As String = "https://github.com/Julesc013/compact-cassette-catalogue/releases"
    Public Const UPDATELINKCHECK As String = "https://raw.githubusercontent.com/Julesc013/compact-cassette-catalogue/master/VERSION"
    Public Const FEEDBACKLINK As String = "https://github.com/Julesc013/compact-cassette-catalogue/issues/new/choose"

    Public ReadOnly catalogueSession As New CatalogueSession("New Catalogue")
    Public ReadOnly catalogueStore As New LegacyXmlCatalogueStore()

    Public Property filePath As String
        Get
            Return catalogueSession.FilePath
        End Get
        Set(value As String)
            catalogueSession.SetDocumentLocation(value, catalogueSession.DisplayName)
        End Set
    End Property

    Public fileDirectory As String

    Public Property fileName As String
        Get
            Return catalogueSession.DisplayName
        End Get
        Set(value As String)
            catalogueSession.SetDocumentLocation(catalogueSession.FilePath, value)
        End Set
    End Property

    Public Property changes As Boolean
        Get
            Return catalogueSession.IsDirty
        End Get
        Set(value As Boolean)
            catalogueSession.SetDirtyForMigration(value)
        End Set
    End Property

    Public updates As Boolean
    Public timeLoaded As String
    Public duringSetup As Boolean

    Public catalogue As DataSet = CreateInitialCatalogue()
    Public information As DataTable = catalogue.Tables("Information")
    Public counters As DataTable = catalogue.Tables("Counters")
    Public decks As DataTable = catalogue.Tables("Decks")
    Public brands As DataTable = catalogue.Tables("Brands")
    Public models As DataTable = catalogue.Tables("Models")
    Public tapes As DataTable = catalogue.Tables("Tapes")

    Public deckCount As Integer
    Public brandCount As Integer
    Public modelCount As Integer
    Public tapeCount As Integer

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
        information = replacement.Tables("Information")
        counters = replacement.Tables("Counters")
        decks = replacement.Tables("Decks")
        brands = replacement.Tables("Brands")
        models = replacement.Tables("Models")
        tapes = replacement.Tables("Tapes")

        deckCount = decks.Rows.Count
        brandCount = brands.Rows.Count
        modelCount = models.Rows.Count
        tapeCount = tapes.Rows.Count
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
        frmConsole.lstConsole.Items.Add(stamp & " " & message)
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
