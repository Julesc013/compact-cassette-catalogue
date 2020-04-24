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
        Me.btnInstall = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.lblHeadingIntroduction = New System.Windows.Forms.Label()
        Me.lblIntroduction = New System.Windows.Forms.Label()
        Me.pnlIntroduction = New System.Windows.Forms.Panel()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.pnlOptions = New System.Windows.Forms.Panel()
        Me.chkStartMenu = New System.Windows.Forms.CheckBox()
        Me.chkDesktop = New System.Windows.Forms.CheckBox()
        Me.btnChangeDirectory = New System.Windows.Forms.Button()
        Me.txtDirectory = New System.Windows.Forms.TextBox()
        Me.pnlHeaderOptions = New System.Windows.Forms.Panel()
        Me.lblOptionsHeading = New System.Windows.Forms.Label()
        Me.lblDirectory = New System.Windows.Forms.Label()
        Me.pnlReady = New System.Windows.Forms.Panel()
        Me.pnlHeaderReady = New System.Windows.Forms.Panel()
        Me.lblReady = New System.Windows.Forms.Label()
        Me.lblReadyInstructions = New System.Windows.Forms.Label()
        Me.dialogDirectory = New System.Windows.Forms.FolderBrowserDialog()
        Me.pnlButtons.SuspendLayout()
        Me.pnlIntroduction.SuspendLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlOptions.SuspendLayout()
        Me.pnlHeaderOptions.SuspendLayout()
        Me.pnlReady.SuspendLayout()
        Me.pnlHeaderReady.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlButtons
        '
        Me.pnlButtons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnInstall)
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Controls.Add(Me.btnNext)
        Me.pnlButtons.Location = New System.Drawing.Point(0, 388)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(701, 70)
        Me.pnlButtons.TabIndex = 0
        '
        'btnInstall
        '
        Me.btnInstall.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnInstall.Enabled = False
        Me.btnInstall.Location = New System.Drawing.Point(461, 20)
        Me.btnInstall.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnInstall.Name = "btnInstall"
        Me.btnInstall.Size = New System.Drawing.Size(100, 30)
        Me.btnInstall.TabIndex = 3
        Me.btnInstall.Text = "Install"
        Me.btnInstall.UseVisualStyleBackColor = True
        Me.btnInstall.Visible = False
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
        'btnNext
        '
        Me.btnNext.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNext.Location = New System.Drawing.Point(461, 20)
        Me.btnNext.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(100, 30)
        Me.btnNext.TabIndex = 0
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'lblHeadingIntroduction
        '
        Me.lblHeadingIntroduction.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingIntroduction.Location = New System.Drawing.Point(243, 39)
        Me.lblHeadingIntroduction.Margin = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.lblHeadingIntroduction.Name = "lblHeadingIntroduction"
        Me.lblHeadingIntroduction.Size = New System.Drawing.Size(429, 57)
        Me.lblHeadingIntroduction.TabIndex = 2
        Me.lblHeadingIntroduction.Text = "Welcome to the Compact Cassette Catalogue Setup Wizard"
        '
        'lblIntroduction
        '
        Me.lblIntroduction.Location = New System.Drawing.Point(245, 126)
        Me.lblIntroduction.Margin = New System.Windows.Forms.Padding(20, 10, 20, 10)
        Me.lblIntroduction.Name = "lblIntroduction"
        Me.lblIntroduction.Size = New System.Drawing.Size(427, 57)
        Me.lblIntroduction.TabIndex = 3
        Me.lblIntroduction.Text = "The setup wizard will install Compact Cassette Catalogue on your computer. Click " &
    "Next to continue or Cancel to exit the Setup Wizard."
        '
        'pnlIntroduction
        '
        Me.pnlIntroduction.Controls.Add(Me.picSideBanner)
        Me.pnlIntroduction.Controls.Add(Me.lblIntroduction)
        Me.pnlIntroduction.Controls.Add(Me.lblHeadingIntroduction)
        Me.pnlIntroduction.Location = New System.Drawing.Point(0, 0)
        Me.pnlIntroduction.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlIntroduction.Name = "pnlIntroduction"
        Me.pnlIntroduction.Size = New System.Drawing.Size(701, 388)
        Me.pnlIntroduction.TabIndex = 4
        '
        'picSideBanner
        '
        Me.picSideBanner.Image = CType(resources.GetObject("picSideBanner.Image"), System.Drawing.Image)
        Me.picSideBanner.Location = New System.Drawing.Point(0, 0)
        Me.picSideBanner.Margin = New System.Windows.Forms.Padding(0)
        Me.picSideBanner.Name = "picSideBanner"
        Me.picSideBanner.Size = New System.Drawing.Size(223, 388)
        Me.picSideBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSideBanner.TabIndex = 1
        Me.picSideBanner.TabStop = False
        '
        'pnlOptions
        '
        Me.pnlOptions.Controls.Add(Me.chkStartMenu)
        Me.pnlOptions.Controls.Add(Me.chkDesktop)
        Me.pnlOptions.Controls.Add(Me.btnChangeDirectory)
        Me.pnlOptions.Controls.Add(Me.txtDirectory)
        Me.pnlOptions.Controls.Add(Me.pnlHeaderOptions)
        Me.pnlOptions.Controls.Add(Me.lblDirectory)
        Me.pnlOptions.Enabled = False
        Me.pnlOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlOptions.Name = "pnlOptions"
        Me.pnlOptions.Size = New System.Drawing.Size(701, 388)
        Me.pnlOptions.TabIndex = 5
        Me.pnlOptions.Visible = False
        '
        'chkStartMenu
        '
        Me.chkStartMenu.AutoSize = True
        Me.chkStartMenu.Checked = True
        Me.chkStartMenu.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStartMenu.Location = New System.Drawing.Point(35, 183)
        Me.chkStartMenu.Margin = New System.Windows.Forms.Padding(30, 3, 30, 30)
        Me.chkStartMenu.Name = "chkStartMenu"
        Me.chkStartMenu.Size = New System.Drawing.Size(202, 21)
        Me.chkStartMenu.TabIndex = 7
        Me.chkStartMenu.Text = "Create Start Menu Shortcut"
        Me.chkStartMenu.UseVisualStyleBackColor = True
        '
        'chkDesktop
        '
        Me.chkDesktop.AutoSize = True
        Me.chkDesktop.Checked = True
        Me.chkDesktop.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDesktop.Location = New System.Drawing.Point(35, 210)
        Me.chkDesktop.Margin = New System.Windows.Forms.Padding(30, 30, 30, 3)
        Me.chkDesktop.Name = "chkDesktop"
        Me.chkDesktop.Size = New System.Drawing.Size(185, 21)
        Me.chkDesktop.TabIndex = 6
        Me.chkDesktop.Text = "Create Desktop Shortcut"
        Me.chkDesktop.UseVisualStyleBackColor = True
        '
        'btnChangeDirectory
        '
        Me.btnChangeDirectory.Location = New System.Drawing.Point(562, 127)
        Me.btnChangeDirectory.Margin = New System.Windows.Forms.Padding(20, 10, 30, 10)
        Me.btnChangeDirectory.Name = "btnChangeDirectory"
        Me.btnChangeDirectory.Size = New System.Drawing.Size(100, 30)
        Me.btnChangeDirectory.TabIndex = 5
        Me.btnChangeDirectory.Text = "Change..."
        Me.btnChangeDirectory.UseVisualStyleBackColor = True
        '
        'txtDirectory
        '
        Me.txtDirectory.Location = New System.Drawing.Point(35, 131)
        Me.txtDirectory.Margin = New System.Windows.Forms.Padding(0)
        Me.txtDirectory.Name = "txtDirectory"
        Me.txtDirectory.Size = New System.Drawing.Size(507, 22)
        Me.txtDirectory.TabIndex = 4
        Me.txtDirectory.Text = "C:\Program Files (x86)\Compact Cassette Catalogue\"
        '
        'pnlHeaderOptions
        '
        Me.pnlHeaderOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlHeaderOptions.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderOptions.Controls.Add(Me.lblOptionsHeading)
        Me.pnlHeaderOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderOptions.Name = "pnlHeaderOptions"
        Me.pnlHeaderOptions.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderOptions.TabIndex = 3
        '
        'lblOptionsHeading
        '
        Me.lblOptionsHeading.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOptionsHeading.Location = New System.Drawing.Point(30, 22)
        Me.lblOptionsHeading.Margin = New System.Windows.Forms.Padding(30, 20, 30, 20)
        Me.lblOptionsHeading.Name = "lblOptionsHeading"
        Me.lblOptionsHeading.Size = New System.Drawing.Size(641, 30)
        Me.lblOptionsHeading.TabIndex = 2
        Me.lblOptionsHeading.Text = "Installation Options"
        '
        'lblDirectory
        '
        Me.lblDirectory.Location = New System.Drawing.Point(32, 100)
        Me.lblDirectory.Margin = New System.Windows.Forms.Padding(30, 30, 30, 10)
        Me.lblDirectory.Name = "lblDirectory"
        Me.lblDirectory.Size = New System.Drawing.Size(630, 21)
        Me.lblDirectory.TabIndex = 3
        Me.lblDirectory.Text = "Install Compact Cassette Catalogue to:"
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
        Me.lblReady.Text = "Ready to Install"
        '
        'lblReadyInstructions
        '
        Me.lblReadyInstructions.Location = New System.Drawing.Point(32, 100)
        Me.lblReadyInstructions.Margin = New System.Windows.Forms.Padding(30, 30, 30, 10)
        Me.lblReadyInstructions.Name = "lblReadyInstructions"
        Me.lblReadyInstructions.Size = New System.Drawing.Size(630, 43)
        Me.lblReadyInstructions.TabIndex = 3
        Me.lblReadyInstructions.Text = "Click Install to begin the installation. Click Back to review or change any of yo" &
    "ur installation settings. Click Cancel to abort and exit the wizard."
        '
        'dialogDirectory
        '
        Me.dialogDirectory.Description = "The folder to install to."
        Me.dialogDirectory.RootFolder = System.Environment.SpecialFolder.CommonProgramFilesX86
        Me.dialogDirectory.SelectedPath = "C:\Program Files (x86)\Compact Cassette Catalogue\"
        '
        'frmMain
        '
        Me.AcceptButton = Me.btnNext
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.pnlReady)
        Me.Controls.Add(Me.pnlOptions)
        Me.Controls.Add(Me.pnlIntroduction)
        Me.Controls.Add(Me.pnlButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlIntroduction.ResumeLayout(False)
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOptions.ResumeLayout(False)
        Me.pnlOptions.PerformLayout()
        Me.pnlHeaderOptions.ResumeLayout(False)
        Me.pnlReady.ResumeLayout(False)
        Me.pnlHeaderReady.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnNext As Button
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents lblHeadingIntroduction As Label
    Friend WithEvents lblIntroduction As Label
    Friend WithEvents pnlIntroduction As Panel
    Friend WithEvents pnlOptions As Panel
    Friend WithEvents lblDirectory As Label
    Friend WithEvents lblOptionsHeading As Label
    Friend WithEvents pnlHeaderOptions As Panel
    Friend WithEvents txtDirectory As TextBox
    Friend WithEvents btnChangeDirectory As Button
    Friend WithEvents chkStartMenu As CheckBox
    Friend WithEvents chkDesktop As CheckBox
    Friend WithEvents pnlReady As Panel
    Friend WithEvents pnlHeaderReady As Panel
    Friend WithEvents lblReady As Label
    Friend WithEvents lblReadyInstructions As Label
    Friend WithEvents btnInstall As Button
    Friend WithEvents dialogDirectory As FolderBrowserDialog
End Class
