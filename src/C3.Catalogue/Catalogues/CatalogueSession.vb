Namespace Catalogues

    Public NotInheritable Class CatalogueSession

        Private ReadOnly _value As C3.Domain.Catalogues.CatalogueSession
        Private _revision As CatalogueRevision
        Private _coreRaisedChanged As Boolean

        Public Sub New(newCatalogueDisplayName As String)
            _value = New C3.Domain.Catalogues.CatalogueSession(newCatalogueDisplayName)
            AddHandler _value.SessionChanged, AddressOf OnCoreSessionChanged
        End Sub

        Public Event SessionChanged As EventHandler

        Public ReadOnly Property FilePath As String
            Get
                Return _value.FilePath
            End Get
        End Property

        Public ReadOnly Property DisplayName As String
            Get
                Return _value.DisplayName
            End Get
        End Property

        Public ReadOnly Property Revision As CatalogueRevision
            Get
                Return _revision
            End Get
        End Property

        Public ReadOnly Property IsDirty As Boolean
            Get
                Return _value.IsDirty
            End Get
        End Property

        Public ReadOnly Property ChangeSequence As Long
            Get
                Return _value.ChangeSequence
            End Get
        End Property

        Public Sub BeginNew(displayName As String)
            BeginCoreChange()
            _value.BeginNew(displayName)
            _revision = Nothing
            CompleteCoreChange()
        End Sub

        Public Sub SetDocumentLocation(path As String, displayName As String)
            BeginCoreChange()
            _value.SetDocumentLocation(path, displayName)
            CompleteCoreChange()
        End Sub

        Public Sub MarkChanged()
            BeginCoreChange()
            _value.MarkChanged()
            CompleteCoreChange()
        End Sub

        Public Sub SetDirtyForMigration(isDirty As Boolean)
            BeginCoreChange()
            _value.SetDirtyForMigration(isDirty)
            CompleteCoreChange()
        End Sub

        Public Sub MarkLoaded(path As String, displayName As String, revision As CatalogueRevision)
            BeginCoreChange()
            _value.MarkLoaded(path, displayName, NativeRevision(revision))
            _revision = revision
            CompleteCoreChange()
        End Sub

        Public Sub MarkSaved(path As String, displayName As String, revision As CatalogueRevision)
            BeginCoreChange()
            _value.MarkSaved(path, displayName, NativeRevision(revision))
            _revision = revision
            CompleteCoreChange()
        End Sub

        Private Shared Function NativeRevision(
                revision As CatalogueRevision) As C3.Domain.Catalogues.CatalogueRevision
            If revision Is Nothing Then
                Return Nothing
            End If
            Return revision.Value
        End Function

        Private Sub BeginCoreChange()
            _coreRaisedChanged = False
        End Sub

        Private Sub OnCoreSessionChanged(sender As Object, arguments As EventArgs)
            _coreRaisedChanged = True
        End Sub

        Private Sub CompleteCoreChange()
            If _coreRaisedChanged Then
                RaiseEvent SessionChanged(Me, EventArgs.Empty)
            End If
        End Sub

    End Class

End Namespace
