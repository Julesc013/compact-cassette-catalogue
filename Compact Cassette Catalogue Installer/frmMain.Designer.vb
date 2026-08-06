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
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnInstall = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.pnlIntroduction = New System.Windows.Forms.Panel()
        Me.tlpIntroductionCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.tlpIntroductionContent = New System.Windows.Forms.TableLayoutPanel()
        Me.lblHeadingIntroduction = New System.Windows.Forms.Label()
        Me.lblIntroduction = New System.Windows.Forms.Label()
        Me.pnlOptions = New System.Windows.Forms.Panel()
        Me.tlpOptionsCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlHeaderOptions = New System.Windows.Forms.Panel()
        Me.lblOptionsHeading = New System.Windows.Forms.Label()
        Me.tlpOptionsContent = New System.Windows.Forms.TableLayoutPanel()
        Me.lblDirectory = New System.Windows.Forms.Label()
        Me.tlpDirectory = New System.Windows.Forms.TableLayoutPanel()
        Me.txtDirectory = New System.Windows.Forms.TextBox()
        Me.btnChangeDirectory = New System.Windows.Forms.Button()
        Me.chkStartMenu = New System.Windows.Forms.CheckBox()
        Me.chkDesktop = New System.Windows.Forms.CheckBox()
        Me.pnlReady = New System.Windows.Forms.Panel()
        Me.tlpReadyCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlHeaderReady = New System.Windows.Forms.Panel()
        Me.lblReady = New System.Windows.Forms.Label()
        Me.lblReadyInstructions = New System.Windows.Forms.Label()
        Me.pnlInstall = New System.Windows.Forms.Panel()
        Me.tlpInstallCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlHeaderInstall = New System.Windows.Forms.Panel()
        Me.lblInstall = New System.Windows.Forms.Label()
        Me.lblInstallInstructions = New System.Windows.Forms.Label()
        Me.tlpInstallStatus = New System.Windows.Forms.TableLayoutPanel()
        Me.lblStatusHeader = New System.Windows.Forms.Label()
        Me.lblStatusProcess = New System.Windows.Forms.Label()
        Me.barInstallProgress = New System.Windows.Forms.ProgressBar()
        Me.dialogDirectory = New System.Windows.Forms.FolderBrowserDialog()
        Me.tlpWizardRoot.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.pnlIntroduction.SuspendLayout()
        Me.tlpIntroductionCanvas.SuspendLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpIntroductionContent.SuspendLayout()
        Me.pnlOptions.SuspendLayout()
        Me.tlpOptionsCanvas.SuspendLayout()
        Me.pnlHeaderOptions.SuspendLayout()
        Me.tlpOptionsContent.SuspendLayout()
        Me.tlpDirectory.SuspendLayout()
        Me.pnlReady.SuspendLayout()
        Me.tlpReadyCanvas.SuspendLayout()
        Me.pnlHeaderReady.SuspendLayout()
        Me.pnlInstall.SuspendLayout()
        Me.tlpInstallCanvas.SuspendLayout()
        Me.pnlHeaderInstall.SuspendLayout()
        Me.tlpInstallStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpWizardRoot
        '
        Me.tlpWizardRoot.ColumnCount = 1
        Me.tlpWizardRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.Controls.Add(Me.pnlIntroduction, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlOptions, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlReady, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlInstall, 0, 0)
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
        'pnlButtons
        '
        Me.pnlButtons.AutoSize = True
        Me.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnInstall)
        Me.pnlButtons.Controls.Add(Me.btnNext)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.pnlButtons.Location = New System.Drawing.Point(0, 408)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(701, 50)
        Me.pnlButtons.TabIndex = 4
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
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnInstall
        '
        Me.btnInstall.AutoSize = True
        Me.btnInstall.Enabled = False
        Me.btnInstall.Location = New System.Drawing.Point(481, 13)
        Me.btnInstall.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnInstall.Name = "btnInstall"
        Me.btnInstall.Size = New System.Drawing.Size(100, 30)
        Me.btnInstall.TabIndex = 2
        Me.btnInstall.Text = "Install"
        Me.btnInstall.UseVisualStyleBackColor = True
        Me.btnInstall.Visible = False
        '
        'btnNext
        '
        Me.btnNext.AutoSize = True
        Me.btnNext.Location = New System.Drawing.Point(375, 13)
        Me.btnNext.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(100, 30)
        Me.btnNext.TabIndex = 1
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.AutoSize = True
        Me.btnBack.Enabled = False
        Me.btnBack.Location = New System.Drawing.Point(269, 13)
        Me.btnBack.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(100, 30)
        Me.btnBack.TabIndex = 0
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'pnlIntroduction
        '
        Me.pnlIntroduction.AutoScroll = True
        Me.pnlIntroduction.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlIntroduction.Controls.Add(Me.tlpIntroductionCanvas)
        Me.pnlIntroduction.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlIntroduction.Location = New System.Drawing.Point(0, 0)
        Me.pnlIntroduction.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlIntroduction.Name = "pnlIntroduction"
        Me.pnlIntroduction.Size = New System.Drawing.Size(701, 408)
        Me.pnlIntroduction.TabIndex = 0
        '
        'tlpIntroductionCanvas
        '
        Me.tlpIntroductionCanvas.AutoSize = True
        Me.tlpIntroductionCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpIntroductionCanvas.ColumnCount = 2
        Me.tlpIntroductionCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 223.0!))
        Me.tlpIntroductionCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIntroductionCanvas.Controls.Add(Me.picSideBanner, 0, 0)
        Me.tlpIntroductionCanvas.Controls.Add(Me.tlpIntroductionContent, 1, 0)
        Me.tlpIntroductionCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpIntroductionCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpIntroductionCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpIntroductionCanvas.MinimumSize = New System.Drawing.Size(677, 388)
        Me.tlpIntroductionCanvas.Name = "tlpIntroductionCanvas"
        Me.tlpIntroductionCanvas.RowCount = 1
        Me.tlpIntroductionCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIntroductionCanvas.Size = New System.Drawing.Size(701, 388)
        Me.tlpIntroductionCanvas.TabIndex = 0
        '
        'picSideBanner
        '
        Me.picSideBanner.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picSideBanner.Image = Global.Compact_Cassette_Catalogue_Installer.My.Resources.Resources.cassette_tapes_transparent_jpg
        Me.picSideBanner.Location = New System.Drawing.Point(0, 0)
        Me.picSideBanner.Margin = New System.Windows.Forms.Padding(0)
        Me.picSideBanner.Name = "picSideBanner"
        Me.picSideBanner.Size = New System.Drawing.Size(223, 388)
        Me.picSideBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSideBanner.TabIndex = 0
        Me.picSideBanner.TabStop = False
        '
        'tlpIntroductionContent
        '
        Me.tlpIntroductionContent.ColumnCount = 1
        Me.tlpIntroductionContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIntroductionContent.Controls.Add(Me.lblHeadingIntroduction, 0, 0)
        Me.tlpIntroductionContent.Controls.Add(Me.lblIntroduction, 0, 1)
        Me.tlpIntroductionContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpIntroductionContent.Location = New System.Drawing.Point(223, 0)
        Me.tlpIntroductionContent.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpIntroductionContent.Name = "tlpIntroductionContent"
        Me.tlpIntroductionContent.Padding = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.tlpIntroductionContent.RowCount = 3
        Me.tlpIntroductionContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpIntroductionContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.tlpIntroductionContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIntroductionContent.Size = New System.Drawing.Size(478, 388)
        Me.tlpIntroductionContent.TabIndex = 1
        '
        'lblHeadingIntroduction
        '
        Me.lblHeadingIntroduction.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHeadingIntroduction.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingIntroduction.Location = New System.Drawing.Point(23, 30)
        Me.lblHeadingIntroduction.Name = "lblHeadingIntroduction"
        Me.lblHeadingIntroduction.Size = New System.Drawing.Size(432, 70)
        Me.lblHeadingIntroduction.TabIndex = 0
        Me.lblHeadingIntroduction.Text = "Welcome to the Compact Cassette Catalogue Setup Wizard"
        '
        'lblIntroduction
        '
        Me.lblIntroduction.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblIntroduction.Location = New System.Drawing.Point(23, 103)
        Me.lblIntroduction.Name = "lblIntroduction"
        Me.lblIntroduction.Size = New System.Drawing.Size(432, 87)
        Me.lblIntroduction.TabIndex = 1
        Me.lblIntroduction.Text = "The setup wizard will install Compact Cassette Catalogue on your computer. Click Next to continue or Cancel to exit the Setup Wizard."
        '
        'pnlOptions
        '
        Me.pnlOptions.AutoScroll = True
        Me.pnlOptions.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlOptions.Controls.Add(Me.tlpOptionsCanvas)
        Me.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOptions.Enabled = False
        Me.pnlOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlOptions.Name = "pnlOptions"
        Me.pnlOptions.Size = New System.Drawing.Size(701, 408)
        Me.pnlOptions.TabIndex = 1
        Me.pnlOptions.Visible = False
        '
        'tlpOptionsCanvas
        '
        Me.tlpOptionsCanvas.AutoSize = True
        Me.tlpOptionsCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpOptionsCanvas.ColumnCount = 1
        Me.tlpOptionsCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpOptionsCanvas.Controls.Add(Me.pnlHeaderOptions, 0, 0)
        Me.tlpOptionsCanvas.Controls.Add(Me.tlpOptionsContent, 0, 1)
        Me.tlpOptionsCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpOptionsCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpOptionsCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpOptionsCanvas.MinimumSize = New System.Drawing.Size(677, 300)
        Me.tlpOptionsCanvas.Name = "tlpOptionsCanvas"
        Me.tlpOptionsCanvas.RowCount = 2
        Me.tlpOptionsCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpOptionsCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpOptionsCanvas.Size = New System.Drawing.Size(701, 300)
        Me.tlpOptionsCanvas.TabIndex = 0
        '
        'pnlHeaderOptions
        '
        Me.pnlHeaderOptions.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderOptions.Controls.Add(Me.lblOptionsHeading)
        Me.pnlHeaderOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHeaderOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderOptions.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderOptions.Name = "pnlHeaderOptions"
        Me.pnlHeaderOptions.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderOptions.TabIndex = 0
        '
        'lblOptionsHeading
        '
        Me.lblOptionsHeading.AutoSize = True
        Me.lblOptionsHeading.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOptionsHeading.Location = New System.Drawing.Point(30, 22)
        Me.lblOptionsHeading.Name = "lblOptionsHeading"
        Me.lblOptionsHeading.Size = New System.Drawing.Size(156, 25)
        Me.lblOptionsHeading.TabIndex = 0
        Me.lblOptionsHeading.Text = "Installation Options"
        '
        'tlpOptionsContent
        '
        Me.tlpOptionsContent.AutoSize = True
        Me.tlpOptionsContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpOptionsContent.ColumnCount = 1
        Me.tlpOptionsContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpOptionsContent.Controls.Add(Me.lblDirectory, 0, 0)
        Me.tlpOptionsContent.Controls.Add(Me.tlpDirectory, 0, 1)
        Me.tlpOptionsContent.Controls.Add(Me.chkStartMenu, 0, 2)
        Me.tlpOptionsContent.Controls.Add(Me.chkDesktop, 0, 3)
        Me.tlpOptionsContent.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpOptionsContent.Location = New System.Drawing.Point(0, 70)
        Me.tlpOptionsContent.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpOptionsContent.Name = "tlpOptionsContent"
        Me.tlpOptionsContent.Padding = New System.Windows.Forms.Padding(30)
        Me.tlpOptionsContent.RowCount = 4
        Me.tlpOptionsContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpOptionsContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpOptionsContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpOptionsContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpOptionsContent.Size = New System.Drawing.Size(701, 195)
        Me.tlpOptionsContent.TabIndex = 1
        '
        'lblDirectory
        '
        Me.lblDirectory.AutoSize = True
        Me.lblDirectory.Location = New System.Drawing.Point(33, 30)
        Me.lblDirectory.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        Me.lblDirectory.Name = "lblDirectory"
        Me.lblDirectory.Size = New System.Drawing.Size(244, 17)
        Me.lblDirectory.TabIndex = 0
        Me.lblDirectory.Text = "Install Compact Cassette Catalogue to:"
        '
        'tlpDirectory
        '
        Me.tlpDirectory.AutoSize = True
        Me.tlpDirectory.ColumnCount = 2
        Me.tlpDirectory.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDirectory.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDirectory.Controls.Add(Me.txtDirectory, 0, 0)
        Me.tlpDirectory.Controls.Add(Me.btnChangeDirectory, 1, 0)
        Me.tlpDirectory.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpDirectory.Location = New System.Drawing.Point(33, 58)
        Me.tlpDirectory.Name = "tlpDirectory"
        Me.tlpDirectory.RowCount = 1
        Me.tlpDirectory.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDirectory.Size = New System.Drawing.Size(635, 36)
        Me.tlpDirectory.TabIndex = 1
        '
        'txtDirectory
        '
        Me.txtDirectory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtDirectory.Location = New System.Drawing.Point(0, 6)
        Me.txtDirectory.Margin = New System.Windows.Forms.Padding(0, 6, 8, 6)
        Me.txtDirectory.Name = "txtDirectory"
        Me.txtDirectory.Size = New System.Drawing.Size(521, 22)
        Me.txtDirectory.TabIndex = 0
        Me.txtDirectory.Text = "C:\Program Files (x86)\Compact Cassette Catalogue\"
        '
        'btnChangeDirectory
        '
        Me.btnChangeDirectory.AutoSize = True
        Me.btnChangeDirectory.Location = New System.Drawing.Point(529, 3)
        Me.btnChangeDirectory.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnChangeDirectory.Name = "btnChangeDirectory"
        Me.btnChangeDirectory.Size = New System.Drawing.Size(100, 30)
        Me.btnChangeDirectory.TabIndex = 1
        Me.btnChangeDirectory.Text = "Change..."
        Me.btnChangeDirectory.UseVisualStyleBackColor = True
        '
        'chkStartMenu
        '
        Me.chkStartMenu.AutoSize = True
        Me.chkStartMenu.Checked = True
        Me.chkStartMenu.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStartMenu.Location = New System.Drawing.Point(33, 105)
        Me.chkStartMenu.Margin = New System.Windows.Forms.Padding(3, 8, 3, 3)
        Me.chkStartMenu.Name = "chkStartMenu"
        Me.chkStartMenu.Size = New System.Drawing.Size(202, 21)
        Me.chkStartMenu.TabIndex = 2
        Me.chkStartMenu.Text = "Create Start Menu Shortcut"
        Me.chkStartMenu.UseVisualStyleBackColor = True
        '
        'chkDesktop
        '
        Me.chkDesktop.AutoSize = True
        Me.chkDesktop.Checked = True
        Me.chkDesktop.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDesktop.Location = New System.Drawing.Point(33, 135)
        Me.chkDesktop.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.chkDesktop.Name = "chkDesktop"
        Me.chkDesktop.Size = New System.Drawing.Size(185, 21)
        Me.chkDesktop.TabIndex = 3
        Me.chkDesktop.Text = "Create Desktop Shortcut"
        Me.chkDesktop.UseVisualStyleBackColor = True
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
        Me.pnlReady.TabIndex = 2
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
        Me.lblReady.Size = New System.Drawing.Size(148, 25)
        Me.lblReady.TabIndex = 0
        Me.lblReady.Text = "Ready to Install"
        '
        'lblReadyInstructions
        '
        Me.lblReadyInstructions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblReadyInstructions.Location = New System.Drawing.Point(30, 100)
        Me.lblReadyInstructions.Margin = New System.Windows.Forms.Padding(30)
        Me.lblReadyInstructions.Name = "lblReadyInstructions"
        Me.lblReadyInstructions.Size = New System.Drawing.Size(641, 50)
        Me.lblReadyInstructions.TabIndex = 1
        Me.lblReadyInstructions.Text = "Click Install to begin the installation. Click Back to review or change any of your installation settings. Click Cancel to abort and exit the wizard."
        '
        'pnlInstall
        '
        Me.pnlInstall.AutoScroll = True
        Me.pnlInstall.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlInstall.Controls.Add(Me.tlpInstallCanvas)
        Me.pnlInstall.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlInstall.Enabled = False
        Me.pnlInstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlInstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlInstall.Name = "pnlInstall"
        Me.pnlInstall.Size = New System.Drawing.Size(701, 408)
        Me.pnlInstall.TabIndex = 3
        Me.pnlInstall.Visible = False
        '
        'tlpInstallCanvas
        '
        Me.tlpInstallCanvas.AutoSize = True
        Me.tlpInstallCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpInstallCanvas.ColumnCount = 1
        Me.tlpInstallCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpInstallCanvas.Controls.Add(Me.pnlHeaderInstall, 0, 0)
        Me.tlpInstallCanvas.Controls.Add(Me.lblInstallInstructions, 0, 1)
        Me.tlpInstallCanvas.Controls.Add(Me.tlpInstallStatus, 0, 2)
        Me.tlpInstallCanvas.Controls.Add(Me.barInstallProgress, 0, 3)
        Me.tlpInstallCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpInstallCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpInstallCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpInstallCanvas.MinimumSize = New System.Drawing.Size(677, 260)
        Me.tlpInstallCanvas.Name = "tlpInstallCanvas"
        Me.tlpInstallCanvas.Padding = New System.Windows.Forms.Padding(0, 0, 0, 20)
        Me.tlpInstallCanvas.RowCount = 4
        Me.tlpInstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpInstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpInstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpInstallCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpInstallCanvas.Size = New System.Drawing.Size(701, 260)
        Me.tlpInstallCanvas.TabIndex = 0
        '
        'pnlHeaderInstall
        '
        Me.pnlHeaderInstall.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderInstall.Controls.Add(Me.lblInstall)
        Me.pnlHeaderInstall.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHeaderInstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderInstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderInstall.Name = "pnlHeaderInstall"
        Me.pnlHeaderInstall.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderInstall.TabIndex = 0
        '
        'lblInstall
        '
        Me.lblInstall.AutoSize = True
        Me.lblInstall.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInstall.Location = New System.Drawing.Point(30, 22)
        Me.lblInstall.Name = "lblInstall"
        Me.lblInstall.Size = New System.Drawing.Size(334, 25)
        Me.lblInstall.TabIndex = 0
        Me.lblInstall.Text = "Installing Compact Cassette Catalogue"
        '
        'lblInstallInstructions
        '
        Me.lblInstallInstructions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblInstallInstructions.Location = New System.Drawing.Point(30, 92)
        Me.lblInstallInstructions.Margin = New System.Windows.Forms.Padding(30, 22, 30, 15)
        Me.lblInstallInstructions.Name = "lblInstallInstructions"
        Me.lblInstallInstructions.Size = New System.Drawing.Size(641, 33)
        Me.lblInstallInstructions.TabIndex = 1
        Me.lblInstallInstructions.Text = "Please wait while the Setup Wizard installs Compact Cassette Catalogue."
        '
        'tlpInstallStatus
        '
        Me.tlpInstallStatus.AutoSize = True
        Me.tlpInstallStatus.ColumnCount = 2
        Me.tlpInstallStatus.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpInstallStatus.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpInstallStatus.Controls.Add(Me.lblStatusHeader, 0, 0)
        Me.tlpInstallStatus.Controls.Add(Me.lblStatusProcess, 1, 0)
        Me.tlpInstallStatus.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpInstallStatus.Location = New System.Drawing.Point(30, 143)
        Me.tlpInstallStatus.Margin = New System.Windows.Forms.Padding(30, 3, 30, 8)
        Me.tlpInstallStatus.Name = "tlpInstallStatus"
        Me.tlpInstallStatus.RowCount = 1
        Me.tlpInstallStatus.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpInstallStatus.Size = New System.Drawing.Size(641, 23)
        Me.tlpInstallStatus.TabIndex = 2
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
        Me.Controls.Add(Me.tlpWizardRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.tlpWizardRoot.ResumeLayout(False)
        Me.tlpWizardRoot.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlButtons.PerformLayout()
        Me.pnlIntroduction.ResumeLayout(False)
        Me.pnlIntroduction.PerformLayout()
        Me.tlpIntroductionCanvas.ResumeLayout(False)
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpIntroductionContent.ResumeLayout(False)
        Me.pnlOptions.ResumeLayout(False)
        Me.pnlOptions.PerformLayout()
        Me.tlpOptionsCanvas.ResumeLayout(False)
        Me.tlpOptionsCanvas.PerformLayout()
        Me.pnlHeaderOptions.ResumeLayout(False)
        Me.pnlHeaderOptions.PerformLayout()
        Me.tlpOptionsContent.ResumeLayout(False)
        Me.tlpOptionsContent.PerformLayout()
        Me.tlpDirectory.ResumeLayout(False)
        Me.tlpDirectory.PerformLayout()
        Me.pnlReady.ResumeLayout(False)
        Me.pnlReady.PerformLayout()
        Me.tlpReadyCanvas.ResumeLayout(False)
        Me.pnlHeaderReady.ResumeLayout(False)
        Me.pnlHeaderReady.PerformLayout()
        Me.pnlInstall.ResumeLayout(False)
        Me.pnlInstall.PerformLayout()
        Me.tlpInstallCanvas.ResumeLayout(False)
        Me.tlpInstallCanvas.PerformLayout()
        Me.pnlHeaderInstall.ResumeLayout(False)
        Me.pnlHeaderInstall.PerformLayout()
        Me.tlpInstallStatus.ResumeLayout(False)
        Me.tlpInstallStatus.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpWizardRoot As TableLayoutPanel
    Friend WithEvents pnlButtons As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnNext As Button
    Friend WithEvents btnInstall As Button
    Friend WithEvents pnlIntroduction As Panel
    Friend WithEvents tlpIntroductionCanvas As TableLayoutPanel
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents tlpIntroductionContent As TableLayoutPanel
    Friend WithEvents lblHeadingIntroduction As Label
    Friend WithEvents lblIntroduction As Label
    Friend WithEvents pnlOptions As Panel
    Friend WithEvents tlpOptionsCanvas As TableLayoutPanel
    Friend WithEvents pnlHeaderOptions As Panel
    Friend WithEvents lblOptionsHeading As Label
    Friend WithEvents tlpOptionsContent As TableLayoutPanel
    Friend WithEvents lblDirectory As Label
    Friend WithEvents tlpDirectory As TableLayoutPanel
    Friend WithEvents txtDirectory As TextBox
    Friend WithEvents btnChangeDirectory As Button
    Friend WithEvents chkStartMenu As CheckBox
    Friend WithEvents chkDesktop As CheckBox
    Friend WithEvents pnlReady As Panel
    Friend WithEvents tlpReadyCanvas As TableLayoutPanel
    Friend WithEvents pnlHeaderReady As Panel
    Friend WithEvents lblReady As Label
    Friend WithEvents lblReadyInstructions As Label
    Friend WithEvents pnlInstall As Panel
    Friend WithEvents tlpInstallCanvas As TableLayoutPanel
    Friend WithEvents pnlHeaderInstall As Panel
    Friend WithEvents lblInstall As Label
    Friend WithEvents lblInstallInstructions As Label
    Friend WithEvents tlpInstallStatus As TableLayoutPanel
    Friend WithEvents lblStatusHeader As Label
    Friend WithEvents lblStatusProcess As Label
    Friend WithEvents barInstallProgress As ProgressBar
    Friend WithEvents dialogDirectory As FolderBrowserDialog
End Class
