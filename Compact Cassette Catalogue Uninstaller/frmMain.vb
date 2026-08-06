Public Class frmMain

    Private _state As C3Setup.InstalledState
    Private _facts As C3Setup.SetupEnvironmentFacts
    Private _operationInProgress As Boolean
    Private _allowClose As Boolean

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigureAccessibility()
        Try
            If String.IsNullOrWhiteSpace(uninstallStatePath) Then
                Throw New C3Setup.SetupContractException("The relocated uninstaller has no authenticated installed-state path.")
            End If
            _state = C3Setup.InstalledStateCodec.Read(uninstallStatePath)
            C3Setup.SetupBundleRuntime.RequireCurrentRelease(_state.Manifest)
            _facts = C3Setup.SetupEnvironment.Capture()
            C3Setup.SetupEnvironment.ValidateRemoval(_state, _facts)
            ShowReadyPage()
            btnUninstall.Select()
        Catch ex As Exception
            ShowFailure("Uninstall could not validate the installed ownership or computer." & Environment.NewLine & Environment.NewLine & ex.Message)
        End Try
    End Sub

    Private Sub ShowReadyPage()
        pnlUninstall.Visible = False
        pnlUninstall.Enabled = False
        pnlReady.Visible = True
        pnlReady.Enabled = True
        btnUninstall.Visible = True
        btnUninstall.Enabled = True
        btnCancel.Enabled = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If _operationInProgress Then Return
        If ConfirmCancel() = DialogResult.Yes Then
            _allowClose = True
            Application.Exit()
        End If
    End Sub

    Private Sub btnUninstall_Click(sender As Object, e As EventArgs) Handles btnUninstall.Click
        If _state Is Nothing OrElse _operationInProgress Then Return
        _operationInProgress = True
        pnlReady.Visible = False
        pnlReady.Enabled = False
        pnlUninstall.Visible = True
        pnlUninstall.Enabled = True
        btnUninstall.Enabled = False
        btnCancel.Enabled = False
        Me.AcceptButton = Nothing
        barInstallProgress.Minimum = 0
        barInstallProgress.Maximum = 4
        barInstallProgress.Value = 0
        Try
            SetStatus("Revalidating installed ownership", 1)
            _state = C3Setup.InstalledStateCodec.Read(uninstallStatePath)
            _facts = C3Setup.SetupEnvironment.Capture()
            C3Setup.SetupEnvironment.ValidateRemoval(_state, _facts)
            SetStatus("Removing owned program and system entries", 2)
            C3Setup.SetupUninstallOperation.Execute(_state.InstallRoot,
                                                    New C3Setup.WindowsSetupShortcutAccess(),
                                                    New C3Setup.WindowsSetupRegistryAccess(),
                                                    Nothing)
            SetStatus("Uninstallation complete", 4)
            _allowClose = True
            frmSuccess.Show()
            Me.Close()
        Catch ex As Exception
            ShowFailure("Uninstall did not complete. Owned changes were restored where removal had begun." & Environment.NewLine & Environment.NewLine & ex.Message)
        Finally
            _operationInProgress = False
        End Try
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
                               "Are you sure you want to cancel Compact Cassette Catalogue uninstallation?",
                               "Cancel Uninstall",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button2)
    End Function

    Private Sub SetStatus(message As String, progress As Integer)
        lblStatusProcess.Text = message
        lblStatusProcess.AccessibleDescription = message
        lblStatusProcess.Update()
        barInstallProgress.Value = progress
        barInstallProgress.Update()
    End Sub

    Private Sub ShowFailure(message As String)
        uninstallFailureMessage = message
        _allowClose = True
        frmFailure.Show()
        Me.Close()
    End Sub

    Private Sub ConfigureAccessibility()
        Me.AccessibleName = "Compact Cassette Catalogue Uninstaller"
        btnUninstall.Text = "&Uninstall"
        btnUninstall.AccessibleDescription = "Remove only files and system entries authenticated by installed ownership."
        btnCancel.Text = "&Cancel"
        btnCancel.AccessibleDescription = "Cancel without removing Compact Cassette Catalogue."
        btnBack.Visible = False
        barInstallProgress.AccessibleName = "Uninstallation progress"
        barInstallProgress.AccessibleDescription = "Progress of the current reversible removal transaction."
        lblStatusProcess.AccessibleName = "Uninstallation status"
        Me.AcceptButton = btnUninstall
    End Sub

End Class
