Namespace Catalogues

    Public NotInheritable Class CatalogueSession

        Private _filePath As String
        Private _displayName As String
        Private _revision As CatalogueRevision
        Private _isDirty As Boolean
        Private _changeSequence As Long

        Public Sub New(newCatalogueDisplayName As String)
            If String.IsNullOrWhiteSpace(newCatalogueDisplayName) Then
                Throw New ArgumentException("A display name is required.", "newCatalogueDisplayName")
            End If
            _displayName = newCatalogueDisplayName
        End Sub

        Public Event SessionChanged As EventHandler

        Public ReadOnly Property FilePath As String
            Get
                Return _filePath
            End Get
        End Property

        Public ReadOnly Property DisplayName As String
            Get
                Return _displayName
            End Get
        End Property

        Public ReadOnly Property Revision As CatalogueRevision
            Get
                Return _revision
            End Get
        End Property

        Public ReadOnly Property IsDirty As Boolean
            Get
                Return _isDirty
            End Get
        End Property

        Public ReadOnly Property ChangeSequence As Long
            Get
                Return _changeSequence
            End Get
        End Property

        Public Sub BeginNew(displayName As String)
            _filePath = Nothing
            _displayName = RequireDisplayName(displayName)
            _revision = Nothing
            _isDirty = False
            RaiseChanged()
        End Sub

        Public Sub SetDocumentLocation(path As String, displayName As String)
            _filePath = path
            _displayName = RequireDisplayName(displayName)
            RaiseChanged()
        End Sub

        Public Sub MarkChanged()
            _changeSequence += 1
            _isDirty = True
            RaiseChanged()
        End Sub

        Public Sub SetDirtyForMigration(isDirty As Boolean)
            If isDirty Then
                MarkChanged()
                Return
            End If

            If _isDirty Then
                _isDirty = False
                RaiseChanged()
            End If
        End Sub

        Public Sub MarkLoaded(path As String, displayName As String, revision As CatalogueRevision)
            _filePath = path
            _displayName = RequireDisplayName(displayName)
            _revision = revision
            _isDirty = False
            RaiseChanged()
        End Sub

        Public Sub MarkSaved(path As String, displayName As String, revision As CatalogueRevision)
            _filePath = path
            _displayName = RequireDisplayName(displayName)
            _revision = revision
            _isDirty = False
            RaiseChanged()
        End Sub

        Private Shared Function RequireDisplayName(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then
                Throw New ArgumentException("A display name is required.", "value")
            End If
            Return value
        End Function

        Private Sub RaiseChanged()
            RaiseEvent SessionChanged(Me, EventArgs.Empty)
        End Sub

    End Class

End Namespace

