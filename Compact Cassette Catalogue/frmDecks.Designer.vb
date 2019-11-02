<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDecks
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDecks))
        Me.numYearMin = New System.Windows.Forms.NumericUpDown()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.grpTapes = New System.Windows.Forms.GroupBox()
        Me.lstTapes = New System.Windows.Forms.ListView()
        Me.colManufacturer = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDeck = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colYear = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
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
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.numYearMax = New System.Windows.Forms.NumericUpDown()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.txtManufacturer = New System.Windows.Forms.TextBox()
        Me.lblYearTo = New System.Windows.Forms.Label()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        CType(Me.numYearMin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTapes.SuspendLayout()
        Me.grpActions.SuspendLayout()
        CType(Me.numYearMax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpBasic.SuspendLayout()
        Me.grpFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'numYearMin
        '
        Me.numYearMin.Location = New System.Drawing.Point(54, 49)
        Me.numYearMin.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYearMin.Minimum = New Decimal(New Integer() {1962, 0, 0, 0})
        Me.numYearMin.Name = "numYearMin"
        Me.numYearMin.Size = New System.Drawing.Size(66, 22)
        Me.numYearMin.TabIndex = 2
        Me.numYearMin.Value = New Decimal(New Integer() {1962, 0, 0, 0})
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(12, 218)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(120, 17)
        Me.lblResults.TabIndex = 33
        Me.lblResults.Text = "Results (Filtered):"
        '
        'grpTapes
        '
        Me.grpTapes.Controls.Add(Me.lstTapes)
        Me.grpTapes.Location = New System.Drawing.Point(252, 12)
        Me.grpTapes.Name = "grpTapes"
        Me.grpTapes.Size = New System.Drawing.Size(1258, 336)
        Me.grpTapes.TabIndex = 2
        Me.grpTapes.TabStop = False
        Me.grpTapes.Text = "Tapes"
        '
        'lstTapes
        '
        Me.lstTapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colManufacturer, Me.colDeck, Me.colYear, Me.colTypes, Me.colHeads, Me.colSpeeds, Me.colNRs, Me.colHX, Me.colMPX, Me.colStereo, Me.colWells, Me.colDubbing, Me.colAutoReverse, Me.colProgramSearch, Me.colCalibration, Me.colAzimuth, Me.colFrequency, Me.colSignalRatio, Me.colWowFlutter, Me.colDistortion, Me.colCondition})
        Me.lstTapes.Location = New System.Drawing.Point(6, 21)
        Me.lstTapes.Name = "lstTapes"
        Me.lstTapes.Size = New System.Drawing.Size(1246, 309)
        Me.lstTapes.TabIndex = 0
        Me.lstTapes.UseCompatibleStateImageBehavior = False
        Me.lstTapes.View = System.Windows.Forms.View.Details
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
        'colYear
        '
        Me.colYear.Text = "Year"
        Me.colYear.Width = 47
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
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Location = New System.Drawing.Point(12, 243)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Size = New System.Drawing.Size(234, 105)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(6, 21)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(222, 25)
        Me.btnRefresh.TabIndex = 1
        Me.btnRefresh.Text = "Refresh List"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 80)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(209, 17)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Confirm deletions via main form."
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(6, 52)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(222, 25)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete Deck"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Enabled = False
        Me.TextBox2.Location = New System.Drawing.Point(135, 215)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(111, 22)
        Me.TextBox2.TabIndex = 4
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'numYearMax
        '
        Me.numYearMax.Location = New System.Drawing.Point(150, 49)
        Me.numYearMax.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.numYearMax.Minimum = New Decimal(New Integer() {1962, 0, 0, 0})
        Me.numYearMax.Name = "numYearMax"
        Me.numYearMax.Size = New System.Drawing.Size(66, 22)
        Me.numYearMax.TabIndex = 3
        Me.numYearMax.Value = New Decimal(New Integer() {1962, 0, 0, 0})
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.txtManufacturer)
        Me.grpBasic.Controls.Add(Me.numYearMin)
        Me.grpBasic.Controls.Add(Me.numYearMax)
        Me.grpBasic.Controls.Add(Me.lblYearTo)
        Me.grpBasic.Controls.Add(Me.lblYear)
        Me.grpBasic.Controls.Add(Me.lblManufacturer)
        Me.grpBasic.Location = New System.Drawing.Point(6, 21)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Size = New System.Drawing.Size(222, 78)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'txtManufacturer
        '
        Me.txtManufacturer.Location = New System.Drawing.Point(108, 21)
        Me.txtManufacturer.MaxLength = 100
        Me.txtManufacturer.Name = "txtManufacturer"
        Me.txtManufacturer.Size = New System.Drawing.Size(108, 22)
        Me.txtManufacturer.TabIndex = 1
        '
        'lblYearTo
        '
        Me.lblYearTo.AutoSize = True
        Me.lblYearTo.Location = New System.Drawing.Point(126, 51)
        Me.lblYearTo.Name = "lblYearTo"
        Me.lblYearTo.Size = New System.Drawing.Size(20, 17)
        Me.lblYearTo.TabIndex = 11
        Me.lblYearTo.Text = "to"
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Location = New System.Drawing.Point(6, 51)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(42, 17)
        Me.lblYear.TabIndex = 2
        Me.lblYear.Text = "Year:"
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Location = New System.Drawing.Point(6, 24)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(96, 17)
        Me.lblManufacturer.TabIndex = 1
        Me.lblManufacturer.Text = "Manufacturer:"
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.grpBasic)
        Me.grpFilters.Location = New System.Drawing.Point(12, 12)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Size = New System.Drawing.Size(234, 105)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'frmDecks
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1522, 360)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.grpTapes)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.grpFilters)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmDecks"
        Me.Text = "View Decks"
        CType(Me.numYearMin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTapes.ResumeLayout(False)
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        CType(Me.numYearMax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.grpFilters.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents numYearMin As NumericUpDown
    Friend WithEvents lblResults As Label
    Friend WithEvents grpTapes As GroupBox
    Friend WithEvents lstTapes As ListView
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnDelete As Button
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents numYearMax As NumericUpDown
    Friend WithEvents grpBasic As GroupBox
    Private WithEvents lblYearTo As Label
    Private WithEvents lblYear As Label
    Private WithEvents lblManufacturer As Label
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents colManufacturer As ColumnHeader
    Friend WithEvents colDeck As ColumnHeader
    Friend WithEvents colYear As ColumnHeader
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
    Friend WithEvents txtManufacturer As TextBox
    Friend WithEvents colCondition As ColumnHeader
End Class
