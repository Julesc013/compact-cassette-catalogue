<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnUninstall = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.pnlReady = New System.Windows.Forms.Panel()
        Me.pnlHeaderReady = New System.Windows.Forms.Panel()
        Me.lblReady = New System.Windows.Forms.Label()
        Me.lblReadyInstructions = New System.Windows.Forms.Label()
        Me.dialogDirectory = New System.Windows.Forms.FolderBrowserDialog()
        Me.pnlUninstall = New System.Windows.Forms.Panel()
        Me.barInstallProgress = New System.Windows.Forms.ProgressBar()
        Me.lblStatusProcess = New System.Windows.Forms.Label()
        Me.lblStatusHeader = New System.Windows.Forms.Label()
        Me.pnlHeaderUninstall = New System.Windows.Forms.Panel()
        Me.lblInstall = New System.Windows.Forms.Label()
        Me.lblInstallInstructions = New System.Windows.Forms.Label()
        Me.pnlButtons.SuspendLayout()
        Me.pnlReady.SuspendLayout()
        Me.pnlHeaderReady.SuspendLayout()
        Me.pnlUninstall.SuspendLayout()
        Me.pnlHeaderUninstall.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlButtons
        '
        Me.pnlButtons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnUninstall)
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Location = New System.Drawing.Point(0, 388)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(701, 70)
        Me.pnlButtons.TabIndex = 0
        '
        'btnUninstall
        '
        Me.btnUninstall.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUninstall.Enabled = False
        Me.btnUninstall.Location = New System.Drawing.Point(461, 20)
        Me.btnUninstall.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnUninstall.Name = "btnUninstall"
        Me.btnUninstall.Size = New System.Drawing.Size(100, 30)
        Me.btnUninstall.TabIndex = 0
        Me.btnUninstall.Text = "Uninstall"
        Me.btnUninstall.UseVisualStyleBackColor = True
        Me.btnUninstall.Visible = False
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(581, 20)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(20)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBack.Enabled = False
        Me.btnBack.Location = New System.Drawing.Point(361, 20)
        Me.btnBack.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(100, 30)
        Me.btnBack.TabIndex = 1
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'pnlReady
        '
        Me.pnlReady.Controls.Add(Me.pnlHeaderReady)
        Me.pnlReady.Controls.Add(Me.lblReadyInstructions)
        Me.pnlReady.Enabled = False
        Me.pnlReady.Location = New System.Drawing.Point(0, 0)
        Me.pnlReady.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlReady.Name = "pnlReady"
        Me.pnlReady.Size = New System.Drawing.Size(701, 388)
        Me.pnlReady.TabIndex = 8
        Me.pnlReady.Visible = False
        '
        'pnlHeaderReady
        '
        Me.pnlHeaderReady.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlHeaderReady.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderReady.Controls.Add(Me.lblReady)
        Me.pnlHeaderReady.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderReady.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderReady.Name = "pnlHeaderReady"
        Me.pnlHeaderReady.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderReady.TabIndex = 3
        '
        'lblReady
        '
        Me.lblReady.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblReady.Location = New System.Drawing.Point(30, 22)
        Me.lblReady.Margin = New System.Windows.Forms.Padding(30, 20, 30, 20)
        Me.lblReady.Name = "lblReady"
        Me.lblReady.Size = New System.Drawing.Size(641, 30)
        Me.lblReady.TabIndex = 2
        Me.lblReady.Text = "Ready to Uninstall"
        '
        'lblReadyInstructions
        '
        Me.lblReadyInstructions.Location = New System.Drawing.Point(32, 100)
        Me.lblReadyInstructions.Margin = New System.Windows.Forms.Padding(30, 30, 30, 10)
        Me.lblReadyInstructions.Name = "lblReadyInstructions"
        Me.lblReadyInstructions.Size = New System.Drawing.Size(630, 43)
        Me.lblReadyInstructions.TabIndex = 3
        Me.lblReadyInstructions.Text = "Click Uninstall to begin the uninstallation. Click Cancel to abort and exit the u" &
    "ninstaller."
        '
        'dialogDirectory
        '
        Me.dialogDirectory.Description = "The folder to install to."
        Me.dialogDirectory.RootFolder = System.Environment.SpecialFolder.CommonProgramFilesX86
        Me.dialogDirectory.SelectedPath = "C:\Program Files (x86)\Compact Cassette Catalogue\"
        '
        'pnlUninstall
        '
        Me.pnlUninstall.Controls.Add(Me.barInstallProgress)
        Me.pnlUninstall.Controls.Add(Me.lblStatusProcess)
        Me.pnlUninstall.Controls.Add(Me.lblStatusHeader)
        Me.pnlUninstall.Controls.Add(Me.pnlHeaderUninstall)
        Me.pnlUninstall.Controls.Add(Me.lblInstallInstructions)
        Me.pnlUninstall.Enabled = False
        Me.pnlUninstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlUninstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlUninstall.Name = "pnlUninstall"
        Me.pnlUninstall.Size = New System.Drawing.Size(701, 388)
        Me.pnlUninstall.TabIndex = 10
        Me.pnlUninstall.Visible = False
        '
        'barInstallProgress
        '
        Me.barInstallProgress.Location = New System.Drawing.Point(35, 186)
        Me.barInstallProgress.Margin = New System.Windows.Forms.Padding(30, 0, 30, 10)
        Me.barInstallProgress.Name = "barInstallProgress"
        Me.barInstallProgress.Size = New System.Drawing.Size(627, 23)
        Me.barInstallProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.barInstallProgress.TabIndex = 6
        '
        'lblStatusProcess
        '
        Me.lblStatusProcess.Location = New System.Drawing.Point(102, 153)
        Me.lblStatusProcess.Margin = New System.Windows.Forms.Padding(0, 10, 30, 10)
        Me.lblStatusProcess.Name = "lblStatusProcess"
        Me.lblStatusProcess.Size = New System.Drawing.Size(560, 23)
        Me.lblStatusProcess.TabIndex = 5
        '
        'lblStatusHeader
        '
        Me.lblStatusHeader.Location = New System.Drawing.Point(32, 153)
        Me.lblStatusHeader.Margin = New System.Windows.Forms.Padding(30, 10, 30, 10)
        Me.lblStatusHeader.Name = "lblStatusHeader"
        Me.lblStatusHeader.Size = New System.Drawing.Size(52, 23)
        Me.lblStatusHeader.TabIndex = 4
        Me.lblStatusHeader.Text = "Status:"
        '
        'pnlHeaderUninstall
        '
        Me.pnlHeaderUninstall.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlHeaderUninstall.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderUninstall.Controls.Add(Me.lblInstall)
        Me.pnlHeaderUninstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderUninstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderUninstall.Name = "pnlHeaderUninstall"
        Me.pnlHeaderUninstall.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderUninstall.TabIndex = 3
        '
        'lblInstall
        '
        Me.lblInstall.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInstall.Location = New System.Drawing.Point(30, 22)
        Me.lblInstall.Margin = New System.Windows.Forms.Padding(30, 20, 30, 20)
        Me.lblInstall.Name = "lblInstall"
        Me.lblInstall.Size = New System.Drawing.Size(641, 30)
        Me.lblInstall.TabIndex = 2
        Me.lblInstall.Text = "Uninstalling Compact Cassette Catalogue"
        '
        'lblInstallInstructions
        '
        Me.lblInstallInstructions.Location = New System.Drawing.Point(32, 100)
        Me.lblInstallInstructions.Margin = New System.Windows.Forms.Padding(30, 30, 30, 20)
        Me.lblInstallInstructions.Name = "lblInstallInstructions"
        Me.lblInstallInstructions.Size = New System.Drawing.Size(630, 23)
        Me.lblInstallInstructions.TabIndex = 3
        Me.lblInstallInstructions.Text = "Please wait while the Uninstaller removes Compact Cassette Catalogue."
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.pnlUninstall)
        Me.Controls.Add(Me.pnlReady)
        Me.Controls.Add(Me.pnlButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlReady.ResumeLayout(False)
        Me.pnlHeaderReady.ResumeLayout(False)
        Me.pnlUninstall.ResumeLayout(False)
        Me.pnlHeaderUninstall.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents pnlReady As Panel
    Friend WithEvents pnlHeaderReady As Panel
    Friend WithEvents lblReady As Label
    Friend WithEvents lblReadyInstructions As Label
    Friend WithEvents btnUninstall As Button
    Friend WithEvents dialogDirectory As FolderBrowserDialog
    Friend WithEvents pnlUninstall As Panel
    Friend WithEvents barInstallProgress As ProgressBar
    Friend WithEvents lblStatusProcess As Label
    Friend WithEvents lblStatusHeader As Label
    Friend WithEvents pnlHeaderUninstall As Panel
    Friend WithEvents lblInstall As Label
    Friend WithEvents lblInstallInstructions As Label
End Class
