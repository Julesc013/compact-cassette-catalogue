<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDeckNew
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDeckNew))
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.lblCondition = New System.Windows.Forms.Label()
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.txtManufacturer = New System.Windows.Forms.TextBox()
        Me.cmbCondition = New System.Windows.Forms.ComboBox()
        Me.numYear = New System.Windows.Forms.NumericUpDown()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.lblHeads = New System.Windows.Forms.Label()
        Me.cmbHeads = New System.Windows.Forms.ComboBox()
        Me.lblWells = New System.Windows.Forms.Label()
        Me.chkSpeedSlow = New System.Windows.Forms.CheckBox()
        Me.cmbWells = New System.Windows.Forms.ComboBox()
        Me.chkSpeedFast = New System.Windows.Forms.CheckBox()
        Me.lblSpeeds = New System.Windows.Forms.Label()
        Me.chkSpeedNormal = New System.Windows.Forms.CheckBox()
        Me.grpTransport = New System.Windows.Forms.GroupBox()
        Me.lblDolby = New System.Windows.Forms.Label()
        Me.chkDolbyB = New System.Windows.Forms.CheckBox()
        Me.chkDolbyS = New System.Windows.Forms.CheckBox()
        Me.chkDolbyC = New System.Windows.Forms.CheckBox()
        Me.lblDBX = New System.Windows.Forms.Label()
        Me.chkHX = New System.Windows.Forms.CheckBox()
        Me.chkDBX1 = New System.Windows.Forms.CheckBox()
        Me.chkDBX2 = New System.Windows.Forms.CheckBox()
        Me.chkMPX = New System.Windows.Forms.CheckBox()
        Me.grpNR = New System.Windows.Forms.GroupBox()
        Me.chkType1 = New System.Windows.Forms.CheckBox()
        Me.chkType2 = New System.Windows.Forms.CheckBox()
        Me.chkType3 = New System.Windows.Forms.CheckBox()
        Me.chkType4 = New System.Windows.Forms.CheckBox()
        Me.grpTypes = New System.Windows.Forms.GroupBox()
        Me.chkStereo = New System.Windows.Forms.CheckBox()
        Me.lblDubbing = New System.Windows.Forms.Label()
        Me.chkDubbingDouble = New System.Windows.Forms.CheckBox()
        Me.chkDubbingHalf = New System.Windows.Forms.CheckBox()
        Me.chkReverse = New System.Windows.Forms.CheckBox()
        Me.chkCalibration = New System.Windows.Forms.CheckBox()
        Me.chkSearch = New System.Windows.Forms.CheckBox()
        Me.chkAzimuth = New System.Windows.Forms.CheckBox()
        Me.grpFeatures = New System.Windows.Forms.GroupBox()
        Me.numFrequencyMin = New System.Windows.Forms.NumericUpDown()
        Me.lvlFrequency = New System.Windows.Forms.Label()
        Me.numFrequencyMax = New System.Windows.Forms.NumericUpDown()
        Me.lblFrequencyTo = New System.Windows.Forms.Label()
        Me.numSignalRatio = New System.Windows.Forms.NumericUpDown()
        Me.cmbSignalRatioNR = New System.Windows.Forms.ComboBox()
        Me.lblSignalRatio = New System.Windows.Forms.Label()
        Me.lblSignalRatioNR = New System.Windows.Forms.Label()
        Me.numWowFlutter = New System.Windows.Forms.NumericUpDown()
        Me.lblWowFlutter = New System.Windows.Forms.Label()
        Me.numDistortion = New System.Windows.Forms.NumericUpDown()
        Me.lblDistortion = New System.Windows.Forms.Label()
        Me.grpSpecifications = New System.Windows.Forms.GroupBox()
        Me.grpNotes.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTransport.SuspendLayout()
        Me.grpNR.SuspendLayout()
        Me.grpTypes.SuspendLayout()
        Me.grpFeatures.SuspendLayout()
        CType(Me.numFrequencyMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numFrequencyMax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numSignalRatio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numWowFlutter, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numDistortion, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpSpecifications.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(12, 356)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(170, 25)
        Me.btnAdd.TabIndex = 8
        Me.btnAdd.Text = "Add Deck"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(452, 292)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Size = New System.Drawing.Size(196, 89)
        Me.grpNotes.TabIndex = 7
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(6, 21)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(184, 62)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.lblModel)
        Me.grpBasic.Controls.Add(Me.lblManufacturer)
        Me.grpBasic.Controls.Add(Me.lblCondition)
        Me.grpBasic.Controls.Add(Me.txtModel)
        Me.grpBasic.Controls.Add(Me.txtManufacturer)
        Me.grpBasic.Controls.Add(Me.cmbCondition)
        Me.grpBasic.Controls.Add(Me.numYear)
        Me.grpBasic.Controls.Add(Me.lblYear)
        Me.grpBasic.Location = New System.Drawing.Point(12, 12)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Size = New System.Drawing.Size(276, 137)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(6, 52)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(91, 17)
        Me.lblModel.TabIndex = 24
        Me.lblModel.Text = "Model/Name:"
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Location = New System.Drawing.Point(6, 24)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(96, 17)
        Me.lblManufacturer.TabIndex = 23
        Me.lblManufacturer.Text = "Manufacturer:"
        '
        'lblCondition
        '
        Me.lblCondition.AutoSize = True
        Me.lblCondition.Location = New System.Drawing.Point(6, 108)
        Me.lblCondition.Name = "lblCondition"
        Me.lblCondition.Size = New System.Drawing.Size(71, 17)
        Me.lblCondition.TabIndex = 9
        Me.lblCondition.Text = "Condition:"
        '
        'txtModel
        '
        Me.txtModel.Location = New System.Drawing.Point(108, 49)
        Me.txtModel.MaxLength = 100
        Me.txtModel.Name = "txtModel"
        Me.txtModel.Size = New System.Drawing.Size(162, 22)
        Me.txtModel.TabIndex = 2
        '
        'txtManufacturer
        '
        Me.txtManufacturer.Location = New System.Drawing.Point(108, 21)
        Me.txtManufacturer.MaxLength = 100
        Me.txtManufacturer.Name = "txtManufacturer"
        Me.txtManufacturer.Size = New System.Drawing.Size(162, 22)
        Me.txtManufacturer.TabIndex = 1
        '
        'cmbCondition
        '
        Me.cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCondition.FormattingEnabled = True
        Me.cmbCondition.Items.AddRange(New Object() {"Mint", "Near Mint", "Very Good Plus", "Very Good", "Good Plus", "Good", "Fair", "Poor", "Broken"})
        Me.cmbCondition.Location = New System.Drawing.Point(108, 105)
        Me.cmbCondition.Name = "cmbCondition"
        Me.cmbCondition.Size = New System.Drawing.Size(162, 24)
        Me.cmbCondition.TabIndex = 4
        '
        'numYear
        '
        Me.numYear.Location = New System.Drawing.Point(108, 77)
        Me.numYear.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYear.Minimum = New Decimal(New Integer() {1963, 0, 0, 0})
        Me.numYear.Name = "numYear"
        Me.numYear.Size = New System.Drawing.Size(62, 22)
        Me.numYear.TabIndex = 3
        Me.numYear.Value = New Decimal(New Integer() {1980, 0, 0, 0})
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Location = New System.Drawing.Point(6, 79)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(42, 17)
        Me.lblYear.TabIndex = 17
        Me.lblYear.Text = "Year:"
        '
        'lblAdd
        '
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(187, 360)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(202, 17)
        Me.lblAdd.TabIndex = 36
        Me.lblAdd.Text = "Save new decks via main form."
        '
        'lblHeads
        '
        Me.lblHeads.AutoSize = True
        Me.lblHeads.Location = New System.Drawing.Point(6, 24)
        Me.lblHeads.Name = "lblHeads"
        Me.lblHeads.Size = New System.Drawing.Size(53, 17)
        Me.lblHeads.TabIndex = 33
        Me.lblHeads.Text = "Heads:"
        '
        'cmbHeads
        '
        Me.cmbHeads.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbHeads.FormattingEnabled = True
        Me.cmbHeads.Items.AddRange(New Object() {"One (No Rec.)", "Two", "Three"})
        Me.cmbHeads.Location = New System.Drawing.Point(65, 21)
        Me.cmbHeads.Name = "cmbHeads"
        Me.cmbHeads.Size = New System.Drawing.Size(125, 24)
        Me.cmbHeads.TabIndex = 1
        '
        'lblWells
        '
        Me.lblWells.AutoSize = True
        Me.lblWells.Location = New System.Drawing.Point(6, 54)
        Me.lblWells.Name = "lblWells"
        Me.lblWells.Size = New System.Drawing.Size(46, 17)
        Me.lblWells.TabIndex = 35
        Me.lblWells.Text = "Wells:"
        '
        'chkSpeedSlow
        '
        Me.chkSpeedSlow.AutoSize = True
        Me.chkSpeedSlow.Location = New System.Drawing.Point(9, 105)
        Me.chkSpeedSlow.Name = "chkSpeedSlow"
        Me.chkSpeedSlow.Size = New System.Drawing.Size(66, 21)
        Me.chkSpeedSlow.TabIndex = 41
        Me.chkSpeedSlow.Text = "15/16"
        Me.chkSpeedSlow.UseVisualStyleBackColor = True
        '
        'cmbWells
        '
        Me.cmbWells.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbWells.FormattingEnabled = True
        Me.cmbWells.Items.AddRange(New Object() {"One", "Two", "Three"})
        Me.cmbWells.Location = New System.Drawing.Point(65, 51)
        Me.cmbWells.Name = "cmbWells"
        Me.cmbWells.Size = New System.Drawing.Size(125, 24)
        Me.cmbWells.TabIndex = 2
        '
        'chkSpeedFast
        '
        Me.chkSpeedFast.AutoSize = True
        Me.chkSpeedFast.Location = New System.Drawing.Point(137, 105)
        Me.chkSpeedFast.Name = "chkSpeedFast"
        Me.chkSpeedFast.Size = New System.Drawing.Size(50, 21)
        Me.chkSpeedFast.TabIndex = 5
        Me.chkSpeedFast.Text = "3¾"
        Me.chkSpeedFast.UseVisualStyleBackColor = True
        '
        'lblSpeeds
        '
        Me.lblSpeeds.AutoSize = True
        Me.lblSpeeds.Location = New System.Drawing.Point(6, 85)
        Me.lblSpeeds.Name = "lblSpeeds"
        Me.lblSpeeds.Size = New System.Drawing.Size(95, 17)
        Me.lblSpeeds.TabIndex = 33
        Me.lblSpeeds.Text = "Speeds (IPS):"
        '
        'chkSpeedNormal
        '
        Me.chkSpeedNormal.AutoSize = True
        Me.chkSpeedNormal.Checked = True
        Me.chkSpeedNormal.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSpeedNormal.Location = New System.Drawing.Point(81, 105)
        Me.chkSpeedNormal.Name = "chkSpeedNormal"
        Me.chkSpeedNormal.Size = New System.Drawing.Size(50, 21)
        Me.chkSpeedNormal.TabIndex = 4
        Me.chkSpeedNormal.Text = "1⅞"
        Me.chkSpeedNormal.UseVisualStyleBackColor = True
        '
        'grpTransport
        '
        Me.grpTransport.Controls.Add(Me.chkSpeedNormal)
        Me.grpTransport.Controls.Add(Me.lblSpeeds)
        Me.grpTransport.Controls.Add(Me.chkSpeedFast)
        Me.grpTransport.Controls.Add(Me.cmbWells)
        Me.grpTransport.Controls.Add(Me.chkSpeedSlow)
        Me.grpTransport.Controls.Add(Me.lblWells)
        Me.grpTransport.Controls.Add(Me.cmbHeads)
        Me.grpTransport.Controls.Add(Me.lblHeads)
        Me.grpTransport.Location = New System.Drawing.Point(452, 155)
        Me.grpTransport.Name = "grpTransport"
        Me.grpTransport.Size = New System.Drawing.Size(196, 131)
        Me.grpTransport.TabIndex = 6
        Me.grpTransport.TabStop = False
        Me.grpTransport.Text = "Transport"
        '
        'lblDolby
        '
        Me.lblDolby.AutoSize = True
        Me.lblDolby.Location = New System.Drawing.Point(6, 44)
        Me.lblDolby.Name = "lblDolby"
        Me.lblDolby.Size = New System.Drawing.Size(48, 17)
        Me.lblDolby.TabIndex = 32
        Me.lblDolby.Text = "Dolby:"
        '
        'chkDolbyB
        '
        Me.chkDolbyB.AutoSize = True
        Me.chkDolbyB.Location = New System.Drawing.Point(9, 64)
        Me.chkDolbyB.Name = "chkDolbyB"
        Me.chkDolbyB.Size = New System.Drawing.Size(39, 21)
        Me.chkDolbyB.TabIndex = 3
        Me.chkDolbyB.Text = "B"
        Me.chkDolbyB.UseVisualStyleBackColor = True
        '
        'chkDolbyS
        '
        Me.chkDolbyS.AutoSize = True
        Me.chkDolbyS.Location = New System.Drawing.Point(107, 64)
        Me.chkDolbyS.Name = "chkDolbyS"
        Me.chkDolbyS.Size = New System.Drawing.Size(39, 21)
        Me.chkDolbyS.TabIndex = 5
        Me.chkDolbyS.Text = "S"
        Me.chkDolbyS.UseVisualStyleBackColor = True
        '
        'chkDolbyC
        '
        Me.chkDolbyC.AutoSize = True
        Me.chkDolbyC.Location = New System.Drawing.Point(58, 64)
        Me.chkDolbyC.Name = "chkDolbyC"
        Me.chkDolbyC.Size = New System.Drawing.Size(39, 21)
        Me.chkDolbyC.TabIndex = 4
        Me.chkDolbyC.Text = "C"
        Me.chkDolbyC.UseVisualStyleBackColor = True
        '
        'lblDBX
        '
        Me.lblDBX.AutoSize = True
        Me.lblDBX.Location = New System.Drawing.Point(6, 88)
        Me.lblDBX.Name = "lblDBX"
        Me.lblDBX.Size = New System.Drawing.Size(40, 17)
        Me.lblDBX.TabIndex = 36
        Me.lblDBX.Text = "DBX:"
        '
        'chkHX
        '
        Me.chkHX.AutoSize = True
        Me.chkHX.Location = New System.Drawing.Point(9, 20)
        Me.chkHX.Name = "chkHX"
        Me.chkHX.Size = New System.Drawing.Size(75, 21)
        Me.chkHX.TabIndex = 1
        Me.chkHX.Text = "HX Pro"
        Me.chkHX.UseVisualStyleBackColor = True
        '
        'chkDBX1
        '
        Me.chkDBX1.AutoSize = True
        Me.chkDBX1.Location = New System.Drawing.Point(9, 108)
        Me.chkDBX1.Name = "chkDBX1"
        Me.chkDBX1.Size = New System.Drawing.Size(69, 21)
        Me.chkDBX1.TabIndex = 6
        Me.chkDBX1.Text = "Type I"
        Me.chkDBX1.UseVisualStyleBackColor = True
        '
        'chkDBX2
        '
        Me.chkDBX2.AutoSize = True
        Me.chkDBX2.Location = New System.Drawing.Point(84, 108)
        Me.chkDBX2.Name = "chkDBX2"
        Me.chkDBX2.Size = New System.Drawing.Size(72, 21)
        Me.chkDBX2.TabIndex = 7
        Me.chkDBX2.Text = "Type II"
        Me.chkDBX2.UseVisualStyleBackColor = True
        '
        'chkMPX
        '
        Me.chkMPX.AutoSize = True
        Me.chkMPX.Location = New System.Drawing.Point(84, 20)
        Me.chkMPX.Name = "chkMPX"
        Me.chkMPX.Size = New System.Drawing.Size(59, 21)
        Me.chkMPX.TabIndex = 2
        Me.chkMPX.Text = "MPX"
        Me.chkMPX.UseVisualStyleBackColor = True
        '
        'grpNR
        '
        Me.grpNR.Controls.Add(Me.chkMPX)
        Me.grpNR.Controls.Add(Me.chkDBX2)
        Me.grpNR.Controls.Add(Me.chkDBX1)
        Me.grpNR.Controls.Add(Me.chkHX)
        Me.grpNR.Controls.Add(Me.lblDBX)
        Me.grpNR.Controls.Add(Me.chkDolbyC)
        Me.grpNR.Controls.Add(Me.chkDolbyS)
        Me.grpNR.Controls.Add(Me.chkDolbyB)
        Me.grpNR.Controls.Add(Me.lblDolby)
        Me.grpNR.Location = New System.Drawing.Point(488, 12)
        Me.grpNR.Name = "grpNR"
        Me.grpNR.Size = New System.Drawing.Size(160, 137)
        Me.grpNR.TabIndex = 3
        Me.grpNR.TabStop = False
        Me.grpNR.Text = "NR"
        '
        'chkType1
        '
        Me.chkType1.AutoSize = True
        Me.chkType1.Checked = True
        Me.chkType1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkType1.Location = New System.Drawing.Point(9, 23)
        Me.chkType1.Name = "chkType1"
        Me.chkType1.Size = New System.Drawing.Size(119, 21)
        Me.chkType1.TabIndex = 1
        Me.chkType1.Text = "Type I (Ferric)"
        Me.chkType1.UseVisualStyleBackColor = True
        '
        'chkType2
        '
        Me.chkType2.AutoSize = True
        Me.chkType2.Location = New System.Drawing.Point(9, 51)
        Me.chkType2.Name = "chkType2"
        Me.chkType2.Size = New System.Drawing.Size(135, 21)
        Me.chkType2.TabIndex = 2
        Me.chkType2.Text = "Type II (Chrome)"
        Me.chkType2.UseVisualStyleBackColor = True
        '
        'chkType3
        '
        Me.chkType3.AutoSize = True
        Me.chkType3.Location = New System.Drawing.Point(9, 78)
        Me.chkType3.Name = "chkType3"
        Me.chkType3.Size = New System.Drawing.Size(177, 21)
        Me.chkType3.TabIndex = 3
        Me.chkType3.Text = "Type III (Ferro-Chrome)"
        Me.chkType3.UseVisualStyleBackColor = True
        '
        'chkType4
        '
        Me.chkType4.AutoSize = True
        Me.chkType4.Location = New System.Drawing.Point(9, 107)
        Me.chkType4.Name = "chkType4"
        Me.chkType4.Size = New System.Drawing.Size(126, 21)
        Me.chkType4.TabIndex = 4
        Me.chkType4.Text = "Type IV (Metal)"
        Me.chkType4.UseVisualStyleBackColor = True
        '
        'grpTypes
        '
        Me.grpTypes.Controls.Add(Me.chkType4)
        Me.grpTypes.Controls.Add(Me.chkType3)
        Me.grpTypes.Controls.Add(Me.chkType2)
        Me.grpTypes.Controls.Add(Me.chkType1)
        Me.grpTypes.Location = New System.Drawing.Point(294, 12)
        Me.grpTypes.Name = "grpTypes"
        Me.grpTypes.Size = New System.Drawing.Size(188, 137)
        Me.grpTypes.TabIndex = 2
        Me.grpTypes.TabStop = False
        Me.grpTypes.Text = "Types"
        '
        'chkStereo
        '
        Me.chkStereo.AutoSize = True
        Me.chkStereo.Checked = True
        Me.chkStereo.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStereo.Location = New System.Drawing.Point(9, 19)
        Me.chkStereo.Name = "chkStereo"
        Me.chkStereo.Size = New System.Drawing.Size(72, 21)
        Me.chkStereo.TabIndex = 1
        Me.chkStereo.Text = "Stereo"
        Me.chkStereo.UseVisualStyleBackColor = True
        '
        'lblDubbing
        '
        Me.lblDubbing.AutoSize = True
        Me.lblDubbing.Location = New System.Drawing.Point(6, 151)
        Me.lblDubbing.Name = "lblDubbing"
        Me.lblDubbing.Size = New System.Drawing.Size(65, 17)
        Me.lblDubbing.TabIndex = 36
        Me.lblDubbing.Text = "Dubbing:"
        '
        'chkDubbingDouble
        '
        Me.chkDubbingDouble.AutoSize = True
        Me.chkDubbingDouble.Location = New System.Drawing.Point(9, 169)
        Me.chkDubbingDouble.Name = "chkDubbingDouble"
        Me.chkDubbingDouble.Size = New System.Drawing.Size(108, 21)
        Me.chkDubbingDouble.TabIndex = 6
        Me.chkDubbingDouble.Text = "Double/High"
        Me.chkDubbingDouble.UseVisualStyleBackColor = True
        '
        'chkDubbingHalf
        '
        Me.chkDubbingHalf.AutoSize = True
        Me.chkDubbingHalf.Location = New System.Drawing.Point(120, 169)
        Me.chkDubbingHalf.Name = "chkDubbingHalf"
        Me.chkDubbingHalf.Size = New System.Drawing.Size(55, 21)
        Me.chkDubbingHalf.TabIndex = 7
        Me.chkDubbingHalf.Text = "Half"
        Me.chkDubbingHalf.UseVisualStyleBackColor = True
        '
        'chkReverse
        '
        Me.chkReverse.AutoSize = True
        Me.chkReverse.Location = New System.Drawing.Point(9, 73)
        Me.chkReverse.Name = "chkReverse"
        Me.chkReverse.Size = New System.Drawing.Size(116, 21)
        Me.chkReverse.TabIndex = 3
        Me.chkReverse.Text = "Auto Reverse"
        Me.chkReverse.UseVisualStyleBackColor = True
        '
        'chkCalibration
        '
        Me.chkCalibration.AutoSize = True
        Me.chkCalibration.Location = New System.Drawing.Point(9, 100)
        Me.chkCalibration.Name = "chkCalibration"
        Me.chkCalibration.Size = New System.Drawing.Size(97, 21)
        Me.chkCalibration.TabIndex = 4
        Me.chkCalibration.Text = "Calibration"
        Me.chkCalibration.UseVisualStyleBackColor = True
        '
        'chkSearch
        '
        Me.chkSearch.AutoSize = True
        Me.chkSearch.Location = New System.Drawing.Point(9, 46)
        Me.chkSearch.Name = "chkSearch"
        Me.chkSearch.Size = New System.Drawing.Size(133, 21)
        Me.chkSearch.TabIndex = 2
        Me.chkSearch.Text = "Program Search"
        Me.chkSearch.UseVisualStyleBackColor = True
        '
        'chkAzimuth
        '
        Me.chkAzimuth.AutoSize = True
        Me.chkAzimuth.Location = New System.Drawing.Point(9, 127)
        Me.chkAzimuth.Name = "chkAzimuth"
        Me.chkAzimuth.Size = New System.Drawing.Size(149, 21)
        Me.chkAzimuth.TabIndex = 5
        Me.chkAzimuth.Text = "Azimuth Correction"
        Me.chkAzimuth.UseVisualStyleBackColor = True
        '
        'grpFeatures
        '
        Me.grpFeatures.Controls.Add(Me.chkAzimuth)
        Me.grpFeatures.Controls.Add(Me.chkSearch)
        Me.grpFeatures.Controls.Add(Me.chkCalibration)
        Me.grpFeatures.Controls.Add(Me.chkReverse)
        Me.grpFeatures.Controls.Add(Me.chkDubbingHalf)
        Me.grpFeatures.Controls.Add(Me.chkDubbingDouble)
        Me.grpFeatures.Controls.Add(Me.lblDubbing)
        Me.grpFeatures.Controls.Add(Me.chkStereo)
        Me.grpFeatures.Location = New System.Drawing.Point(12, 155)
        Me.grpFeatures.Name = "grpFeatures"
        Me.grpFeatures.Size = New System.Drawing.Size(179, 195)
        Me.grpFeatures.TabIndex = 4
        Me.grpFeatures.TabStop = False
        Me.grpFeatures.Text = "Features"
        '
        'numFrequencyMin
        '
        Me.numFrequencyMin.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.numFrequencyMin.Location = New System.Drawing.Point(190, 23)
        Me.numFrequencyMin.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
        Me.numFrequencyMin.Name = "numFrequencyMin"
        Me.numFrequencyMin.Size = New System.Drawing.Size(52, 22)
        Me.numFrequencyMin.TabIndex = 1
        Me.numFrequencyMin.Value = New Decimal(New Integer() {30, 0, 0, 0})
        '
        'lvlFrequency
        '
        Me.lvlFrequency.AutoSize = True
        Me.lvlFrequency.Location = New System.Drawing.Point(6, 25)
        Me.lvlFrequency.Name = "lvlFrequency"
        Me.lvlFrequency.Size = New System.Drawing.Size(178, 17)
        Me.lvlFrequency.TabIndex = 33
        Me.lvlFrequency.Text = "Frequency Response (Hz):"
        '
        'numFrequencyMax
        '
        Me.numFrequencyMax.DecimalPlaces = 1
        Me.numFrequencyMax.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.numFrequencyMax.Location = New System.Drawing.Point(190, 51)
        Me.numFrequencyMax.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.numFrequencyMax.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numFrequencyMax.Name = "numFrequencyMax"
        Me.numFrequencyMax.Size = New System.Drawing.Size(52, 22)
        Me.numFrequencyMax.TabIndex = 2
        Me.numFrequencyMax.Value = New Decimal(New Integer() {17, 0, 0, 0})
        '
        'lblFrequencyTo
        '
        Me.lblFrequencyTo.AutoSize = True
        Me.lblFrequencyTo.Location = New System.Drawing.Point(20, 53)
        Me.lblFrequencyTo.Name = "lblFrequencyTo"
        Me.lblFrequencyTo.Size = New System.Drawing.Size(70, 17)
        Me.lblFrequencyTo.TabIndex = 35
        Me.lblFrequencyTo.Text = "…to (kHz):"
        '
        'numSignalRatio
        '
        Me.numSignalRatio.Location = New System.Drawing.Point(190, 79)
        Me.numSignalRatio.Name = "numSignalRatio"
        Me.numSignalRatio.Size = New System.Drawing.Size(52, 22)
        Me.numSignalRatio.TabIndex = 3
        Me.numSignalRatio.Value = New Decimal(New Integer() {60, 0, 0, 0})
        '
        'cmbSignalRatioNR
        '
        Me.cmbSignalRatioNR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSignalRatioNR.FormattingEnabled = True
        Me.cmbSignalRatioNR.Items.AddRange(New Object() {"None", "Dolby B", "Dolby C", "Dolby S", "DBX I", "DBX II"})
        Me.cmbSignalRatioNR.Location = New System.Drawing.Point(138, 107)
        Me.cmbSignalRatioNR.Name = "cmbSignalRatioNR"
        Me.cmbSignalRatioNR.Size = New System.Drawing.Size(104, 24)
        Me.cmbSignalRatioNR.TabIndex = 4
        '
        'lblSignalRatio
        '
        Me.lblSignalRatio.AutoSize = True
        Me.lblSignalRatio.Location = New System.Drawing.Point(6, 81)
        Me.lblSignalRatio.Name = "lblSignalRatio"
        Me.lblSignalRatio.Size = New System.Drawing.Size(175, 17)
        Me.lblSignalRatio.TabIndex = 37
        Me.lblSignalRatio.Text = "Signal to Noise Ratio (dB):"
        '
        'lblSignalRatioNR
        '
        Me.lblSignalRatioNR.AutoSize = True
        Me.lblSignalRatioNR.Location = New System.Drawing.Point(20, 110)
        Me.lblSignalRatioNR.Name = "lblSignalRatioNR"
        Me.lblSignalRatioNR.Size = New System.Drawing.Size(68, 17)
        Me.lblSignalRatioNR.TabIndex = 38
        Me.lblSignalRatioNR.Text = "…with NR:"
        '
        'numWowFlutter
        '
        Me.numWowFlutter.DecimalPlaces = 2
        Me.numWowFlutter.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.numWowFlutter.Location = New System.Drawing.Point(178, 137)
        Me.numWowFlutter.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numWowFlutter.Name = "numWowFlutter"
        Me.numWowFlutter.Size = New System.Drawing.Size(64, 22)
        Me.numWowFlutter.TabIndex = 5
        Me.numWowFlutter.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'lblWowFlutter
        '
        Me.lblWowFlutter.AutoSize = True
        Me.lblWowFlutter.Location = New System.Drawing.Point(6, 139)
        Me.lblWowFlutter.Name = "lblWowFlutter"
        Me.lblWowFlutter.Size = New System.Drawing.Size(125, 17)
        Me.lblWowFlutter.TabIndex = 40
        Me.lblWowFlutter.Text = "Wow && Flutter (%):"
        '
        'numDistortion
        '
        Me.numDistortion.DecimalPlaces = 2
        Me.numDistortion.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.numDistortion.Location = New System.Drawing.Point(178, 165)
        Me.numDistortion.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numDistortion.Name = "numDistortion"
        Me.numDistortion.Size = New System.Drawing.Size(64, 22)
        Me.numDistortion.TabIndex = 6
        Me.numDistortion.Value = New Decimal(New Integer() {15, 0, 0, 65536})
        '
        'lblDistortion
        '
        Me.lblDistortion.AutoSize = True
        Me.lblDistortion.Location = New System.Drawing.Point(6, 167)
        Me.lblDistortion.Name = "lblDistortion"
        Me.lblDistortion.Size = New System.Drawing.Size(166, 17)
        Me.lblDistortion.TabIndex = 42
        Me.lblDistortion.Text = "Total Harmonic Dist. (%):"
        '
        'grpSpecifications
        '
        Me.grpSpecifications.Controls.Add(Me.lblDistortion)
        Me.grpSpecifications.Controls.Add(Me.numDistortion)
        Me.grpSpecifications.Controls.Add(Me.lblWowFlutter)
        Me.grpSpecifications.Controls.Add(Me.numWowFlutter)
        Me.grpSpecifications.Controls.Add(Me.lblSignalRatioNR)
        Me.grpSpecifications.Controls.Add(Me.lblSignalRatio)
        Me.grpSpecifications.Controls.Add(Me.cmbSignalRatioNR)
        Me.grpSpecifications.Controls.Add(Me.numSignalRatio)
        Me.grpSpecifications.Controls.Add(Me.lblFrequencyTo)
        Me.grpSpecifications.Controls.Add(Me.numFrequencyMax)
        Me.grpSpecifications.Controls.Add(Me.lvlFrequency)
        Me.grpSpecifications.Controls.Add(Me.numFrequencyMin)
        Me.grpSpecifications.Location = New System.Drawing.Point(197, 155)
        Me.grpSpecifications.Name = "grpSpecifications"
        Me.grpSpecifications.Size = New System.Drawing.Size(249, 195)
        Me.grpSpecifications.TabIndex = 5
        Me.grpSpecifications.TabStop = False
        Me.grpSpecifications.Text = "Specifications"
        '
        'frmDeckNew
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 393)
        Me.Controls.Add(Me.grpSpecifications)
        Me.Controls.Add(Me.lblAdd)
        Me.Controls.Add(Me.grpFeatures)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.grpNR)
        Me.Controls.Add(Me.grpTransport)
        Me.Controls.Add(Me.grpBasic)
        Me.Controls.Add(Me.grpTypes)
        Me.Controls.Add(Me.grpNotes)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmDeckNew"
        Me.Text = "Add New Deck"
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTransport.ResumeLayout(False)
        Me.grpTransport.PerformLayout()
        Me.grpNR.ResumeLayout(False)
        Me.grpNR.PerformLayout()
        Me.grpTypes.ResumeLayout(False)
        Me.grpTypes.PerformLayout()
        Me.grpFeatures.ResumeLayout(False)
        Me.grpFeatures.PerformLayout()
        CType(Me.numFrequencyMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numFrequencyMax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numSignalRatio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numWowFlutter, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numDistortion, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpSpecifications.ResumeLayout(False)
        Me.grpSpecifications.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnAdd As Button
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents lblModel As Label
    Friend WithEvents lblManufacturer As Label
    Friend WithEvents txtModel As TextBox
    Friend WithEvents txtManufacturer As TextBox
    Friend WithEvents lblAdd As Label
    Friend WithEvents lblCondition As Label
    Friend WithEvents cmbCondition As ComboBox
    Friend WithEvents numYear As NumericUpDown
    Friend WithEvents lblYear As Label
    Friend WithEvents lblHeads As Label
    Friend WithEvents cmbHeads As ComboBox
    Friend WithEvents lblWells As Label
    Friend WithEvents chkSpeedSlow As CheckBox
    Friend WithEvents cmbWells As ComboBox
    Friend WithEvents chkSpeedFast As CheckBox
    Friend WithEvents lblSpeeds As Label
    Friend WithEvents chkSpeedNormal As CheckBox
    Friend WithEvents grpTransport As GroupBox
    Friend WithEvents lblDolby As Label
    Friend WithEvents chkDolbyB As CheckBox
    Friend WithEvents chkDolbyS As CheckBox
    Friend WithEvents chkDolbyC As CheckBox
    Friend WithEvents lblDBX As Label
    Friend WithEvents chkHX As CheckBox
    Friend WithEvents chkDBX1 As CheckBox
    Friend WithEvents chkDBX2 As CheckBox
    Friend WithEvents chkMPX As CheckBox
    Friend WithEvents grpNR As GroupBox
    Friend WithEvents chkType1 As CheckBox
    Friend WithEvents chkType2 As CheckBox
    Friend WithEvents chkType3 As CheckBox
    Friend WithEvents chkType4 As CheckBox
    Friend WithEvents grpTypes As GroupBox
    Friend WithEvents chkStereo As CheckBox
    Friend WithEvents lblDubbing As Label
    Friend WithEvents chkDubbingDouble As CheckBox
    Friend WithEvents chkDubbingHalf As CheckBox
    Friend WithEvents chkReverse As CheckBox
    Friend WithEvents chkCalibration As CheckBox
    Friend WithEvents chkSearch As CheckBox
    Friend WithEvents chkAzimuth As CheckBox
    Friend WithEvents grpFeatures As GroupBox
    Friend WithEvents numFrequencyMin As NumericUpDown
    Friend WithEvents lvlFrequency As Label
    Friend WithEvents numFrequencyMax As NumericUpDown
    Friend WithEvents lblFrequencyTo As Label
    Friend WithEvents numSignalRatio As NumericUpDown
    Friend WithEvents cmbSignalRatioNR As ComboBox
    Friend WithEvents lblSignalRatio As Label
    Friend WithEvents lblSignalRatioNR As Label
    Friend WithEvents numWowFlutter As NumericUpDown
    Friend WithEvents lblWowFlutter As Label
    Friend WithEvents numDistortion As NumericUpDown
    Friend WithEvents lblDistortion As Label
    Friend WithEvents grpSpecifications As GroupBox
End Class
