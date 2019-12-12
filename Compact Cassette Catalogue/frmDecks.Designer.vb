<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDecks
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDecks))
        Me.lblResults = New System.Windows.Forms.Label()
        Me.grpTapes = New System.Windows.Forms.GroupBox()
        Me.lstDecks = New System.Windows.Forms.ListView()
        Me.colManufacturer = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDeck = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colTypes = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colHeads = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSpeeds = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNRs = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colHX = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colMPX = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colStereo = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colWells = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDubbing = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAutoReverse = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colProgramSearch = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCalibration = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAzimuth = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFrequency = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSignalRatio = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colWowFlutter = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDistortion = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCondition = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.txtResults = New System.Windows.Forms.TextBox()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.numFrequencyMax = New System.Windows.Forms.NumericUpDown()
        Me.chkCalibration = New System.Windows.Forms.CheckBox()
        Me.chkMPX = New System.Windows.Forms.CheckBox()
        Me.chkHX = New System.Windows.Forms.CheckBox()
        Me.cmbTypes = New System.Windows.Forms.ComboBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.lblNR = New System.Windows.Forms.Label()
        Me.cmbNR = New System.Windows.Forms.ComboBox()
        Me.txtManufacturer = New System.Windows.Forms.TextBox()
        Me.lblFrequencyTo = New System.Windows.Forms.Label()
        Me.lblFrequency = New System.Windows.Forms.Label()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.grpTapes.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        CType(Me.numFrequencyMax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(11, 216)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 33
        Me.lblResults.Text = "Results (Filtered):"
        '
        'grpTapes
        '
        Me.grpTapes.Controls.Add(Me.lstDecks)
        Me.grpTapes.Location = New System.Drawing.Point(219, 11)
        Me.grpTapes.Margin = New System.Windows.Forms.Padding(2)
        Me.grpTapes.Name = "grpTapes"
        Me.grpTapes.Padding = New System.Windows.Forms.Padding(2)
        Me.grpTapes.Size = New System.Drawing.Size(946, 310)
        Me.grpTapes.TabIndex = 2
        Me.grpTapes.TabStop = False
        Me.grpTapes.Text = "Tapes"
        '
        'lstDecks
        '
        Me.lstDecks.AllowColumnReorder = True
        Me.lstDecks.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colManufacturer, Me.colDeck, Me.ColumnHeader7, Me.colTypes, Me.colHeads, Me.colSpeeds, Me.colNRs, Me.colHX, Me.colMPX, Me.colStereo, Me.colWells, Me.colDubbing, Me.colAutoReverse, Me.colProgramSearch, Me.colCalibration, Me.colAzimuth, Me.colFrequency, Me.colSignalRatio, Me.colWowFlutter, Me.colDistortion, Me.colCondition})
        Me.lstDecks.HideSelection = False
        Me.lstDecks.Location = New System.Drawing.Point(5, 17)
        Me.lstDecks.Margin = New System.Windows.Forms.Padding(2)
        Me.lstDecks.Name = "lstDecks"
        Me.lstDecks.Size = New System.Drawing.Size(936, 287)
        Me.lstDecks.TabIndex = 46
        Me.lstDecks.UseCompatibleStateImageBehavior = False
        Me.lstDecks.View = System.Windows.Forms.View.Details
        '
        'colManufacturer
        '
        Me.colManufacturer.Text = "Manufacturer"
        Me.colManufacturer.Width = 95
        '
        'colDeck
        '
        Me.colDeck.Text = "Model"
        Me.colDeck.Width = 71
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Year"
        Me.ColumnHeader7.Width = 47
        '
        'colTypes
        '
        Me.colTypes.Text = "Types"
        Me.colTypes.Width = 55
        '
        'colHeads
        '
        Me.colHeads.Text = "Heads"
        Me.colHeads.Width = 54
        '
        'colSpeeds
        '
        Me.colSpeeds.Text = "Speeds"
        '
        'colNRs
        '
        Me.colNRs.Text = "NRs"
        Me.colNRs.Width = 54
        '
        'colHX
        '
        Me.colHX.Text = "HX"
        Me.colHX.Width = 35
        '
        'colMPX
        '
        Me.colMPX.Text = "MPX"
        Me.colMPX.Width = 40
        '
        'colStereo
        '
        Me.colStereo.Text = "Stereo"
        Me.colStereo.Width = 54
        '
        'colWells
        '
        Me.colWells.Text = "Wells"
        Me.colWells.Width = 47
        '
        'colDubbing
        '
        Me.colDubbing.Text = "Dubbing"
        Me.colDubbing.Width = 64
        '
        'colAutoReverse
        '
        Me.colAutoReverse.Text = "Reverse"
        Me.colAutoReverse.Width = 65
        '
        'colProgramSearch
        '
        Me.colProgramSearch.Text = "Search Program"
        Me.colProgramSearch.Width = 64
        '
        'colCalibration
        '
        Me.colCalibration.Text = "Calibration"
        Me.colCalibration.Width = 57
        '
        'colAzimuth
        '
        Me.colAzimuth.Text = "Azimuth Correction"
        Me.colAzimuth.Width = 69
        '
        'colFrequency
        '
        Me.colFrequency.Text = "Frequency Response"
        Me.colFrequency.Width = 87
        '
        'colSignalRatio
        '
        Me.colSignalRatio.Text = "Signal Noise Ratio"
        '
        'colWowFlutter
        '
        Me.colWowFlutter.Text = "Wow & Flutter"
        Me.colWowFlutter.Width = 52
        '
        'colDistortion
        '
        Me.colDistortion.Text = "Harmonic Distortion"
        Me.colDistortion.Width = 57
        '
        'colCondition
        '
        Me.colCondition.Text = "Condition"
        Me.colCondition.Width = 54
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnEdit)
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Location = New System.Drawing.Point(11, 237)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2)
        Me.grpActions.Size = New System.Drawing.Size(204, 84)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnEdit
        '
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(5, 42)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(95, 21)
        Me.btnEdit.TabIndex = 14
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(5, 17)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(194, 21)
        Me.btnRefresh.TabIndex = 1
        Me.btnRefresh.Text = "Refresh List"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 65)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(144, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Save changes via main form."
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(104, 42)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(95, 21)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'txtResults
        '
        Me.txtResults.Location = New System.Drawing.Point(115, 213)
        Me.txtResults.Margin = New System.Windows.Forms.Padding(2)
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ReadOnly = True
        Me.txtResults.Size = New System.Drawing.Size(100, 20)
        Me.txtResults.TabIndex = 4
        Me.txtResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.numFrequencyMax)
        Me.grpBasic.Controls.Add(Me.chkCalibration)
        Me.grpBasic.Controls.Add(Me.chkMPX)
        Me.grpBasic.Controls.Add(Me.chkHX)
        Me.grpBasic.Controls.Add(Me.cmbTypes)
        Me.grpBasic.Controls.Add(Me.lblType)
        Me.grpBasic.Controls.Add(Me.lblNR)
        Me.grpBasic.Controls.Add(Me.cmbNR)
        Me.grpBasic.Controls.Add(Me.txtManufacturer)
        Me.grpBasic.Controls.Add(Me.lblFrequencyTo)
        Me.grpBasic.Controls.Add(Me.lblFrequency)
        Me.grpBasic.Controls.Add(Me.lblManufacturer)
        Me.grpBasic.Location = New System.Drawing.Point(5, 16)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Size = New System.Drawing.Size(194, 161)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'numFrequencyMax
        '
        Me.numFrequencyMax.DecimalPlaces = 1
        Me.numFrequencyMax.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.numFrequencyMax.Location = New System.Drawing.Point(70, 41)
        Me.numFrequencyMax.Margin = New System.Windows.Forms.Padding(2)
        Me.numFrequencyMax.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.numFrequencyMax.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numFrequencyMax.Name = "numFrequencyMax"
        Me.numFrequencyMax.Size = New System.Drawing.Size(55, 20)
        Me.numFrequencyMax.TabIndex = 20
        Me.numFrequencyMax.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'chkCalibration
        '
        Me.chkCalibration.AutoSize = True
        Me.chkCalibration.Location = New System.Drawing.Point(70, 138)
        Me.chkCalibration.Margin = New System.Windows.Forms.Padding(2)
        Me.chkCalibration.Name = "chkCalibration"
        Me.chkCalibration.Size = New System.Drawing.Size(103, 17)
        Me.chkCalibration.TabIndex = 18
        Me.chkCalibration.Text = "Tape Calibration"
        Me.chkCalibration.UseVisualStyleBackColor = True
        '
        'chkMPX
        '
        Me.chkMPX.AutoSize = True
        Me.chkMPX.Location = New System.Drawing.Point(140, 91)
        Me.chkMPX.Margin = New System.Windows.Forms.Padding(2)
        Me.chkMPX.Name = "chkMPX"
        Me.chkMPX.Size = New System.Drawing.Size(49, 17)
        Me.chkMPX.TabIndex = 17
        Me.chkMPX.Text = "MPX"
        Me.chkMPX.UseVisualStyleBackColor = True
        '
        'chkHX
        '
        Me.chkHX.AutoSize = True
        Me.chkHX.Location = New System.Drawing.Point(70, 91)
        Me.chkHX.Margin = New System.Windows.Forms.Padding(2)
        Me.chkHX.Name = "chkHX"
        Me.chkHX.Size = New System.Drawing.Size(60, 17)
        Me.chkHX.TabIndex = 16
        Me.chkHX.Text = "HX Pro"
        Me.chkHX.UseVisualStyleBackColor = True
        '
        'cmbTypes
        '
        Me.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTypes.FormattingEnabled = True
        Me.cmbTypes.Items.AddRange(New Object() {"All Types", "Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbTypes.Location = New System.Drawing.Point(70, 111)
        Me.cmbTypes.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbTypes.Name = "cmbTypes"
        Me.cmbTypes.Size = New System.Drawing.Size(119, 21)
        Me.cmbTypes.TabIndex = 14
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 114)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 15
        Me.lblType.Text = "Type:"
        '
        'lblNR
        '
        Me.lblNR.AutoSize = True
        Me.lblNR.Location = New System.Drawing.Point(4, 68)
        Me.lblNR.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNR.Name = "lblNR"
        Me.lblNR.Size = New System.Drawing.Size(63, 13)
        Me.lblNR.TabIndex = 13
        Me.lblNR.Text = "Noise Red.:"
        '
        'cmbNR
        '
        Me.cmbNR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNR.FormattingEnabled = True
        Me.cmbNR.Items.AddRange(New Object() {"All NRs", "None", "Dolby B", "Dolby C", "Dolby S", "DBX I", "DBX II"})
        Me.cmbNR.Location = New System.Drawing.Point(70, 65)
        Me.cmbNR.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbNR.Name = "cmbNR"
        Me.cmbNR.Size = New System.Drawing.Size(119, 21)
        Me.cmbNR.TabIndex = 12
        '
        'txtManufacturer
        '
        Me.txtManufacturer.Location = New System.Drawing.Point(70, 17)
        Me.txtManufacturer.Margin = New System.Windows.Forms.Padding(2)
        Me.txtManufacturer.MaxLength = 100
        Me.txtManufacturer.Name = "txtManufacturer"
        Me.txtManufacturer.Size = New System.Drawing.Size(119, 20)
        Me.txtManufacturer.TabIndex = 1
        '
        'lblFrequencyTo
        '
        Me.lblFrequencyTo.AutoSize = True
        Me.lblFrequencyTo.Location = New System.Drawing.Point(129, 43)
        Me.lblFrequencyTo.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblFrequencyTo.Name = "lblFrequencyTo"
        Me.lblFrequencyTo.Size = New System.Drawing.Size(29, 13)
        Me.lblFrequencyTo.TabIndex = 11
        Me.lblFrequencyTo.Text = "kHz."
        '
        'lblFrequency
        '
        Me.lblFrequency.AutoSize = True
        Me.lblFrequency.Location = New System.Drawing.Point(4, 43)
        Me.lblFrequency.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblFrequency.Name = "lblFrequency"
        Me.lblFrequency.Size = New System.Drawing.Size(60, 13)
        Me.lblFrequency.TabIndex = 2
        Me.lblFrequency.Text = "Frequency:"
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Location = New System.Drawing.Point(4, 20)
        Me.lblManufacturer.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(58, 13)
        Me.lblManufacturer.TabIndex = 1
        Me.lblManufacturer.Text = "Manufact.:"
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.grpBasic)
        Me.grpFilters.Location = New System.Drawing.Point(11, 11)
        Me.grpFilters.Margin = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Padding = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Size = New System.Drawing.Size(204, 182)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'frmDecks
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1177, 331)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.grpTapes)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.txtResults)
        Me.Controls.Add(Me.grpFilters)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.Name = "frmDecks"
        Me.Text = "View Decks"
        Me.grpTapes.ResumeLayout(False)
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        CType(Me.numFrequencyMax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpFilters.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblResults As Label
    Friend WithEvents grpTapes As GroupBox
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnDelete As Button
    Friend WithEvents txtResults As TextBox
    Friend WithEvents grpBasic As GroupBox
    Private WithEvents lblFrequencyTo As Label
    Private WithEvents lblFrequency As Label
    Private WithEvents lblManufacturer As Label
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents txtManufacturer As TextBox
    Friend WithEvents btnEdit As Button
    Friend WithEvents lstDecks As ListView
    Friend WithEvents colManufacturer As ColumnHeader
    Friend WithEvents colDeck As ColumnHeader
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents colTypes As ColumnHeader
    Friend WithEvents colHeads As ColumnHeader
    Friend WithEvents colSpeeds As ColumnHeader
    Friend WithEvents colNRs As ColumnHeader
    Friend WithEvents colHX As ColumnHeader
    Friend WithEvents colMPX As ColumnHeader
    Friend WithEvents colStereo As ColumnHeader
    Friend WithEvents colWells As ColumnHeader
    Friend WithEvents colDubbing As ColumnHeader
    Friend WithEvents colAutoReverse As ColumnHeader
    Friend WithEvents colProgramSearch As ColumnHeader
    Friend WithEvents colCalibration As ColumnHeader
    Friend WithEvents colAzimuth As ColumnHeader
    Friend WithEvents colFrequency As ColumnHeader
    Friend WithEvents colSignalRatio As ColumnHeader
    Friend WithEvents colWowFlutter As ColumnHeader
    Friend WithEvents colDistortion As ColumnHeader
    Friend WithEvents colCondition As ColumnHeader
    Friend WithEvents cmbNR As ComboBox
    Private WithEvents lblNR As Label
    Friend WithEvents cmbTypes As ComboBox
    Private WithEvents lblType As Label
    Friend WithEvents chkHX As CheckBox
    Friend WithEvents chkMPX As CheckBox
    Friend WithEvents chkCalibration As CheckBox
    Friend WithEvents numFrequencyMax As NumericUpDown
End Class
