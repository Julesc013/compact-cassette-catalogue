Imports C3.Infrastructure.CatalogueFiles.Xml.V1_1
Imports C3.Presentation.WinForms.Features.Brands
Imports C3.Presentation.WinForms.Workspace
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Friend Module BrandWorkspaceFormContractTests

    Public Sub PreservesDesignerDpiKeyboardAndAccessibilityContracts()
        RunOnStaThread(AddressOf VerifyFormContract)
    End Sub

    Public Sub ExecutesCreateEditFilterUndoAndRedoThroughControls()
        RunOnStaThread(AddressOf VerifyControlWorkflow)
    End Sub

    Public Sub PreservesLayoutUnderRelativeScalingAndLongContent()
        RunOnStaThread(AddressOf VerifyLayoutStress)
    End Sub

    Private Sub RunOnStaThread(action As Action)
        Dim failure As Exception = Nothing
        Dim worker As New Thread(
            Sub()
                Try
                    action()
                Catch ex As Exception
                    failure = ex
                End Try
            End Sub)
        worker.Name = "C3 Brand workspace UI contract"
        worker.SetApartmentState(ApartmentState.STA)
        worker.Start()
        If Not worker.Join(TimeSpan.FromSeconds(20)) Then
            Throw New TimeoutException("Brand workspace UI contract did not finish.")
        End If
        If failure IsNot Nothing Then
            Throw New InvalidOperationException(
                "Brand workspace UI contract failed on its STA thread.",
                failure)
        End If
    End Sub

    Private Sub VerifyControlWorkflow()
        Dim document As DataSet = Program.CreateFixtureSchema()
        Dim service As New BrandService(New LegacyBrandRepository(Function() document))
        Dim session As New CatalogueSession("New Catalogue")
        Dim workspace As New WorkspaceController(
            session,
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            20)

        Using form As New BrandWorkspaceForm(service, workspace, False)
            form.ShowInTaskbar = False
            form.Opacity = 0
            form.Show()
            Application.DoEvents()

            Dim list As ListView = RequireControl(Of ListView)(form, "brandListView")
            Dim name As TextBox = RequireControl(Of TextBox)(form, "brandNameTextBox")
            Dim code As TextBox = RequireControl(Of TextBox)(form, "brandCodeTextBox")
            Dim notes As TextBox = RequireControl(Of TextBox)(form, "brandNotesTextBox")
            Dim create As Button = RequireControl(Of Button)(form, "newButton")
            Dim edit As Button = RequireControl(Of Button)(form, "editButton")
            Dim apply As Button = RequireControl(Of Button)(form, "applyButton")
            Dim undo As Button = RequireControl(Of Button)(form, "undoButton")
            Dim redo As Button = RequireControl(Of Button)(form, "redoButton")
            Dim filter As TextBox = RequireControl(Of TextBox)(form, "filterTextBox")
            Dim applyFilter As Button = RequireControl(Of Button)(form, "applyFilterButton")
            Dim clearFilter As Button = RequireControl(Of Button)(form, "clearFilterButton")
            Dim emptyState As Label = RequireControl(Of Label)(form, "emptyStateLabel")

            create.PerformClick()
            name.Text = "Maxell"
            code.Text = "mx"
            notes.Text = "Reference stock"
            apply.PerformClick()
            Application.DoEvents()
            AssertEqual("Maxell", service.Find("MX").Name, "control create")
            AssertEqual(1, list.Items.Count, "created list row")
            AssertEqual(True, session.IsDirty, "control create dirty state")

            list.Items(0).Selected = True
            list.Items(0).Focused = True
            Application.DoEvents()
            AssertEqual(True, edit.Enabled, "selected edit enabled")
            edit.PerformClick()
            name.Text = "Maxell Audio"
            notes.Text = "Updated through controls"
            apply.PerformClick()
            Application.DoEvents()
            AssertEqual("Maxell Audio", service.Find("MX").Name, "control edit")

            AssertEqual(True, undo.Enabled, "control undo enabled")
            undo.PerformClick()
            Application.DoEvents()
            AssertEqual("Maxell", service.Find("MX").Name, "control undo")
            AssertEqual(True, redo.Enabled, "control redo enabled")
            redo.PerformClick()
            Application.DoEvents()
            AssertEqual("Maxell Audio", service.Find("MX").Name, "control redo")

            filter.Text = "no-match"
            applyFilter.PerformClick()
            Application.DoEvents()
            AssertEqual(0, list.Items.Count, "control filter")
            AssertEqual(True, emptyState.Visible, "filtered empty presentation")
            clearFilter.PerformClick()
            Application.DoEvents()
            AssertEqual(1, list.Items.Count, "control filter clear")

            form.Close()
            Application.DoEvents()
        End Using
    End Sub

    Private Sub VerifyFormContract()
        Dim document As DataSet = Program.CreateFixtureSchema()
        Dim service As New BrandService(New LegacyBrandRepository(Function() document))
        Dim workspace As New WorkspaceController(
            New CatalogueSession("New Catalogue"),
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            20)

        Using form As New BrandWorkspaceForm(service, workspace, False)
            form.CreateControl()
            AssertEqual(AutoScaleMode.Dpi, form.AutoScaleMode, "DPI scaling")
            AssertEqual(True, form.KeyPreview, "form keyboard routing")
            AssertEqual(FormBorderStyle.Sizable, form.FormBorderStyle, "resizable shell")
            AssertEqual(SystemColors.Control, form.BackColor, "system form color")
            AssertEqual(
                SystemFonts.MessageBoxFont.Name,
                form.Font.Name,
                "system message font family")
            AssertEqual(
                True,
                form.MinimumSize.Width <= form.Size.Width,
                "scaled minimum width fits initial window")
            AssertEqual(
                True,
                form.MinimumSize.Height <= form.Size.Height,
                "scaled minimum height fits initial window")

            Dim list As ListView = RequireControl(Of ListView)(form, "brandListView")
            AssertEqual("Brands", list.AccessibleName, "list accessible name")
            AssertEqual(True, list.FullRowSelect, "full-row selection")
            AssertEqual(True, list.MultiSelect, "stable multi-selection")
            AssertEqual(3, list.Columns.Count, "list columns")

            Dim filter As TextBox = RequireControl(Of TextBox)(form, "filterTextBox")
            AssertEqual("Brand notes filter", filter.AccessibleName, "filter accessible name")
            Dim name As TextBox = RequireControl(Of TextBox)(form, "brandNameTextBox")
            Dim code As TextBox = RequireControl(Of TextBox)(form, "brandCodeTextBox")
            Dim notes As TextBox = RequireControl(Of TextBox)(form, "brandNotesTextBox")
            AssertEqual("Brand name", name.AccessibleName, "name accessible name")
            AssertEqual("Brand code", code.AccessibleName, "code accessible name")
            AssertEqual("Brand notes", notes.AccessibleName, "notes accessible name")
            AssertEqual(2, code.MaxLength, "legacy code input boundary")

            form.BeginCreate()
            AssertEqual(False, name.ReadOnly, "create name editable")
            AssertEqual(False, code.ReadOnly, "create code editable")
            AssertEqual(False, notes.ReadOnly, "create notes editable")
            AssertEqual(
                "C3.Presentation.WinForms",
                form.GetType().Assembly.GetName().Name,
                "single shared presentation assembly")
        End Using
    End Sub

    Private Sub VerifyLayoutStress()
        Dim scaleFactors As Single() = {1.0F, 1.25F, 1.5F, 2.0F}
        For Each scaleFactor As Single In scaleFactors
            Dim document As DataSet = Program.CreateFixtureSchema()
            Dim service As New BrandService(New LegacyBrandRepository(Function() document))
            Dim longName As String = New String("W"c, 100)
            Dim longNotes As String = New String("M"c, 2048)
            Dim created = service.Create(
                New BrandDraft(longName, "LX", longNotes),
                New DateTime(2026, 8, 5, 12, 0, 0))
            AssertEqual(True, created.IsSuccess, "long-content fixture")

            Dim workspace As New WorkspaceController(
                New CatalogueSession("Long Content Catalogue"),
                CatalogueCompatibilityMode.LegacyV1_1,
                False,
                20)

            Using form As New BrandWorkspaceForm(service, workspace, False)
                form.ShowInTaskbar = False
                form.Opacity = 0
                form.Scale(New SizeF(scaleFactor, scaleFactor))
                form.Show()
                Application.DoEvents()
                form.Size = form.MinimumSize
                Application.DoEvents()

                Dim scaleContext As String =
                    CInt(scaleFactor * 100.0F).ToString() & "% relative scale"
                Dim split As SplitContainer =
                    RequireControl(Of SplitContainer)(form, "contentSplit")
                Dim list As ListView = RequireControl(Of ListView)(form, "brandListView")
                Dim filter As TextBox = RequireControl(Of TextBox)(form, "filterTextBox")
                Dim name As TextBox = RequireControl(Of TextBox)(form, "brandNameTextBox")
                Dim notes As TextBox = RequireControl(Of TextBox)(form, "brandNotesTextBox")
                Dim commandBar As FlowLayoutPanel =
                    RequireControl(Of FlowLayoutPanel)(form, "commandBar")

                AssertEqual(True, split.Panel1.ClientSize.Width >= split.Panel1MinSize, scaleContext & " list panel")
                AssertEqual(True, split.Panel2.ClientSize.Width >= split.Panel2MinSize, scaleContext & " inspector panel")
                AssertEqual(True, list.ClientSize.Width >= 200, scaleContext & " list width")
                AssertEqual(True, list.ClientSize.Height >= 100, scaleContext & " list height")
                AssertEqual(True, filter.ClientSize.Width >= 100, scaleContext & " filter width")
                AssertEqual(True, notes.ClientSize.Height >= notes.Font.Height * 3, scaleContext & " notes height")
                AssertEqual(True, commandBar.PreferredSize.Width <= commandBar.ClientSize.Width, scaleContext & " command fit")

                AssertEqual(1, list.Items.Count, scaleContext & " long row count")
                AssertEqual(longName, list.Items(0).SubItems(1).Text, scaleContext & " long name row")
                AssertEqual(longNotes, list.Items(0).SubItems(2).Text, scaleContext & " long notes row")
                AssertEqual(SystemColors.Control, name.BackColor, scaleContext & " system read-only editor color")
                AssertEqual(SystemColors.Control, notes.BackColor, scaleContext & " system read-only notes color")

                AssertFitsParent(filter, scaleContext & " filter bounds")
                AssertFitsParent(list, scaleContext & " list bounds")
                AssertFitsParent(name, scaleContext & " name bounds")
                AssertFitsParent(notes, scaleContext & " notes bounds")
                For Each buttonName As String In {
                        "newButton",
                        "editButton",
                        "deleteButton",
                        "undoButton",
                        "redoButton"}
                    AssertFitsParent(
                        RequireControl(Of Button)(form, buttonName),
                        scaleContext & " " & buttonName)
                Next

                form.Close()
                Application.DoEvents()
            End Using
        Next
    End Sub

    Private Sub AssertFitsParent(control As Control, context As String)
        Dim parentClient As Size = control.Parent.ClientSize
        AssertEqual(True, control.Width > 0 AndAlso control.Height > 0, context & " positive size")
        AssertEqual(True, control.Left >= 0 AndAlso control.Top >= 0, context & " nonnegative origin")
        AssertEqual(True, control.Right <= parentClient.Width + 1, context & " right edge")
        AssertEqual(True, control.Bottom <= parentClient.Height + 1, context & " bottom edge")
    End Sub

    Private Function RequireControl(Of T As Control)(root As Control, name As String) As T
        Dim matches As Control() = root.Controls.Find(name, True)
        If matches.Length <> 1 Then
            Throw New InvalidOperationException(
                "Expected one control named '" & name & "' but found " & matches.Length.ToString() & ".")
        End If

        Dim result As T = TryCast(matches(0), T)
        If result Is Nothing Then
            Throw New InvalidOperationException(
                "Control '" & name & "' has unexpected type '" & matches(0).GetType().FullName & "'.")
        End If
        Return result
    End Function

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
