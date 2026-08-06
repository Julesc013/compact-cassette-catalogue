Imports System.Reflection
Imports Compact_Cassette_Catalogue

Module Program

    <STAThread()>
    Sub Main(arguments As String())
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim outputPath As String = RequiredArgument(arguments, "--output")
        Dim result As New LayoutCellResult() With {
            .SchemaVersion = 2,
            .SourceCommit = RequiredArgument(arguments, "--source-commit"),
            .FormName = RequiredArgument(arguments, "--form"),
            .AssemblyPath = OptionalArgument(arguments, "--assembly-path"),
            .TypeName = OptionalArgument(arguments, "--type-name"),
            .PageState = OptionalArgument(arguments, "--page-state"),
            .ContentProfile = RequiredArgument(arguments, "--profile"),
            .RequestedWidth = ParseInteger(RequiredArgument(arguments, "--width"), "width"),
            .RequestedHeight = ParseInteger(RequiredArgument(arguments, "--height"), "height"),
            .ScaleFactor = ParseSingle(RequiredArgument(arguments, "--scale"), "scale"),
            .Controls = New List(Of ControlRecord)(),
            .Failures = New List(Of String)()
        }

        Dim started As DateTime = DateTime.UtcNow
        Try
            ExecuteCell(result, OptionalArgument(arguments, "--screenshot"))
        Catch ex As Exception
            result.Failures.Add("HARNESS_EXCEPTION:" & ex.GetType().FullName & ":" & ex.Message)
        Finally
            result.DurationMilliseconds = CLng((DateTime.UtcNow - started).TotalMilliseconds)
            result.Passed = result.Failures.Count = 0
            WriteResult(outputPath, result)
        End Try

        If Not result.Passed Then Environment.ExitCode = 2
    End Sub

    Private Sub ExecuteCell(result As LayoutCellResult, screenshotPath As String)
        Dim form As Form = CreateForm(result.FormName, result.AssemblyPath, result.TypeName)
        Try
            If Not String.IsNullOrEmpty(result.AssemblyPath) Then
                SuppressLoadHandlers(form)
                ConfigureSetupPage(form, result.PageState)
            End If
            form.StartPosition = FormStartPosition.Manual
            form.Location = New Point(8, 8)
            form.ShowInTaskbar = False
            form.Font = New Font(
                form.Font.FontFamily,
                form.Font.SizeInPoints * result.ScaleFactor,
                form.Font.Style,
                GraphicsUnit.Point)
            PrepareFormState(form)
            form.Show()
            Application.DoEvents()
            PopulateRepresentativeContent(form, result.ContentProfile)
            Application.DoEvents()
            form.Size = New Size(result.RequestedWidth, result.RequestedHeight)
            form.PerformLayout()
            Application.DoEvents()

            result.ActualClientWidth = form.ClientSize.Width
            result.ActualClientHeight = form.ClientSize.Height
            result.HandleCreated = form.IsHandleCreated
            result.AutoScroll = form.AutoScroll
            result.AutoScaleMode = form.AutoScaleMode.ToString()
            CaptureControls(form, form, result.Controls)
            ApplyAssertions(form, result)

            If Not String.IsNullOrEmpty(screenshotPath) Then
                SaveDiagnosticScreenshot(form, screenshotPath)
            End If
        Finally
            form.Hide()
            form.Dispose()
            Application.DoEvents()
        End Try
    End Sub

    Private Function CreateForm(formName As String, assemblyPath As String, explicitTypeName As String) As Form
        Dim formType As Type
        If String.IsNullOrEmpty(assemblyPath) Then
            Dim typeName As String = "Compact_Cassette_Catalogue." & formName
            formType = GetType(frmMain).Assembly.GetType(typeName, True, False)
        Else
            If Not IO.File.Exists(assemblyPath) Then
                Throw New IO.FileNotFoundException("External form assembly was not found.", assemblyPath)
            End If
            If String.IsNullOrEmpty(explicitTypeName) Then
                Throw New ArgumentException("--type-name is required with --assembly-path.")
            End If
            formType = Assembly.LoadFrom(IO.Path.GetFullPath(assemblyPath)).GetType(explicitTypeName, True, False)
        End If
        Dim instance As Object = Activator.CreateInstance(formType)
        Dim form As Form = TryCast(instance, Form)
        If form Is Nothing Then Throw New InvalidOperationException(formType.FullName & " is not a Form.")
        Return form
    End Function

    Private Sub SuppressLoadHandlers(form As Form)
        Dim eventField As FieldInfo = Nothing
        For Each candidate As FieldInfo In GetType(Form).GetFields(BindingFlags.NonPublic Or BindingFlags.Static)
            Dim normalized As String = candidate.Name.Replace("_", String.Empty)
            If String.Equals(normalized, "eventload", StringComparison.OrdinalIgnoreCase) OrElse
                    String.Equals(normalized, "loadevent", StringComparison.OrdinalIgnoreCase) Then
                eventField = candidate
                Exit For
            End If
        Next
        If eventField Is Nothing Then Throw New InvalidOperationException("The WinForms Load event key could not be located.")

        Dim eventsProperty As PropertyInfo = GetType(ComponentModel.Component).GetProperty(
            "Events", BindingFlags.NonPublic Or BindingFlags.Instance)
        If eventsProperty Is Nothing Then Throw New InvalidOperationException("The component event list could not be located.")
        Dim eventList As ComponentModel.EventHandlerList = DirectCast(
            eventsProperty.GetValue(form, Nothing), ComponentModel.EventHandlerList)
        Dim eventKey As Object = eventField.GetValue(Nothing)
        Dim handler As [Delegate] = eventList(eventKey)
        If handler IsNot Nothing Then eventList.RemoveHandler(eventKey, handler)
    End Sub

    Private Sub ConfigureSetupPage(form As Form, pageState As String)
        If String.IsNullOrEmpty(pageState) Then Return

        Dim pageNames As String() = {
            "pnlIntroduction", "pnlOptions", "pnlReady", "pnlInstall",
            "pnlUninstall", "pnlSuccess", "pnlFailure"
        }
        For Each pageName As String In pageNames
            Dim page As Control = FindControl(form, pageName)
            If page IsNot Nothing Then page.Visible = String.Equals(pageName, pageState, StringComparison.Ordinal)
        Next

        Dim nextButton As Control = FindControl(form, "btnNext")
        Dim installButton As Control = FindControl(form, "btnInstall")
        Dim uninstallButton As Control = FindControl(form, "btnUninstall")
        Dim backButton As Control = FindControl(form, "btnBack")
        Dim cancelButton As Control = FindControl(form, "btnCancel")

        If nextButton IsNot Nothing Then
            nextButton.Visible = Not String.Equals(pageState, "pnlReady", StringComparison.Ordinal)
            nextButton.Enabled = String.Equals(pageState, "pnlIntroduction", StringComparison.Ordinal) OrElse
                String.Equals(pageState, "pnlOptions", StringComparison.Ordinal)
        End If
        If installButton IsNot Nothing Then
            installButton.Visible = String.Equals(pageState, "pnlReady", StringComparison.Ordinal)
            installButton.Enabled = installButton.Visible
        End If
        If uninstallButton IsNot Nothing Then
            uninstallButton.Visible = True
            uninstallButton.Enabled = String.Equals(pageState, "pnlReady", StringComparison.Ordinal)
        End If
        If backButton IsNot Nothing Then
            backButton.Visible = form.Name = "frmMain" AndAlso nextButton IsNot Nothing
            backButton.Enabled = String.Equals(pageState, "pnlOptions", StringComparison.Ordinal) OrElse
                String.Equals(pageState, "pnlReady", StringComparison.Ordinal)
        End If
        If cancelButton IsNot Nothing Then
            cancelButton.Visible = form.Name = "frmMain"
            cancelButton.Enabled = Not String.Equals(pageState, "pnlInstall", StringComparison.Ordinal) AndAlso
                Not String.Equals(pageState, "pnlUninstall", StringComparison.Ordinal)
        End If
    End Sub

    Private Sub PrepareFormState(form As Form)
        Dim brandEdit As frmBrandEdit = TryCast(form, frmBrandEdit)
        If brandEdit IsNot Nothing Then
            Dim brandRow As DataRow = CreateRepresentativeRow(brands)
            brandRow("Brand") = "Representative Brand"
            brandRow("Code") = "RB"
            brandRow("Date") = DateTime.Today
            brandRow("Notes") = "Representative Brand notes"
            brands.Rows.Add(brandRow)
            brandEdit.brandRow = brandRow
            Return
        End If

        Dim modelEdit As frmModelEdit = TryCast(form, frmModelEdit)
        If modelEdit IsNot Nothing Then
            Dim modelRow As DataRow = CreateRepresentativeRow(models)
            modelRow("Brand") = "Representative Brand"
            modelRow("Type") = 1
            modelRow("Model") = "Representative Model"
            modelRow("Code") = "RM"
            modelRow("Identifier") = "RB-1-RM"
            modelRow("Name") = "Representative Model Name"
            modelRow("Number") = 1
            modelRow("Date") = DateTime.Today
            modelRow("Notes") = "Representative Model notes"
            models.Rows.Add(modelRow)
            modelEdit.modelRow = modelRow
            Return
        End If

        Dim deckEdit As frmDeckEdit = TryCast(form, frmDeckEdit)
        If deckEdit Is Nothing Then Return

        Dim row As DataRow = CreateRepresentativeRow(decks)
        row("Manufacturer") = "Representative Manufacturer"
        row("Model") = "Representative Model"
        row("Name") = "Representative Manufacturer Representative Model"
        row("Year") = 2020
        row("Condition") = 8
        row("Type1") = True
        row("SpeedNorm") = True
        row("FrequencyLow") = 20
        row("FrequencyHigh") = 20000
        row("SignalRatio") = 60
        row("SignalRatioNR") = "None"
        row("WowFlutter") = CDec(0.1)
        row("Distortion") = CDec(0.1)
        row("Heads") = 2
        row("Wells") = 1
        decks.Rows.Add(row)
        deckEdit.deckRow = row
    End Sub

    Private Function CreateRepresentativeRow(table As DataTable) As DataRow
        Dim row As DataRow = table.NewRow()
        For Each column As DataColumn In table.Columns
            If column.DataType Is GetType(String) Then
                row(column) = String.Empty
            ElseIf column.DataType Is GetType(Boolean) Then
                row(column) = False
            ElseIf column.DataType Is GetType(Integer) Then
                row(column) = 0
            ElseIf column.DataType Is GetType(Decimal) Then
                row(column) = Decimal.Zero
            ElseIf column.DataType Is GetType(DateTime) Then
                row(column) = DateTime.Today
            End If
        Next
        Return row
    End Function

    Private Sub PopulateRepresentativeContent(form As Form, profile As String)
        If String.Equals(profile, "maximum", StringComparison.Ordinal) Then
            For Each control As Control In Descendants(form)
                Dim textBox As TextBox = TryCast(control, TextBox)
                If textBox IsNot Nothing AndAlso Not textBox.ReadOnly Then
                    textBox.Text = "Representative maximum-length catalogue value — WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW"
                End If
                Dim combo As ComboBox = TryCast(control, ComboBox)
                If combo IsNot Nothing Then
                    Dim representativeChoice As String = "Representative maximum-length catalogue choice — WWWWWWWWWWWWWWWW"
                    If combo.DropDownStyle = ComboBoxStyle.DropDownList Then
                        'Key-bound catalogue choices must retain a real loaded item: selecting an
                        'invented display value would correctly fail the stable-identity contract.
                        combo.Items.Add(representativeChoice)
                    Else
                        combo.Text = representativeChoice
                    End If
                End If
            Next

            Dim directoryText As Control = FindControl(form, "txtDirectory")
            If directoryText IsNot Nothing Then
                directoryText.Text = "C:\Program Files\Compact Cassette Catalogue\Representative Long Installation Directory"
            End If
            Dim statusText As Control = FindControl(form, "lblStatusProcess")
            If statusText IsNot Nothing Then
                statusText.Text = "Revalidating representative installed ownership and verified offline payload"
            End If
            Dim failureText As Control = FindControl(form, "lblFailure")
            If failureText IsNot Nothing Then
                failureText.Text = "The representative setup operation could not complete because a deliberately long diagnostic message must remain visible and readable."
            End If
        End If
    End Sub

    Private Sub CaptureControls(form As Form, parent As Control, records As List(Of ControlRecord))
        For Each control As Control In parent.Controls
            If control.Visible Then
                Dim bounds As Rectangle = BoundsInForm(form, control)
                Dim preferred As Size = SafePreferredSize(control)
                Dim measured As Size = MeasureText(control)
                records.Add(New ControlRecord() With {
                    .Name = control.Name,
                    .TypeName = control.GetType().FullName,
                    .ParentPath = ControlPath(control.Parent),
                    .Left = bounds.Left,
                    .Top = bounds.Top,
                    .Width = bounds.Width,
                    .Height = bounds.Height,
                    .PreferredWidth = preferred.Width,
                    .PreferredHeight = preferred.Height,
                    .MeasuredTextWidth = measured.Width,
                    .MeasuredTextHeight = measured.Height,
                    .Enabled = control.Enabled,
                    .TabStop = control.TabStop,
                    .TabIndex = control.TabIndex,
                    .Dock = control.Dock.ToString(),
                    .Anchor = control.Anchor.ToString(),
                    .AutoSize = control.AutoSize,
                    .AccessibleName = If(control.AccessibleName, String.Empty),
                    .AccessibleRole = control.AccessibleRole.ToString(),
                    .ScrollAncestor = ScrollAncestorName(control),
                    .HandleCreated = control.IsHandleCreated
                })
                CaptureControls(form, control, records)
            End If
        Next
    End Sub

    Private Sub ApplyAssertions(form As Form, result As LayoutCellResult)
        If form.AutoScroll Then result.Failures.Add("FORM_AUTOSCROLL")
        If form.AutoScaleMode <> AutoScaleMode.Font Then result.Failures.Add("AUTOSCALE_NOT_FONT")

        Dim names As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        For Each control As Control In Descendants(form)
            If control.Name.Length > 0 Then
                If Not names.ContainsKey(control.Name) Then names.Add(control.Name, 0)
                names(control.Name) += 1
            End If
        Next
        For Each pair As KeyValuePair(Of String, Integer) In names
            If pair.Value > 1 Then result.Failures.Add("DUPLICATE_CONTROL_NAME:" & pair.Key)
        Next

        CheckParentScopes(form, result.Failures)
        CheckCommandBars(form, result.Failures)
        CheckReachability(form, result.Failures)
        CheckAutoSizeLabels(form, result.Failures)
        CheckRootAnchors(form, result.Failures)
        CheckKnownAlpha4Collisions(form, result.Failures)
    End Sub

    Private Sub CheckParentScopes(parent As Control, failures As List(Of String))
        Dim tabIndexes As New Dictionary(Of Integer, String)()
        Dim visibleChildren As New List(Of Control)()
        For Each child As Control In parent.Controls
            If child.Visible Then visibleChildren.Add(child)
            If child.Visible AndAlso child.Enabled AndAlso child.TabStop Then
                If tabIndexes.ContainsKey(child.TabIndex) Then
                    failures.Add("DUPLICATE_TAB_INDEX:" & ControlPath(parent) & ":" & child.TabIndex.ToString(CultureInfo.InvariantCulture))
                Else
                    tabIndexes.Add(child.TabIndex, child.Name)
                End If
            End If
        Next

        For firstIndex As Integer = 0 To visibleChildren.Count - 1
            For secondIndex As Integer = firstIndex + 1 To visibleChildren.Count - 1
                Dim first As Control = visibleChildren(firstIndex)
                Dim second As Control = visibleChildren(secondIndex)
                If IsMajorContainer(first) AndAlso IsMajorContainer(second) AndAlso
                        Not IsAllowedOverlay(first, second) AndAlso
                        first.Bounds.IntersectsWith(second.Bounds) Then
                    failures.Add("SIBLING_INTERSECTION:" & ControlPath(parent) & ":" & first.Name & ":" & second.Name)
                End If
            Next
        Next

        For Each child As Control In parent.Controls
            CheckParentScopes(child, failures)
        Next
    End Sub

    Private Sub CheckCommandBars(form As Form, failures As List(Of String))
        For Each control As Control In Descendants(form)
            Dim commandBar As FlowLayoutPanel = TryCast(control, FlowLayoutPanel)
            If commandBar Is Nothing OrElse
                    commandBar.Name.IndexOf("Commands", StringComparison.OrdinalIgnoreCase) < 0 Then
                Continue For
            End If

            For Each command As Control In commandBar.Controls
                If command.Visible AndAlso Not commandBar.ClientRectangle.Contains(command.Bounds) Then
                    failures.Add("COMMAND_OUTSIDE_BAR:" & commandBar.Name & ":" & command.Name)
                End If
            Next
        Next
    End Sub

    Private Sub CheckReachability(form As Form, failures As List(Of String))
        For Each control As Control In Descendants(form)
            If Not control.Visible OrElse String.IsNullOrEmpty(control.Name) Then Continue For

            Dim bounds As Rectangle = BoundsInForm(form, control)
            If Not form.ClientRectangle.Contains(bounds) AndAlso
                    String.IsNullOrEmpty(ScrollAncestorName(control)) Then
                failures.Add("CONTROL_OUTSIDE_NONSCROLLABLE_VIEW:" & ControlPath(control))
                Continue For
            End If

            If control.Parent Is Nothing OrElse control.Parent Is form OrElse
                    Not String.IsNullOrEmpty(ScrollAncestorName(control)) Then Continue For
            If Not control.Parent.ClientRectangle.Contains(control.Bounds) Then
                failures.Add("CONTROL_CLIPPED_BY_PARENT:" & ControlPath(control))
            End If
        Next
    End Sub

    Private Sub CheckAutoSizeLabels(form As Form, failures As List(Of String))
        For Each control As Control In Descendants(form)
            Dim label As Label = TryCast(control, Label)
            If label Is Nothing OrElse Not label.Visible OrElse Not label.AutoSize Then Continue For
            Dim measured As Size = MeasureText(label)
            If measured.Width > label.ClientSize.Width OrElse measured.Height > label.ClientSize.Height Then
                failures.Add("AUTOSIZE_LABEL_CLIPPED:" & ControlPath(label))
            End If
        Next
    End Sub

    Private Sub CheckRootAnchors(form As Form, failures As List(Of String))
        If Not form.AutoScroll Then Return
        For Each child As Control In form.Controls
            If (child.Anchor And AnchorStyles.Right) = AnchorStyles.Right OrElse
                    (child.Anchor And AnchorStyles.Bottom) = AnchorStyles.Bottom Then
                failures.Add("FORM_SCROLL_RIGHT_BOTTOM_ANCHOR:" & child.Name)
            End If
        Next
    End Sub

    Private Sub CheckKnownAlpha4Collisions(form As Form, failures As List(Of String))
        If Not String.Equals(form.Name, "frmMain", StringComparison.Ordinal) Then Return
        Dim identification As Control = FindControl(form, "grpIdentification")
        Dim scroll As Control = FindControl(form, "grpScroll")
        If identification IsNot Nothing AndAlso scroll IsNot Nothing AndAlso
                BoundsInForm(form, identification).IntersectsWith(BoundsInForm(form, scroll)) Then
            failures.Add("KNOWN_MAIN_IDENTIFICATION_SCROLL_COLLISION")
        End If
    End Sub

    Private Function IsMajorContainer(control As Control) As Boolean
        Return TypeOf control Is GroupBox OrElse TypeOf control Is Panel OrElse
            TypeOf control Is TableLayoutPanel OrElse TypeOf control Is FlowLayoutPanel OrElse
            TypeOf control Is SplitContainer
    End Function

    Private Function IsAllowedOverlay(first As Control, second As Control) As Boolean
        Dim names As String = first.Name & "|" & second.Name
        Return String.Equals(names, "pnlEmptyCatalogue|pnlEditorViewport", StringComparison.Ordinal) OrElse
            String.Equals(names, "pnlEditorViewport|pnlEmptyCatalogue", StringComparison.Ordinal)
    End Function

    Private Function BoundsInForm(form As Form, control As Control) As Rectangle
        Return form.RectangleToClient(control.RectangleToScreen(control.ClientRectangle))
    End Function

    Private Function SafePreferredSize(control As Control) As Size
        Try
            Return control.GetPreferredSize(Size.Empty)
        Catch
            Return control.Size
        End Try
    End Function

    Private Function MeasureText(control As Control) As Size
        If String.IsNullOrEmpty(control.Text) OrElse Not (
                TypeOf control Is Label OrElse TypeOf control Is Button OrElse
                TypeOf control Is CheckBox OrElse TypeOf control Is RadioButton) Then
            Return Size.Empty
        End If
        Return TextRenderer.MeasureText(control.Text, control.Font, Size.Empty, TextFormatFlags.SingleLine)
    End Function

    Private Function ScrollAncestorName(control As Control) As String
        Dim current As Control = control.Parent
        While current IsNot Nothing
            Dim scrollable As ScrollableControl = TryCast(current, ScrollableControl)
            If scrollable IsNot Nothing AndAlso scrollable.AutoScroll Then Return ControlPath(current)
            current = current.Parent
        End While
        Return String.Empty
    End Function

    Private Function ControlPath(control As Control) As String
        If control Is Nothing Then Return String.Empty
        Dim segments As New List(Of String)()
        Dim current As Control = control
        While current IsNot Nothing
            segments.Insert(0, If(current.Name.Length > 0, current.Name, current.GetType().Name))
            current = current.Parent
        End While
        Return String.Join("/", segments.ToArray())
    End Function

    Private Function Descendants(root As Control) As List(Of Control)
        Dim result As New List(Of Control)()
        AddDescendants(root, result)
        Return result
    End Function

    Private Sub AddDescendants(parent As Control, result As List(Of Control))
        For Each child As Control In parent.Controls
            result.Add(child)
            AddDescendants(child, result)
        Next
    End Sub

    Private Function FindControl(parent As Control, name As String) As Control
        Dim matches As Control() = parent.Controls.Find(name, True)
        If matches.Length = 0 Then Return Nothing
        Return matches(0)
    End Function

    Private Sub SaveDiagnosticScreenshot(form As Form, path As String)
        Dim directory As String = IO.Path.GetDirectoryName(path)
        If directory.Length > 0 Then IO.Directory.CreateDirectory(directory)
        Using image As New Bitmap(Math.Max(1, form.ClientSize.Width), Math.Max(1, form.ClientSize.Height))
            form.DrawToBitmap(image, New Rectangle(Point.Empty, image.Size))
            image.Save(path, Imaging.ImageFormat.Png)
        End Using
    End Sub

    Private Sub WriteResult(path As String, result As LayoutCellResult)
        Dim directory As String = IO.Path.GetDirectoryName(path)
        If directory.Length > 0 Then IO.Directory.CreateDirectory(directory)
        Dim serializer As New DataContractJsonSerializer(GetType(LayoutCellResult))
        Using stream As New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)
            serializer.WriteObject(stream, result)
        End Using
    End Sub

    Private Function RequiredArgument(arguments As String(), name As String) As String
        Dim value As String = OptionalArgument(arguments, name)
        If String.IsNullOrEmpty(value) Then Throw New ArgumentException("Missing required argument " & name & ".")
        Return value
    End Function

    Private Function OptionalArgument(arguments As String(), name As String) As String
        For index As Integer = 0 To arguments.Length - 2
            If String.Equals(arguments(index), name, StringComparison.Ordinal) Then Return arguments(index + 1)
        Next
        Return String.Empty
    End Function

    Private Function ParseInteger(value As String, name As String) As Integer
        Dim parsed As Integer
        If Not Integer.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, parsed) Then
            Throw New ArgumentException("Invalid " & name & ": " & value)
        End If
        Return parsed
    End Function

    Private Function ParseSingle(value As String, name As String) As Single
        Dim parsed As Single
        If Not Single.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
            Throw New ArgumentException("Invalid " & name & ": " & value)
        End If
        Return parsed
    End Function

End Module

<DataContract()>
Public Class LayoutCellResult
    <DataMember(Order:=0)> Public Property SchemaVersion As Integer
    <DataMember(Order:=1)> Public Property SourceCommit As String
    <DataMember(Order:=2)> Public Property FormName As String
    <DataMember(Order:=3)> Public Property ContentProfile As String
    <DataMember(Order:=4)> Public Property RequestedWidth As Integer
    <DataMember(Order:=5)> Public Property RequestedHeight As Integer
    <DataMember(Order:=6)> Public Property ScaleFactor As Single
    <DataMember(Order:=7)> Public Property ActualClientWidth As Integer
    <DataMember(Order:=8)> Public Property ActualClientHeight As Integer
    <DataMember(Order:=9)> Public Property HandleCreated As Boolean
    <DataMember(Order:=10)> Public Property AutoScroll As Boolean
    <DataMember(Order:=11)> Public Property AutoScaleMode As String
    <DataMember(Order:=12)> Public Property DurationMilliseconds As Long
    <DataMember(Order:=13)> Public Property Passed As Boolean
    <DataMember(Order:=14)> Public Property Failures As List(Of String)
    <DataMember(Order:=15)> Public Property Controls As List(Of ControlRecord)
    Public Property AssemblyPath As String
    <DataMember(Order:=16)> Public Property TypeName As String
    <DataMember(Order:=17)> Public Property PageState As String
End Class

<DataContract()>
Public Class ControlRecord
    <DataMember(Order:=0)> Public Property Name As String
    <DataMember(Order:=1)> Public Property TypeName As String
    <DataMember(Order:=2)> Public Property ParentPath As String
    <DataMember(Order:=3)> Public Property Left As Integer
    <DataMember(Order:=4)> Public Property Top As Integer
    <DataMember(Order:=5)> Public Property Width As Integer
    <DataMember(Order:=6)> Public Property Height As Integer
    <DataMember(Order:=7)> Public Property PreferredWidth As Integer
    <DataMember(Order:=8)> Public Property PreferredHeight As Integer
    <DataMember(Order:=9)> Public Property MeasuredTextWidth As Integer
    <DataMember(Order:=10)> Public Property MeasuredTextHeight As Integer
    <DataMember(Order:=11)> Public Property Enabled As Boolean
    <DataMember(Order:=12)> Public Property TabStop As Boolean
    <DataMember(Order:=13)> Public Property TabIndex As Integer
    <DataMember(Order:=14)> Public Property Dock As String
    <DataMember(Order:=15)> Public Property Anchor As String
    <DataMember(Order:=16)> Public Property AutoSize As Boolean
    <DataMember(Order:=17)> Public Property AccessibleName As String
    <DataMember(Order:=18)> Public Property AccessibleRole As String
    <DataMember(Order:=19)> Public Property ScrollAncestor As String
    <DataMember(Order:=20)> Public Property HandleCreated As Boolean
End Class
