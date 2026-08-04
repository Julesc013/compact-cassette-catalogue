Imports C3.Presentation.WinForms.Workspace

''' <summary>
''' Owns the concrete object graph for one C3 process. Forms receive or access
''' this root; no mutable catalogue or service is owned by a module-level global.
''' </summary>
Friend NotInheritable Class ApplicationComposition

    Private _catalogue As DataSet

    Private Sub New()
        _catalogue = CreateInitialCatalogue()
        CatalogueSession = New CatalogueSession("New Catalogue")
        Workspace = New WorkspaceController(
            CatalogueSession,
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            100)
        CatalogueStore = New LegacyXmlCatalogueStore()
        Preferences = UserPreferencesFactory.CreateDefault()
        CatalogueMetadata = New LegacyCatalogueMetadataWriter(Function() Catalogue)
        BrandService = New BrandService(New LegacyBrandRepository(Function() Catalogue))
        CassetteModelService = New CassetteModelService(
            New LegacyCassetteModelRepository(Function() Catalogue))
        DeckService = New DeckService(New LegacyDeckRepository(Function() Catalogue))
        TapeService = New TapeService(New LegacyTapeRepository(Function() Catalogue))
    End Sub

    Public ReadOnly Property CatalogueSession As CatalogueSession

    Public ReadOnly Property Workspace As WorkspaceController

    Public ReadOnly Property CatalogueStore As LegacyXmlCatalogueStore

    Public ReadOnly Property Preferences As UserPreferencesService

    Public ReadOnly Property CatalogueMetadata As LegacyCatalogueMetadataWriter

    Public ReadOnly Property BrandService As BrandService

    Public ReadOnly Property CassetteModelService As CassetteModelService

    Public ReadOnly Property DeckService As DeckService

    Public ReadOnly Property TapeService As TapeService

    Public ReadOnly Property Catalogue As DataSet
        Get
            Return _catalogue
        End Get
    End Property

    Public Shared Function CreateDefault() As ApplicationComposition
        Return New ApplicationComposition()
    End Function

    Public Sub ReplaceCatalogue(replacement As DataSet)
        If replacement Is Nothing Then
            Throw New ArgumentNullException("replacement")
        End If

        For Each tableName As String In {
                "Information", "Counters", "Decks", "Brands", "Models", "Tapes"}
            If replacement.Tables(tableName) Is Nothing Then
                Throw New InvalidOperationException(
                    "Catalogue is missing required table '" & tableName & "'.")
            End If
        Next

        _catalogue = replacement
    End Sub

    Private Shared Function CreateInitialCatalogue() As DataSet
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

End Class
