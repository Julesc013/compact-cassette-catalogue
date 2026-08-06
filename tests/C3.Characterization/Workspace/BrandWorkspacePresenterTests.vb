Imports C3.Infrastructure.CatalogueFiles.Xml.V1_1
Imports C3.Presentation.WinForms.Features.Brands
Imports C3.Presentation.WinForms.Interaction
Imports C3.Presentation.WinForms.Workspace

Friend Module BrandWorkspacePresenterTests

    Public Sub CoordinatesValidationSelectionAndReversibleCommands()
        Dim document As DataSet = Program.CreateFixtureSchema()
        Dim service As New BrandService(New LegacyBrandRepository(Function() document))
        Dim session As New CatalogueSession("New Catalogue")
        Dim workspace As New WorkspaceController(
            session,
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            20)
        Dim addedAt As New DateTime(2026, 8, 5, 9, 30, 0)
        Dim presenter As New BrandWorkspacePresenter(
            service,
            workspace,
            Function() addedAt)

        presenter.Refresh(String.Empty)
        AssertEqual(True, presenter.List.EmptyState.IsVisible, "blank empty state")
        AssertEqual(3, presenter.Fields.Count, "brand field definitions")

        presenter.BeginCreate()
        presenter.UpdateDraft("", "1", "")
        AssertEqual(False, presenter.Apply(), "invalid create rejected")
        AssertEqual(True, presenter.Validation.HasErrors, "validation visible")
        AssertEqual(
            "Brand name is required.",
            presenter.Validation.ForField(BrandWorkspacePresenter.NameField),
            "name validation is field-bound")
        AssertEqual(0, workspace.History.UndoCount, "failed create not recorded")

        presenter.UpdateDraft("Maxell", "mx", "Reference brand")
        AssertEqual(True, presenter.Apply(), "valid create")
        AssertEqual("MX", service.Find("MX").Code, "normalized stable selection")
        AssertEqual("MX", presenter.Inspector.Value.Code, "created brand selected")
        AssertEqual(True, session.IsDirty, "create marks document dirty")
        AssertEqual("Create brand MX", presenter.UndoDescription, "semantic undo description")
        AssertEqual(addedAt, service.Find("MX").AddedAt, "create timestamp")

        Dim revision As New CatalogueRevision(
            "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")
        workspace.MarkSaved("catalogue.xml", "catalogue.xml", revision)
        AssertEqual(False, session.IsDirty, "save checkpoint")

        AssertEqual(True, presenter.BeginEdit(), "begin selected edit")
        presenter.UpdateDraft("Maxell Audio", "ZZ", "Updated notes")
        AssertEqual(True, presenter.Apply(), "valid update")
        AssertEqual("MX", service.Find("MX").Code, "code remains immutable")
        AssertEqual("Maxell Audio", service.Find("MX").Name, "updated name")
        AssertEqual(True, presenter.Undo(), "undo update")
        AssertEqual("Maxell", service.Find("MX").Name, "undo restores prior brand")
        AssertEqual(False, session.IsDirty, "undo returns to saved checkpoint")
        AssertEqual(True, presenter.Redo(), "redo update")
        AssertEqual("Maxell Audio", service.Find("MX").Name, "redo reapplies update")

        presenter.BeginCreate()
        presenter.UpdateDraft("Duplicate", "MX", "")
        AssertEqual(False, presenter.Apply(), "duplicate create rejected")
        AssertEqual(
            "Brand code 'MX' is already in use.",
            presenter.Validation.ForField(BrandWorkspacePresenter.CodeField),
            "duplicate validation is field-bound")
        presenter.CancelEditor()

        presenter.Select("MX")
        AssertEqual(True, presenter.DeleteSelected(), "delete selected brand")
        AssertEqual(Nothing, service.Find("MX"), "brand deleted")
        AssertEqual(True, presenter.Undo(), "undo delete")
        AssertEqual("Maxell Audio", service.Find("MX").Name, "undo delete restores value")
        AssertEqual(addedAt, service.Find("MX").AddedAt, "undo delete restores timestamp")

        presenter.BeginCreate()
        presenter.UpdateDraft("TDK", "TD", "Archive stock")
        AssertEqual(True, presenter.Apply(), "second brand create")
        presenter.Select(New String() {"MX", "TD"})
        AssertEqual(2, presenter.SelectedCount, "stable multiple selection")
        AssertEqual(False, presenter.CanEdit, "multiple selection cannot edit one")
        AssertEqual(True, presenter.CanDelete, "multiple selection can delete")
        AssertEqual(True, presenter.DeleteSelected(), "multiple delete")
        AssertEqual(Nothing, service.Find("MX"), "first selected brand deleted")
        AssertEqual(Nothing, service.Find("TD"), "second selected brand deleted")
        AssertEqual(True, presenter.Undo(), "undo second selected delete")
        AssertEqual("TDK", service.Find("TD").Name, "second selected brand restored")
        AssertEqual(True, presenter.Undo(), "undo first selected delete")
        AssertEqual("Maxell Audio", service.Find("MX").Name, "first selected brand restored")

        Dim model As DataRow = document.Tables("Models").NewRow()
        model("Brand") = "MX"
        model("Identifier") = "MX-2-XLII"
        document.Tables("Models").Rows.Add(model)
        presenter.Select("MX")
        Dim historyBefore As Integer = workspace.History.UndoCount
        AssertEqual(False, presenter.DeleteSelected(), "referenced delete rejected")
        AssertEqual(historyBefore, workspace.History.UndoCount, "rejected delete not recorded")
        AssertEqual(True, presenter.Feedback.IsVisible, "delete error is presented")

        presenter.Refresh("does-not-match")
        AssertEqual(True, presenter.List.EmptyState.IsVisible, "filtered empty state")
        AssertEqual(False, presenter.Inspector.HasSelection, "filtered selection clears safely")
        AssertEqual(0, presenter.SelectedCount, "hidden selection cannot drive commands")
        presenter.Refresh(String.Empty)
        AssertEqual("MX", presenter.Inspector.Value.Code, "clearing filter restores selection")
        AssertEqual(1, presenter.SelectedCount, "restored selection drives commands")
    End Sub

    Public Sub SharedPatternsExposeExplicitStates()
        Dim feedback As New FeedbackPresentation()
        feedback.Show(FeedbackKind.Warning, "Review this change.")
        AssertEqual(True, feedback.IsVisible, "feedback visible")
        feedback.Clear()
        AssertEqual(False, feedback.IsVisible, "feedback cleared")

        Dim progress As New ProgressPresentation()
        progress.Start("Importing catalogue", 4, True)
        progress.Report(3)
        AssertEqual(True, progress.IsActive, "progress active")
        AssertEqual(3, progress.Completed, "progress completed count")
        progress.Complete()
        AssertEqual(False, progress.IsActive, "progress complete")

        Dim validation As New ValidationPresentation()
        validation.Show(
            New ValidationMessage("brand.name", "Brand name is required."),
            New ValidationMessage(String.Empty, "Review the highlighted fields."))
        AssertEqual(True, validation.HasErrors, "validation has errors")
        AssertEqual(
            "Brand name is required.",
            validation.ForField("brand.name"),
            "field error lookup")
    End Sub

    Private Sub AssertEqual(expected As Object, actual As Object, context As String)
        If Object.Equals(expected, actual) Then
            Return
        End If

        Dim expectedText As String = If(expected Is Nothing, "<null>", expected.ToString())
        Dim actualText As String = If(actual Is Nothing, "<null>", actual.ToString())
        Throw New InvalidOperationException(
            context & ": expected '" & expectedText & "' but got '" & actualText & "'.")
    End Sub

End Module
