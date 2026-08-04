Imports C3.Presentation.WinForms.Workspace

Friend Module WorkspaceControllerTests

    Public Sub WorkspaceOwnsExplicitInteractionState()
        Dim session As New C3.Catalogue.Catalogues.CatalogueSession("New Catalogue")
        Dim controller As New WorkspaceController(
            session,
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            8)

        controller.State.Selection.SelectOnly("brands", "brand-01")
        controller.State.View.Apply("brands", "metal", "name", SortDirection.Ascending)
        controller.State.EditorDraft.Begin("brands", "brand-01")
        controller.State.EditorDraft.MarkChanged()
        controller.State.Recovery.Report(RecoveryStatus.Available, "Recovery copy available.")
        controller.State.BackgroundOperation.Start("Refreshing brands", True)
        controller.State.BackgroundOperation.RequestCancellation()

        AssertEqual("New Catalogue", controller.State.Document.DisplayName, "document projection")
        AssertEqual("brand-01", controller.State.Selection.SelectedIds(0), "stable selection")
        AssertEqual("metal", controller.State.View.FilterText, "view filter")
        AssertEqual(True, controller.State.EditorDraft.IsDirty, "draft dirty state")
        AssertEqual(RecoveryStatus.Available, controller.State.Recovery.Status, "recovery status")
        AssertEqual(True, controller.State.BackgroundOperation.CancellationRequested, "cancellation request")

        controller.BeginNew("Another Catalogue", CatalogueCompatibilityMode.NativeV2_0, False)

        AssertEqual("Another Catalogue", controller.State.Document.DisplayName, "replacement document")
        AssertEqual(0, controller.State.Selection.SelectedIds.Count, "selection reset")
        AssertEqual(False, controller.State.EditorDraft.IsActive, "draft reset")
        AssertEqual(RecoveryStatus.None, controller.State.Recovery.Status, "recovery reset")
        AssertEqual(False, controller.State.BackgroundOperation.IsActive, "operation reset")
        AssertEqual(CatalogueCompatibilityMode.NativeV2_0, controller.State.Compatibility.Mode, "compatibility mode")
    End Sub

    Public Sub CommandHistoryCoordinatesDirtyUndoAndRedo()
        Dim session As New C3.Catalogue.Catalogues.CatalogueSession("New Catalogue")
        Dim controller As New WorkspaceController(
            session,
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            2)
        Dim value As Integer
        Dim first As New IntegerCommand("Create brand", value, 1)

        AssertEqual(True, controller.Execute(first).IsSuccess, "execute succeeds")
        value = first.CurrentValue
        AssertEqual(1, value, "execute mutation")
        AssertEqual(True, session.IsDirty, "execute marks document dirty")
        AssertEqual(True, controller.History.CanUndo, "undo becomes available")

        Dim revision As New C3.Catalogue.Catalogues.CatalogueRevision(
            "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")
        controller.MarkSaved("catalogue.xml", "catalogue.xml", revision)
        AssertEqual(False, session.IsDirty, "save clears dirty state")
        AssertEqual(True, controller.History.IsAtCheckpoint, "save marks history checkpoint")

        AssertEqual(True, controller.Undo().IsSuccess, "undo succeeds")
        AssertEqual(0, first.CurrentValue, "undo mutation")
        AssertEqual(True, session.IsDirty, "undo away from checkpoint is dirty")
        AssertEqual(True, controller.History.CanRedo, "redo becomes available")

        AssertEqual(True, controller.Redo().IsSuccess, "redo succeeds")
        AssertEqual(1, first.CurrentValue, "redo mutation")
        AssertEqual(False, session.IsDirty, "redo to saved checkpoint is clean")

        Dim rejected As New IntegerCommand("Rejected command", first.CurrentValue, 2, True)
        AssertEqual(False, controller.Execute(rejected).IsSuccess, "rejected command")
        AssertEqual(1, controller.History.UndoCount, "rejected command is not recorded")

        AssertEqual(True, controller.Undo().IsSuccess, "undo before branch")
        Dim replacement As New IntegerCommand("Replacement command", first.CurrentValue, 5)
        AssertEqual(True, controller.Execute(replacement).IsSuccess, "replacement command")
        AssertEqual(False, controller.History.CanRedo, "new command clears redo branch")
    End Sub

    Private Sub AssertEqual(expected As Object, actual As Object, context As String)
        If Not Object.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                context & ": expected '" & expected.ToString() & "' but got '" & actual.ToString() & "'.")
        End If
    End Sub

    Private NotInheritable Class IntegerCommand
        Implements IReversibleWorkspaceCommand

        Private ReadOnly _description As String
        Private ReadOnly _delta As Integer
        Private ReadOnly _reject As Boolean
        Private _value As Integer

        Public Sub New(description As String, value As Integer, delta As Integer, Optional reject As Boolean = False)
            _description = description
            _value = value
            _delta = delta
            _reject = reject
        End Sub

        Public ReadOnly Property CurrentValue As Integer
            Get
                Return _value
            End Get
        End Property

        Public ReadOnly Property Description As String Implements IReversibleWorkspaceCommand.Description
            Get
                Return _description
            End Get
        End Property

        Public Function Execute() As WorkspaceCommandResult Implements IReversibleWorkspaceCommand.Execute
            If _reject Then
                Return WorkspaceCommandResult.Failed("Rejected for test.")
            End If

            _value += _delta
            Return WorkspaceCommandResult.Success()
        End Function

        Public Function Undo() As WorkspaceCommandResult Implements IReversibleWorkspaceCommand.Undo
            _value -= _delta
            Return WorkspaceCommandResult.Success()
        End Function
    End Class

End Module
