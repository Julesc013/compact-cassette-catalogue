Imports C3.Infrastructure.CatalogueFiles.Xml.V1_1
Imports C3.Presentation.WinForms.Features.Brands
Imports C3.Presentation.WinForms.Workspace
Imports System.Diagnostics

Friend Module BrandWorkspacePerformanceTests

    Private Const MaximumLegacyBrands As Integer = 26 * 26
    Private Const RefreshIterations As Integer = 20
    Private Const ConservativeRefreshBudgetMilliseconds As Double = 1000.0
    Private Const ConservativeEditorBudgetMilliseconds As Double = 250.0

    Public Sub MaximumWorkspaceMeetsConservativeBudgets()
        Dim result As BrandPerformanceMeasurement = Measure()
        AssertEqual(MaximumLegacyBrands, result.BrandCount, "maximum legacy Brand count")
        AssertAtMost(
            ConservativeRefreshBudgetMilliseconds,
            result.MaximumRefreshMilliseconds,
            "unfiltered maximum refresh")
        AssertAtMost(
            ConservativeRefreshBudgetMilliseconds,
            result.MaximumFilterMilliseconds,
            "filtered maximum refresh")
        AssertAtMost(
            ConservativeEditorBudgetMilliseconds,
            result.MaximumEditorMilliseconds,
            "editor activation")
    End Sub

    Public Sub WriteMeasurements()
        Dim result As BrandPerformanceMeasurement = Measure()
        Console.WriteLine(
            String.Format(
                CultureInfo.InvariantCulture,
                "BRAND_WORKSPACE_PERFORMANCE|brands={0}|iterations={1}|refresh-max-ms={2:F3}|filter-max-ms={3:F3}|editor-max-ms={4:F3}",
                result.BrandCount,
                RefreshIterations,
                result.MaximumRefreshMilliseconds,
                result.MaximumFilterMilliseconds,
                result.MaximumEditorMilliseconds))
    End Sub

    Private Function Measure() As BrandPerformanceMeasurement
        Dim document As DataSet = Program.CreateFixtureSchema()
        Dim service As New BrandService(New LegacyBrandRepository(Function() document))
        For first As Integer = AscW("A"c) To AscW("Z"c)
            For second As Integer = AscW("A"c) To AscW("Z"c)
                Dim code As String = ChrW(first) & ChrW(second)
                Dim created As BrandOperationResult = service.Create(
                    New BrandDraft("Brand " & code, code, "Synthetic performance fixture " & code),
                    New DateTime(2026, 8, 5))
                If Not created.IsSuccess Then
                    Throw New InvalidOperationException(
                        "Could not create synthetic Brand " & code & ": " & created.Message)
                End If
            Next
        Next

        Dim workspace As New WorkspaceController(
            New CatalogueSession("Performance Catalogue"),
            CatalogueCompatibilityMode.LegacyV1_1,
            False,
            100)
        Dim presenter As New BrandWorkspacePresenter(service, workspace)
        presenter.Refresh(String.Empty)

        Dim maximumRefresh As Double
        Dim maximumFilter As Double
        Dim stopwatch As New Stopwatch()
        For iteration As Integer = 1 To RefreshIterations
            stopwatch.Restart()
            presenter.Refresh(String.Empty)
            stopwatch.Stop()
            maximumRefresh = Math.Max(maximumRefresh, stopwatch.Elapsed.TotalMilliseconds)

            stopwatch.Restart()
            presenter.Refresh("fixture ZZ")
            stopwatch.Stop()
            maximumFilter = Math.Max(maximumFilter, stopwatch.Elapsed.TotalMilliseconds)
        Next

        presenter.Refresh(String.Empty)
        presenter.Select("ZZ")
        Dim maximumEditor As Double
        For iteration As Integer = 1 To RefreshIterations
            stopwatch.Restart()
            If Not presenter.BeginEdit() Then
                Throw New InvalidOperationException("Could not begin the measured Brand editor.")
            End If
            presenter.CancelEditor()
            stopwatch.Stop()
            maximumEditor = Math.Max(maximumEditor, stopwatch.Elapsed.TotalMilliseconds)
        Next

        Return New BrandPerformanceMeasurement(
            presenter.List.Count,
            maximumRefresh,
            maximumFilter,
            maximumEditor)
    End Function

    Private Sub AssertAtMost(limit As Double, actual As Double, context As String)
        If actual > limit Then
            Throw New InvalidOperationException(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}: expected no more than {1:F3} ms but measured {2:F3} ms.",
                    context,
                    limit,
                    actual))
        End If
    End Sub

    Private Sub AssertEqual(expected As Object, actual As Object, context As String)
        If Not Object.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                context & ": expected '" & expected.ToString() & "' but got '" & actual.ToString() & "'.")
        End If
    End Sub

    Private NotInheritable Class BrandPerformanceMeasurement
        Public Sub New(
                brandCount As Integer,
                maximumRefreshMilliseconds As Double,
                maximumFilterMilliseconds As Double,
                maximumEditorMilliseconds As Double)
            Me.BrandCount = brandCount
            Me.MaximumRefreshMilliseconds = maximumRefreshMilliseconds
            Me.MaximumFilterMilliseconds = maximumFilterMilliseconds
            Me.MaximumEditorMilliseconds = maximumEditorMilliseconds
        End Sub

        Public ReadOnly Property BrandCount As Integer

        Public ReadOnly Property MaximumRefreshMilliseconds As Double

        Public ReadOnly Property MaximumFilterMilliseconds As Double

        Public ReadOnly Property MaximumEditorMilliseconds As Double
    End Class

End Module
