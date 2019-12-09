<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmTapes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTapes))
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.grpContents = New System.Windows.Forms.GroupBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblArtist = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.txtArtist = New System.Windows.Forms.TextBox()
        Me.lblContents = New System.Windows.Forms.Label()
        Me.cmbContents = New System.Windows.Forms.ComboBox()
        Me.grpTape = New System.Windows.Forms.GroupBox()
        Me.cmbDeck = New System.Windows.Forms.ComboBox()
        Me.lblDeck = New System.Windows.Forms.Label()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.lblRecordedTo = New System.Windows.Forms.Label()
        Me.datRecordedMin = New System.Windows.Forms.DateTimePicker()
        Me.cmbCondition = New System.Windows.Forms.ComboBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.datRecordedMax = New System.Windows.Forms.DateTimePicker()
        Me.lblRecorded = New System.Windows.Forms.Label()
        Me.cmbNR = New System.Windows.Forms.ComboBox()
        Me.lblNR = New System.Windows.Forms.Label()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblCondition = New System.Windows.Forms.Label()
        Me.grpModel = New System.Windows.Forms.GroupBox()
        Me.chkTypeBetter = New System.Windows.Forms.CheckBox()
        Me.numLengthMin = New System.Windows.Forms.NumericUpDown()
        Me.numYearMin = New System.Windows.Forms.NumericUpDown()
        Me.numLengthMax = New System.Windows.Forms.NumericUpDown()
        Me.numYearMax = New System.Windows.Forms.NumericUpDown()
        Me.lblYearTo = New System.Windows.Forms.Label()
        Me.lblLengthTo = New System.Windows.Forms.Label()
        Me.cmbTypes = New System.Windows.Forms.ComboBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.cmbBrand = New System.Windows.Forms.ComboBox()
        Me.lblLength = New System.Windows.Forms.Label()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.grpTapes = New System.Windows.Forms.GroupBox()
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.txtResults = New System.Windows.Forms.TextBox()
        Me.lstTapes = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colYear = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colLength = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCondition = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colRecorded = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colContents = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNoise = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAlbum = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colTitle = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.grpFilters.SuspendLayout()
        Me.grpContents.SuspendLayout()
        Me.grpTape.SuspendLayout()
        Me.grpModel.SuspendLayout()
        CType(Me.numLengthMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numYearMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLengthMax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numYearMax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTapes.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.grpContents)
        Me.grpFilters.Controls.Add(Me.grpTape)
        Me.grpFilters.Controls.Add(Me.grpModel)
        Me.grpFilters.Location = New System.Drawing.Point(11, 11)
        Me.grpFilters.Margin = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Padding = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Size = New System.Drawing.Size(187, 500)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'grpContents
        '
        Me.grpContents.Controls.Add(Me.lblTitle)
        Me.grpContents.Controls.Add(Me.lblArtist)
        Me.grpContents.Controls.Add(Me.txtTitle)
        Me.grpContents.Controls.Add(Me.txtArtist)
        Me.grpContents.Controls.Add(Me.lblContents)
        Me.grpContents.Controls.Add(Me.cmbContents)
        Me.grpContents.Location = New System.Drawing.Point(5, 403)
        Me.grpContents.Margin = New System.Windows.Forms.Padding(2)
        Me.grpContents.Name = "grpContents"
        Me.grpContents.Padding = New System.Windows.Forms.Padding(2)
        Me.grpContents.Size = New System.Drawing.Size(177, 92)
        Me.grpContents.TabIndex = 3
        Me.grpContents.TabStop = False
        Me.grpContents.Text = "Contents"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Location = New System.Drawing.Point(4, 69)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(30, 13)
        Me.lblTitle.TabIndex = 26
        Me.lblTitle.Text = "Title:"
        '
        'lblArtist
        '
        Me.lblArtist.AutoSize = True
        Me.lblArtist.Location = New System.Drawing.Point(4, 45)
        Me.lblArtist.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblArtist.Name = "lblArtist"
        Me.lblArtist.Size = New System.Drawing.Size(33, 13)
        Me.lblArtist.TabIndex = 25
        Me.lblArtist.Text = "Artist:"
        '
        'txtTitle
        '
        Me.txtTitle.Enabled = False
        Me.txtTitle.Location = New System.Drawing.Point(51, 66)
        Me.txtTitle.Margin = New System.Windows.Forms.Padding(2)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(121, 20)
        Me.txtTitle.TabIndex = 3
        '
        'txtArtist
        '
        Me.txtArtist.Enabled = False
        Me.txtArtist.Location = New System.Drawing.Point(51, 42)
        Me.txtArtist.Margin = New System.Windows.Forms.Padding(2)
        Me.txtArtist.Name = "txtArtist"
        Me.txtArtist.Size = New System.Drawing.Size(121, 20)
        Me.txtArtist.TabIndex = 2
        '
        'lblContents
        '
        Me.lblContents.AutoSize = True
        Me.lblContents.Location = New System.Drawing.Point(4, 20)
        Me.lblContents.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblContents.Name = "lblContents"
        Me.lblContents.Size = New System.Drawing.Size(34, 13)
        Me.lblContents.TabIndex = 22
        Me.lblContents.Text = "Type:"
        '
        'cmbContents
        '
        Me.cmbContents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbContents.FormattingEnabled = True
        Me.cmbContents.Items.AddRange(New Object() {"All Content Types", "Mixtape", "Album", "Compilation", "Soundtrack", "EP", "Single"})
        Me.cmbContents.Location = New System.Drawing.Point(51, 17)
        Me.cmbContents.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbContents.Name = "cmbContents"
        Me.cmbContents.Size = New System.Drawing.Size(121, 21)
        Me.cmbContents.TabIndex = 1
        '
        'grpTape
        '
        Me.grpTape.Controls.Add(Me.cmbDeck)
        Me.grpTape.Controls.Add(Me.lblDeck)
        Me.grpTape.Controls.Add(Me.CheckBox1)
        Me.grpTape.Controls.Add(Me.lblRecordedTo)
        Me.grpTape.Controls.Add(Me.datRecordedMin)
        Me.grpTape.Controls.Add(Me.cmbCondition)
        Me.grpTape.Controls.Add(Me.TextBox1)
        Me.grpTape.Controls.Add(Me.lblNotes)
        Me.grpTape.Controls.Add(Me.datRecordedMax)
        Me.grpTape.Controls.Add(Me.lblRecorded)
        Me.grpTape.Controls.Add(Me.cmbNR)
        Me.grpTape.Controls.Add(Me.lblNR)
        Me.grpTape.Controls.Add(Me.lblName)
        Me.grpTape.Controls.Add(Me.txtName)
        Me.grpTape.Controls.Add(Me.lblCondition)
        Me.grpTape.Location = New System.Drawing.Point(5, 186)
        Me.grpTape.Margin = New System.Windows.Forms.Padding(2)
        Me.grpTape.Name = "grpTape"
        Me.grpTape.Padding = New System.Windows.Forms.Padding(2)
        Me.grpTape.Size = New System.Drawing.Size(177, 213)
        Me.grpTape.TabIndex = 2
        Me.grpTape.TabStop = False
        Me.grpTape.Text = "Tape"
        '
        'cmbDeck
        '
        Me.cmbDeck.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDeck.FormattingEnabled = True
        Me.cmbDeck.Items.AddRange(New Object() {"All Decks"})
        Me.cmbDeck.Location = New System.Drawing.Point(49, 89)
        Me.cmbDeck.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbDeck.Name = "cmbDeck"
        Me.cmbDeck.Size = New System.Drawing.Size(123, 21)
        Me.cmbDeck.TabIndex = 4
        '
        'lblDeck
        '
        Me.lblDeck.AutoSize = True
        Me.lblDeck.Location = New System.Drawing.Point(4, 92)
        Me.lblDeck.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDeck.Name = "lblDeck"
        Me.lblDeck.Size = New System.Drawing.Size(36, 13)
        Me.lblDeck.TabIndex = 28
        Me.lblDeck.Text = "Deck:"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Enabled = False
        Me.CheckBox1.Location = New System.Drawing.Point(49, 68)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(93, 17)
        Me.CheckBox1.TabIndex = 3
        Me.CheckBox1.Text = "Poor or better."
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'lblRecordedTo
        '
        Me.lblRecordedTo.AutoSize = True
        Me.lblRecordedTo.Location = New System.Drawing.Point(9, 144)
        Me.lblRecordedTo.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRecordedTo.Name = "lblRecordedTo"
        Me.lblRecordedTo.Size = New System.Drawing.Size(28, 13)
        Me.lblRecordedTo.TabIndex = 27
        Me.lblRecordedTo.Text = "...to:"
        '
        'datRecordedMin
        '
        Me.datRecordedMin.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.datRecordedMin.Location = New System.Drawing.Point(49, 114)
        Me.datRecordedMin.Margin = New System.Windows.Forms.Padding(2)
        Me.datRecordedMin.MinDate = New Date(1962, 1, 1, 0, 0, 0, 0)
        Me.datRecordedMin.Name = "datRecordedMin"
        Me.datRecordedMin.Size = New System.Drawing.Size(123, 20)
        Me.datRecordedMin.TabIndex = 5
        Me.datRecordedMin.Value = New Date(2019, 1, 1, 0, 0, 0, 0)
        '
        'cmbCondition
        '
        Me.cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCondition.FormattingEnabled = True
        Me.cmbCondition.Items.AddRange(New Object() {"All Conditions", "Mint", "Near Mint", "Very Good Plus", "Very Good", "Good Plus", "Good", "Fair", "Poor", "Broken"})
        Me.cmbCondition.Location = New System.Drawing.Point(49, 41)
        Me.cmbCondition.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbCondition.Name = "cmbCondition"
        Me.cmbCondition.Size = New System.Drawing.Size(123, 21)
        Me.cmbCondition.TabIndex = 2
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(49, 187)
        Me.TextBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(123, 20)
        Me.TextBox1.TabIndex = 9
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(4, 190)
        Me.lblNotes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(38, 13)
        Me.lblNotes.TabIndex = 8
        Me.lblNotes.Text = "Notes:"
        '
        'datRecordedMax
        '
        Me.datRecordedMax.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.datRecordedMax.Location = New System.Drawing.Point(49, 138)
        Me.datRecordedMax.Margin = New System.Windows.Forms.Padding(2)
        Me.datRecordedMax.MinDate = New Date(1962, 1, 1, 0, 0, 0, 0)
        Me.datRecordedMax.Name = "datRecordedMax"
        Me.datRecordedMax.Size = New System.Drawing.Size(123, 20)
        Me.datRecordedMax.TabIndex = 6
        Me.datRecordedMax.Value = New Date(2019, 1, 1, 0, 0, 0, 0)
        '
        'lblRecorded
        '
        Me.lblRecorded.AutoSize = True
        Me.lblRecorded.Location = New System.Drawing.Point(4, 120)
        Me.lblRecorded.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRecorded.Name = "lblRecorded"
        Me.lblRecorded.Size = New System.Drawing.Size(33, 13)
        Me.lblRecorded.TabIndex = 13
        Me.lblRecorded.Text = "Rec.:"
        '
        'cmbNR
        '
        Me.cmbNR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNR.FormattingEnabled = True
        Me.cmbNR.Items.AddRange(New Object() {"All NRs", "None", "Dolby B", "Dolby C", "Dolby S", "DBX I", "DBX II"})
        Me.cmbNR.Location = New System.Drawing.Point(49, 162)
        Me.cmbNR.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbNR.Name = "cmbNR"
        Me.cmbNR.Size = New System.Drawing.Size(123, 21)
        Me.cmbNR.TabIndex = 7
        '
        'lblNR
        '
        Me.lblNR.AutoSize = True
        Me.lblNR.Location = New System.Drawing.Point(4, 165)
        Me.lblNR.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNR.Name = "lblNR"
        Me.lblNR.Size = New System.Drawing.Size(26, 13)
        Me.lblNR.TabIndex = 10
        Me.lblNR.Text = "NR:"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(4, 20)
        Me.lblName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 20
        Me.lblName.Text = "Name:"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(49, 17)
        Me.txtName.Margin = New System.Windows.Forms.Padding(2)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(123, 20)
        Me.txtName.TabIndex = 1
        '
        'lblCondition
        '
        Me.lblCondition.AutoSize = True
        Me.lblCondition.Location = New System.Drawing.Point(4, 44)
        Me.lblCondition.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCondition.Name = "lblCondition"
        Me.lblCondition.Size = New System.Drawing.Size(38, 13)
        Me.lblCondition.TabIndex = 8
        Me.lblCondition.Text = "Cond.:"
        '
        'grpModel
        '
        Me.grpModel.Controls.Add(Me.chkTypeBetter)
        Me.grpModel.Controls.Add(Me.numLengthMin)
        Me.grpModel.Controls.Add(Me.numYearMin)
        Me.grpModel.Controls.Add(Me.numLengthMax)
        Me.grpModel.Controls.Add(Me.numYearMax)
        Me.grpModel.Controls.Add(Me.lblYearTo)
        Me.grpModel.Controls.Add(Me.lblLengthTo)
        Me.grpModel.Controls.Add(Me.cmbTypes)
        Me.grpModel.Controls.Add(Me.lblType)
        Me.grpModel.Controls.Add(Me.cmbModel)
        Me.grpModel.Controls.Add(Me.cmbBrand)
        Me.grpModel.Controls.Add(Me.lblLength)
        Me.grpModel.Controls.Add(Me.lblYear)
        Me.grpModel.Controls.Add(Me.lblBrand)
        Me.grpModel.Controls.Add(Me.lblModel)
        Me.grpModel.Location = New System.Drawing.Point(5, 17)
        Me.grpModel.Margin = New System.Windows.Forms.Padding(2)
        Me.grpModel.Name = "grpModel"
        Me.grpModel.Padding = New System.Windows.Forms.Padding(2)
        Me.grpModel.Size = New System.Drawing.Size(177, 165)
        Me.grpModel.TabIndex = 1
        Me.grpModel.TabStop = False
        Me.grpModel.Text = "Model"
        '
        'chkTypeBetter
        '
        Me.chkTypeBetter.AutoSize = True
        Me.chkTypeBetter.Enabled = False
        Me.chkTypeBetter.Location = New System.Drawing.Point(49, 69)
        Me.chkTypeBetter.Margin = New System.Windows.Forms.Padding(2)
        Me.chkTypeBetter.Name = "chkTypeBetter"
        Me.chkTypeBetter.Size = New System.Drawing.Size(101, 17)
        Me.chkTypeBetter.TabIndex = 3
        Me.chkTypeBetter.Text = "Type I or better."
        Me.chkTypeBetter.UseVisualStyleBackColor = True
        '
        'numLengthMin
        '
        Me.numLengthMin.Location = New System.Drawing.Point(49, 115)
        Me.numLengthMin.Margin = New System.Windows.Forms.Padding(2)
        Me.numLengthMin.Maximum = New Decimal(New Integer() {180, 0, 0, 0})
        Me.numLengthMin.Name = "numLengthMin"
        Me.numLengthMin.Size = New System.Drawing.Size(50, 20)
        Me.numLengthMin.TabIndex = 5
        '
        'numYearMin
        '
        Me.numYearMin.Location = New System.Drawing.Point(49, 139)
        Me.numYearMin.Margin = New System.Windows.Forms.Padding(2)
        Me.numYearMin.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYearMin.Minimum = New Decimal(New Integer() {1963, 0, 0, 0})
        Me.numYearMin.Name = "numYearMin"
        Me.numYearMin.Size = New System.Drawing.Size(50, 20)
        Me.numYearMin.TabIndex = 7
        Me.numYearMin.Value = New Decimal(New Integer() {1963, 0, 0, 0})
        '
        'numLengthMax
        '
        Me.numLengthMax.Location = New System.Drawing.Point(122, 115)
        Me.numLengthMax.Margin = New System.Windows.Forms.Padding(2)
        Me.numLengthMax.Maximum = New Decimal(New Integer() {180, 0, 0, 0})
        Me.numLengthMax.Name = "numLengthMax"
        Me.numLengthMax.Size = New System.Drawing.Size(50, 20)
        Me.numLengthMax.TabIndex = 6
        Me.numLengthMax.Value = New Decimal(New Integer() {180, 0, 0, 0})
        '
        'numYearMax
        '
        Me.numYearMax.Location = New System.Drawing.Point(122, 139)
        Me.numYearMax.Margin = New System.Windows.Forms.Padding(2)
        Me.numYearMax.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYearMax.Minimum = New Decimal(New Integer() {1963, 0, 0, 0})
        Me.numYearMax.Name = "numYearMax"
        Me.numYearMax.Size = New System.Drawing.Size(50, 20)
        Me.numYearMax.TabIndex = 8
        Me.numYearMax.Value = New Decimal(New Integer() {2019, 0, 0, 0})
        '
        'lblYearTo
        '
        Me.lblYearTo.AutoSize = True
        Me.lblYearTo.Location = New System.Drawing.Point(103, 141)
        Me.lblYearTo.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblYearTo.Name = "lblYearTo"
        Me.lblYearTo.Size = New System.Drawing.Size(16, 13)
        Me.lblYearTo.TabIndex = 11
        Me.lblYearTo.Text = "to"
        '
        'lblLengthTo
        '
        Me.lblLengthTo.AutoSize = True
        Me.lblLengthTo.Location = New System.Drawing.Point(103, 117)
        Me.lblLengthTo.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLengthTo.Name = "lblLengthTo"
        Me.lblLengthTo.Size = New System.Drawing.Size(16, 13)
        Me.lblLengthTo.TabIndex = 10
        Me.lblLengthTo.Text = "to"
        '
        'cmbTypes
        '
        Me.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTypes.FormattingEnabled = True
        Me.cmbTypes.Items.AddRange(New Object() {"All Types", "Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbTypes.Location = New System.Drawing.Point(49, 42)
        Me.cmbTypes.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbTypes.Name = "cmbTypes"
        Me.cmbTypes.Size = New System.Drawing.Size(123, 21)
        Me.cmbTypes.TabIndex = 2
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 45)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 8
        Me.lblType.Text = "Type:"
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Enabled = False
        Me.cmbModel.FormattingEnabled = True
        Me.cmbModel.Items.AddRange(New Object() {"All Models"})
        Me.cmbModel.Location = New System.Drawing.Point(49, 90)
        Me.cmbModel.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(123, 21)
        Me.cmbModel.TabIndex = 4
        '
        'cmbBrand
        '
        Me.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBrand.FormattingEnabled = True
        Me.cmbBrand.Items.AddRange(New Object() {"All Brands"})
        Me.cmbBrand.Location = New System.Drawing.Point(49, 17)
        Me.cmbBrand.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbBrand.Name = "cmbBrand"
        Me.cmbBrand.Size = New System.Drawing.Size(123, 21)
        Me.cmbBrand.TabIndex = 1
        '
        'lblLength
        '
        Me.lblLength.AutoSize = True
        Me.lblLength.Location = New System.Drawing.Point(4, 117)
        Me.lblLength.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLength.Name = "lblLength"
        Me.lblLength.Size = New System.Drawing.Size(43, 13)
        Me.lblLength.TabIndex = 3
        Me.lblLength.Text = "Length:"
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Location = New System.Drawing.Point(4, 141)
        Me.lblYear.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(32, 13)
        Me.lblYear.TabIndex = 2
        Me.lblYear.Text = "Year:"
        '
        'lblBrand
        '
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(4, 20)
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(38, 13)
        Me.lblBrand.TabIndex = 1
        Me.lblBrand.Text = "Brand:"
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 93)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
        Me.lblModel.TabIndex = 0
        Me.lblModel.Text = "Model:"
        '
        'grpTapes
        '
        Me.grpTapes.Controls.Add(Me.lstTapes)
        Me.grpTapes.Location = New System.Drawing.Point(202, 11)
        Me.grpTapes.Margin = New System.Windows.Forms.Padding(2)
        Me.grpTapes.Name = "grpTapes"
        Me.grpTapes.Padding = New System.Windows.Forms.Padding(2)
        Me.grpTapes.Size = New System.Drawing.Size(904, 656)
        Me.grpTapes.TabIndex = 2
        Me.grpTapes.TabStop = False
        Me.grpTapes.Text = "Tapes"
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Controls.Add(Me.btnEdit)
        Me.grpActions.Location = New System.Drawing.Point(11, 582)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2)
        Me.grpActions.Size = New System.Drawing.Size(187, 85)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(5, 17)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(177, 21)
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
        Me.btnDelete.Location = New System.Drawing.Point(95, 42)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(86, 21)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnEdit
        '
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(5, 42)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(86, 21)
        Me.btnEdit.TabIndex = 2
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(11, 561)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 28
        Me.lblResults.Text = "Results (Filtered):"
        '
        'txtResults
        '
        Me.txtResults.Location = New System.Drawing.Point(114, 558)
        Me.txtResults.Margin = New System.Windows.Forms.Padding(2)
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ReadOnly = True
        Me.txtResults.Size = New System.Drawing.Size(84, 20)
        Me.txtResults.TabIndex = 4
        Me.txtResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lstTapes
        '
        Me.lstTapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.colYear, Me.colLength, Me.colCondition, Me.colRecorded, Me.colContents, Me.colNoise, Me.colAlbum, Me.colTitle, Me.ColumnHeader6})
        Me.lstTapes.HideSelection = False
        Me.lstTapes.Location = New System.Drawing.Point(5, 17)
        Me.lstTapes.Margin = New System.Windows.Forms.Padding(2)
        Me.lstTapes.Name = "lstTapes"
        Me.lstTapes.Size = New System.Drawing.Size(894, 633)
        Me.lstTapes.TabIndex = 43
        Me.lstTapes.UseCompatibleStateImageBehavior = False
        Me.lstTapes.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Identifier"
        Me.ColumnHeader1.Width = 82
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Name"
        Me.ColumnHeader2.Width = 165
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Brand"
        Me.ColumnHeader3.Width = 91
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Model"
        Me.ColumnHeader4.Width = 78
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Type"
        Me.ColumnHeader5.Width = 51
        '
        'colYear
        '
        Me.colYear.Text = "Year"
        Me.colYear.Width = 44
        '
        'colLength
        '
        Me.colLength.Text = "Length"
        Me.colLength.Width = 48
        '
        'colCondition
        '
        Me.colCondition.Text = "Condition"
        Me.colCondition.Width = 59
        '
        'colRecorded
        '
        Me.colRecorded.Text = "Dates Recorded"
        Me.colRecorded.Width = 114
        '
        'colContents
        '
        Me.colContents.Text = "Contents"
        Me.colContents.Width = 74
        '
        'colNoise
        '
        Me.colNoise.Text = "NRs"
        Me.colNoise.Width = 41
        '
        'colAlbum
        '
        Me.colAlbum.Text = "Artists"
        Me.colAlbum.Width = 81
        '
        'colTitle
        '
        Me.colTitle.Text = "Titles"
        Me.colTitle.Width = 86
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Notes"
        Me.ColumnHeader6.Width = 90
        '
        'frmTapes
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1118, 677)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.txtResults)
        Me.Controls.Add(Me.grpTapes)
        Me.Controls.Add(Me.grpFilters)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.Name = "frmTapes"
        Me.Text = "View Tapes"
        Me.grpFilters.ResumeLayout(False)
        Me.grpContents.ResumeLayout(False)
        Me.grpContents.PerformLayout()
        Me.grpTape.ResumeLayout(False)
        Me.grpTape.PerformLayout()
        Me.grpModel.ResumeLayout(False)
        Me.grpModel.PerformLayout()
        CType(Me.numLengthMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numYearMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLengthMax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numYearMax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTapes.ResumeLayout(False)
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents grpTapes As GroupBox
    Friend WithEvents grpContents As GroupBox
    Friend WithEvents grpTape As GroupBox
    Friend WithEvents grpModel As GroupBox
    Friend WithEvents cmbModel As ComboBox
    Friend WithEvents cmbBrand As ComboBox
    Private WithEvents lblLength As Label
    Private WithEvents lblYear As Label
    Private WithEvents lblBrand As Label
    Private WithEvents lblModel As Label
    Private WithEvents lblCondition As Label
    Friend WithEvents cmbNR As ComboBox
    Private WithEvents lblNR As Label
    Friend WithEvents datRecordedMax As DateTimePicker
    Friend WithEvents lblRecorded As Label
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblArtist As Label
    Friend WithEvents txtTitle As TextBox
    Friend WithEvents txtArtist As TextBox
    Friend WithEvents lblContents As Label
    Friend WithEvents cmbContents As ComboBox
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents TextBox1 As TextBox
    Private WithEvents lblNotes As Label
    Friend WithEvents cmbTypes As ComboBox
    Private WithEvents lblType As Label
    Friend WithEvents cmbCondition As ComboBox
    Private WithEvents lblYearTo As Label
    Private WithEvents lblLengthTo As Label
    Friend WithEvents chkTypeBetter As CheckBox
    Friend WithEvents numLengthMin As NumericUpDown
    Friend WithEvents numYearMin As NumericUpDown
    Friend WithEvents numLengthMax As NumericUpDown
    Friend WithEvents numYearMax As NumericUpDown
    Friend WithEvents datRecordedMin As DateTimePicker
    Friend WithEvents CheckBox1 As CheckBox
    Private WithEvents lblRecordedTo As Label
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnEdit As Button
    Friend WithEvents btnRefresh As Button
    Friend WithEvents cmbDeck As ComboBox
    Friend WithEvents lblDeck As Label
    Friend WithEvents lblResults As Label
    Friend WithEvents txtResults As TextBox
    Friend WithEvents lstTapes As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents colYear As ColumnHeader
    Friend WithEvents colLength As ColumnHeader
    Friend WithEvents colCondition As ColumnHeader
    Friend WithEvents colRecorded As ColumnHeader
    Friend WithEvents colContents As ColumnHeader
    Friend WithEvents colNoise As ColumnHeader
    Friend WithEvents colAlbum As ColumnHeader
    Friend WithEvents colTitle As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
End Class
