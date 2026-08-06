<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tlpWizardRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlReady = New System.Windows.Forms.Panel()
        Me.tlpReadyCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlHeaderReady = New System.Windows.Forms.Panel()
        Me.lblReady = New System.Windows.Forms.Label()
        Me.lblReadyInstructions = New System.Windows.Forms.Label()
        Me.pnlUninstall = New System.Windows.Forms.Panel()
        Me.tlpUninstallCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlHeaderUninstall = New System.Windows.Forms.Panel()
        Me.lblInstall = New System.Windows.Forms.Label()
        Me.lblInstallInstructions = New System.Windows.Forms.Label()
        Me.tlpUninstallStatus = New System.Windows.Forms.TableLayoutPanel()
        Me.lblStatusHeader = New System.Windows.Forms.Label()
        Me.lblStatusProcess = New System.Windows.Forms.Label()
        Me.barInstallProgress = New System.Windows.Forms.ProgressBar()
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnUninstall = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.dialogDirectory = New System.Windows.Forms.FolderBrowserDialog()
        Me.tlpWizardRoot.SuspendLayout()
        Me.pnlReady.SuspendLayout()
        Me.tlpReadyCanvas.SuspendLayout()
        Me.pnlHeaderReady.SuspendLayout()
        Me.pnlUninstall.SuspendLayout()
        Me.tlpUninstallCanvas.SuspendLayout()
        Me.pnlHeaderUninstall.SuspendLayout()
        Me.tlpUninstallStatus.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpWizardRoot
        '
        Me.tlpWizardRoot.ColumnCount = 1
        Me.tlpWizardRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.Controls.Add(Me.pnlReady, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlUninstall, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlButtons, 0, 1)
        Me.tlpWizardRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpWizardRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpWizardRoot.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpWizardRoot.Name = "tlpWizardRoot"
        Me.tlpWizardRoot.RowCount = 2
        Me.tlpWizardRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpWizardRoot.Size = New System.Drawing.Size(701, 458)
        Me.tlpWizardRoot.TabIndex = 0
        '
        'pnlReady
        '
        Me.pnlReady.AutoScroll = True
        Me.pnlReady.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlReady.Controls.Add(Me.tlpReadyCanvas)
        Me.pnlReady.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlReady.Enabled = False
        Me.pnlReady.Location = New System.Drawing.Point(0, 0)
        Me.pnlReady.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlReady.Name = "pnlReady"
        Me.pnlReady.Size = New System.Drawing.Size(701, 408)
        Me.pnlReady.TabIndex = 0
        Me.pnlReady.Visible = False
        '
        'tlpReadyCanvas
        '
        Me.tlpReadyCanvas.AutoSize = True
        Me.tlpReadyCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpReadyCanvas.ColumnCount = 1
        Me.tlpReadyCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpReadyCanvas.Controls.Add(Me.pnlHeaderReady, 0, 0)
        Me.tlpReadyCanvas.Controls.Add(Me.lblReadyInstructions, 0, 1)
        Me.tlpReadyCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpReadyCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpReadyCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpReadyCanvas.MinimumSize = New System.Drawing.Size(677, 220)
        Me.tlpReadyCanvas.Name = "tlpReadyCanvas"
        Me.tlpReadyCanvas.RowCount = 2
        Me.tlpReadyCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpReadyCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110.0!))
        Me.tlpReadyCanvas.Size = New System.Drawing.Size(701, 220)
        Me.tlpReadyCanvas.TabIndex = 0
        '
        'pnlHeaderReady
        '
        Me.pnlHeaderReady.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderReady.Controls.Add(Me.lblReady)
        Me.pnlHeaderReady.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHeaderReady.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderReady.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderReady.Name = "pnlHeaderReady"
        Me.pnlHeaderReady.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderReady.TabIndex = 0
        '
        'lblReady
        '
        Me.lblReady.AutoSize = True
        Me.lblReady.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblReady.Location = New System.Drawing.Point(30, 22)
        Me.lblReady.Name = "lblReady"
        Me.lblReady.Size = New System.Drawing.Size(170, 25)
        Me.lblReady.TabIndex = 0
        Me.lblReady.Text = "Ready to Uninstall"
        '
        'lblReadyInstructions
        '
        Me.lblReadyInstructions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblReadyInstructions.Location = New System.Drawing.Point(30, 100)
        Me.lblReadyInstructions.Margin = New System.Windows.Forms.Padding(30)
        Me.lblReadyInstructions.Name = "lblReadyInstructions"
        Me.lblReadyInstructions.Size = New System.Drawing.Size(641, 50)
        Me.lblReadyInstructions.TabIndex = 1
        Me.lblReadyInstructions.Text = "Click Uninstall to begin the uninstallation. Click Cancel to abort and exit the uninstaller."
        '
        'pnlUninstall
        '
        Me.pnlUninstall.AutoScroll = True
        Me.pnlUninstall.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlUninstall.Controls.Add(Me.tlpUninstallCanvas)
        Me.pnlUninstall.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlUninstall.Enabled = False
        Me.pnlUninstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlUninstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlUninstall.Name = "pnlUninstall"
        Me.pnlUninstall.Size = New System.Drawing.Size(701, 408)
        Me.pnlUninstall.TabIndex = 1
        Me.pnlUninstall.Visible = False
        '
        'tlpUninstallCanvas
        '
        Me.tlpUninstallCanvas.AutoSize = True
        Me.tlpUninstallCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpUninstallCanvas.ColumnCount = 1
        Me.tlpUninstallCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpUninstallCanvas.Controls.Add(Me.pnlHeaderUninstall, 0, 0)
        Me.tlpUninstallCanvas.Controls.Add(Me.lblInstallInstructions, 0, 1)
        Me.tlpUninstallCanvas.Controls.Add(Me.tlpUninstallStatus, 0, 2)
        Me.tlpUninstallCanvas.Controls.Add(Me.barInstallProgress, 0, 3)
        Me.tlpUninstallCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpUninstallCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpUninstallCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpUninstallCanvas.MinimumSize = New System.Drawing.Size(677, 260)
        Me.tlpUninstallCanvas.Name = "tlpUninstallCanvas"
        Me.tlpUninstallCanvas.Padding = New System.Windows.Forms.Padding(0, 0, 0, 20)
        Me.tlpUninstallCanvas.RowCount = 4
        Me.tlpUninstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpUninstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpUninstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpUninstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpUninstallCanvas.Size = New System.Drawing.Size(701, 260)
        Me.tlpUninstallCanvas.TabIndex = 0
        '
        'pnlHeaderUninstall
        '
        Me.pnlHeaderUninstall.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderUninstall.Controls.Add(Me.lblInstall)
        Me.pnlHeaderUninstall.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHeaderUninstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderUninstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderUninstall.Name = "pnlHeaderUninstall"
        Me.pnlHeaderUninstall.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderUninstall.TabIndex = 0
        '
        'lblInstall
        '
        Me.lblInstall.AutoSize = True
        Me.lblInstall.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInstall.Location = New System.Drawing.Point(30, 22)
        Me.lblInstall.Name = "lblInstall"
        Me.lblInstall.Size = New System.Drawing.Size(356, 25)
        Me.lblInstall.TabIndex = 0
        Me.lblInstall.Text = "Uninstalling Compact Cassette Catalogue"
        '
        'lblInstallInstructions
        '
        Me.lblInstallInstructions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblInstallInstructions.Location = New System.Drawing.Point(30, 92)
        Me.lblInstallInstructions.Margin = New System.Windows.Forms.Padding(30, 22, 30, 15)
        Me.lblInstallInstructions.Name = "lblInstallInstructions"
        Me.lblInstallInstructions.Size = New System.Drawing.Size(641, 33)
        Me.lblInstallInstructions.TabIndex = 1
        Me.lblInstallInstructions.Text = "Please wait while the Uninstaller removes Compact Cassette Catalogue."
        '
        'tlpUninstallStatus
        '
        Me.tlpUninstallStatus.AutoSize = True
        Me.tlpUninstallStatus.ColumnCount = 2
        Me.tlpUninstallStatus.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpUninstallStatus.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpUninstallStatus.Controls.Add(Me.lblStatusHeader, 0, 0)
        Me.tlpUninstallStatus.Controls.Add(Me.lblStatusProcess, 1, 0)
        Me.tlpUninstallStatus.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpUninstallStatus.Location = New System.Drawing.Point(30, 143)
        Me.tlpUninstallStatus.Margin = New System.Windows.Forms.Padding(30, 3, 30, 8)
        Me.tlpUninstallStatus.Name = "tlpUninstallStatus"
        Me.tlpUninstallStatus.RowCount = 1
        Me.tlpUninstallStatus.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpUninstallStatus.Size = New System.Drawing.Size(641, 23)
        Me.tlpUninstallStatus.TabIndex = 2
        '
        'lblStatusHeader
        '
        Me.lblStatusHeader.AutoSize = True
        Me.lblStatusHeader.Location = New System.Drawing.Point(3, 0)
        Me.lblStatusHeader.Name = "lblStatusHeader"
        Me.lblStatusHeader.Size = New System.Drawing.Size(52, 17)
        Me.lblStatusHeader.TabIndex = 0
        Me.lblStatusHeader.Text = "Status:"
        '
        'lblStatusProcess
        '
        Me.lblStatusProcess.AutoSize = True
        Me.lblStatusProcess.Location = New System.Drawing.Point(61, 0)
        Me.lblStatusProcess.Name = "lblStatusProcess"
        Me.lblStatusProcess.Size = New System.Drawing.Size(0, 17)
        Me.lblStatusProcess.TabIndex = 1
        '
        'barInstallProgress
        '
        Me.barInstallProgress.Dock = System.Windows.Forms.DockStyle.Top
        Me.barInstallProgress.Location = New System.Drawing.Point(30, 177)
        Me.barInstallProgress.Margin = New System.Windows.Forms.Padding(30, 3, 30, 10)
        Me.barInstallProgress.Name = "barInstallProgress"
        Me.barInstallProgress.Size = New System.Drawing.Size(641, 23)
        Me.barInstallProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.barInstallProgress.TabIndex = 3
        '
        'pnlButtons
        '
        Me.pnlButtons.AutoSize = True
        Me.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnUninstall)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.pnlButtons.Location = New System.Drawing.Point(0, 408)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(701, 50)
        Me.pnlButtons.TabIndex = 2
        Me.pnlButtons.WrapContents = True
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(587, 13)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnUninstall
        '
        Me.btnUninstall.AutoSize = True
        Me.btnUninstall.Enabled = False
        Me.btnUninstall.Location = New System.Drawing.Point(481, 13)
        Me.btnUninstall.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnUninstall.Name = "btnUninstall"
        Me.btnUninstall.Size = New System.Drawing.Size(100, 30)
        Me.btnUninstall.TabIndex = 0
        Me.btnUninstall.Text = "Uninstall"
        Me.btnUninstall.UseVisualStyleBackColor = True
        Me.btnUninstall.Visible = False
        '
        'btnBack
        '
        Me.btnBack.AutoSize = True
        Me.btnBack.Enabled = False
        Me.btnBack.Location = New System.Drawing.Point(375, 13)
        Me.btnBack.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(100, 30)
        Me.btnBack.TabIndex = 1
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'dialogDirectory
        '
        Me.dialogDirectory.Description = "The folder to install to."
        Me.dialogDirectory.RootFolder = System.Environment.SpecialFolder.CommonProgramFilesX86
        Me.dialogDirectory.SelectedPath = "C:\Program Files (x86)\Compact Cassette Catalogue\"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.tlpWizardRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.tlpWizardRoot.ResumeLayout(False)
        Me.tlpWizardRoot.PerformLayout()
        Me.pnlReady.ResumeLayout(False)
        Me.pnlReady.PerformLayout()
        Me.tlpReadyCanvas.ResumeLayout(False)
        Me.pnlHeaderReady.ResumeLayout(False)
        Me.pnlHeaderReady.PerformLayout()
        Me.pnlUninstall.ResumeLayout(False)
        Me.pnlUninstall.PerformLayout()
        Me.tlpUninstallCanvas.ResumeLayout(False)
        Me.tlpUninstallCanvas.PerformLayout()
        Me.pnlHeaderUninstall.ResumeLayout(False)
        Me.pnlHeaderUninstall.PerformLayout()
        Me.tlpUninstallStatus.ResumeLayout(False)
        Me.tlpUninstallStatus.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlButtons.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpWizardRoot As TableLayoutPanel
    Friend WithEvents pnlReady As Panel
    Friend WithEvents tlpReadyCanvas As TableLayoutPanel
    Friend WithEvents pnlHeaderReady As Panel
    Friend WithEvents lblReady As Label
    Friend WithEvents lblReadyInstructions As Label
    Friend WithEvents pnlUninstall As Panel
    Friend WithEvents tlpUninstallCanvas As TableLayoutPanel
    Friend WithEvents pnlHeaderUninstall As Panel
    Friend WithEvents lblInstall As Label
    Friend WithEvents lblInstallInstructions As Label
    Friend WithEvents tlpUninstallStatus As TableLayoutPanel
    Friend WithEvents lblStatusHeader As Label
    Friend WithEvents lblStatusProcess As Label
    Friend WithEvents barInstallProgress As ProgressBar
    Friend WithEvents pnlButtons As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnUninstall As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents dialogDirectory As FolderBrowserDialog
End Class
