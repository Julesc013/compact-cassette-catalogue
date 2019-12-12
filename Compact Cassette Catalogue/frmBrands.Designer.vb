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
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.grpModels = New System.Windows.Forms.GroupBox()
        Me.lstBrands = New System.Windows.Forms.ListView()
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblResults = New System.Windows.Forms.Label()
        Me.txtResults = New System.Windows.Forms.TextBox()
        Me.grpFilters.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpModels.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(46, 17)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(145, 20)
        Me.txtNotes.TabIndex = 1
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(4, 20)
        Me.lblNotes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(38, 13)
        Me.lblNotes.TabIndex = 8
        Me.lblNotes.Text = "Notes:"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(4, 17)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(188, 21)
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
        Me.Label1.Size = New System.Drawing.Size(154, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Confirm changes via main form."
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.txtNotes)
        Me.grpFilters.Controls.Add(Me.lblNotes)
        Me.grpFilters.Location = New System.Drawing.Point(9, 10)
        Me.grpFilters.Margin = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Padding = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Size = New System.Drawing.Size(196, 43)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(99, 42)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(93, 21)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnEdit)
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Location = New System.Drawing.Point(9, 250)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2)
        Me.grpActions.Size = New System.Drawing.Size(196, 85)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnEdit
        '
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(4, 42)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(93, 21)
        Me.btnEdit.TabIndex = 14
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'grpModels
        '
        Me.grpModels.Controls.Add(Me.lstBrands)
        Me.grpModels.Location = New System.Drawing.Point(209, 11)
        Me.grpModels.Margin = New System.Windows.Forms.Padding(2)
        Me.grpModels.Name = "grpModels"
        Me.grpModels.Padding = New System.Windows.Forms.Padding(2)
        Me.grpModels.Size = New System.Drawing.Size(428, 324)
        Me.grpModels.TabIndex = 2
        Me.grpModels.TabStop = False
        Me.grpModels.Text = "Models"
        '
        'lstBrands
        '
        Me.lstBrands.AllowColumnReorder = True
        Me.lstBrands.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader9, Me.ColumnHeader10, Me.ColumnHeader11})
        Me.lstBrands.HideSelection = False
        Me.lstBrands.Location = New System.Drawing.Point(4, 17)
        Me.lstBrands.Margin = New System.Windows.Forms.Padding(2)
        Me.lstBrands.Name = "lstBrands"
        Me.lstBrands.Size = New System.Drawing.Size(420, 302)
        Me.lstBrands.TabIndex = 45
        Me.lstBrands.UseCompatibleStateImageBehavior = False
        Me.lstBrands.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Code"
        Me.ColumnHeader9.Width = 51
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Brand"
        Me.ColumnHeader10.Width = 135
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "Notes"
        Me.ColumnHeader11.Width = 219
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(6, 229)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 35
        Me.lblResults.Text = "Results (Filtered):"
        '
        'txtResults
        '
        Me.txtResults.Location = New System.Drawing.Point(98, 226)
        Me.txtResults.Margin = New System.Windows.Forms.Padding(2)
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ReadOnly = True
        Me.txtResults.Size = New System.Drawing.Size(107, 20)
        Me.txtResults.TabIndex = 4
        Me.txtResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'frmViewBrands
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(647, 343)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.txtResults)
        Me.Controls.Add(Me.grpFilters)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.grpModels)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
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
    Friend WithEvents txtNotes As TextBox
    Private WithEvents lblNotes As Label
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents btnDelete As Button
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents grpModels As GroupBox
    Friend WithEvents lblResults As Label
    Friend WithEvents txtResults As TextBox
    Friend WithEvents btnEdit As Button
    Friend WithEvents lstBrands As ListView
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents ColumnHeader11 As ColumnHeader
End Class
