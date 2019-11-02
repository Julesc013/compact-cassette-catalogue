<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmViewBrands
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewBrands))
        Me.colCode = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.colNotes = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colBrand = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.grpModels = New System.Windows.Forms.GroupBox()
        Me.lstModels = New System.Windows.Forms.ListView()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.grpFilters.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpModels.SuspendLayout()
        Me.SuspendLayout()
        '
        'colCode
        '
        Me.colCode.Text = "Code"
        Me.colCode.Width = 51
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(61, 21)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(167, 22)
        Me.TextBox1.TabIndex = 1
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(6, 24)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(49, 17)
        Me.lblNotes.TabIndex = 8
        Me.lblNotes.Text = "Notes:"
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
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.TextBox1)
        Me.grpFilters.Controls.Add(Me.lblNotes)
        Me.grpFilters.Location = New System.Drawing.Point(12, 12)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Size = New System.Drawing.Size(234, 49)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(6, 52)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(222, 25)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete Brand"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Location = New System.Drawing.Point(12, 276)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Size = New System.Drawing.Size(234, 105)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'colNotes
        '
        Me.colNotes.Text = "Notes"
        Me.colNotes.Width = 218
        '
        'colBrand
        '
        Me.colBrand.Text = "Brand"
        Me.colBrand.Width = 169
        '
        'grpModels
        '
        Me.grpModels.Controls.Add(Me.lstModels)
        Me.grpModels.Location = New System.Drawing.Point(252, 12)
        Me.grpModels.Name = "grpModels"
        Me.grpModels.Size = New System.Drawing.Size(457, 369)
        Me.grpModels.TabIndex = 2
        Me.grpModels.TabStop = False
        Me.grpModels.Text = "Models"
        '
        'lstModels
        '
        Me.lstModels.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colCode, Me.colBrand, Me.colNotes})
        Me.lstModels.Location = New System.Drawing.Point(6, 21)
        Me.lstModels.Name = "lstModels"
        Me.lstModels.Size = New System.Drawing.Size(445, 342)
        Me.lstModels.TabIndex = 0
        Me.lstModels.UseCompatibleStateImageBehavior = False
        Me.lstModels.View = System.Windows.Forms.View.Details
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(12, 251)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(120, 17)
        Me.lblResults.TabIndex = 35
        Me.lblResults.Text = "Results (Filtered):"
        '
        'TextBox2
        '
        Me.TextBox2.Enabled = False
        Me.TextBox2.Location = New System.Drawing.Point(135, 248)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(111, 22)
        Me.TextBox2.TabIndex = 4
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'frmViewBrands
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(721, 392)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.grpFilters)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.grpModels)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmViewBrands"
        Me.Text = "View Brands"
        Me.grpFilters.ResumeLayout(False)
        Me.grpFilters.PerformLayout()
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.grpModels.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents colCode As ColumnHeader
    Friend WithEvents TextBox1 As TextBox
    Private WithEvents lblNotes As Label
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents btnDelete As Button
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents colNotes As ColumnHeader
    Friend WithEvents colBrand As ColumnHeader
    Friend WithEvents grpModels As GroupBox
    Friend WithEvents lstModels As ListView
    Friend WithEvents lblResults As Label
    Friend WithEvents TextBox2 As TextBox
End Class
