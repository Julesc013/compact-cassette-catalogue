<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFindResults
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFindResults))
        Me.tlpFindRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.lstTapes = New System.Windows.Forms.ListView()
        Me.colIdentifier = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colBrand = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colModel = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colYear = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colLength = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCondition = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colRecorded = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colContents = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNoise = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAlbum = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colTitle = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNotes = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.tlpFindFooter = New System.Windows.Forms.TableLayoutPanel()
        Me.flpFindStatus = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.flpFindCommands = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.tlpFindRoot.SuspendLayout()
        Me.tlpFindFooter.SuspendLayout()
        Me.flpFindStatus.SuspendLayout()
        Me.flpFindCommands.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpFindRoot
        '
        Me.tlpFindRoot.ColumnCount = 1
        Me.tlpFindRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFindRoot.Controls.Add(Me.lstTapes, 0, 0)
        Me.tlpFindRoot.Controls.Add(Me.tlpFindFooter, 0, 1)
        Me.tlpFindRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpFindRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpFindRoot.Name = "tlpFindRoot"
        Me.tlpFindRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpFindRoot.RowCount = 2
        Me.tlpFindRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFindRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpFindRoot.Size = New System.Drawing.Size(1000, 430)
        Me.tlpFindRoot.TabIndex = 0
        '
        'lstTapes
        '
        Me.lstTapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colIdentifier, Me.colName, Me.colBrand, Me.colModel, Me.colType, Me.colYear, Me.colLength, Me.colCondition, Me.colRecorded, Me.colContents, Me.colNoise, Me.colAlbum, Me.colTitle, Me.colNotes})
        Me.lstTapes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstTapes.HideSelection = False
        Me.lstTapes.Location = New System.Drawing.Point(12, 12)
        Me.lstTapes.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.lstTapes.Name = "lstTapes"
        Me.lstTapes.Size = New System.Drawing.Size(976, 371)
        Me.lstTapes.TabIndex = 0
        Me.lstTapes.UseCompatibleStateImageBehavior = False
        Me.lstTapes.View = System.Windows.Forms.View.Details
        '
        'colIdentifier
        '
        Me.colIdentifier.Text = "Identifier"
        Me.colIdentifier.Width = 82
        '
        'colName
        '
        Me.colName.Text = "Name"
        Me.colName.Width = 165
        '
        'colBrand
        '
        Me.colBrand.Text = "Brand"
        Me.colBrand.Width = 91
        '
        'colModel
        '
        Me.colModel.Text = "Model"
        Me.colModel.Width = 62
        '
        'colType
        '
        Me.colType.Text = "Type"
        Me.colType.Width = 51
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
        Me.colCondition.Width = 53
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
        'colNotes
        '
        Me.colNotes.Text = "Notes"
        Me.colNotes.Width = 90
        '
        'tlpFindFooter
        '
        Me.tlpFindFooter.AutoSize = True
        Me.tlpFindFooter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpFindFooter.ColumnCount = 2
        Me.tlpFindFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFindFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpFindFooter.Controls.Add(Me.flpFindStatus, 0, 0)
        Me.tlpFindFooter.Controls.Add(Me.flpFindCommands, 1, 0)
        Me.tlpFindFooter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpFindFooter.Location = New System.Drawing.Point(12, 391)
        Me.tlpFindFooter.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpFindFooter.Name = "tlpFindFooter"
        Me.tlpFindFooter.RowCount = 1
        Me.tlpFindFooter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpFindFooter.Size = New System.Drawing.Size(976, 27)
        Me.tlpFindFooter.TabIndex = 1
        '
        'flpFindStatus
        '
        Me.flpFindStatus.AutoSize = True
        Me.flpFindStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpFindStatus.Controls.Add(Me.lblResults)
        Me.flpFindStatus.Controls.Add(Me.TextBox2)
        Me.flpFindStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpFindStatus.Location = New System.Drawing.Point(0, 0)
        Me.flpFindStatus.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.flpFindStatus.Name = "flpFindStatus"
        Me.flpFindStatus.Size = New System.Drawing.Size(806, 27)
        Me.flpFindStatus.TabIndex = 0
        Me.flpFindStatus.WrapContents = False
        '
        'lblResults
        '
        Me.lblResults.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(0, 7)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 0
        Me.lblResults.Text = "Results (Filtered):"
        '
        'TextBox2
        '
        Me.TextBox2.Enabled = False
        Me.TextBox2.Location = New System.Drawing.Point(94, 3)
        Me.TextBox2.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(87, 20)
        Me.TextBox2.TabIndex = 0
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'flpFindCommands
        '
        Me.flpFindCommands.AutoSize = True
        Me.flpFindCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpFindCommands.Controls.Add(Me.btnEdit)
        Me.flpFindCommands.Controls.Add(Me.btnDelete)
        Me.flpFindCommands.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpFindCommands.Location = New System.Drawing.Point(814, 0)
        Me.flpFindCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.flpFindCommands.Name = "flpFindCommands"
        Me.flpFindCommands.Size = New System.Drawing.Size(162, 27)
        Me.flpFindCommands.TabIndex = 1
        Me.flpFindCommands.WrapContents = False
        '
        'btnEdit
        '
        Me.btnEdit.AutoSize = True
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(0, 0)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.btnEdit.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(75, 27)
        Me.btnEdit.TabIndex = 0
        Me.btnEdit.Text = "&Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.AutoSize = True
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(81, 0)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDelete.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(75, 27)
        Me.btnDelete.TabIndex = 1
        Me.btnDelete.Text = "&Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'frmFindResults
        '
        Me.AcceptButton = Me.btnEdit
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 430)
        Me.Controls.Add(Me.tlpFindRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(640, 360)
        Me.Name = "frmFindResults"
        Me.Text = "Find Results"
        Me.tlpFindRoot.ResumeLayout(False)
        Me.tlpFindRoot.PerformLayout()
        Me.tlpFindFooter.ResumeLayout(False)
        Me.tlpFindFooter.PerformLayout()
        Me.flpFindStatus.ResumeLayout(False)
        Me.flpFindStatus.PerformLayout()
        Me.flpFindCommands.ResumeLayout(False)
        Me.flpFindCommands.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpFindRoot As TableLayoutPanel
    Friend WithEvents lstTapes As ListView
    Friend WithEvents colIdentifier As ColumnHeader
    Friend WithEvents colName As ColumnHeader
    Friend WithEvents colBrand As ColumnHeader
    Friend WithEvents colModel As ColumnHeader
    Friend WithEvents colType As ColumnHeader
    Friend WithEvents colYear As ColumnHeader
    Friend WithEvents colLength As ColumnHeader
    Friend WithEvents colCondition As ColumnHeader
    Friend WithEvents colRecorded As ColumnHeader
    Friend WithEvents colContents As ColumnHeader
    Friend WithEvents colNoise As ColumnHeader
    Friend WithEvents colAlbum As ColumnHeader
    Friend WithEvents colTitle As ColumnHeader
    Friend WithEvents colNotes As ColumnHeader
    Friend WithEvents tlpFindFooter As TableLayoutPanel
    Friend WithEvents flpFindStatus As FlowLayoutPanel
    Friend WithEvents lblResults As Label
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents flpFindCommands As FlowLayoutPanel
    Friend WithEvents btnEdit As Button
    Friend WithEvents btnDelete As Button
End Class
