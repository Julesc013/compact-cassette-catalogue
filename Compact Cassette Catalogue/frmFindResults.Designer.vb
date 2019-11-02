<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFindResults
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFindResults))
        Me.lblResults = New System.Windows.Forms.Label()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
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
        Me.SuspendLayout()
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(12, 375)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(120, 17)
        Me.lblResults.TabIndex = 32
        Me.lblResults.Text = "Results (Filtered):"
        '
        'btnEdit
        '
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(907, 371)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(193, 25)
        Me.btnEdit.TabIndex = 2
        Me.btnEdit.Text = "Edit Tape"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Enabled = False
        Me.TextBox2.Location = New System.Drawing.Point(138, 372)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(105, 22)
        Me.TextBox2.TabIndex = 2
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lstTapes
        '
        Me.lstTapes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colIdentifier, Me.colName, Me.colBrand, Me.colModel, Me.colType, Me.colYear, Me.colLength, Me.colCondition, Me.colRecorded, Me.colContents, Me.colNoise, Me.colAlbum, Me.colTitle, Me.colNotes})
        Me.lstTapes.Location = New System.Drawing.Point(12, 12)
        Me.lstTapes.Name = "lstTapes"
        Me.lstTapes.Size = New System.Drawing.Size(1088, 353)
        Me.lstTapes.TabIndex = 1
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
        'frmFindResults
        '
        Me.AcceptButton = Me.btnEdit
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1112, 407)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.btnEdit)
        Me.Controls.Add(Me.lstTapes)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmFindResults"
        Me.Text = "Find Results"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblResults As Label
    Friend WithEvents btnEdit As Button
    Friend WithEvents TextBox2 As TextBox
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
End Class
