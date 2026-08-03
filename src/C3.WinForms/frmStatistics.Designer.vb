<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmStatistics
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStatistics))
        Me.grpRecords = New System.Windows.Forms.GroupBox()
        Me.grpRecordings = New System.Windows.Forms.GroupBox()
        Me.txtNewest = New System.Windows.Forms.TextBox()
        Me.txtOldest = New System.Windows.Forms.TextBox()
        Me.lblNewest = New System.Windows.Forms.Label()
        Me.lblOldest = New System.Windows.Forms.Label()
        Me.grpLengths = New System.Windows.Forms.GroupBox()
        Me.txtLongest = New System.Windows.Forms.TextBox()
        Me.lblLongest = New System.Windows.Forms.Label()
        Me.txtShortest = New System.Windows.Forms.TextBox()
        Me.lblShortest = New System.Windows.Forms.Label()
        Me.grpTapes = New System.Windows.Forms.GroupBox()
        Me.txtNewestTape = New System.Windows.Forms.TextBox()
        Me.lblNewestTape = New System.Windows.Forms.Label()
        Me.txtOldestTape = New System.Windows.Forms.TextBox()
        Me.lblOldestTape = New System.Windows.Forms.Label()
        Me.lblTapes = New System.Windows.Forms.Label()
        Me.txtTotal = New System.Windows.Forms.TextBox()
        Me.grpTotals = New System.Windows.Forms.GroupBox()
        Me.txtDecks = New System.Windows.Forms.TextBox()
        Me.lblDecks = New System.Windows.Forms.Label()
        Me.grpContents = New System.Windows.Forms.GroupBox()
        Me.txtSingles = New System.Windows.Forms.TextBox()
        Me.lblSingles = New System.Windows.Forms.Label()
        Me.txtEPs = New System.Windows.Forms.TextBox()
        Me.lblEPs = New System.Windows.Forms.Label()
        Me.txtSoundtracks = New System.Windows.Forms.TextBox()
        Me.lblSoundtracks = New System.Windows.Forms.Label()
        Me.txtCompilations = New System.Windows.Forms.TextBox()
        Me.lblCompilations = New System.Windows.Forms.Label()
        Me.txtAlbums = New System.Windows.Forms.TextBox()
        Me.lblAlbums = New System.Windows.Forms.Label()
        Me.txtMixtapes = New System.Windows.Forms.TextBox()
        Me.lblMixtapes = New System.Windows.Forms.Label()
        Me.txtLengthTotal = New System.Windows.Forms.TextBox()
        Me.grpSides = New System.Windows.Forms.GroupBox()
        Me.txtDubbed = New System.Windows.Forms.TextBox()
        Me.lblDubbed = New System.Windows.Forms.Label()
        Me.txtRecordings = New System.Windows.Forms.TextBox()
        Me.lblRecordings = New System.Windows.Forms.Label()
        Me.txtASides = New System.Windows.Forms.TextBox()
        Me.lblASides = New System.Windows.Forms.Label()
        Me.lblBSides = New System.Windows.Forms.Label()
        Me.txtBSides = New System.Windows.Forms.TextBox()
        Me.lblLengthTotal = New System.Windows.Forms.Label()
        Me.txtPackaged = New System.Windows.Forms.TextBox()
        Me.grpYears = New System.Windows.Forms.GroupBox()
        Me.txt30s = New System.Windows.Forms.TextBox()
        Me.lbl30s = New System.Windows.Forms.Label()
        Me.txt20s = New System.Windows.Forms.TextBox()
        Me.lbl20s = New System.Windows.Forms.Label()
        Me.txt10s = New System.Windows.Forms.TextBox()
        Me.lbl10s = New System.Windows.Forms.Label()
        Me.txt00s = New System.Windows.Forms.TextBox()
        Me.lbl00s = New System.Windows.Forms.Label()
        Me.txt90sLate = New System.Windows.Forms.TextBox()
        Me.lbl90sLate = New System.Windows.Forms.Label()
        Me.txt80sEarly = New System.Windows.Forms.TextBox()
        Me.lbl80sEarly = New System.Windows.Forms.Label()
        Me.txt70sEarly = New System.Windows.Forms.TextBox()
        Me.lbl70sEarly = New System.Windows.Forms.Label()
        Me.txt60s = New System.Windows.Forms.TextBox()
        Me.lbl60s = New System.Windows.Forms.Label()
        Me.lblPackaged = New System.Windows.Forms.Label()
        Me.txtModels = New System.Windows.Forms.TextBox()
        Me.lblModels = New System.Windows.Forms.Label()
        Me.txtBrands = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.grpTypes = New System.Windows.Forms.GroupBox()
        Me.txtType4 = New System.Windows.Forms.TextBox()
        Me.lblType4 = New System.Windows.Forms.Label()
        Me.txtType3 = New System.Windows.Forms.TextBox()
        Me.lblType3 = New System.Windows.Forms.Label()
        Me.txtType2 = New System.Windows.Forms.TextBox()
        Me.lblType2 = New System.Windows.Forms.Label()
        Me.txtType1 = New System.Windows.Forms.TextBox()
        Me.lblType1 = New System.Windows.Forms.Label()
        Me.grpPopular = New System.Windows.Forms.GroupBox()
        Me.txtContents = New System.Windows.Forms.TextBox()
        Me.lblContents = New System.Windows.Forms.Label()
        Me.txtCondition = New System.Windows.Forms.TextBox()
        Me.lblCondition = New System.Windows.Forms.Label()
        Me.txtLength = New System.Windows.Forms.TextBox()
        Me.lblLength = New System.Windows.Forms.Label()
        Me.txtYear = New System.Windows.Forms.TextBox()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.txtType = New System.Windows.Forms.TextBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtDeck = New System.Windows.Forms.TextBox()
        Me.lblDeck = New System.Windows.Forms.Label()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnGraphs = New System.Windows.Forms.Button()
        Me.txtNR = New System.Windows.Forms.TextBox()
        Me.lblNR = New System.Windows.Forms.Label()
        Me.txt90sEarly = New System.Windows.Forms.TextBox()
        Me.lbl90sEarly = New System.Windows.Forms.Label()
        Me.txt80sLate = New System.Windows.Forms.TextBox()
        Me.lbl80sLate = New System.Windows.Forms.Label()
        Me.txtLengthRecorded = New System.Windows.Forms.TextBox()
        Me.lblLengthRecorded = New System.Windows.Forms.Label()
        Me.txt70sLate = New System.Windows.Forms.TextBox()
        Me.lbl70sLate = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtInput = New System.Windows.Forms.TextBox()
        Me.lblInput = New System.Windows.Forms.Label()
        Me.grpRecords.SuspendLayout()
        Me.grpRecordings.SuspendLayout()
        Me.grpLengths.SuspendLayout()
        Me.grpTapes.SuspendLayout()
        Me.grpTotals.SuspendLayout()
        Me.grpContents.SuspendLayout()
        Me.grpSides.SuspendLayout()
        Me.grpYears.SuspendLayout()
        Me.grpTypes.SuspendLayout()
        Me.grpPopular.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpRecords
        '
        Me.grpRecords.Controls.Add(Me.grpRecordings)
        Me.grpRecords.Controls.Add(Me.grpLengths)
        Me.grpRecords.Controls.Add(Me.grpTapes)
        Me.grpRecords.Location = New System.Drawing.Point(259, 274)
        Me.grpRecords.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpRecords.Name = "grpRecords"
        Me.grpRecords.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpRecords.Size = New System.Drawing.Size(171, 231)
        Me.grpRecords.TabIndex = 3
        Me.grpRecords.TabStop = False
        Me.grpRecords.Text = "Records"
        '
        'grpRecordings
        '
        Me.grpRecordings.Controls.Add(Me.txtNewest)
        Me.grpRecordings.Controls.Add(Me.txtOldest)
        Me.grpRecordings.Controls.Add(Me.lblNewest)
        Me.grpRecordings.Controls.Add(Me.lblOldest)
        Me.grpRecordings.Location = New System.Drawing.Point(5, 159)
        Me.grpRecordings.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpRecordings.Name = "grpRecordings"
        Me.grpRecordings.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpRecordings.Size = New System.Drawing.Size(161, 67)
        Me.grpRecordings.TabIndex = 3
        Me.grpRecordings.TabStop = False
        Me.grpRecordings.Text = "Recordings"
        '
        'txtNewest
        '
        Me.txtNewest.Location = New System.Drawing.Point(55, 41)
        Me.txtNewest.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtNewest.Name = "txtNewest"
        Me.txtNewest.ReadOnly = True
        Me.txtNewest.Size = New System.Drawing.Size(101, 20)
        Me.txtNewest.TabIndex = 38
        '
        'txtOldest
        '
        Me.txtOldest.Location = New System.Drawing.Point(55, 17)
        Me.txtOldest.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtOldest.Name = "txtOldest"
        Me.txtOldest.ReadOnly = True
        Me.txtOldest.Size = New System.Drawing.Size(101, 20)
        Me.txtOldest.TabIndex = 37
        '
        'lblNewest
        '
        Me.lblNewest.AutoSize = True
        Me.lblNewest.Location = New System.Drawing.Point(4, 44)
        Me.lblNewest.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNewest.Name = "lblNewest"
        Me.lblNewest.Size = New System.Drawing.Size(46, 13)
        Me.lblNewest.TabIndex = 6
        Me.lblNewest.Text = "Newest:"
        '
        'lblOldest
        '
        Me.lblOldest.AutoSize = True
        Me.lblOldest.Location = New System.Drawing.Point(4, 20)
        Me.lblOldest.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblOldest.Name = "lblOldest"
        Me.lblOldest.Size = New System.Drawing.Size(40, 13)
        Me.lblOldest.TabIndex = 4
        Me.lblOldest.Text = "Oldest:"
        '
        'grpLengths
        '
        Me.grpLengths.Controls.Add(Me.txtLongest)
        Me.grpLengths.Controls.Add(Me.lblLongest)
        Me.grpLengths.Controls.Add(Me.txtShortest)
        Me.grpLengths.Controls.Add(Me.lblShortest)
        Me.grpLengths.Location = New System.Drawing.Point(5, 88)
        Me.grpLengths.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpLengths.Name = "grpLengths"
        Me.grpLengths.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpLengths.Size = New System.Drawing.Size(161, 67)
        Me.grpLengths.TabIndex = 2
        Me.grpLengths.TabStop = False
        Me.grpLengths.Text = "Length"
        '
        'txtLongest
        '
        Me.txtLongest.Location = New System.Drawing.Point(55, 41)
        Me.txtLongest.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtLongest.Name = "txtLongest"
        Me.txtLongest.ReadOnly = True
        Me.txtLongest.Size = New System.Drawing.Size(101, 20)
        Me.txtLongest.TabIndex = 36
        Me.txtLongest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblLongest
        '
        Me.lblLongest.AutoSize = True
        Me.lblLongest.Location = New System.Drawing.Point(4, 44)
        Me.lblLongest.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLongest.Name = "lblLongest"
        Me.lblLongest.Size = New System.Drawing.Size(48, 13)
        Me.lblLongest.TabIndex = 2
        Me.lblLongest.Text = "Longest:"
        '
        'txtShortest
        '
        Me.txtShortest.Location = New System.Drawing.Point(55, 17)
        Me.txtShortest.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtShortest.Name = "txtShortest"
        Me.txtShortest.ReadOnly = True
        Me.txtShortest.Size = New System.Drawing.Size(101, 20)
        Me.txtShortest.TabIndex = 35
        Me.txtShortest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblShortest
        '
        Me.lblShortest.AutoSize = True
        Me.lblShortest.Location = New System.Drawing.Point(4, 20)
        Me.lblShortest.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblShortest.Name = "lblShortest"
        Me.lblShortest.Size = New System.Drawing.Size(49, 13)
        Me.lblShortest.TabIndex = 0
        Me.lblShortest.Text = "Shortest:"
        '
        'grpTapes
        '
        Me.grpTapes.Controls.Add(Me.txtNewestTape)
        Me.grpTapes.Controls.Add(Me.lblNewestTape)
        Me.grpTapes.Controls.Add(Me.txtOldestTape)
        Me.grpTapes.Controls.Add(Me.lblOldestTape)
        Me.grpTapes.Location = New System.Drawing.Point(5, 17)
        Me.grpTapes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTapes.Name = "grpTapes"
        Me.grpTapes.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTapes.Size = New System.Drawing.Size(161, 67)
        Me.grpTapes.TabIndex = 1
        Me.grpTapes.TabStop = False
        Me.grpTapes.Text = "Tapes"
        '
        'txtNewestTape
        '
        Me.txtNewestTape.Location = New System.Drawing.Point(55, 41)
        Me.txtNewestTape.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtNewestTape.Name = "txtNewestTape"
        Me.txtNewestTape.ReadOnly = True
        Me.txtNewestTape.Size = New System.Drawing.Size(101, 20)
        Me.txtNewestTape.TabIndex = 34
        Me.txtNewestTape.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblNewestTape
        '
        Me.lblNewestTape.AutoSize = True
        Me.lblNewestTape.Location = New System.Drawing.Point(4, 44)
        Me.lblNewestTape.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNewestTape.Name = "lblNewestTape"
        Me.lblNewestTape.Size = New System.Drawing.Size(46, 13)
        Me.lblNewestTape.TabIndex = 2
        Me.lblNewestTape.Text = "Newest:"
        '
        'txtOldestTape
        '
        Me.txtOldestTape.Location = New System.Drawing.Point(55, 17)
        Me.txtOldestTape.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtOldestTape.Name = "txtOldestTape"
        Me.txtOldestTape.ReadOnly = True
        Me.txtOldestTape.Size = New System.Drawing.Size(101, 20)
        Me.txtOldestTape.TabIndex = 33
        Me.txtOldestTape.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblOldestTape
        '
        Me.lblOldestTape.AutoSize = True
        Me.lblOldestTape.Location = New System.Drawing.Point(4, 20)
        Me.lblOldestTape.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblOldestTape.Name = "lblOldestTape"
        Me.lblOldestTape.Size = New System.Drawing.Size(40, 13)
        Me.lblOldestTape.TabIndex = 0
        Me.lblOldestTape.Text = "Oldest:"
        '
        'lblTapes
        '
        Me.lblTapes.AutoSize = True
        Me.lblTapes.Location = New System.Drawing.Point(4, 92)
        Me.lblTapes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTapes.Name = "lblTapes"
        Me.lblTapes.Size = New System.Drawing.Size(40, 13)
        Me.lblTapes.TabIndex = 0
        Me.lblTapes.Text = "Tapes:"
        '
        'txtTotal
        '
        Me.txtTotal.Location = New System.Drawing.Point(52, 89)
        Me.txtTotal.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtTotal.Name = "txtTotal"
        Me.txtTotal.ReadOnly = True
        Me.txtTotal.Size = New System.Drawing.Size(68, 20)
        Me.txtTotal.TabIndex = 1
        Me.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'grpTotals
        '
        Me.grpTotals.Controls.Add(Me.txtModels)
        Me.grpTotals.Controls.Add(Me.txtDecks)
        Me.grpTotals.Controls.Add(Me.lblModels)
        Me.grpTotals.Controls.Add(Me.lblDecks)
        Me.grpTotals.Controls.Add(Me.txtBrands)
        Me.grpTotals.Controls.Add(Me.Label6)
        Me.grpTotals.Controls.Add(Me.grpContents)
        Me.grpTotals.Controls.Add(Me.txtLengthTotal)
        Me.grpTotals.Controls.Add(Me.grpSides)
        Me.grpTotals.Controls.Add(Me.lblLengthTotal)
        Me.grpTotals.Controls.Add(Me.txtPackaged)
        Me.grpTotals.Controls.Add(Me.grpYears)
        Me.grpTotals.Controls.Add(Me.lblPackaged)
        Me.grpTotals.Controls.Add(Me.txtTotal)
        Me.grpTotals.Controls.Add(Me.lblTapes)
        Me.grpTotals.Controls.Add(Me.grpTypes)
        Me.grpTotals.Location = New System.Drawing.Point(11, 11)
        Me.grpTotals.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTotals.Name = "grpTotals"
        Me.grpTotals.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTotals.Size = New System.Drawing.Size(244, 448)
        Me.grpTotals.TabIndex = 1
        Me.grpTotals.TabStop = False
        Me.grpTotals.Text = "Totals"
        '
        'txtDecks
        '
        Me.txtDecks.Location = New System.Drawing.Point(52, 17)
        Me.txtDecks.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtDecks.Name = "txtDecks"
        Me.txtDecks.ReadOnly = True
        Me.txtDecks.Size = New System.Drawing.Size(68, 20)
        Me.txtDecks.TabIndex = 4
        Me.txtDecks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblDecks
        '
        Me.lblDecks.AutoSize = True
        Me.lblDecks.Location = New System.Drawing.Point(4, 20)
        Me.lblDecks.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDecks.Name = "lblDecks"
        Me.lblDecks.Size = New System.Drawing.Size(41, 13)
        Me.lblDecks.TabIndex = 13
        Me.lblDecks.Text = "Decks:"
        '
        'grpContents
        '
        Me.grpContents.Controls.Add(Me.txtSingles)
        Me.grpContents.Controls.Add(Me.lblSingles)
        Me.grpContents.Controls.Add(Me.txtEPs)
        Me.grpContents.Controls.Add(Me.lblEPs)
        Me.grpContents.Controls.Add(Me.txtSoundtracks)
        Me.grpContents.Controls.Add(Me.lblSoundtracks)
        Me.grpContents.Controls.Add(Me.txtCompilations)
        Me.grpContents.Controls.Add(Me.lblCompilations)
        Me.grpContents.Controls.Add(Me.txtAlbums)
        Me.grpContents.Controls.Add(Me.lblAlbums)
        Me.grpContents.Controls.Add(Me.txtMixtapes)
        Me.grpContents.Controls.Add(Me.lblMixtapes)
        Me.grpContents.Location = New System.Drawing.Point(5, 280)
        Me.grpContents.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpContents.Name = "grpContents"
        Me.grpContents.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpContents.Size = New System.Drawing.Size(115, 163)
        Me.grpContents.TabIndex = 8
        Me.grpContents.TabStop = False
        Me.grpContents.Text = "Contents"
        '
        'txtSingles
        '
        Me.txtSingles.Location = New System.Drawing.Point(65, 137)
        Me.txtSingles.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtSingles.Name = "txtSingles"
        Me.txtSingles.ReadOnly = True
        Me.txtSingles.Size = New System.Drawing.Size(45, 20)
        Me.txtSingles.TabIndex = 20
        Me.txtSingles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblSingles
        '
        Me.lblSingles.AutoSize = True
        Me.lblSingles.Location = New System.Drawing.Point(4, 140)
        Me.lblSingles.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSingles.Name = "lblSingles"
        Me.lblSingles.Size = New System.Drawing.Size(44, 13)
        Me.lblSingles.TabIndex = 10
        Me.lblSingles.Text = "Singles:"
        '
        'txtEPs
        '
        Me.txtEPs.Location = New System.Drawing.Point(65, 113)
        Me.txtEPs.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtEPs.Name = "txtEPs"
        Me.txtEPs.ReadOnly = True
        Me.txtEPs.Size = New System.Drawing.Size(45, 20)
        Me.txtEPs.TabIndex = 19
        Me.txtEPs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblEPs
        '
        Me.lblEPs.AutoSize = True
        Me.lblEPs.Location = New System.Drawing.Point(4, 116)
        Me.lblEPs.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblEPs.Name = "lblEPs"
        Me.lblEPs.Size = New System.Drawing.Size(29, 13)
        Me.lblEPs.TabIndex = 8
        Me.lblEPs.Text = "EPs:"
        '
        'txtSoundtracks
        '
        Me.txtSoundtracks.Location = New System.Drawing.Point(65, 89)
        Me.txtSoundtracks.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtSoundtracks.Name = "txtSoundtracks"
        Me.txtSoundtracks.ReadOnly = True
        Me.txtSoundtracks.Size = New System.Drawing.Size(45, 20)
        Me.txtSoundtracks.TabIndex = 18
        Me.txtSoundtracks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblSoundtracks
        '
        Me.lblSoundtracks.AutoSize = True
        Me.lblSoundtracks.Location = New System.Drawing.Point(4, 92)
        Me.lblSoundtracks.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSoundtracks.Name = "lblSoundtracks"
        Me.lblSoundtracks.Size = New System.Drawing.Size(37, 13)
        Me.lblSoundtracks.TabIndex = 6
        Me.lblSoundtracks.Text = "OSTs:"
        '
        'txtCompilations
        '
        Me.txtCompilations.Location = New System.Drawing.Point(65, 65)
        Me.txtCompilations.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtCompilations.Name = "txtCompilations"
        Me.txtCompilations.ReadOnly = True
        Me.txtCompilations.Size = New System.Drawing.Size(45, 20)
        Me.txtCompilations.TabIndex = 17
        Me.txtCompilations.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblCompilations
        '
        Me.lblCompilations.AutoSize = True
        Me.lblCompilations.Location = New System.Drawing.Point(4, 68)
        Me.lblCompilations.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCompilations.Name = "lblCompilations"
        Me.lblCompilations.Size = New System.Drawing.Size(45, 13)
        Me.lblCompilations.TabIndex = 4
        Me.lblCompilations.Text = "Comp.s:"
        '
        'txtAlbums
        '
        Me.txtAlbums.Location = New System.Drawing.Point(65, 41)
        Me.txtAlbums.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtAlbums.Name = "txtAlbums"
        Me.txtAlbums.ReadOnly = True
        Me.txtAlbums.Size = New System.Drawing.Size(45, 20)
        Me.txtAlbums.TabIndex = 16
        Me.txtAlbums.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblAlbums
        '
        Me.lblAlbums.AutoSize = True
        Me.lblAlbums.Location = New System.Drawing.Point(4, 44)
        Me.lblAlbums.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAlbums.Name = "lblAlbums"
        Me.lblAlbums.Size = New System.Drawing.Size(44, 13)
        Me.lblAlbums.TabIndex = 2
        Me.lblAlbums.Text = "Albums:"
        '
        'txtMixtapes
        '
        Me.txtMixtapes.Location = New System.Drawing.Point(65, 17)
        Me.txtMixtapes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtMixtapes.Name = "txtMixtapes"
        Me.txtMixtapes.ReadOnly = True
        Me.txtMixtapes.Size = New System.Drawing.Size(45, 20)
        Me.txtMixtapes.TabIndex = 15
        Me.txtMixtapes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblMixtapes
        '
        Me.lblMixtapes.AutoSize = True
        Me.lblMixtapes.Location = New System.Drawing.Point(4, 20)
        Me.lblMixtapes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMixtapes.Name = "lblMixtapes"
        Me.lblMixtapes.Size = New System.Drawing.Size(52, 13)
        Me.lblMixtapes.TabIndex = 0
        Me.lblMixtapes.Text = "Mixtapes:"
        '
        'txtLengthTotal
        '
        Me.txtLengthTotal.Location = New System.Drawing.Point(52, 137)
        Me.txtLengthTotal.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtLengthTotal.Name = "txtLengthTotal"
        Me.txtLengthTotal.ReadOnly = True
        Me.txtLengthTotal.Size = New System.Drawing.Size(68, 20)
        Me.txtLengthTotal.TabIndex = 3
        Me.txtLengthTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'grpSides
        '
        Me.grpSides.Controls.Add(Me.txtLengthRecorded)
        Me.grpSides.Controls.Add(Me.lblLengthRecorded)
        Me.grpSides.Controls.Add(Me.txtDubbed)
        Me.grpSides.Controls.Add(Me.lblDubbed)
        Me.grpSides.Controls.Add(Me.txtRecordings)
        Me.grpSides.Controls.Add(Me.lblRecordings)
        Me.grpSides.Controls.Add(Me.txtASides)
        Me.grpSides.Controls.Add(Me.lblASides)
        Me.grpSides.Controls.Add(Me.lblBSides)
        Me.grpSides.Controls.Add(Me.txtBSides)
        Me.grpSides.Location = New System.Drawing.Point(124, 17)
        Me.grpSides.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpSides.Name = "grpSides"
        Me.grpSides.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpSides.Size = New System.Drawing.Size(115, 140)
        Me.grpSides.TabIndex = 5
        Me.grpSides.TabStop = False
        Me.grpSides.Text = "Recordings"
        '
        'txtDubbed
        '
        Me.txtDubbed.Location = New System.Drawing.Point(52, 89)
        Me.txtDubbed.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtDubbed.Name = "txtDubbed"
        Me.txtDubbed.ReadOnly = True
        Me.txtDubbed.Size = New System.Drawing.Size(58, 20)
        Me.txtDubbed.TabIndex = 8
        Me.txtDubbed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblDubbed
        '
        Me.lblDubbed.AutoSize = True
        Me.lblDubbed.Location = New System.Drawing.Point(4, 92)
        Me.lblDubbed.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDubbed.Name = "lblDubbed"
        Me.lblDubbed.Size = New System.Drawing.Size(48, 13)
        Me.lblDubbed.TabIndex = 15
        Me.lblDubbed.Text = "Dubbed:"
        '
        'txtRecordings
        '
        Me.txtRecordings.Location = New System.Drawing.Point(42, 17)
        Me.txtRecordings.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtRecordings.Name = "txtRecordings"
        Me.txtRecordings.ReadOnly = True
        Me.txtRecordings.Size = New System.Drawing.Size(68, 20)
        Me.txtRecordings.TabIndex = 5
        Me.txtRecordings.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblRecordings
        '
        Me.lblRecordings.AutoSize = True
        Me.lblRecordings.Location = New System.Drawing.Point(4, 20)
        Me.lblRecordings.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblRecordings.Name = "lblRecordings"
        Me.lblRecordings.Size = New System.Drawing.Size(34, 13)
        Me.lblRecordings.TabIndex = 8
        Me.lblRecordings.Text = "Total:"
        '
        'txtASides
        '
        Me.txtASides.Location = New System.Drawing.Point(42, 41)
        Me.txtASides.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtASides.Name = "txtASides"
        Me.txtASides.ReadOnly = True
        Me.txtASides.Size = New System.Drawing.Size(68, 20)
        Me.txtASides.TabIndex = 6
        Me.txtASides.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblASides
        '
        Me.lblASides.AutoSize = True
        Me.lblASides.Location = New System.Drawing.Point(21, 44)
        Me.lblASides.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblASides.Name = "lblASides"
        Me.lblASides.Size = New System.Drawing.Size(17, 13)
        Me.lblASides.TabIndex = 4
        Me.lblASides.Text = "A:"
        '
        'lblBSides
        '
        Me.lblBSides.AutoSize = True
        Me.lblBSides.Location = New System.Drawing.Point(21, 68)
        Me.lblBSides.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBSides.Name = "lblBSides"
        Me.lblBSides.Size = New System.Drawing.Size(17, 13)
        Me.lblBSides.TabIndex = 6
        Me.lblBSides.Text = "B:"
        '
        'txtBSides
        '
        Me.txtBSides.Location = New System.Drawing.Point(42, 65)
        Me.txtBSides.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBSides.Name = "txtBSides"
        Me.txtBSides.ReadOnly = True
        Me.txtBSides.Size = New System.Drawing.Size(68, 20)
        Me.txtBSides.TabIndex = 7
        Me.txtBSides.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblLengthTotal
        '
        Me.lblLengthTotal.AutoSize = True
        Me.lblLengthTotal.Location = New System.Drawing.Point(4, 139)
        Me.lblLengthTotal.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLengthTotal.Name = "lblLengthTotal"
        Me.lblLengthTotal.Size = New System.Drawing.Size(38, 13)
        Me.lblLengthTotal.TabIndex = 0
        Me.lblLengthTotal.Text = "Hours:"
        '
        'txtPackaged
        '
        Me.txtPackaged.Location = New System.Drawing.Point(52, 113)
        Me.txtPackaged.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtPackaged.Name = "txtPackaged"
        Me.txtPackaged.ReadOnly = True
        Me.txtPackaged.Size = New System.Drawing.Size(68, 20)
        Me.txtPackaged.TabIndex = 2
        Me.txtPackaged.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'grpYears
        '
        Me.grpYears.Controls.Add(Me.txt70sLate)
        Me.grpYears.Controls.Add(Me.lbl70sLate)
        Me.grpYears.Controls.Add(Me.txt90sEarly)
        Me.grpYears.Controls.Add(Me.txt30s)
        Me.grpYears.Controls.Add(Me.lbl30s)
        Me.grpYears.Controls.Add(Me.lbl90sEarly)
        Me.grpYears.Controls.Add(Me.txt20s)
        Me.grpYears.Controls.Add(Me.lbl20s)
        Me.grpYears.Controls.Add(Me.txt80sLate)
        Me.grpYears.Controls.Add(Me.lbl80sLate)
        Me.grpYears.Controls.Add(Me.txt10s)
        Me.grpYears.Controls.Add(Me.lbl10s)
        Me.grpYears.Controls.Add(Me.txt00s)
        Me.grpYears.Controls.Add(Me.lbl00s)
        Me.grpYears.Controls.Add(Me.txt90sLate)
        Me.grpYears.Controls.Add(Me.lbl90sLate)
        Me.grpYears.Controls.Add(Me.txt80sEarly)
        Me.grpYears.Controls.Add(Me.lbl80sEarly)
        Me.grpYears.Controls.Add(Me.txt70sEarly)
        Me.grpYears.Controls.Add(Me.lbl70sEarly)
        Me.grpYears.Controls.Add(Me.txt60s)
        Me.grpYears.Controls.Add(Me.lbl60s)
        Me.grpYears.Location = New System.Drawing.Point(124, 161)
        Me.grpYears.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpYears.Name = "grpYears"
        Me.grpYears.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpYears.Size = New System.Drawing.Size(115, 282)
        Me.grpYears.TabIndex = 9
        Me.grpYears.TabStop = False
        Me.grpYears.Text = "Years"
        '
        'txt30s
        '
        Me.txt30s.Location = New System.Drawing.Point(52, 256)
        Me.txt30s.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt30s.Name = "txt30s"
        Me.txt30s.ReadOnly = True
        Me.txt30s.Size = New System.Drawing.Size(58, 20)
        Me.txt30s.TabIndex = 28
        Me.txt30s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl30s
        '
        Me.lbl30s.AutoSize = True
        Me.lbl30s.Location = New System.Drawing.Point(4, 259)
        Me.lbl30s.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl30s.Name = "lbl30s"
        Me.lbl30s.Size = New System.Drawing.Size(39, 13)
        Me.lbl30s.TabIndex = 14
        Me.lbl30s.Text = "2030s:"
        '
        'txt20s
        '
        Me.txt20s.Location = New System.Drawing.Point(52, 232)
        Me.txt20s.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt20s.Name = "txt20s"
        Me.txt20s.ReadOnly = True
        Me.txt20s.Size = New System.Drawing.Size(58, 20)
        Me.txt20s.TabIndex = 27
        Me.txt20s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl20s
        '
        Me.lbl20s.AutoSize = True
        Me.lbl20s.Location = New System.Drawing.Point(4, 235)
        Me.lbl20s.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl20s.Name = "lbl20s"
        Me.lbl20s.Size = New System.Drawing.Size(39, 13)
        Me.lbl20s.TabIndex = 12
        Me.lbl20s.Text = "2020s:"
        '
        'txt10s
        '
        Me.txt10s.Location = New System.Drawing.Point(52, 208)
        Me.txt10s.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt10s.Name = "txt10s"
        Me.txt10s.ReadOnly = True
        Me.txt10s.Size = New System.Drawing.Size(58, 20)
        Me.txt10s.TabIndex = 26
        Me.txt10s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl10s
        '
        Me.lbl10s.AutoSize = True
        Me.lbl10s.Location = New System.Drawing.Point(4, 211)
        Me.lbl10s.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl10s.Name = "lbl10s"
        Me.lbl10s.Size = New System.Drawing.Size(39, 13)
        Me.lbl10s.TabIndex = 10
        Me.lbl10s.Text = "2010s:"
        '
        'txt00s
        '
        Me.txt00s.Location = New System.Drawing.Point(52, 184)
        Me.txt00s.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt00s.Name = "txt00s"
        Me.txt00s.ReadOnly = True
        Me.txt00s.Size = New System.Drawing.Size(58, 20)
        Me.txt00s.TabIndex = 25
        Me.txt00s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl00s
        '
        Me.lbl00s.AutoSize = True
        Me.lbl00s.Location = New System.Drawing.Point(4, 187)
        Me.lbl00s.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl00s.Name = "lbl00s"
        Me.lbl00s.Size = New System.Drawing.Size(39, 13)
        Me.lbl00s.TabIndex = 8
        Me.lbl00s.Text = "2000s:"
        '
        'txt90sLate
        '
        Me.txt90sLate.Location = New System.Drawing.Point(52, 160)
        Me.txt90sLate.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt90sLate.Name = "txt90sLate"
        Me.txt90sLate.ReadOnly = True
        Me.txt90sLate.Size = New System.Drawing.Size(58, 20)
        Me.txt90sLate.TabIndex = 24
        Me.txt90sLate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl90sLate
        '
        Me.lbl90sLate.AutoSize = True
        Me.lbl90sLate.Location = New System.Drawing.Point(4, 163)
        Me.lbl90sLate.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl90sLate.Name = "lbl90sLate"
        Me.lbl90sLate.Size = New System.Drawing.Size(41, 13)
        Me.lbl90sLate.TabIndex = 6
        Me.lbl90sLate.Text = "'95-'99:"
        '
        'txt80sEarly
        '
        Me.txt80sEarly.Location = New System.Drawing.Point(52, 88)
        Me.txt80sEarly.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt80sEarly.Name = "txt80sEarly"
        Me.txt80sEarly.ReadOnly = True
        Me.txt80sEarly.Size = New System.Drawing.Size(58, 20)
        Me.txt80sEarly.TabIndex = 23
        Me.txt80sEarly.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl80sEarly
        '
        Me.lbl80sEarly.AutoSize = True
        Me.lbl80sEarly.Location = New System.Drawing.Point(4, 91)
        Me.lbl80sEarly.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl80sEarly.Name = "lbl80sEarly"
        Me.lbl80sEarly.Size = New System.Drawing.Size(41, 13)
        Me.lbl80sEarly.TabIndex = 4
        Me.lbl80sEarly.Text = "'80-'84:"
        '
        'txt70sEarly
        '
        Me.txt70sEarly.Location = New System.Drawing.Point(52, 40)
        Me.txt70sEarly.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt70sEarly.Name = "txt70sEarly"
        Me.txt70sEarly.ReadOnly = True
        Me.txt70sEarly.Size = New System.Drawing.Size(58, 20)
        Me.txt70sEarly.TabIndex = 22
        Me.txt70sEarly.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl70sEarly
        '
        Me.lbl70sEarly.AutoSize = True
        Me.lbl70sEarly.Location = New System.Drawing.Point(4, 43)
        Me.lbl70sEarly.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl70sEarly.Name = "lbl70sEarly"
        Me.lbl70sEarly.Size = New System.Drawing.Size(41, 13)
        Me.lbl70sEarly.TabIndex = 2
        Me.lbl70sEarly.Text = "'70-'74:"
        '
        'txt60s
        '
        Me.txt60s.Location = New System.Drawing.Point(52, 16)
        Me.txt60s.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txt60s.Name = "txt60s"
        Me.txt60s.ReadOnly = True
        Me.txt60s.Size = New System.Drawing.Size(58, 20)
        Me.txt60s.TabIndex = 21
        Me.txt60s.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl60s
        '
        Me.lbl60s.AutoSize = True
        Me.lbl60s.Location = New System.Drawing.Point(4, 19)
        Me.lbl60s.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl60s.Name = "lbl60s"
        Me.lbl60s.Size = New System.Drawing.Size(39, 13)
        Me.lbl60s.TabIndex = 0
        Me.lbl60s.Text = "1960s:"
        '
        'lblPackaged
        '
        Me.lblPackaged.AutoSize = True
        Me.lblPackaged.Location = New System.Drawing.Point(4, 116)
        Me.lblPackaged.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblPackaged.Name = "lblPackaged"
        Me.lblPackaged.Size = New System.Drawing.Size(43, 13)
        Me.lblPackaged.TabIndex = 2
        Me.lblPackaged.Text = "Sealed:"
        '
        'txtModels
        '
        Me.txtModels.Location = New System.Drawing.Point(52, 65)
        Me.txtModels.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtModels.Name = "txtModels"
        Me.txtModels.ReadOnly = True
        Me.txtModels.Size = New System.Drawing.Size(68, 20)
        Me.txtModels.TabIndex = 10
        Me.txtModels.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblModels
        '
        Me.lblModels.AutoSize = True
        Me.lblModels.Location = New System.Drawing.Point(4, 68)
        Me.lblModels.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModels.Name = "lblModels"
        Me.lblModels.Size = New System.Drawing.Size(44, 13)
        Me.lblModels.TabIndex = 2
        Me.lblModels.Text = "Models:"
        '
        'txtBrands
        '
        Me.txtBrands.Location = New System.Drawing.Point(52, 41)
        Me.txtBrands.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBrands.Name = "txtBrands"
        Me.txtBrands.ReadOnly = True
        Me.txtBrands.Size = New System.Drawing.Size(68, 20)
        Me.txtBrands.TabIndex = 9
        Me.txtBrands.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(4, 44)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(43, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Brands:"
        '
        'grpTypes
        '
        Me.grpTypes.Controls.Add(Me.txtType4)
        Me.grpTypes.Controls.Add(Me.lblType4)
        Me.grpTypes.Controls.Add(Me.txtType3)
        Me.grpTypes.Controls.Add(Me.lblType3)
        Me.grpTypes.Controls.Add(Me.txtType2)
        Me.grpTypes.Controls.Add(Me.lblType2)
        Me.grpTypes.Controls.Add(Me.txtType1)
        Me.grpTypes.Controls.Add(Me.lblType1)
        Me.grpTypes.Location = New System.Drawing.Point(5, 161)
        Me.grpTypes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTypes.Name = "grpTypes"
        Me.grpTypes.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpTypes.Size = New System.Drawing.Size(115, 115)
        Me.grpTypes.TabIndex = 7
        Me.grpTypes.TabStop = False
        Me.grpTypes.Text = "Types"
        '
        'txtType4
        '
        Me.txtType4.Location = New System.Drawing.Point(55, 89)
        Me.txtType4.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtType4.Name = "txtType4"
        Me.txtType4.ReadOnly = True
        Me.txtType4.Size = New System.Drawing.Size(55, 20)
        Me.txtType4.TabIndex = 14
        Me.txtType4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblType4
        '
        Me.lblType4.AutoSize = True
        Me.lblType4.Location = New System.Drawing.Point(4, 92)
        Me.lblType4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType4.Name = "lblType4"
        Me.lblType4.Size = New System.Drawing.Size(47, 13)
        Me.lblType4.TabIndex = 6
        Me.lblType4.Text = "Type IV:"
        '
        'txtType3
        '
        Me.txtType3.Location = New System.Drawing.Point(55, 65)
        Me.txtType3.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtType3.Name = "txtType3"
        Me.txtType3.ReadOnly = True
        Me.txtType3.Size = New System.Drawing.Size(55, 20)
        Me.txtType3.TabIndex = 13
        Me.txtType3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblType3
        '
        Me.lblType3.AutoSize = True
        Me.lblType3.Location = New System.Drawing.Point(4, 68)
        Me.lblType3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType3.Name = "lblType3"
        Me.lblType3.Size = New System.Drawing.Size(46, 13)
        Me.lblType3.TabIndex = 4
        Me.lblType3.Text = "Type III:"
        '
        'txtType2
        '
        Me.txtType2.Location = New System.Drawing.Point(55, 41)
        Me.txtType2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtType2.Name = "txtType2"
        Me.txtType2.ReadOnly = True
        Me.txtType2.Size = New System.Drawing.Size(55, 20)
        Me.txtType2.TabIndex = 12
        Me.txtType2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblType2
        '
        Me.lblType2.AutoSize = True
        Me.lblType2.Location = New System.Drawing.Point(4, 44)
        Me.lblType2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType2.Name = "lblType2"
        Me.lblType2.Size = New System.Drawing.Size(43, 13)
        Me.lblType2.TabIndex = 2
        Me.lblType2.Text = "Type II:"
        '
        'txtType1
        '
        Me.txtType1.Location = New System.Drawing.Point(55, 17)
        Me.txtType1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtType1.Name = "txtType1"
        Me.txtType1.ReadOnly = True
        Me.txtType1.Size = New System.Drawing.Size(55, 20)
        Me.txtType1.TabIndex = 11
        Me.txtType1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblType1
        '
        Me.lblType1.AutoSize = True
        Me.lblType1.Location = New System.Drawing.Point(4, 20)
        Me.lblType1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType1.Name = "lblType1"
        Me.lblType1.Size = New System.Drawing.Size(40, 13)
        Me.lblType1.TabIndex = 0
        Me.lblType1.Text = "Type I:"
        '
        'grpPopular
        '
        Me.grpPopular.Controls.Add(Me.txtInput)
        Me.grpPopular.Controls.Add(Me.lblInput)
        Me.grpPopular.Controls.Add(Me.txtNR)
        Me.grpPopular.Controls.Add(Me.lblNR)
        Me.grpPopular.Controls.Add(Me.txtContents)
        Me.grpPopular.Controls.Add(Me.lblContents)
        Me.grpPopular.Controls.Add(Me.txtCondition)
        Me.grpPopular.Controls.Add(Me.lblCondition)
        Me.grpPopular.Controls.Add(Me.txtLength)
        Me.grpPopular.Controls.Add(Me.lblLength)
        Me.grpPopular.Controls.Add(Me.txtYear)
        Me.grpPopular.Controls.Add(Me.lblYear)
        Me.grpPopular.Controls.Add(Me.txtType)
        Me.grpPopular.Controls.Add(Me.lblType)
        Me.grpPopular.Controls.Add(Me.txtModel)
        Me.grpPopular.Controls.Add(Me.lblModel)
        Me.grpPopular.Controls.Add(Me.txtBrand)
        Me.grpPopular.Controls.Add(Me.lblBrand)
        Me.grpPopular.Controls.Add(Me.txtDeck)
        Me.grpPopular.Controls.Add(Me.lblDeck)
        Me.grpPopular.Location = New System.Drawing.Point(259, 11)
        Me.grpPopular.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpPopular.Name = "grpPopular"
        Me.grpPopular.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpPopular.Size = New System.Drawing.Size(171, 258)
        Me.grpPopular.TabIndex = 4
        Me.grpPopular.TabStop = False
        Me.grpPopular.Text = "Most Popular"
        '
        'txtContents
        '
        Me.txtContents.Location = New System.Drawing.Point(60, 232)
        Me.txtContents.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtContents.Name = "txtContents"
        Me.txtContents.ReadOnly = True
        Me.txtContents.Size = New System.Drawing.Size(106, 20)
        Me.txtContents.TabIndex = 48
        '
        'lblContents
        '
        Me.lblContents.AutoSize = True
        Me.lblContents.Location = New System.Drawing.Point(4, 236)
        Me.lblContents.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblContents.Name = "lblContents"
        Me.lblContents.Size = New System.Drawing.Size(52, 13)
        Me.lblContents.TabIndex = 20
        Me.lblContents.Text = "Contents:"
        '
        'txtCondition
        '
        Me.txtCondition.Location = New System.Drawing.Point(60, 137)
        Me.txtCondition.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtCondition.Name = "txtCondition"
        Me.txtCondition.ReadOnly = True
        Me.txtCondition.Size = New System.Drawing.Size(106, 20)
        Me.txtCondition.TabIndex = 47
        '
        'lblCondition
        '
        Me.lblCondition.AutoSize = True
        Me.lblCondition.Location = New System.Drawing.Point(4, 140)
        Me.lblCondition.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCondition.Name = "lblCondition"
        Me.lblCondition.Size = New System.Drawing.Size(54, 13)
        Me.lblCondition.TabIndex = 18
        Me.lblCondition.Text = "Condition:"
        '
        'txtLength
        '
        Me.txtLength.Location = New System.Drawing.Point(60, 113)
        Me.txtLength.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtLength.Name = "txtLength"
        Me.txtLength.ReadOnly = True
        Me.txtLength.Size = New System.Drawing.Size(106, 20)
        Me.txtLength.TabIndex = 46
        '
        'lblLength
        '
        Me.lblLength.AutoSize = True
        Me.lblLength.Location = New System.Drawing.Point(4, 116)
        Me.lblLength.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLength.Name = "lblLength"
        Me.lblLength.Size = New System.Drawing.Size(43, 13)
        Me.lblLength.TabIndex = 16
        Me.lblLength.Text = "Length:"
        '
        'txtYear
        '
        Me.txtYear.Location = New System.Drawing.Point(60, 89)
        Me.txtYear.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtYear.Name = "txtYear"
        Me.txtYear.ReadOnly = True
        Me.txtYear.Size = New System.Drawing.Size(106, 20)
        Me.txtYear.TabIndex = 45
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Location = New System.Drawing.Point(4, 92)
        Me.lblYear.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(32, 13)
        Me.lblYear.TabIndex = 14
        Me.lblYear.Text = "Year:"
        '
        'txtType
        '
        Me.txtType.Location = New System.Drawing.Point(60, 65)
        Me.txtType.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtType.Name = "txtType"
        Me.txtType.ReadOnly = True
        Me.txtType.Size = New System.Drawing.Size(106, 20)
        Me.txtType.TabIndex = 42
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 68)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 12
        Me.lblType.Text = "Type:"
        '
        'txtModel
        '
        Me.txtModel.Location = New System.Drawing.Point(60, 41)
        Me.txtModel.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtModel.Name = "txtModel"
        Me.txtModel.ReadOnly = True
        Me.txtModel.Size = New System.Drawing.Size(106, 20)
        Me.txtModel.TabIndex = 44
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 44)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
        Me.lblModel.TabIndex = 10
        Me.lblModel.Text = "Model:"
        '
        'txtBrand
        '
        Me.txtBrand.Location = New System.Drawing.Point(60, 17)
        Me.txtBrand.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.ReadOnly = True
        Me.txtBrand.Size = New System.Drawing.Size(106, 20)
        Me.txtBrand.TabIndex = 43
        '
        'lblBrand
        '
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(4, 20)
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(38, 13)
        Me.lblBrand.TabIndex = 8
        Me.lblBrand.Text = "Brand:"
        '
        'txtDeck
        '
        Me.txtDeck.Location = New System.Drawing.Point(60, 161)
        Me.txtDeck.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtDeck.Name = "txtDeck"
        Me.txtDeck.ReadOnly = True
        Me.txtDeck.Size = New System.Drawing.Size(106, 20)
        Me.txtDeck.TabIndex = 41
        '
        'lblDeck
        '
        Me.lblDeck.AutoSize = True
        Me.lblDeck.Location = New System.Drawing.Point(4, 164)
        Me.lblDeck.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblDeck.Name = "lblDeck"
        Me.lblDeck.Size = New System.Drawing.Size(36, 13)
        Me.lblDeck.TabIndex = 6
        Me.lblDeck.Text = "Deck:"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(11, 484)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(120, 21)
        Me.btnRefresh.TabIndex = 5
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnGraphs
        '
        Me.btnGraphs.Location = New System.Drawing.Point(135, 484)
        Me.btnGraphs.Margin = New System.Windows.Forms.Padding(2)
        Me.btnGraphs.Name = "btnGraphs"
        Me.btnGraphs.Size = New System.Drawing.Size(120, 21)
        Me.btnGraphs.TabIndex = 6
        Me.btnGraphs.Text = "Show Graphs"
        Me.btnGraphs.UseVisualStyleBackColor = True
        '
        'txtNR
        '
        Me.txtNR.Location = New System.Drawing.Point(60, 209)
        Me.txtNR.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNR.Name = "txtNR"
        Me.txtNR.ReadOnly = True
        Me.txtNR.Size = New System.Drawing.Size(106, 20)
        Me.txtNR.TabIndex = 50
        '
        'lblNR
        '
        Me.lblNR.AutoSize = True
        Me.lblNR.Location = New System.Drawing.Point(4, 212)
        Me.lblNR.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNR.Name = "lblNR"
        Me.lblNR.Size = New System.Drawing.Size(26, 13)
        Me.lblNR.TabIndex = 49
        Me.lblNR.Text = "NR:"
        '
        'txt90sEarly
        '
        Me.txt90sEarly.Location = New System.Drawing.Point(52, 136)
        Me.txt90sEarly.Margin = New System.Windows.Forms.Padding(2)
        Me.txt90sEarly.Name = "txt90sEarly"
        Me.txt90sEarly.ReadOnly = True
        Me.txt90sEarly.Size = New System.Drawing.Size(58, 20)
        Me.txt90sEarly.TabIndex = 32
        Me.txt90sEarly.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl90sEarly
        '
        Me.lbl90sEarly.AutoSize = True
        Me.lbl90sEarly.Location = New System.Drawing.Point(4, 139)
        Me.lbl90sEarly.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl90sEarly.Name = "lbl90sEarly"
        Me.lbl90sEarly.Size = New System.Drawing.Size(41, 13)
        Me.lbl90sEarly.TabIndex = 30
        Me.lbl90sEarly.Text = "'90-'94:"
        '
        'txt80sLate
        '
        Me.txt80sLate.Location = New System.Drawing.Point(52, 112)
        Me.txt80sLate.Margin = New System.Windows.Forms.Padding(2)
        Me.txt80sLate.Name = "txt80sLate"
        Me.txt80sLate.ReadOnly = True
        Me.txt80sLate.Size = New System.Drawing.Size(58, 20)
        Me.txt80sLate.TabIndex = 31
        Me.txt80sLate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl80sLate
        '
        Me.lbl80sLate.AutoSize = True
        Me.lbl80sLate.Location = New System.Drawing.Point(4, 115)
        Me.lbl80sLate.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl80sLate.Name = "lbl80sLate"
        Me.lbl80sLate.Size = New System.Drawing.Size(41, 13)
        Me.lbl80sLate.TabIndex = 29
        Me.lbl80sLate.Text = "'85-'89:"
        '
        'txtLengthRecorded
        '
        Me.txtLengthRecorded.Location = New System.Drawing.Point(52, 113)
        Me.txtLengthRecorded.Margin = New System.Windows.Forms.Padding(2)
        Me.txtLengthRecorded.Name = "txtLengthRecorded"
        Me.txtLengthRecorded.ReadOnly = True
        Me.txtLengthRecorded.Size = New System.Drawing.Size(58, 20)
        Me.txtLengthRecorded.TabIndex = 15
        Me.txtLengthRecorded.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblLengthRecorded
        '
        Me.lblLengthRecorded.AutoSize = True
        Me.lblLengthRecorded.Location = New System.Drawing.Point(4, 116)
        Me.lblLengthRecorded.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblLengthRecorded.Name = "lblLengthRecorded"
        Me.lblLengthRecorded.Size = New System.Drawing.Size(38, 13)
        Me.lblLengthRecorded.TabIndex = 14
        Me.lblLengthRecorded.Text = "Hours:"
        '
        'txt70sLate
        '
        Me.txt70sLate.Location = New System.Drawing.Point(52, 64)
        Me.txt70sLate.Margin = New System.Windows.Forms.Padding(2)
        Me.txt70sLate.Name = "txt70sLate"
        Me.txt70sLate.ReadOnly = True
        Me.txt70sLate.Size = New System.Drawing.Size(58, 20)
        Me.txt70sLate.TabIndex = 34
        Me.txt70sLate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lbl70sLate
        '
        Me.lbl70sLate.AutoSize = True
        Me.lbl70sLate.Location = New System.Drawing.Point(4, 67)
        Me.lbl70sLate.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl70sLate.Name = "lbl70sLate"
        Me.lbl70sLate.Size = New System.Drawing.Size(41, 13)
        Me.lbl70sLate.TabIndex = 33
        Me.lbl70sLate.Text = "'75-'79:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 464)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(235, 13)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Catalogue created dd/MM/yyyy hh:mm (vx.x.xx)."
        '
        'txtInput
        '
        Me.txtInput.Location = New System.Drawing.Point(60, 185)
        Me.txtInput.Margin = New System.Windows.Forms.Padding(2)
        Me.txtInput.Name = "txtInput"
        Me.txtInput.ReadOnly = True
        Me.txtInput.Size = New System.Drawing.Size(106, 20)
        Me.txtInput.TabIndex = 52
        '
        'lblInput
        '
        Me.lblInput.AutoSize = True
        Me.lblInput.Location = New System.Drawing.Point(4, 188)
        Me.lblInput.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblInput.Name = "lblInput"
        Me.lblInput.Size = New System.Drawing.Size(34, 13)
        Me.lblInput.TabIndex = 51
        Me.lblInput.Text = "Input:"
        '
        'frmStatistics
        '
        Me.AcceptButton = Me.btnGraphs
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(442, 516)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.grpTotals)
        Me.Controls.Add(Me.btnGraphs)
        Me.Controls.Add(Me.grpPopular)
        Me.Controls.Add(Me.grpRecords)
        Me.Controls.Add(Me.btnRefresh)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.Name = "frmStatistics"
        Me.Text = "Statistics"
        Me.grpRecords.ResumeLayout(False)
        Me.grpRecordings.ResumeLayout(False)
        Me.grpRecordings.PerformLayout()
        Me.grpLengths.ResumeLayout(False)
        Me.grpLengths.PerformLayout()
        Me.grpTapes.ResumeLayout(False)
        Me.grpTapes.PerformLayout()
        Me.grpTotals.ResumeLayout(False)
        Me.grpTotals.PerformLayout()
        Me.grpContents.ResumeLayout(False)
        Me.grpContents.PerformLayout()
        Me.grpSides.ResumeLayout(False)
        Me.grpSides.PerformLayout()
        Me.grpYears.ResumeLayout(False)
        Me.grpYears.PerformLayout()
        Me.grpTypes.ResumeLayout(False)
        Me.grpTypes.PerformLayout()
        Me.grpPopular.ResumeLayout(False)
        Me.grpPopular.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpRecords As GroupBox
    Friend WithEvents txtTotal As TextBox
    Friend WithEvents lblTapes As Label
    Friend WithEvents grpTotals As GroupBox
    Friend WithEvents txtPackaged As TextBox
    Friend WithEvents lblPackaged As Label
    Friend WithEvents txtBSides As TextBox
    Friend WithEvents lblBSides As Label
    Friend WithEvents txtASides As TextBox
    Friend WithEvents lblASides As Label
    Friend WithEvents txtLengthTotal As TextBox
    Friend WithEvents lblLengthTotal As Label
    Friend WithEvents txtModels As TextBox
    Friend WithEvents lblModels As Label
    Friend WithEvents txtBrands As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents grpTypes As GroupBox
    Friend WithEvents txtType4 As TextBox
    Friend WithEvents lblType4 As Label
    Friend WithEvents txtType3 As TextBox
    Friend WithEvents lblType3 As Label
    Friend WithEvents txtType2 As TextBox
    Friend WithEvents lblType2 As Label
    Friend WithEvents txtType1 As TextBox
    Friend WithEvents lblType1 As Label
    Friend WithEvents grpYears As GroupBox
    Friend WithEvents txt10s As TextBox
    Friend WithEvents lbl10s As Label
    Friend WithEvents txt00s As TextBox
    Friend WithEvents lbl00s As Label
    Friend WithEvents txt90sLate As TextBox
    Friend WithEvents lbl90sLate As Label
    Friend WithEvents txt80sEarly As TextBox
    Friend WithEvents lbl80sEarly As Label
    Friend WithEvents txt70sEarly As TextBox
    Friend WithEvents lbl70sEarly As Label
    Friend WithEvents txt60s As TextBox
    Friend WithEvents lbl60s As Label
    Friend WithEvents grpSides As GroupBox
    Friend WithEvents grpContents As GroupBox
    Friend WithEvents txtSingles As TextBox
    Friend WithEvents lblSingles As Label
    Friend WithEvents txtEPs As TextBox
    Friend WithEvents lblEPs As Label
    Friend WithEvents txtSoundtracks As TextBox
    Friend WithEvents lblSoundtracks As Label
    Friend WithEvents txtCompilations As TextBox
    Friend WithEvents lblCompilations As Label
    Friend WithEvents txtAlbums As TextBox
    Friend WithEvents lblAlbums As Label
    Friend WithEvents txtMixtapes As TextBox
    Friend WithEvents lblMixtapes As Label
    Friend WithEvents grpLengths As GroupBox
    Friend WithEvents txtLongest As TextBox
    Friend WithEvents lblLongest As Label
    Friend WithEvents txtShortest As TextBox
    Friend WithEvents lblShortest As Label
    Friend WithEvents grpTapes As GroupBox
    Friend WithEvents txtNewestTape As TextBox
    Friend WithEvents lblNewestTape As Label
    Friend WithEvents txtOldestTape As TextBox
    Friend WithEvents lblOldestTape As Label
    Friend WithEvents grpPopular As GroupBox
    Friend WithEvents grpRecordings As GroupBox
    Friend WithEvents txtNewest As TextBox
    Friend WithEvents txtOldest As TextBox
    Friend WithEvents lblNewest As Label
    Friend WithEvents lblOldest As Label
    Friend WithEvents txtContents As TextBox
    Friend WithEvents lblContents As Label
    Friend WithEvents txtCondition As TextBox
    Friend WithEvents lblCondition As Label
    Friend WithEvents txtLength As TextBox
    Friend WithEvents lblLength As Label
    Friend WithEvents txtYear As TextBox
    Friend WithEvents lblYear As Label
    Friend WithEvents txtType As TextBox
    Friend WithEvents lblType As Label
    Friend WithEvents txtModel As TextBox
    Friend WithEvents lblModel As Label
    Friend WithEvents txtBrand As TextBox
    Friend WithEvents lblBrand As Label
    Friend WithEvents txtDeck As TextBox
    Friend WithEvents lblDeck As Label
    Friend WithEvents txt30s As TextBox
    Friend WithEvents lbl30s As Label
    Friend WithEvents txt20s As TextBox
    Friend WithEvents lbl20s As Label
    Friend WithEvents txtRecordings As TextBox
    Friend WithEvents lblRecordings As Label
    Friend WithEvents txtDecks As TextBox
    Friend WithEvents lblDecks As Label
    Friend WithEvents txtDubbed As TextBox
    Friend WithEvents lblDubbed As Label
    Friend WithEvents btnRefresh As Button
    Friend WithEvents btnGraphs As Button
    Friend WithEvents txtNR As TextBox
    Friend WithEvents lblNR As Label
    Friend WithEvents txt90sEarly As TextBox
    Friend WithEvents lbl90sEarly As Label
    Friend WithEvents txt80sLate As TextBox
    Friend WithEvents lbl80sLate As Label
    Friend WithEvents txtLengthRecorded As TextBox
    Friend WithEvents lblLengthRecorded As Label
    Friend WithEvents txt70sLate As TextBox
    Friend WithEvents lbl70sLate As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtInput As TextBox
    Friend WithEvents lblInput As Label
End Class
