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
        Me.grpIdentification = New System.Windows.Forms.GroupBox()
        Me.lblMax = New System.Windows.Forms.Label()
        Me.txtIndex = New System.Windows.Forms.TextBox()
        Me.txtTotal = New System.Windows.Forms.TextBox()
        Me.lblIndex = New System.Windows.Forms.Label()
        Me.txtLong = New System.Windows.Forms.TextBox()
        Me.txtShort = New System.Windows.Forms.TextBox()
        Me.lblLong = New System.Windows.Forms.Label()
        Me.lblShort = New System.Windows.Forms.Label()
        Me.menuStripMain = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenCatalogueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.PreferencesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewTapeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewModelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewManufactererToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.NewDeckToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.UpdateTapeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteTapeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchTapesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchModelsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchManufacturersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ViewDecksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ViewStatisticsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowConsoleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OutputConsoleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TutorialToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FeedbackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.grpData = New System.Windows.Forms.GroupBox()
        Me.grpSideB = New System.Windows.Forms.GroupBox()
        Me.numPeakB = New System.Windows.Forms.NumericUpDown()
        Me.lblPeakB = New System.Windows.Forms.Label()
        Me.cmbInputB = New System.Windows.Forms.ComboBox()
        Me.lblInputB = New System.Windows.Forms.Label()
        Me.grpConfigB = New System.Windows.Forms.GroupBox()
        Me.lblProcessingB = New System.Windows.Forms.Label()
        Me.chkDubbedB = New System.Windows.Forms.CheckBox()
        Me.cmbNRB = New System.Windows.Forms.ComboBox()
        Me.lblNRB = New System.Windows.Forms.Label()
        Me.cmbSpeedB = New System.Windows.Forms.ComboBox()
        Me.chkHXB = New System.Windows.Forms.CheckBox()
        Me.chkMPXB = New System.Windows.Forms.CheckBox()
        Me.lblSpeedB = New System.Windows.Forms.Label()
        Me.grpContentsB = New System.Windows.Forms.GroupBox()
        Me.lblTitleB = New System.Windows.Forms.Label()
        Me.lblArtistB = New System.Windows.Forms.Label()
        Me.txtTitleB = New System.Windows.Forms.TextBox()
        Me.txtArtistB = New System.Windows.Forms.TextBox()
        Me.lblContentsB = New System.Windows.Forms.Label()
        Me.cmbContentsB = New System.Windows.Forms.ComboBox()
        Me.cmbBiasB = New System.Windows.Forms.ComboBox()
        Me.lblBiasB = New System.Windows.Forms.Label()
        Me.cmbEQB = New System.Windows.Forms.ComboBox()
        Me.numBiasCalB = New System.Windows.Forms.NumericUpDown()
        Me.lblBiasCalB = New System.Windows.Forms.Label()
        Me.lblEQB = New System.Windows.Forms.Label()
        Me.numLevelB = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelB = New System.Windows.Forms.Label()
        Me.numLevelCalB = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelCalB = New System.Windows.Forms.Label()
        Me.lblNameB = New System.Windows.Forms.Label()
        Me.txtNameB = New System.Windows.Forms.TextBox()
        Me.datRecordedB = New System.Windows.Forms.DateTimePicker()
        Me.lblRecordedB = New System.Windows.Forms.Label()
        Me.cmbDeckB = New System.Windows.Forms.ComboBox()
        Me.lblDeckB = New System.Windows.Forms.Label()
        Me.grpTaped = New System.Windows.Forms.GroupBox()
        Me.lblSides = New System.Windows.Forms.Label()
        Me.chkTapedB = New System.Windows.Forms.CheckBox()
        Me.chkTapedA = New System.Windows.Forms.CheckBox()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.chkPackaged = New System.Windows.Forms.CheckBox()
        Me.lblCondition = New System.Windows.Forms.Label()
        Me.cmbCondition = New System.Windows.Forms.ComboBox()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.grpSideA = New System.Windows.Forms.GroupBox()
        Me.numPeakA = New System.Windows.Forms.NumericUpDown()
        Me.lblPeakA = New System.Windows.Forms.Label()
        Me.cmbInputA = New System.Windows.Forms.ComboBox()
        Me.lblInputA = New System.Windows.Forms.Label()
        Me.grpConfigA = New System.Windows.Forms.GroupBox()
        Me.lblProcessingA = New System.Windows.Forms.Label()
        Me.chkDubbedA = New System.Windows.Forms.CheckBox()
        Me.cmbNRA = New System.Windows.Forms.ComboBox()
        Me.lblNRA = New System.Windows.Forms.Label()
        Me.cmbSpeedA = New System.Windows.Forms.ComboBox()
        Me.chkHXA = New System.Windows.Forms.CheckBox()
        Me.chkMPXA = New System.Windows.Forms.CheckBox()
        Me.lblSpeed = New System.Windows.Forms.Label()
        Me.grpContentsA = New System.Windows.Forms.GroupBox()
        Me.lblTitleA = New System.Windows.Forms.Label()
        Me.lblArtistA = New System.Windows.Forms.Label()
        Me.txtTitleA = New System.Windows.Forms.TextBox()
        Me.txtArtistA = New System.Windows.Forms.TextBox()
        Me.lblContentsA = New System.Windows.Forms.Label()
        Me.cmbContentsA = New System.Windows.Forms.ComboBox()
        Me.cmbBiasA = New System.Windows.Forms.ComboBox()
        Me.lblBiasA = New System.Windows.Forms.Label()
        Me.cmbEQA = New System.Windows.Forms.ComboBox()
        Me.numBiasCalA = New System.Windows.Forms.NumericUpDown()
        Me.lblBiasCalA = New System.Windows.Forms.Label()
        Me.lblEQA = New System.Windows.Forms.Label()
        Me.numLevelA = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelA = New System.Windows.Forms.Label()
        Me.numLevelCalA = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelCalA = New System.Windows.Forms.Label()
        Me.lblNameA = New System.Windows.Forms.Label()
        Me.txtNameA = New System.Windows.Forms.TextBox()
        Me.datRecordedA = New System.Windows.Forms.DateTimePicker()
        Me.lblRecordedA = New System.Windows.Forms.Label()
        Me.cmbDeckA = New System.Windows.Forms.ComboBox()
        Me.lblDeckA = New System.Windows.Forms.Label()
        Me.grpModel = New System.Windows.Forms.GroupBox()
        Me.cmbRegion = New System.Windows.Forms.ComboBox()
        Me.numLength = New System.Windows.Forms.NumericUpDown()
        Me.numYear = New System.Windows.Forms.NumericUpDown()
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.lblRegion = New System.Windows.Forms.Label()
        Me.lblLength = New System.Windows.Forms.Label()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.grpFind = New System.Windows.Forms.GroupBox()
        Me.cmbField = New System.Windows.Forms.ComboBox()
        Me.txtTerm = New System.Windows.Forms.TextBox()
        Me.btnFind = New System.Windows.Forms.Button()
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblAbout = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.grpScroll = New System.Windows.Forms.GroupBox()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.btnPrevious = New System.Windows.Forms.Button()
        Me.dlgOpen = New System.Windows.Forms.OpenFileDialog()
        Me.dlgSaveAs = New System.Windows.Forms.SaveFileDialog()
        Me.grpIdentification.SuspendLayout()
        Me.menuStripMain.SuspendLayout()
        Me.grpData.SuspendLayout()
        Me.grpSideB.SuspendLayout()
        CType(Me.numPeakB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpConfigB.SuspendLayout()
        Me.grpContentsB.SuspendLayout()
        CType(Me.numBiasCalB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLevelB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLevelCalB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTaped.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        Me.grpNotes.SuspendLayout()
        Me.grpSideA.SuspendLayout()
        CType(Me.numPeakA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpConfigA.SuspendLayout()
        Me.grpContentsA.SuspendLayout()
        CType(Me.numBiasCalA, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLevelA, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLevelCalA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpModel.SuspendLayout()
        CType(Me.numLength, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpFind.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpScroll.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpIdentification
        '
        Me.grpIdentification.Controls.Add(Me.lblMax)
        Me.grpIdentification.Controls.Add(Me.txtIndex)
        Me.grpIdentification.Controls.Add(Me.txtTotal)
        Me.grpIdentification.Controls.Add(Me.lblIndex)
        Me.grpIdentification.Controls.Add(Me.txtLong)
        Me.grpIdentification.Controls.Add(Me.txtShort)
        Me.grpIdentification.Controls.Add(Me.lblLong)
        Me.grpIdentification.Controls.Add(Me.lblShort)
        Me.grpIdentification.Enabled = False
        Me.grpIdentification.Location = New System.Drawing.Point(11, 74)
        Me.grpIdentification.Margin = New System.Windows.Forms.Padding(2)
        Me.grpIdentification.Name = "grpIdentification"
        Me.grpIdentification.Padding = New System.Windows.Forms.Padding(2)
        Me.grpIdentification.Size = New System.Drawing.Size(437, 43)
        Me.grpIdentification.TabIndex = 4
        Me.grpIdentification.TabStop = False
        Me.grpIdentification.Text = "Identification"
        '
        'lblMax
        '
        Me.lblMax.AutoSize = True
        Me.lblMax.Location = New System.Drawing.Point(375, 20)
        Me.lblMax.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMax.Name = "lblMax"
        Me.lblMax.Size = New System.Drawing.Size(12, 13)
        Me.lblMax.TabIndex = 6
        Me.lblMax.Text = "/"
        '
        'txtIndex
        '
        Me.txtIndex.Location = New System.Drawing.Point(328, 17)
        Me.txtIndex.Margin = New System.Windows.Forms.Padding(2)
        Me.txtIndex.Name = "txtIndex"
        Me.txtIndex.ReadOnly = True
        Me.txtIndex.Size = New System.Drawing.Size(45, 20)
        Me.txtIndex.TabIndex = 5
        Me.txtIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTotal
        '
        Me.txtTotal.Location = New System.Drawing.Point(388, 17)
        Me.txtTotal.Margin = New System.Windows.Forms.Padding(2)
        Me.txtTotal.Name = "txtTotal"
        Me.txtTotal.ReadOnly = True
        Me.txtTotal.Size = New System.Drawing.Size(44, 20)
        Me.txtTotal.TabIndex = 3
        Me.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblIndex
        '
        Me.lblIndex.AutoSize = True
        Me.lblIndex.Location = New System.Drawing.Point(278, 21)
        Me.lblIndex.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblIndex.Name = "lblIndex"
        Me.lblIndex.Size = New System.Drawing.Size(47, 13)
        Me.lblIndex.TabIndex = 4
        Me.lblIndex.Text = "Number:"
        '
        'txtLong
        '
        Me.txtLong.Location = New System.Drawing.Point(164, 17)
        Me.txtLong.Margin = New System.Windows.Forms.Padding(2)
        Me.txtLong.Name = "txtLong"
        Me.txtLong.ReadOnly = True
        Me.txtLong.Size = New System.Drawing.Size(109, 20)
        Me.txtLong.TabIndex = 2
        '
        'txtShort
        '
        Me.txtShort.Location = New System.Drawing.Point(44, 17)
        Me.txtShort.Margin = New System.Windows.Forms.Padding(2)
        Me.txtShort.Name = "txtShort"
        Me.txtShort.ReadOnly = True
        Me.txtShort.Size = New System.Drawing.Size(79, 20)
        Me.txtShort.TabIndex = 1
        '
        'lblLong
        '
        Me.lblLong.AutoSize = True
        Me.lblLong.Location = New System.Drawing.Point(127, 20)
        Me.lblLong.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLong.Name = "lblLong"
        Me.lblLong.Size = New System.Drawing.Size(34, 13)
        Me.lblLong.TabIndex = 1
        Me.lblLong.Text = "Long:"
        '
        'lblShort
        '
        Me.lblShort.AutoSize = True
        Me.lblShort.Location = New System.Drawing.Point(4, 20)
        Me.lblShort.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblShort.Name = "lblShort"
        Me.lblShort.Size = New System.Drawing.Size(35, 13)
        Me.lblShort.TabIndex = 0
        Me.lblShort.Text = "Short:"
        '
        'menuStripMain
        '
        Me.menuStripMain.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.menuStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.ToolsToolStripMenuItem1, Me.HelpToolStripMenuItem})
        Me.menuStripMain.Location = New System.Drawing.Point(0, 0)
        Me.menuStripMain.Name = "menuStripMain"
        Me.menuStripMain.Padding = New System.Windows.Forms.Padding(4, 2, 0, 2)
        Me.menuStripMain.Size = New System.Drawing.Size(750, 24)
        Me.menuStripMain.TabIndex = 8
        Me.menuStripMain.Text = "Menu Strip"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenCatalogueToolStripMenuItem, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.ToolStripSeparator1, Me.PreferencesToolStripMenuItem, Me.ToolStripSeparator4, Me.CloseToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.NewToolStripMenuItem.Text = "New"
        '
        'OpenCatalogueToolStripMenuItem
        '
        Me.OpenCatalogueToolStripMenuItem.Name = "OpenCatalogueToolStripMenuItem"
        Me.OpenCatalogueToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenCatalogueToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.OpenCatalogueToolStripMenuItem.Text = "Open"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save As"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(183, 6)
        '
        'PreferencesToolStripMenuItem
        '
        Me.PreferencesToolStripMenuItem.Name = "PreferencesToolStripMenuItem"
        Me.PreferencesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.PreferencesToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.PreferencesToolStripMenuItem.Text = "Settings"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(183, 6)
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewTapeToolStripMenuItem, Me.NewModelToolStripMenuItem, Me.NewManufactererToolStripMenuItem, Me.ToolStripSeparator2, Me.NewDeckToolStripMenuItem, Me.ToolStripSeparator3, Me.UpdateTapeToolStripMenuItem, Me.DeleteTapeToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "Edit"
        '
        'NewTapeToolStripMenuItem
        '
        Me.NewTapeToolStripMenuItem.Name = "NewTapeToolStripMenuItem"
        Me.NewTapeToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.NewTapeToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.NewTapeToolStripMenuItem.Text = "Add Tape"
        '
        'NewModelToolStripMenuItem
        '
        Me.NewModelToolStripMenuItem.Name = "NewModelToolStripMenuItem"
        Me.NewModelToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.M), System.Windows.Forms.Keys)
        Me.NewModelToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.NewModelToolStripMenuItem.Text = "Add Model"
        '
        'NewManufactererToolStripMenuItem
        '
        Me.NewManufactererToolStripMenuItem.Name = "NewManufactererToolStripMenuItem"
        Me.NewManufactererToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.B), System.Windows.Forms.Keys)
        Me.NewManufactererToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.NewManufactererToolStripMenuItem.Text = "Add Brand"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(210, 6)
        '
        'NewDeckToolStripMenuItem
        '
        Me.NewDeckToolStripMenuItem.Name = "NewDeckToolStripMenuItem"
        Me.NewDeckToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D), System.Windows.Forms.Keys)
        Me.NewDeckToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.NewDeckToolStripMenuItem.Text = "Add Deck"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(210, 6)
        '
        'UpdateTapeToolStripMenuItem
        '
        Me.UpdateTapeToolStripMenuItem.Enabled = False
        Me.UpdateTapeToolStripMenuItem.Name = "UpdateTapeToolStripMenuItem"
        Me.UpdateTapeToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.UpdateTapeToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.UpdateTapeToolStripMenuItem.Text = "Update Tape"
        '
        'DeleteTapeToolStripMenuItem
        '
        Me.DeleteTapeToolStripMenuItem.Enabled = False
        Me.DeleteTapeToolStripMenuItem.Name = "DeleteTapeToolStripMenuItem"
        Me.DeleteTapeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.DeleteTapeToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.DeleteTapeToolStripMenuItem.Text = "Delete Tape"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SearchTapesToolStripMenuItem, Me.SearchModelsToolStripMenuItem, Me.SearchManufacturersToolStripMenuItem, Me.ToolStripSeparator5, Me.ViewDecksToolStripMenuItem, Me.ToolStripSeparator6, Me.ViewStatisticsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.ToolsToolStripMenuItem.Text = "Data"
        '
        'SearchTapesToolStripMenuItem
        '
        Me.SearchTapesToolStripMenuItem.Name = "SearchTapesToolStripMenuItem"
        Me.SearchTapesToolStripMenuItem.ShortcutKeyDisplayString = ""
        Me.SearchTapesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4
        Me.SearchTapesToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SearchTapesToolStripMenuItem.Text = "View Tapes"
        '
        'SearchModelsToolStripMenuItem
        '
        Me.SearchModelsToolStripMenuItem.Name = "SearchModelsToolStripMenuItem"
        Me.SearchModelsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.SearchModelsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SearchModelsToolStripMenuItem.Text = "View Models"
        '
        'SearchManufacturersToolStripMenuItem
        '
        Me.SearchManufacturersToolStripMenuItem.Name = "SearchManufacturersToolStripMenuItem"
        Me.SearchManufacturersToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6
        Me.SearchManufacturersToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SearchManufacturersToolStripMenuItem.Text = "View Brands"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(177, 6)
        '
        'ViewDecksToolStripMenuItem
        '
        Me.ViewDecksToolStripMenuItem.Name = "ViewDecksToolStripMenuItem"
        Me.ViewDecksToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7
        Me.ViewDecksToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ViewDecksToolStripMenuItem.Text = "View Decks"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(177, 6)
        '
        'ViewStatisticsToolStripMenuItem
        '
        Me.ViewStatisticsToolStripMenuItem.Enabled = False
        Me.ViewStatisticsToolStripMenuItem.Name = "ViewStatisticsToolStripMenuItem"
        Me.ViewStatisticsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8
        Me.ViewStatisticsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ViewStatisticsToolStripMenuItem.Text = "View Statistics"
        '
        'ToolsToolStripMenuItem1
        '
        Me.ToolsToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowConsoleToolStripMenuItem, Me.OutputConsoleToolStripMenuItem})
        Me.ToolsToolStripMenuItem1.Name = "ToolsToolStripMenuItem1"
        Me.ToolsToolStripMenuItem1.Size = New System.Drawing.Size(46, 20)
        Me.ToolsToolStripMenuItem1.Text = "Tools"
        '
        'ShowConsoleToolStripMenuItem
        '
        Me.ShowConsoleToolStripMenuItem.Name = "ShowConsoleToolStripMenuItem"
        Me.ShowConsoleToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3
        Me.ShowConsoleToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.ShowConsoleToolStripMenuItem.Text = "Show Console"
        '
        'OutputConsoleToolStripMenuItem
        '
        Me.OutputConsoleToolStripMenuItem.Name = "OutputConsoleToolStripMenuItem"
        Me.OutputConsoleToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.OutputConsoleToolStripMenuItem.Text = "Output Console to File"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TutorialToolStripMenuItem, Me.FeedbackToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'TutorialToolStripMenuItem
        '
        Me.TutorialToolStripMenuItem.Name = "TutorialToolStripMenuItem"
        Me.TutorialToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.TutorialToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.TutorialToolStripMenuItem.Text = "Tutorial"
        '
        'FeedbackToolStripMenuItem
        '
        Me.FeedbackToolStripMenuItem.Name = "FeedbackToolStripMenuItem"
        Me.FeedbackToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.FeedbackToolStripMenuItem.Text = "Feedback"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'grpData
        '
        Me.grpData.Controls.Add(Me.grpSideB)
        Me.grpData.Controls.Add(Me.grpTaped)
        Me.grpData.Controls.Add(Me.grpBasic)
        Me.grpData.Controls.Add(Me.grpNotes)
        Me.grpData.Controls.Add(Me.grpSideA)
        Me.grpData.Controls.Add(Me.grpModel)
        Me.grpData.Enabled = False
        Me.grpData.Location = New System.Drawing.Point(11, 121)
        Me.grpData.Margin = New System.Windows.Forms.Padding(2)
        Me.grpData.Name = "grpData"
        Me.grpData.Padding = New System.Windows.Forms.Padding(2)
        Me.grpData.Size = New System.Drawing.Size(728, 374)
        Me.grpData.TabIndex = 5
        Me.grpData.TabStop = False
        Me.grpData.Text = "Data"
        '
        'grpSideB
        '
        Me.grpSideB.Controls.Add(Me.numPeakB)
        Me.grpSideB.Controls.Add(Me.lblPeakB)
        Me.grpSideB.Controls.Add(Me.cmbInputB)
        Me.grpSideB.Controls.Add(Me.lblInputB)
        Me.grpSideB.Controls.Add(Me.grpConfigB)
        Me.grpSideB.Controls.Add(Me.lblNameB)
        Me.grpSideB.Controls.Add(Me.txtNameB)
        Me.grpSideB.Controls.Add(Me.datRecordedB)
        Me.grpSideB.Controls.Add(Me.lblRecordedB)
        Me.grpSideB.Controls.Add(Me.cmbDeckB)
        Me.grpSideB.Controls.Add(Me.lblDeckB)
        Me.grpSideB.Enabled = False
        Me.grpSideB.Location = New System.Drawing.Point(497, 17)
        Me.grpSideB.Margin = New System.Windows.Forms.Padding(2)
        Me.grpSideB.Name = "grpSideB"
        Me.grpSideB.Padding = New System.Windows.Forms.Padding(2)
        Me.grpSideB.Size = New System.Drawing.Size(226, 352)
        Me.grpSideB.TabIndex = 34
        Me.grpSideB.TabStop = False
        Me.grpSideB.Text = "Side B"
        '
        'numPeakB
        '
        Me.numPeakB.Location = New System.Drawing.Point(180, 90)
        Me.numPeakB.Margin = New System.Windows.Forms.Padding(2)
        Me.numPeakB.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numPeakB.Minimum = New Decimal(New Integer() {10, 0, 0, -2147483648})
        Me.numPeakB.Name = "numPeakB"
        Me.numPeakB.Size = New System.Drawing.Size(36, 20)
        Me.numPeakB.TabIndex = 5
        '
        'lblPeakB
        '
        Me.lblPeakB.AutoSize = True
        Me.lblPeakB.Location = New System.Drawing.Point(123, 93)
        Me.lblPeakB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblPeakB.Name = "lblPeakB"
        Me.lblPeakB.Size = New System.Drawing.Size(57, 13)
        Me.lblPeakB.TabIndex = 33
        Me.lblPeakB.Text = "Peak (dB):"
        '
        'cmbInputB
        '
        Me.cmbInputB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbInputB.FormattingEnabled = True
        Me.cmbInputB.Items.AddRange(New Object() {"Cassette", "Reel", "FM", "AM", "DAB", "Record", "CD", "DVD", "VHS", "TV", "Phone", "Other"})
        Me.cmbInputB.Location = New System.Drawing.Point(46, 90)
        Me.cmbInputB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbInputB.Name = "cmbInputB"
        Me.cmbInputB.Size = New System.Drawing.Size(75, 21)
        Me.cmbInputB.TabIndex = 4
        '
        'lblInputB
        '
        Me.lblInputB.AutoSize = True
        Me.lblInputB.Location = New System.Drawing.Point(4, 93)
        Me.lblInputB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblInputB.Name = "lblInputB"
        Me.lblInputB.Size = New System.Drawing.Size(34, 13)
        Me.lblInputB.TabIndex = 31
        Me.lblInputB.Text = "Input:"
        '
        'grpConfigB
        '
        Me.grpConfigB.Controls.Add(Me.lblProcessingB)
        Me.grpConfigB.Controls.Add(Me.chkDubbedB)
        Me.grpConfigB.Controls.Add(Me.cmbNRB)
        Me.grpConfigB.Controls.Add(Me.lblNRB)
        Me.grpConfigB.Controls.Add(Me.cmbSpeedB)
        Me.grpConfigB.Controls.Add(Me.chkHXB)
        Me.grpConfigB.Controls.Add(Me.chkMPXB)
        Me.grpConfigB.Controls.Add(Me.lblSpeedB)
        Me.grpConfigB.Controls.Add(Me.grpContentsB)
        Me.grpConfigB.Controls.Add(Me.cmbBiasB)
        Me.grpConfigB.Controls.Add(Me.lblBiasB)
        Me.grpConfigB.Controls.Add(Me.cmbEQB)
        Me.grpConfigB.Controls.Add(Me.numBiasCalB)
        Me.grpConfigB.Controls.Add(Me.lblBiasCalB)
        Me.grpConfigB.Controls.Add(Me.lblEQB)
        Me.grpConfigB.Controls.Add(Me.numLevelB)
        Me.grpConfigB.Controls.Add(Me.lblLevelB)
        Me.grpConfigB.Controls.Add(Me.numLevelCalB)
        Me.grpConfigB.Controls.Add(Me.lblLevelCalB)
        Me.grpConfigB.Location = New System.Drawing.Point(5, 115)
        Me.grpConfigB.Margin = New System.Windows.Forms.Padding(2)
        Me.grpConfigB.Name = "grpConfigB"
        Me.grpConfigB.Padding = New System.Windows.Forms.Padding(2)
        Me.grpConfigB.Size = New System.Drawing.Size(216, 232)
        Me.grpConfigB.TabIndex = 14
        Me.grpConfigB.TabStop = False
        Me.grpConfigB.Text = "Configuration"
        '
        'lblProcessingB
        '
        Me.lblProcessingB.AutoSize = True
        Me.lblProcessingB.Location = New System.Drawing.Point(4, 117)
        Me.lblProcessingB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblProcessingB.Name = "lblProcessingB"
        Me.lblProcessingB.Size = New System.Drawing.Size(62, 13)
        Me.lblProcessingB.TabIndex = 32
        Me.lblProcessingB.Text = "Processing:"
        '
        'chkDubbedB
        '
        Me.chkDubbedB.AutoSize = True
        Me.chkDubbedB.Location = New System.Drawing.Point(149, 93)
        Me.chkDubbedB.Margin = New System.Windows.Forms.Padding(2)
        Me.chkDubbedB.Name = "chkDubbedB"
        Me.chkDubbedB.Size = New System.Drawing.Size(64, 17)
        Me.chkDubbedB.TabIndex = 9
        Me.chkDubbedB.Text = "Dubbed"
        Me.chkDubbedB.UseVisualStyleBackColor = True
        '
        'cmbNRB
        '
        Me.cmbNRB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNRB.FormattingEnabled = True
        Me.cmbNRB.Items.AddRange(New Object() {"None", "Dolby B", "Dolby C", "Dolby S", "DBX I", "DBX II"})
        Me.cmbNRB.Location = New System.Drawing.Point(41, 17)
        Me.cmbNRB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbNRB.Name = "cmbNRB"
        Me.cmbNRB.Size = New System.Drawing.Size(75, 21)
        Me.cmbNRB.TabIndex = 1
        '
        'lblNRB
        '
        Me.lblNRB.AutoSize = True
        Me.lblNRB.Location = New System.Drawing.Point(4, 20)
        Me.lblNRB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNRB.Name = "lblNRB"
        Me.lblNRB.Size = New System.Drawing.Size(26, 13)
        Me.lblNRB.TabIndex = 4
        Me.lblNRB.Text = "NR:"
        '
        'cmbSpeedB
        '
        Me.cmbSpeedB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSpeedB.FormattingEnabled = True
        Me.cmbSpeedB.Items.AddRange(New Object() {"15/16", "1 7/8", "3 3/4"})
        Me.cmbSpeedB.Location = New System.Drawing.Point(72, 91)
        Me.cmbSpeedB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbSpeedB.Name = "cmbSpeedB"
        Me.cmbSpeedB.Size = New System.Drawing.Size(66, 21)
        Me.cmbSpeedB.TabIndex = 8
        '
        'chkHXB
        '
        Me.chkHXB.AutoSize = True
        Me.chkHXB.Location = New System.Drawing.Point(78, 116)
        Me.chkHXB.Margin = New System.Windows.Forms.Padding(2)
        Me.chkHXB.Name = "chkHXB"
        Me.chkHXB.Size = New System.Drawing.Size(60, 17)
        Me.chkHXB.TabIndex = 7
        Me.chkHXB.Text = "HX-Pro"
        Me.chkHXB.UseVisualStyleBackColor = True
        '
        'chkMPXB
        '
        Me.chkMPXB.AutoSize = True
        Me.chkMPXB.Location = New System.Drawing.Point(149, 116)
        Me.chkMPXB.Margin = New System.Windows.Forms.Padding(2)
        Me.chkMPXB.Name = "chkMPXB"
        Me.chkMPXB.Size = New System.Drawing.Size(49, 17)
        Me.chkMPXB.TabIndex = 10
        Me.chkMPXB.Text = "MPX"
        Me.chkMPXB.UseVisualStyleBackColor = True
        '
        'lblSpeedB
        '
        Me.lblSpeedB.AutoSize = True
        Me.lblSpeedB.Location = New System.Drawing.Point(4, 94)
        Me.lblSpeedB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSpeedB.Name = "lblSpeedB"
        Me.lblSpeedB.Size = New System.Drawing.Size(67, 13)
        Me.lblSpeedB.TabIndex = 31
        Me.lblSpeedB.Text = "Speed (IPS):"
        '
        'grpContentsB
        '
        Me.grpContentsB.Controls.Add(Me.lblTitleB)
        Me.grpContentsB.Controls.Add(Me.lblArtistB)
        Me.grpContentsB.Controls.Add(Me.txtTitleB)
        Me.grpContentsB.Controls.Add(Me.txtArtistB)
        Me.grpContentsB.Controls.Add(Me.lblContentsB)
        Me.grpContentsB.Controls.Add(Me.cmbContentsB)
        Me.grpContentsB.Location = New System.Drawing.Point(5, 137)
        Me.grpContentsB.Margin = New System.Windows.Forms.Padding(2)
        Me.grpContentsB.Name = "grpContentsB"
        Me.grpContentsB.Padding = New System.Windows.Forms.Padding(2)
        Me.grpContentsB.Size = New System.Drawing.Size(206, 90)
        Me.grpContentsB.TabIndex = 11
        Me.grpContentsB.TabStop = False
        Me.grpContentsB.Text = "Contents"
        '
        'lblTitleB
        '
        Me.lblTitleB.AutoSize = True
        Me.lblTitleB.Location = New System.Drawing.Point(4, 67)
        Me.lblTitleB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitleB.Name = "lblTitleB"
        Me.lblTitleB.Size = New System.Drawing.Size(30, 13)
        Me.lblTitleB.TabIndex = 18
        Me.lblTitleB.Text = "Title:"
        '
        'lblArtistB
        '
        Me.lblArtistB.AutoSize = True
        Me.lblArtistB.Location = New System.Drawing.Point(4, 44)
        Me.lblArtistB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblArtistB.Name = "lblArtistB"
        Me.lblArtistB.Size = New System.Drawing.Size(33, 13)
        Me.lblArtistB.TabIndex = 17
        Me.lblArtistB.Text = "Artist:"
        '
        'txtTitleB
        '
        Me.txtTitleB.Location = New System.Drawing.Point(41, 64)
        Me.txtTitleB.Margin = New System.Windows.Forms.Padding(2)
        Me.txtTitleB.Name = "txtTitleB"
        Me.txtTitleB.Size = New System.Drawing.Size(160, 20)
        Me.txtTitleB.TabIndex = 3
        '
        'txtArtistB
        '
        Me.txtArtistB.Location = New System.Drawing.Point(41, 41)
        Me.txtArtistB.Margin = New System.Windows.Forms.Padding(2)
        Me.txtArtistB.Name = "txtArtistB"
        Me.txtArtistB.Size = New System.Drawing.Size(160, 20)
        Me.txtArtistB.TabIndex = 2
        '
        'lblContentsB
        '
        Me.lblContentsB.AutoSize = True
        Me.lblContentsB.Location = New System.Drawing.Point(4, 20)
        Me.lblContentsB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblContentsB.Name = "lblContentsB"
        Me.lblContentsB.Size = New System.Drawing.Size(34, 13)
        Me.lblContentsB.TabIndex = 14
        Me.lblContentsB.Text = "Type:"
        '
        'cmbContentsB
        '
        Me.cmbContentsB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbContentsB.FormattingEnabled = True
        Me.cmbContentsB.Items.AddRange(New Object() {"Mixtape", "Album", "Compilation", "Soundtrack", "EP", "Single"})
        Me.cmbContentsB.Location = New System.Drawing.Point(41, 17)
        Me.cmbContentsB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbContentsB.Name = "cmbContentsB"
        Me.cmbContentsB.Size = New System.Drawing.Size(160, 21)
        Me.cmbContentsB.TabIndex = 1
        '
        'cmbBiasB
        '
        Me.cmbBiasB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBiasB.FormattingEnabled = True
        Me.cmbBiasB.Items.AddRange(New Object() {"Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbBiasB.Location = New System.Drawing.Point(41, 42)
        Me.cmbBiasB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbBiasB.Name = "cmbBiasB"
        Me.cmbBiasB.Size = New System.Drawing.Size(97, 21)
        Me.cmbBiasB.TabIndex = 3
        '
        'lblBiasB
        '
        Me.lblBiasB.AutoSize = True
        Me.lblBiasB.Location = New System.Drawing.Point(4, 45)
        Me.lblBiasB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBiasB.Name = "lblBiasB"
        Me.lblBiasB.Size = New System.Drawing.Size(30, 13)
        Me.lblBiasB.TabIndex = 22
        Me.lblBiasB.Text = "Bias:"
        '
        'cmbEQB
        '
        Me.cmbEQB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEQB.FormattingEnabled = True
        Me.cmbEQB.Items.AddRange(New Object() {"120μs", "70μs"})
        Me.cmbEQB.Location = New System.Drawing.Point(149, 17)
        Me.cmbEQB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbEQB.Name = "cmbEQB"
        Me.cmbEQB.Size = New System.Drawing.Size(62, 21)
        Me.cmbEQB.TabIndex = 2
        '
        'numBiasCalB
        '
        Me.numBiasCalB.Location = New System.Drawing.Point(175, 42)
        Me.numBiasCalB.Margin = New System.Windows.Forms.Padding(2)
        Me.numBiasCalB.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
        Me.numBiasCalB.Minimum = New Decimal(New Integer() {20, 0, 0, -2147483648})
        Me.numBiasCalB.Name = "numBiasCalB"
        Me.numBiasCalB.Size = New System.Drawing.Size(36, 20)
        Me.numBiasCalB.TabIndex = 4
        '
        'lblBiasCalB
        '
        Me.lblBiasCalB.AutoSize = True
        Me.lblBiasCalB.Location = New System.Drawing.Point(147, 45)
        Me.lblBiasCalB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBiasCalB.Name = "lblBiasCalB"
        Me.lblBiasCalB.Size = New System.Drawing.Size(24, 13)
        Me.lblBiasCalB.TabIndex = 23
        Me.lblBiasCalB.Text = "±%:"
        '
        'lblEQB
        '
        Me.lblEQB.AutoSize = True
        Me.lblEQB.Location = New System.Drawing.Point(120, 20)
        Me.lblEQB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblEQB.Name = "lblEQB"
        Me.lblEQB.Size = New System.Drawing.Size(25, 13)
        Me.lblEQB.TabIndex = 27
        Me.lblEQB.Text = "EQ:"
        '
        'numLevelB
        '
        Me.numLevelB.DecimalPlaces = 2
        Me.numLevelB.Increment = New Decimal(New Integer() {25, 0, 0, 131072})
        Me.numLevelB.Location = New System.Drawing.Point(72, 67)
        Me.numLevelB.Margin = New System.Windows.Forms.Padding(2)
        Me.numLevelB.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numLevelB.Name = "numLevelB"
        Me.numLevelB.Size = New System.Drawing.Size(66, 20)
        Me.numLevelB.TabIndex = 5
        Me.numLevelB.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'lblLevelB
        '
        Me.lblLevelB.AutoSize = True
        Me.lblLevelB.Location = New System.Drawing.Point(4, 69)
        Me.lblLevelB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLevelB.Name = "lblLevelB"
        Me.lblLevelB.Size = New System.Drawing.Size(58, 13)
        Me.lblLevelB.TabIndex = 26
        Me.lblLevelB.Text = "Level (dB):"
        '
        'numLevelCalB
        '
        Me.numLevelCalB.DecimalPlaces = 1
        Me.numLevelCalB.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.numLevelCalB.Location = New System.Drawing.Point(175, 67)
        Me.numLevelCalB.Margin = New System.Windows.Forms.Padding(2)
        Me.numLevelCalB.Maximum = New Decimal(New Integer() {3, 0, 0, 0})
        Me.numLevelCalB.Minimum = New Decimal(New Integer() {3, 0, 0, -2147483648})
        Me.numLevelCalB.Name = "numLevelCalB"
        Me.numLevelCalB.Size = New System.Drawing.Size(36, 20)
        Me.numLevelCalB.TabIndex = 6
        '
        'lblLevelCalB
        '
        Me.lblLevelCalB.AutoSize = True
        Me.lblLevelCalB.Location = New System.Drawing.Point(142, 69)
        Me.lblLevelCalB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLevelCalB.Name = "lblLevelCalB"
        Me.lblLevelCalB.Size = New System.Drawing.Size(29, 13)
        Me.lblLevelCalB.TabIndex = 25
        Me.lblLevelCalB.Text = "±dB:"
        '
        'lblNameB
        '
        Me.lblNameB.AutoSize = True
        Me.lblNameB.Location = New System.Drawing.Point(4, 20)
        Me.lblNameB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNameB.Name = "lblNameB"
        Me.lblNameB.Size = New System.Drawing.Size(38, 13)
        Me.lblNameB.TabIndex = 13
        Me.lblNameB.Text = "Name:"
        '
        'txtNameB
        '
        Me.txtNameB.Location = New System.Drawing.Point(46, 17)
        Me.txtNameB.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNameB.Name = "txtNameB"
        Me.txtNameB.Size = New System.Drawing.Size(170, 20)
        Me.txtNameB.TabIndex = 1
        '
        'datRecordedB
        '
        Me.datRecordedB.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.datRecordedB.Location = New System.Drawing.Point(90, 41)
        Me.datRecordedB.Margin = New System.Windows.Forms.Padding(2)
        Me.datRecordedB.Name = "datRecordedB"
        Me.datRecordedB.Size = New System.Drawing.Size(126, 20)
        Me.datRecordedB.TabIndex = 2
        Me.datRecordedB.Value = New Date(2019, 1, 1, 0, 0, 0, 0)
        '
        'lblRecordedB
        '
        Me.lblRecordedB.AutoSize = True
        Me.lblRecordedB.Location = New System.Drawing.Point(4, 43)
        Me.lblRecordedB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRecordedB.Name = "lblRecordedB"
        Me.lblRecordedB.Size = New System.Drawing.Size(83, 13)
        Me.lblRecordedB.TabIndex = 11
        Me.lblRecordedB.Text = "Date Recorded:"
        '
        'cmbDeckB
        '
        Me.cmbDeckB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDeckB.FormattingEnabled = True
        Me.cmbDeckB.Location = New System.Drawing.Point(46, 65)
        Me.cmbDeckB.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbDeckB.Name = "cmbDeckB"
        Me.cmbDeckB.Size = New System.Drawing.Size(170, 21)
        Me.cmbDeckB.Sorted = True
        Me.cmbDeckB.TabIndex = 3
        '
        'lblDeckB
        '
        Me.lblDeckB.AutoSize = True
        Me.lblDeckB.Location = New System.Drawing.Point(4, 68)
        Me.lblDeckB.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDeckB.Name = "lblDeckB"
        Me.lblDeckB.Size = New System.Drawing.Size(36, 13)
        Me.lblDeckB.TabIndex = 29
        Me.lblDeckB.Text = "Deck:"
        '
        'grpTaped
        '
        Me.grpTaped.Controls.Add(Me.lblSides)
        Me.grpTaped.Controls.Add(Me.chkTapedB)
        Me.grpTaped.Controls.Add(Me.chkTapedA)
        Me.grpTaped.Location = New System.Drawing.Point(211, 17)
        Me.grpTaped.Margin = New System.Windows.Forms.Padding(2)
        Me.grpTaped.Name = "grpTaped"
        Me.grpTaped.Padding = New System.Windows.Forms.Padding(2)
        Me.grpTaped.Size = New System.Drawing.Size(52, 92)
        Me.grpTaped.TabIndex = 3
        Me.grpTaped.TabStop = False
        Me.grpTaped.Text = "Taped"
        '
        'lblSides
        '
        Me.lblSides.AutoSize = True
        Me.lblSides.Location = New System.Drawing.Point(5, 20)
        Me.lblSides.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSides.Name = "lblSides"
        Me.lblSides.Size = New System.Drawing.Size(36, 13)
        Me.lblSides.TabIndex = 14
        Me.lblSides.Text = "Sides:"
        '
        'chkTapedB
        '
        Me.chkTapedB.AutoSize = True
        Me.chkTapedB.Location = New System.Drawing.Point(13, 63)
        Me.chkTapedB.Margin = New System.Windows.Forms.Padding(2)
        Me.chkTapedB.Name = "chkTapedB"
        Me.chkTapedB.Size = New System.Drawing.Size(33, 17)
        Me.chkTapedB.TabIndex = 2
        Me.chkTapedB.Text = "B"
        Me.chkTapedB.UseVisualStyleBackColor = True
        '
        'chkTapedA
        '
        Me.chkTapedA.AutoSize = True
        Me.chkTapedA.Location = New System.Drawing.Point(13, 42)
        Me.chkTapedA.Margin = New System.Windows.Forms.Padding(2)
        Me.chkTapedA.Name = "chkTapedA"
        Me.chkTapedA.Size = New System.Drawing.Size(33, 17)
        Me.chkTapedA.TabIndex = 1
        Me.chkTapedA.Text = "A"
        Me.chkTapedA.UseVisualStyleBackColor = True
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.chkPackaged)
        Me.grpBasic.Controls.Add(Me.lblCondition)
        Me.grpBasic.Controls.Add(Me.cmbCondition)
        Me.grpBasic.Location = New System.Drawing.Point(5, 113)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Size = New System.Drawing.Size(258, 44)
        Me.grpBasic.TabIndex = 2
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'chkPackaged
        '
        Me.chkPackaged.AutoSize = True
        Me.chkPackaged.Location = New System.Drawing.Point(194, 19)
        Me.chkPackaged.Margin = New System.Windows.Forms.Padding(2)
        Me.chkPackaged.Name = "chkPackaged"
        Me.chkPackaged.Size = New System.Drawing.Size(59, 17)
        Me.chkPackaged.TabIndex = 3
        Me.chkPackaged.Text = "Sealed"
        Me.chkPackaged.UseVisualStyleBackColor = True
        '
        'lblCondition
        '
        Me.lblCondition.AutoSize = True
        Me.lblCondition.Location = New System.Drawing.Point(4, 20)
        Me.lblCondition.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCondition.Name = "lblCondition"
        Me.lblCondition.Size = New System.Drawing.Size(54, 13)
        Me.lblCondition.TabIndex = 9
        Me.lblCondition.Text = "Condition:"
        '
        'cmbCondition
        '
        Me.cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCondition.FormattingEnabled = True
        Me.cmbCondition.Items.AddRange(New Object() {"Mint", "Near Mint", "Very Good Plus", "Very Good", "Good Plus", "Good", "Fair", "Poor", "Broken"})
        Me.cmbCondition.Location = New System.Drawing.Point(62, 17)
        Me.cmbCondition.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbCondition.Name = "cmbCondition"
        Me.cmbCondition.Size = New System.Drawing.Size(124, 21)
        Me.cmbCondition.TabIndex = 2
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(5, 161)
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(2)
        Me.grpNotes.Size = New System.Drawing.Size(258, 208)
        Me.grpNotes.TabIndex = 6
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(5, 17)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(248, 185)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpSideA
        '
        Me.grpSideA.Controls.Add(Me.numPeakA)
        Me.grpSideA.Controls.Add(Me.lblPeakA)
        Me.grpSideA.Controls.Add(Me.cmbInputA)
        Me.grpSideA.Controls.Add(Me.lblInputA)
        Me.grpSideA.Controls.Add(Me.grpConfigA)
        Me.grpSideA.Controls.Add(Me.lblNameA)
        Me.grpSideA.Controls.Add(Me.txtNameA)
        Me.grpSideA.Controls.Add(Me.datRecordedA)
        Me.grpSideA.Controls.Add(Me.lblRecordedA)
        Me.grpSideA.Controls.Add(Me.cmbDeckA)
        Me.grpSideA.Controls.Add(Me.lblDeckA)
        Me.grpSideA.Enabled = False
        Me.grpSideA.Location = New System.Drawing.Point(267, 17)
        Me.grpSideA.Margin = New System.Windows.Forms.Padding(2)
        Me.grpSideA.Name = "grpSideA"
        Me.grpSideA.Padding = New System.Windows.Forms.Padding(2)
        Me.grpSideA.Size = New System.Drawing.Size(226, 352)
        Me.grpSideA.TabIndex = 4
        Me.grpSideA.TabStop = False
        Me.grpSideA.Text = "Side A"
        '
        'numPeakA
        '
        Me.numPeakA.Location = New System.Drawing.Point(180, 90)
        Me.numPeakA.Margin = New System.Windows.Forms.Padding(2)
        Me.numPeakA.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numPeakA.Minimum = New Decimal(New Integer() {10, 0, 0, -2147483648})
        Me.numPeakA.Name = "numPeakA"
        Me.numPeakA.Size = New System.Drawing.Size(36, 20)
        Me.numPeakA.TabIndex = 5
        '
        'lblPeakA
        '
        Me.lblPeakA.AutoSize = True
        Me.lblPeakA.Location = New System.Drawing.Point(123, 93)
        Me.lblPeakA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblPeakA.Name = "lblPeakA"
        Me.lblPeakA.Size = New System.Drawing.Size(57, 13)
        Me.lblPeakA.TabIndex = 33
        Me.lblPeakA.Text = "Peak (dB):"
        '
        'cmbInputA
        '
        Me.cmbInputA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbInputA.FormattingEnabled = True
        Me.cmbInputA.Items.AddRange(New Object() {"Cassette", "Reel", "FM", "AM", "DAB", "Record", "CD", "DVD", "VHS", "TV", "Phone", "Other"})
        Me.cmbInputA.Location = New System.Drawing.Point(46, 90)
        Me.cmbInputA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbInputA.Name = "cmbInputA"
        Me.cmbInputA.Size = New System.Drawing.Size(75, 21)
        Me.cmbInputA.TabIndex = 4
        '
        'lblInputA
        '
        Me.lblInputA.AutoSize = True
        Me.lblInputA.Location = New System.Drawing.Point(4, 93)
        Me.lblInputA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblInputA.Name = "lblInputA"
        Me.lblInputA.Size = New System.Drawing.Size(34, 13)
        Me.lblInputA.TabIndex = 31
        Me.lblInputA.Text = "Input:"
        '
        'grpConfigA
        '
        Me.grpConfigA.Controls.Add(Me.lblProcessingA)
        Me.grpConfigA.Controls.Add(Me.chkDubbedA)
        Me.grpConfigA.Controls.Add(Me.cmbNRA)
        Me.grpConfigA.Controls.Add(Me.lblNRA)
        Me.grpConfigA.Controls.Add(Me.cmbSpeedA)
        Me.grpConfigA.Controls.Add(Me.chkHXA)
        Me.grpConfigA.Controls.Add(Me.chkMPXA)
        Me.grpConfigA.Controls.Add(Me.lblSpeed)
        Me.grpConfigA.Controls.Add(Me.grpContentsA)
        Me.grpConfigA.Controls.Add(Me.cmbBiasA)
        Me.grpConfigA.Controls.Add(Me.lblBiasA)
        Me.grpConfigA.Controls.Add(Me.cmbEQA)
        Me.grpConfigA.Controls.Add(Me.numBiasCalA)
        Me.grpConfigA.Controls.Add(Me.lblBiasCalA)
        Me.grpConfigA.Controls.Add(Me.lblEQA)
        Me.grpConfigA.Controls.Add(Me.numLevelA)
        Me.grpConfigA.Controls.Add(Me.lblLevelA)
        Me.grpConfigA.Controls.Add(Me.numLevelCalA)
        Me.grpConfigA.Controls.Add(Me.lblLevelCalA)
        Me.grpConfigA.Location = New System.Drawing.Point(5, 115)
        Me.grpConfigA.Margin = New System.Windows.Forms.Padding(2)
        Me.grpConfigA.Name = "grpConfigA"
        Me.grpConfigA.Padding = New System.Windows.Forms.Padding(2)
        Me.grpConfigA.Size = New System.Drawing.Size(216, 232)
        Me.grpConfigA.TabIndex = 14
        Me.grpConfigA.TabStop = False
        Me.grpConfigA.Text = "Configuration"
        '
        'lblProcessingA
        '
        Me.lblProcessingA.AutoSize = True
        Me.lblProcessingA.Location = New System.Drawing.Point(4, 117)
        Me.lblProcessingA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblProcessingA.Name = "lblProcessingA"
        Me.lblProcessingA.Size = New System.Drawing.Size(62, 13)
        Me.lblProcessingA.TabIndex = 32
        Me.lblProcessingA.Text = "Processing:"
        '
        'chkDubbedA
        '
        Me.chkDubbedA.AutoSize = True
        Me.chkDubbedA.Location = New System.Drawing.Point(149, 93)
        Me.chkDubbedA.Margin = New System.Windows.Forms.Padding(2)
        Me.chkDubbedA.Name = "chkDubbedA"
        Me.chkDubbedA.Size = New System.Drawing.Size(64, 17)
        Me.chkDubbedA.TabIndex = 9
        Me.chkDubbedA.Text = "Dubbed"
        Me.chkDubbedA.UseVisualStyleBackColor = True
        '
        'cmbNRA
        '
        Me.cmbNRA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNRA.FormattingEnabled = True
        Me.cmbNRA.Items.AddRange(New Object() {"None", "Dolby B", "Dolby C", "Dolby S", "DBX I", "DBX II"})
        Me.cmbNRA.Location = New System.Drawing.Point(41, 17)
        Me.cmbNRA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbNRA.Name = "cmbNRA"
        Me.cmbNRA.Size = New System.Drawing.Size(75, 21)
        Me.cmbNRA.TabIndex = 1
        '
        'lblNRA
        '
        Me.lblNRA.AutoSize = True
        Me.lblNRA.Location = New System.Drawing.Point(4, 20)
        Me.lblNRA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNRA.Name = "lblNRA"
        Me.lblNRA.Size = New System.Drawing.Size(26, 13)
        Me.lblNRA.TabIndex = 4
        Me.lblNRA.Text = "NR:"
        '
        'cmbSpeedA
        '
        Me.cmbSpeedA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSpeedA.FormattingEnabled = True
        Me.cmbSpeedA.Items.AddRange(New Object() {"15/16", "1 7/8", "3 3/4"})
        Me.cmbSpeedA.Location = New System.Drawing.Point(72, 91)
        Me.cmbSpeedA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbSpeedA.Name = "cmbSpeedA"
        Me.cmbSpeedA.Size = New System.Drawing.Size(66, 21)
        Me.cmbSpeedA.TabIndex = 8
        '
        'chkHXA
        '
        Me.chkHXA.AutoSize = True
        Me.chkHXA.Location = New System.Drawing.Point(78, 116)
        Me.chkHXA.Margin = New System.Windows.Forms.Padding(2)
        Me.chkHXA.Name = "chkHXA"
        Me.chkHXA.Size = New System.Drawing.Size(60, 17)
        Me.chkHXA.TabIndex = 7
        Me.chkHXA.Text = "HX-Pro"
        Me.chkHXA.UseVisualStyleBackColor = True
        '
        'chkMPXA
        '
        Me.chkMPXA.AutoSize = True
        Me.chkMPXA.Location = New System.Drawing.Point(149, 116)
        Me.chkMPXA.Margin = New System.Windows.Forms.Padding(2)
        Me.chkMPXA.Name = "chkMPXA"
        Me.chkMPXA.Size = New System.Drawing.Size(49, 17)
        Me.chkMPXA.TabIndex = 10
        Me.chkMPXA.Text = "MPX"
        Me.chkMPXA.UseVisualStyleBackColor = True
        '
        'lblSpeed
        '
        Me.lblSpeed.AutoSize = True
        Me.lblSpeed.Location = New System.Drawing.Point(4, 94)
        Me.lblSpeed.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSpeed.Name = "lblSpeed"
        Me.lblSpeed.Size = New System.Drawing.Size(67, 13)
        Me.lblSpeed.TabIndex = 31
        Me.lblSpeed.Text = "Speed (IPS):"
        '
        'grpContentsA
        '
        Me.grpContentsA.Controls.Add(Me.lblTitleA)
        Me.grpContentsA.Controls.Add(Me.lblArtistA)
        Me.grpContentsA.Controls.Add(Me.txtTitleA)
        Me.grpContentsA.Controls.Add(Me.txtArtistA)
        Me.grpContentsA.Controls.Add(Me.lblContentsA)
        Me.grpContentsA.Controls.Add(Me.cmbContentsA)
        Me.grpContentsA.Location = New System.Drawing.Point(5, 137)
        Me.grpContentsA.Margin = New System.Windows.Forms.Padding(2)
        Me.grpContentsA.Name = "grpContentsA"
        Me.grpContentsA.Padding = New System.Windows.Forms.Padding(2)
        Me.grpContentsA.Size = New System.Drawing.Size(206, 90)
        Me.grpContentsA.TabIndex = 11
        Me.grpContentsA.TabStop = False
        Me.grpContentsA.Text = "Contents"
        '
        'lblTitleA
        '
        Me.lblTitleA.AutoSize = True
        Me.lblTitleA.Location = New System.Drawing.Point(4, 67)
        Me.lblTitleA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitleA.Name = "lblTitleA"
        Me.lblTitleA.Size = New System.Drawing.Size(30, 13)
        Me.lblTitleA.TabIndex = 18
        Me.lblTitleA.Text = "Title:"
        '
        'lblArtistA
        '
        Me.lblArtistA.AutoSize = True
        Me.lblArtistA.Location = New System.Drawing.Point(4, 44)
        Me.lblArtistA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblArtistA.Name = "lblArtistA"
        Me.lblArtistA.Size = New System.Drawing.Size(33, 13)
        Me.lblArtistA.TabIndex = 17
        Me.lblArtistA.Text = "Artist:"
        '
        'txtTitleA
        '
        Me.txtTitleA.Location = New System.Drawing.Point(41, 64)
        Me.txtTitleA.Margin = New System.Windows.Forms.Padding(2)
        Me.txtTitleA.Name = "txtTitleA"
        Me.txtTitleA.Size = New System.Drawing.Size(160, 20)
        Me.txtTitleA.TabIndex = 3
        '
        'txtArtistA
        '
        Me.txtArtistA.Location = New System.Drawing.Point(41, 41)
        Me.txtArtistA.Margin = New System.Windows.Forms.Padding(2)
        Me.txtArtistA.Name = "txtArtistA"
        Me.txtArtistA.Size = New System.Drawing.Size(160, 20)
        Me.txtArtistA.TabIndex = 2
        '
        'lblContentsA
        '
        Me.lblContentsA.AutoSize = True
        Me.lblContentsA.Location = New System.Drawing.Point(4, 20)
        Me.lblContentsA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblContentsA.Name = "lblContentsA"
        Me.lblContentsA.Size = New System.Drawing.Size(34, 13)
        Me.lblContentsA.TabIndex = 14
        Me.lblContentsA.Text = "Type:"
        '
        'cmbContentsA
        '
        Me.cmbContentsA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbContentsA.FormattingEnabled = True
        Me.cmbContentsA.Items.AddRange(New Object() {"Mixtape", "Album", "Compilation", "Soundtrack", "EP", "Single"})
        Me.cmbContentsA.Location = New System.Drawing.Point(41, 17)
        Me.cmbContentsA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbContentsA.Name = "cmbContentsA"
        Me.cmbContentsA.Size = New System.Drawing.Size(160, 21)
        Me.cmbContentsA.TabIndex = 1
        '
        'cmbBiasA
        '
        Me.cmbBiasA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBiasA.FormattingEnabled = True
        Me.cmbBiasA.Items.AddRange(New Object() {"Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbBiasA.Location = New System.Drawing.Point(41, 42)
        Me.cmbBiasA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbBiasA.Name = "cmbBiasA"
        Me.cmbBiasA.Size = New System.Drawing.Size(97, 21)
        Me.cmbBiasA.TabIndex = 3
        '
        'lblBiasA
        '
        Me.lblBiasA.AutoSize = True
        Me.lblBiasA.Location = New System.Drawing.Point(4, 45)
        Me.lblBiasA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBiasA.Name = "lblBiasA"
        Me.lblBiasA.Size = New System.Drawing.Size(30, 13)
        Me.lblBiasA.TabIndex = 22
        Me.lblBiasA.Text = "Bias:"
        '
        'cmbEQA
        '
        Me.cmbEQA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEQA.FormattingEnabled = True
        Me.cmbEQA.Items.AddRange(New Object() {"120μs", "70μs"})
        Me.cmbEQA.Location = New System.Drawing.Point(149, 17)
        Me.cmbEQA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbEQA.Name = "cmbEQA"
        Me.cmbEQA.Size = New System.Drawing.Size(62, 21)
        Me.cmbEQA.TabIndex = 2
        '
        'numBiasCalA
        '
        Me.numBiasCalA.Location = New System.Drawing.Point(175, 42)
        Me.numBiasCalA.Margin = New System.Windows.Forms.Padding(2)
        Me.numBiasCalA.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
        Me.numBiasCalA.Minimum = New Decimal(New Integer() {20, 0, 0, -2147483648})
        Me.numBiasCalA.Name = "numBiasCalA"
        Me.numBiasCalA.Size = New System.Drawing.Size(36, 20)
        Me.numBiasCalA.TabIndex = 4
        '
        'lblBiasCalA
        '
        Me.lblBiasCalA.AutoSize = True
        Me.lblBiasCalA.Location = New System.Drawing.Point(147, 45)
        Me.lblBiasCalA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBiasCalA.Name = "lblBiasCalA"
        Me.lblBiasCalA.Size = New System.Drawing.Size(24, 13)
        Me.lblBiasCalA.TabIndex = 23
        Me.lblBiasCalA.Text = "±%:"
        '
        'lblEQA
        '
        Me.lblEQA.AutoSize = True
        Me.lblEQA.Location = New System.Drawing.Point(120, 20)
        Me.lblEQA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblEQA.Name = "lblEQA"
        Me.lblEQA.Size = New System.Drawing.Size(25, 13)
        Me.lblEQA.TabIndex = 27
        Me.lblEQA.Text = "EQ:"
        '
        'numLevelA
        '
        Me.numLevelA.DecimalPlaces = 2
        Me.numLevelA.Increment = New Decimal(New Integer() {25, 0, 0, 131072})
        Me.numLevelA.Location = New System.Drawing.Point(72, 67)
        Me.numLevelA.Margin = New System.Windows.Forms.Padding(2)
        Me.numLevelA.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numLevelA.Name = "numLevelA"
        Me.numLevelA.Size = New System.Drawing.Size(66, 20)
        Me.numLevelA.TabIndex = 5
        Me.numLevelA.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'lblLevelA
        '
        Me.lblLevelA.AutoSize = True
        Me.lblLevelA.Location = New System.Drawing.Point(4, 69)
        Me.lblLevelA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLevelA.Name = "lblLevelA"
        Me.lblLevelA.Size = New System.Drawing.Size(58, 13)
        Me.lblLevelA.TabIndex = 26
        Me.lblLevelA.Text = "Level (dB):"
        '
        'numLevelCalA
        '
        Me.numLevelCalA.DecimalPlaces = 1
        Me.numLevelCalA.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.numLevelCalA.Location = New System.Drawing.Point(175, 67)
        Me.numLevelCalA.Margin = New System.Windows.Forms.Padding(2)
        Me.numLevelCalA.Maximum = New Decimal(New Integer() {3, 0, 0, 0})
        Me.numLevelCalA.Minimum = New Decimal(New Integer() {3, 0, 0, -2147483648})
        Me.numLevelCalA.Name = "numLevelCalA"
        Me.numLevelCalA.Size = New System.Drawing.Size(36, 20)
        Me.numLevelCalA.TabIndex = 6
        '
        'lblLevelCalA
        '
        Me.lblLevelCalA.AutoSize = True
        Me.lblLevelCalA.Location = New System.Drawing.Point(142, 69)
        Me.lblLevelCalA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLevelCalA.Name = "lblLevelCalA"
        Me.lblLevelCalA.Size = New System.Drawing.Size(29, 13)
        Me.lblLevelCalA.TabIndex = 25
        Me.lblLevelCalA.Text = "±dB:"
        '
        'lblNameA
        '
        Me.lblNameA.AutoSize = True
        Me.lblNameA.Location = New System.Drawing.Point(4, 20)
        Me.lblNameA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNameA.Name = "lblNameA"
        Me.lblNameA.Size = New System.Drawing.Size(38, 13)
        Me.lblNameA.TabIndex = 13
        Me.lblNameA.Text = "Name:"
        '
        'txtNameA
        '
        Me.txtNameA.Location = New System.Drawing.Point(46, 17)
        Me.txtNameA.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNameA.Name = "txtNameA"
        Me.txtNameA.Size = New System.Drawing.Size(170, 20)
        Me.txtNameA.TabIndex = 1
        '
        'datRecordedA
        '
        Me.datRecordedA.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.datRecordedA.Location = New System.Drawing.Point(90, 41)
        Me.datRecordedA.Margin = New System.Windows.Forms.Padding(2)
        Me.datRecordedA.Name = "datRecordedA"
        Me.datRecordedA.Size = New System.Drawing.Size(126, 20)
        Me.datRecordedA.TabIndex = 2
        Me.datRecordedA.Value = New Date(2019, 1, 1, 0, 0, 0, 0)
        '
        'lblRecordedA
        '
        Me.lblRecordedA.AutoSize = True
        Me.lblRecordedA.Location = New System.Drawing.Point(4, 43)
        Me.lblRecordedA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRecordedA.Name = "lblRecordedA"
        Me.lblRecordedA.Size = New System.Drawing.Size(83, 13)
        Me.lblRecordedA.TabIndex = 11
        Me.lblRecordedA.Text = "Date Recorded:"
        '
        'cmbDeckA
        '
        Me.cmbDeckA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDeckA.FormattingEnabled = True
        Me.cmbDeckA.Location = New System.Drawing.Point(46, 65)
        Me.cmbDeckA.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbDeckA.Name = "cmbDeckA"
        Me.cmbDeckA.Size = New System.Drawing.Size(170, 21)
        Me.cmbDeckA.Sorted = True
        Me.cmbDeckA.TabIndex = 3
        '
        'lblDeckA
        '
        Me.lblDeckA.AutoSize = True
        Me.lblDeckA.Location = New System.Drawing.Point(4, 68)
        Me.lblDeckA.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDeckA.Name = "lblDeckA"
        Me.lblDeckA.Size = New System.Drawing.Size(36, 13)
        Me.lblDeckA.TabIndex = 29
        Me.lblDeckA.Text = "Deck:"
        '
        'grpModel
        '
        Me.grpModel.Controls.Add(Me.cmbRegion)
        Me.grpModel.Controls.Add(Me.numLength)
        Me.grpModel.Controls.Add(Me.numYear)
        Me.grpModel.Controls.Add(Me.txtModel)
        Me.grpModel.Controls.Add(Me.lblRegion)
        Me.grpModel.Controls.Add(Me.lblLength)
        Me.grpModel.Controls.Add(Me.lblYear)
        Me.grpModel.Controls.Add(Me.lblModel)
        Me.grpModel.Location = New System.Drawing.Point(5, 17)
        Me.grpModel.Margin = New System.Windows.Forms.Padding(2)
        Me.grpModel.Name = "grpModel"
        Me.grpModel.Padding = New System.Windows.Forms.Padding(2)
        Me.grpModel.Size = New System.Drawing.Size(202, 92)
        Me.grpModel.TabIndex = 1
        Me.grpModel.TabStop = False
        Me.grpModel.Text = "Model"
        '
        'cmbRegion
        '
        Me.cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbRegion.FormattingEnabled = True
        Me.cmbRegion.Items.AddRange(New Object() {"Europe", "USA", "Japan", "Europe/USA", "Europe/Japan", "USA/Japan", "Europe/USA/Japan", "Unkown"})
        Me.cmbRegion.Location = New System.Drawing.Point(49, 65)
        Me.cmbRegion.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbRegion.Name = "cmbRegion"
        Me.cmbRegion.Size = New System.Drawing.Size(148, 21)
        Me.cmbRegion.TabIndex = 25
        '
        'numLength
        '
        Me.numLength.Location = New System.Drawing.Point(152, 41)
        Me.numLength.Margin = New System.Windows.Forms.Padding(2)
        Me.numLength.Maximum = New Decimal(New Integer() {180, 0, 0, 0})
        Me.numLength.Name = "numLength"
        Me.numLength.Size = New System.Drawing.Size(45, 20)
        Me.numLength.TabIndex = 24
        Me.numLength.Value = New Decimal(New Integer() {90, 0, 0, 0})
        '
        'numYear
        '
        Me.numYear.Location = New System.Drawing.Point(49, 41)
        Me.numYear.Margin = New System.Windows.Forms.Padding(2)
        Me.numYear.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYear.Minimum = New Decimal(New Integer() {1963, 0, 0, 0})
        Me.numYear.Name = "numYear"
        Me.numYear.Size = New System.Drawing.Size(52, 20)
        Me.numYear.TabIndex = 23
        Me.numYear.Value = New Decimal(New Integer() {1990, 0, 0, 0})
        '
        'txtModel
        '
        Me.txtModel.Location = New System.Drawing.Point(49, 17)
        Me.txtModel.Margin = New System.Windows.Forms.Padding(2)
        Me.txtModel.Name = "txtModel"
        Me.txtModel.ReadOnly = True
        Me.txtModel.Size = New System.Drawing.Size(148, 20)
        Me.txtModel.TabIndex = 1
        '
        'lblRegion
        '
        Me.lblRegion.AutoSize = True
        Me.lblRegion.Location = New System.Drawing.Point(4, 68)
        Me.lblRegion.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRegion.Name = "lblRegion"
        Me.lblRegion.Size = New System.Drawing.Size(44, 13)
        Me.lblRegion.TabIndex = 22
        Me.lblRegion.Text = "Region:"
        '
        'lblLength
        '
        Me.lblLength.AutoSize = True
        Me.lblLength.Location = New System.Drawing.Point(105, 43)
        Me.lblLength.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLength.Name = "lblLength"
        Me.lblLength.Size = New System.Drawing.Size(43, 13)
        Me.lblLength.TabIndex = 20
        Me.lblLength.Text = "Length:"
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Location = New System.Drawing.Point(4, 43)
        Me.lblYear.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(32, 13)
        Me.lblYear.TabIndex = 17
        Me.lblYear.Text = "Year:"
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 20)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
        Me.lblModel.TabIndex = 15
        Me.lblModel.Text = "Model:"
        '
        'grpFind
        '
        Me.grpFind.Controls.Add(Me.cmbField)
        Me.grpFind.Controls.Add(Me.txtTerm)
        Me.grpFind.Controls.Add(Me.btnFind)
        Me.grpFind.Enabled = False
        Me.grpFind.Location = New System.Drawing.Point(11, 26)
        Me.grpFind.Margin = New System.Windows.Forms.Padding(2)
        Me.grpFind.Name = "grpFind"
        Me.grpFind.Padding = New System.Windows.Forms.Padding(2)
        Me.grpFind.Size = New System.Drawing.Size(493, 44)
        Me.grpFind.TabIndex = 1
        Me.grpFind.TabStop = False
        Me.grpFind.Text = "Find"
        '
        'cmbField
        '
        Me.cmbField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbField.FormattingEnabled = True
        Me.cmbField.Items.AddRange(New Object() {"All Fields", "Identifier", "Name/Label", "Brand", "Model", "Year", "Length", "Condition", "Noise Reduction", "Date Recorded", "Contents Type", "Artist", "Title", "Notes", "Index (Storage)"})
        Me.cmbField.Location = New System.Drawing.Point(313, 17)
        Me.cmbField.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbField.Name = "cmbField"
        Me.cmbField.Size = New System.Drawing.Size(128, 21)
        Me.cmbField.TabIndex = 2
        '
        'txtTerm
        '
        Me.txtTerm.Location = New System.Drawing.Point(5, 18)
        Me.txtTerm.Margin = New System.Windows.Forms.Padding(2)
        Me.txtTerm.Name = "txtTerm"
        Me.txtTerm.Size = New System.Drawing.Size(304, 20)
        Me.txtTerm.TabIndex = 1
        '
        'btnFind
        '
        Me.btnFind.Location = New System.Drawing.Point(445, 17)
        Me.btnFind.Margin = New System.Windows.Forms.Padding(2)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(44, 21)
        Me.btnFind.TabIndex = 3
        Me.btnFind.Text = "Find"
        Me.btnFind.UseVisualStyleBackColor = True
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnUpdate)
        Me.grpActions.Controls.Add(Me.lblAbout)
        Me.grpActions.Controls.Add(Me.btnSave)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Controls.Add(Me.btnAdd)
        Me.grpActions.Location = New System.Drawing.Point(508, 26)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2)
        Me.grpActions.Size = New System.Drawing.Size(231, 91)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnUpdate
        '
        Me.btnUpdate.Enabled = False
        Me.btnUpdate.Location = New System.Drawing.Point(5, 41)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(109, 21)
        Me.btnUpdate.TabIndex = 7
        Me.btnUpdate.Text = "Update Tape"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'lblAbout
        '
        Me.lblAbout.Location = New System.Drawing.Point(5, 67)
        Me.lblAbout.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAbout.Name = "lblAbout"
        Me.lblAbout.Size = New System.Drawing.Size(221, 15)
        Me.lblAbout.TabIndex = 6
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(117, 41)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(109, 21)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save to File"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(117, 17)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(109, 21)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete Tape"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(5, 17)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(2)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(109, 21)
        Me.btnAdd.TabIndex = 1
        Me.btnAdd.Text = "Add Tape"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'grpScroll
        '
        Me.grpScroll.Controls.Add(Me.btnNext)
        Me.grpScroll.Controls.Add(Me.btnPrevious)
        Me.grpScroll.Enabled = False
        Me.grpScroll.Location = New System.Drawing.Point(452, 74)
        Me.grpScroll.Margin = New System.Windows.Forms.Padding(2)
        Me.grpScroll.Name = "grpScroll"
        Me.grpScroll.Padding = New System.Windows.Forms.Padding(2)
        Me.grpScroll.Size = New System.Drawing.Size(52, 43)
        Me.grpScroll.TabIndex = 2
        Me.grpScroll.TabStop = False
        Me.grpScroll.Text = "Scroll"
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(27, 17)
        Me.btnNext.Margin = New System.Windows.Forms.Padding(2)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(21, 21)
        Me.btnNext.TabIndex = 2
        Me.btnNext.Text = ">"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'btnPrevious
        '
        Me.btnPrevious.Location = New System.Drawing.Point(4, 17)
        Me.btnPrevious.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPrevious.Name = "btnPrevious"
        Me.btnPrevious.Size = New System.Drawing.Size(21, 21)
        Me.btnPrevious.TabIndex = 1
        Me.btnPrevious.Text = "<"
        Me.btnPrevious.UseVisualStyleBackColor = True
        '
        'dlgOpen
        '
        Me.dlgOpen.DefaultExt = "xml"
        Me.dlgOpen.Filter = "C3 Catalogues (*.xml)|*.xml|All files (*.*)|*.*"
        Me.dlgOpen.FilterIndex = 0
        Me.dlgOpen.Title = "Open Catalogue"
        '
        'dlgSaveAs
        '
        Me.dlgSaveAs.DefaultExt = "xml"
        Me.dlgSaveAs.FileName = "New Catalogue"
        Me.dlgSaveAs.Filter = "C3 Catalogues (*.xml)|*.xml|All files (*.*)|*.*"
        Me.dlgSaveAs.FilterIndex = 0
        Me.dlgSaveAs.Title = "Save Catalogue As"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(750, 504)
        Me.Controls.Add(Me.grpScroll)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.grpIdentification)
        Me.Controls.Add(Me.grpFind)
        Me.Controls.Add(Me.grpData)
        Me.Controls.Add(Me.menuStripMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuStripMain
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.Text = "Compact Cassette Catalogue"
        Me.grpIdentification.ResumeLayout(False)
        Me.grpIdentification.PerformLayout()
        Me.menuStripMain.ResumeLayout(False)
        Me.menuStripMain.PerformLayout()
        Me.grpData.ResumeLayout(False)
        Me.grpSideB.ResumeLayout(False)
        Me.grpSideB.PerformLayout()
        CType(Me.numPeakB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpConfigB.ResumeLayout(False)
        Me.grpConfigB.PerformLayout()
        Me.grpContentsB.ResumeLayout(False)
        Me.grpContentsB.PerformLayout()
        CType(Me.numBiasCalB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLevelB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLevelCalB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTaped.ResumeLayout(False)
        Me.grpTaped.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.grpSideA.ResumeLayout(False)
        Me.grpSideA.PerformLayout()
        CType(Me.numPeakA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpConfigA.ResumeLayout(False)
        Me.grpConfigA.PerformLayout()
        Me.grpContentsA.ResumeLayout(False)
        Me.grpContentsA.PerformLayout()
        CType(Me.numBiasCalA, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLevelA, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLevelCalA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpModel.ResumeLayout(False)
        Me.grpModel.PerformLayout()
        CType(Me.numLength, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpFind.ResumeLayout(False)
        Me.grpFind.PerformLayout()
        Me.grpActions.ResumeLayout(False)
        Me.grpScroll.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents grpIdentification As GroupBox
    Friend WithEvents txtLong As TextBox
    Friend WithEvents txtShort As TextBox
    Friend WithEvents lblLong As Label
    Friend WithEvents lblShort As Label
    Friend WithEvents menuStripMain As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenCatalogueToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PreferencesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents NewTapeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewModelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewManufactererToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchTapesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchModelsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchManufacturersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents ViewStatisticsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents grpData As GroupBox
    Friend WithEvents grpFind As GroupBox
    Friend WithEvents txtTerm As TextBox
    Friend WithEvents btnFind As Button
    Friend WithEvents cmbField As ComboBox
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents grpScroll As GroupBox
    Friend WithEvents btnNext As Button
    Friend WithEvents btnPrevious As Button
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents lblRecordedA As Label
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents lblCondition As Label
    Friend WithEvents cmbCondition As ComboBox
    Friend WithEvents datRecordedA As DateTimePicker
    Friend WithEvents grpModel As GroupBox
    Friend WithEvents grpContentsA As GroupBox
    Friend WithEvents lblNRA As Label
    Friend WithEvents cmbNRA As ComboBox
    Friend WithEvents txtTotal As TextBox
    Friend WithEvents lblIndex As Label
    Friend WithEvents grpSideA As GroupBox
    Friend WithEvents lblNameA As Label
    Friend WithEvents txtNameA As TextBox
    Friend WithEvents lblTitleA As Label
    Friend WithEvents lblArtistA As Label
    Friend WithEvents txtTitleA As TextBox
    Friend WithEvents txtArtistA As TextBox
    Friend WithEvents lblContentsA As Label
    Friend WithEvents cmbContentsA As ComboBox
    Friend WithEvents chkTapedB As CheckBox
    Friend WithEvents chkTapedA As CheckBox
    Friend WithEvents lblLength As Label
    Friend WithEvents lblYear As Label
    Friend WithEvents lblModel As Label
    Friend WithEvents lblAbout As Label
    Friend WithEvents chkPackaged As CheckBox
    Friend WithEvents grpTaped As GroupBox
    Friend WithEvents lblSides As Label
    Friend WithEvents chkMPXA As CheckBox
    Friend WithEvents chkHXA As CheckBox
    Friend WithEvents numLevelA As NumericUpDown
    Friend WithEvents lblBiasCalA As Label
    Friend WithEvents numBiasCalA As NumericUpDown
    Friend WithEvents lblBiasA As Label
    Friend WithEvents cmbBiasA As ComboBox
    Friend WithEvents lblLevelCalA As Label
    Friend WithEvents numLevelCalA As NumericUpDown
    Friend WithEvents lblLevelA As Label
    Friend WithEvents cmbEQA As ComboBox
    Friend WithEvents lblEQA As Label
    Friend WithEvents cmbDeckA As ComboBox
    Friend WithEvents lblDeckA As Label
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents NewDeckToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewDecksToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents lblSpeed As Label
    Friend WithEvents cmbSpeedA As ComboBox
    Friend WithEvents chkDubbedA As CheckBox
    Friend WithEvents grpConfigA As GroupBox
    Friend WithEvents numPeakA As NumericUpDown
    Friend WithEvents lblPeakA As Label
    Friend WithEvents cmbInputA As ComboBox
    Friend WithEvents lblInputA As Label
    Friend WithEvents lblRegion As Label
    Friend WithEvents dlgOpen As OpenFileDialog
    Friend WithEvents dlgSaveAs As SaveFileDialog
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents txtIndex As TextBox
    Friend WithEvents lblMax As Label
    Friend WithEvents txtModel As TextBox
    Friend WithEvents ToolsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ShowConsoleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnUpdate As Button
    Friend WithEvents TutorialToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FeedbackToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cmbRegion As ComboBox
    Friend WithEvents numLength As NumericUpDown
    Friend WithEvents numYear As NumericUpDown
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents UpdateTapeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteTapeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OutputConsoleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblProcessingA As Label
    Friend WithEvents grpSideB As GroupBox
    Friend WithEvents numPeakB As NumericUpDown
    Friend WithEvents lblPeakB As Label
    Friend WithEvents cmbInputB As ComboBox
    Friend WithEvents lblInputB As Label
    Friend WithEvents grpConfigB As GroupBox
    Friend WithEvents lblProcessingB As Label
    Friend WithEvents chkDubbedB As CheckBox
    Friend WithEvents cmbNRB As ComboBox
    Friend WithEvents lblNRB As Label
    Friend WithEvents cmbSpeedB As ComboBox
    Friend WithEvents chkHXB As CheckBox
    Friend WithEvents chkMPXB As CheckBox
    Friend WithEvents lblSpeedB As Label
    Friend WithEvents grpContentsB As GroupBox
    Friend WithEvents lblTitleB As Label
    Friend WithEvents lblArtistB As Label
    Friend WithEvents txtTitleB As TextBox
    Friend WithEvents txtArtistB As TextBox
    Friend WithEvents lblContentsB As Label
    Friend WithEvents cmbContentsB As ComboBox
    Friend WithEvents cmbBiasB As ComboBox
    Friend WithEvents lblBiasB As Label
    Friend WithEvents cmbEQB As ComboBox
    Friend WithEvents numBiasCalB As NumericUpDown
    Friend WithEvents lblBiasCalB As Label
    Friend WithEvents lblEQB As Label
    Friend WithEvents numLevelB As NumericUpDown
    Friend WithEvents lblLevelB As Label
    Friend WithEvents numLevelCalB As NumericUpDown
    Friend WithEvents lblLevelCalB As Label
    Friend WithEvents lblNameB As Label
    Friend WithEvents txtNameB As TextBox
    Friend WithEvents datRecordedB As DateTimePicker
    Friend WithEvents lblRecordedB As Label
    Friend WithEvents cmbDeckB As ComboBox
    Friend WithEvents lblDeckB As Label
End Class
