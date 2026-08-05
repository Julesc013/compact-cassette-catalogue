Imports System.IO

Public Class frmMain

    Private Const PageCount As Integer = 3
    Private _pageIndex As Integer
    Private _bundle As C3Setup.SetupBundleContext
    Private _facts As C3Setup.SetupEnvironmentFacts
    Private _operationInProgress As Boolean
    Private _allowClose As Boolean

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            _bundle = C3Setup.SetupBundleRuntime.Load(AppDomain.CurrentDomain.BaseDirectory,
                                                       System.Windows.Forms.Application.ExecutablePath)
            _facts = C3Setup.SetupEnvironment.Capture()
            C3Setup.SetupEnvironment.Validate(_bundle.Manifest, _facts, _bundle.PayloadBytes)
            installDirectory = C3Setup.SetupEnvironment.DefaultInstallRoot(_facts)
            txtDirectory.Text = installDirectory
            dialogDirectory.SelectedPath = installDirectory
            chkStartMenu.Checked = True
            chkStartMenu.Enabled = False
            chkDesktop.Checked = True
            DisplayPage()
            btnNext.Select()
        Catch ex As Exception
            ShowFailure("Setup could not validate this offline bundle or computer." & Environment.NewLine & Environment.NewLine & ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If _operationInProgress Then Return
        If ConfirmCancel() = DialogResult.Yes Then
            _allowClose = True
            System.Windows.Forms.Application.Exit()
        End If
    End Sub

    Private Sub btnInstall_Click(sender As Object, e As EventArgs) Handles btnInstall.Click
        If _bundle Is Nothing OrElse _facts Is Nothing OrElse _operationInProgress Then Return
        _operationInProgress = True
        _pageIndex = PageCount
        DisplayPage()
        barInstallProgress.Minimum = 0
        barInstallProgress.Maximum = 5
        barInstallProgress.Value = 0
        Try
            SetStatus("Revalidating offline payload", 1)
            _facts = C3Setup.SetupEnvironment.Capture()
            C3Setup.SetupEnvironment.Validate(_bundle.Manifest, _facts, _bundle.PayloadBytes)
            installDirectory = C3Setup.SetupEnvironment.ValidateInstallRoot(_facts, txtDirectory.Text)
            SetStatus("Installing verified program files", 2)
            Dim state As C3Setup.InstalledState = C3Setup.SetupInstallOperation.Execute(
                _bundle.ManifestPath,
                _bundle.PayloadDirectory,
                installDirectory,
                _bundle.Manifest.SourceCommit,
                _bundle.SetupExecutableSha256,
                chkDesktop.Checked,
                _facts,
                New C3Setup.WindowsSetupShortcutAccess(),
                New C3Setup.WindowsSetupRegistryAccess(),
                Nothing)
            SetStatus("Verifying installed ownership", 4)
            C3Setup.PayloadVerifier.VerifyOwnedFiles(state.Manifest, state.InstallRoot)
            C3Setup.InstalledStateCodec.Read(Path.Combine(state.InstallRoot, C3Setup.InstalledStateCodec.FileName))
            startPath = Path.Combine(state.InstallRoot, "Compact Cassette Catalogue.exe")
            SetStatus("Installation complete", 5)
            _allowClose = True
            frmSuccess.Show()
            Me.Close()
        Catch ex As Exception
            ShowFailure("Setup did not complete. Any partial owned changes were rolled back." & Environment.NewLine & Environment.NewLine & ex.Message)
        Finally
            _operationInProgress = False
        End Try
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If _pageIndex < PageCount - 1 Then _pageIndex += 1
        DisplayPage()
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        If _pageIndex > 0 Then _pageIndex -= 1
        DisplayPage()
    End Sub

    Private Sub btnChangeDirectory_Click(sender As Object, e As EventArgs) Handles btnChangeDirectory.Click
        If Directory.Exists(txtDirectory.Text) Then dialogDirectory.SelectedPath = txtDirectory.Text
        If dialogDirectory.ShowDialog(Me) = DialogResult.OK Then txtDirectory.Text = dialogDirectory.SelectedPath
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _allowClose Then Return
        If _operationInProgress Then
            e.Cancel = True
            Return
        End If
        If ConfirmCancel() <> DialogResult.Yes Then e.Cancel = True
    End Sub

    Private Function ConfirmCancel() As DialogResult
        Return MessageBox.Show(Me,
                               "Are you sure you want to cancel Compact Cassette Catalogue setup?",
                               "Cancel Setup",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button2)
    End Function

    Private Sub SetStatus(message As String, progress As Integer)
        lblStatusProcess.Text = message
        lblStatusProcess.Update()
        barInstallProgress.Value = progress
        barInstallProgress.Update()
    End Sub

    Private Sub ShowFailure(message As String)
        setupFailureMessage = message
        _allowClose = True
        frmFailure.Show()
        Me.Close()
    End Sub

    Private Sub DisplayPage()
        pnlIntroduction.Visible = _pageIndex = 0
        pnlIntroduction.Enabled = pnlIntroduction.Visible
        pnlOptions.Visible = _pageIndex = 1
        pnlOptions.Enabled = pnlOptions.Visible
        pnlReady.Visible = _pageIndex = 2
        pnlReady.Enabled = pnlReady.Visible
        pnlInstall.Visible = _pageIndex = 3
        pnlInstall.Enabled = pnlInstall.Visible

        btnBack.Enabled = _pageIndex > 0 AndAlso _pageIndex < 3
        btnNext.Visible = _pageIndex <> 2
        btnNext.Enabled = _pageIndex < 2
        btnInstall.Visible = _pageIndex = 2
        btnInstall.Enabled = _pageIndex = 2 AndAlso Not _operationInProgress
        btnCancel.Enabled = Not _operationInProgress

        If _pageIndex = 0 OrElse _pageIndex = 1 Then
            btnNext.Select()
        ElseIf _pageIndex = 2 Then
            btnInstall.Select()
        End If
    End Sub

End Class
