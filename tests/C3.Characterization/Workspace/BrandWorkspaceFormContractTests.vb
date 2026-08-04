Imports C3.Infrastructure.CatalogueFiles.Xml.V1_1
Imports C3.Presentation.WinForms.Features.Brands
Imports C3.Presentation.WinForms.Workspace
Imports System.Threading
Imports System.Windows.Forms

Friend Module BrandWorkspaceFormContractTests

    Public Sub PreservesDesignerDpiKeyboardAndAccessibilityContracts()
        Dim failure As Exception = Nothing
        Dim worker As New Thread(
            Sub()
                Try
                    VerifyFormContract()
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
