﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModels
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
        Me.chkTypeBetter = New System.Windows.Forms.CheckBox()
        Me.grpModels = New System.Windows.Forms.GroupBox()
        Me.lstModels = New System.Windows.Forms.ListView()
        Me.colIdentifier = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colBrand = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colModel = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCode = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNotes = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.cmbBrand = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.cmbTypes = New System.Windows.Forms.ComboBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.grpModels.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkTypeBetter
        '
        Me.chkTypeBetter.AutoSize = True
        Me.chkTypeBetter.Enabled = False
        Me.chkTypeBetter.Location = New System.Drawing.Point(46, 66)
        Me.chkTypeBetter.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.chkTypeBetter.Name = "chkTypeBetter"
        Me.chkTypeBetter.Size = New System.Drawing.Size(101, 17)
        Me.chkTypeBetter.TabIndex = 3
        Me.chkTypeBetter.Text = "Type I or better."
        Me.chkTypeBetter.UseVisualStyleBackColor = True
        '
        'grpModels
        '
        Me.grpModels.Controls.Add(Me.lstModels)
        Me.grpModels.Location = New System.Drawing.Point(189, 10)
        Me.grpModels.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpModels.Name = "grpModels"
        Me.grpModels.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpModels.Size = New System.Drawing.Size(537, 403)
        Me.grpModels.TabIndex = 2
        Me.grpModels.TabStop = False
        Me.grpModels.Text = "Models"
        '
        'lstModels
        '
        Me.lstModels.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colIdentifier, Me.colBrand, Me.colType, Me.colModel, Me.colCode, Me.colName, Me.colNotes})
        Me.lstModels.HideSelection = False
        Me.lstModels.Location = New System.Drawing.Point(4, 17)
        Me.lstModels.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.lstModels.Name = "lstModels"
        Me.lstModels.Size = New System.Drawing.Size(529, 382)
        Me.lstModels.TabIndex = 0
        Me.lstModels.UseCompatibleStateImageBehavior = False
        Me.lstModels.View = System.Windows.Forms.View.Details
        '
        'colIdentifier
        '
        Me.colIdentifier.Text = "Identifier"
        Me.colIdentifier.Width = 82
        '
        'colBrand
        '
        Me.colBrand.Text = "Brand"
        Me.colBrand.Width = 141
        '
        'colType
        '
        Me.colType.Text = "Type"
        Me.colType.Width = 69
        '
        'colModel
        '
        Me.colModel.Text = "Model"
        Me.colModel.Width = 83
        '
        'colCode
        '
        Me.colCode.Text = "Code"
        Me.colCode.Width = 51
        '
        'colName
        '
        Me.colName.Text = "Full Name"
        Me.colName.Width = 133
        '
        'colNotes
        '
        Me.colNotes.Text = "Notes"
        Me.colNotes.Width = 140
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Location = New System.Drawing.Point(9, 327)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpActions.Size = New System.Drawing.Size(176, 85)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(4, 17)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(166, 20)
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
        Me.Label1.Size = New System.Drawing.Size(155, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Confirm deletions via main form."
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(4, 42)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(166, 20)
        Me.btnDelete.TabIndex = 2
        Me.btnDelete.Text = "Delete Model"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.txtName)
        Me.grpFilters.Controls.Add(Me.lblName)
        Me.grpFilters.Controls.Add(Me.chkTypeBetter)
        Me.grpFilters.Controls.Add(Me.TextBox1)
        Me.grpFilters.Controls.Add(Me.lblNotes)
        Me.grpFilters.Controls.Add(Me.cmbBrand)
        Me.grpFilters.Controls.Add(Me.lblModel)
        Me.grpFilters.Controls.Add(Me.lblBrand)
        Me.grpFilters.Controls.Add(Me.cmbTypes)
        Me.grpFilters.Controls.Add(Me.lblType)
        Me.grpFilters.Controls.Add(Me.cmbModel)
        Me.grpFilters.Location = New System.Drawing.Point(9, 10)
        Me.grpFilters.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpFilters.Size = New System.Drawing.Size(176, 158)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(46, 112)
        Me.txtName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(126, 20)
        Me.txtName.TabIndex = 5
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(4, 115)
        Me.lblName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 27
        Me.lblName.Text = "Name:"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(46, 135)
        Me.TextBox1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(126, 20)
        Me.TextBox1.TabIndex = 6
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(4, 137)
        Me.lblNotes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(38, 13)
        Me.lblNotes.TabIndex = 8
        Me.lblNotes.Text = "Notes:"
        '
        'cmbBrand
        '
        Me.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBrand.FormattingEnabled = True
        Me.cmbBrand.Items.AddRange(New Object() {"All Brands"})
        Me.cmbBrand.Location = New System.Drawing.Point(46, 17)
        Me.cmbBrand.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbBrand.Name = "cmbBrand"
        Me.cmbBrand.Size = New System.Drawing.Size(126, 21)
        Me.cmbBrand.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 90)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
        Me.lblModel.TabIndex = 0
        Me.lblModel.Text = "Model:"
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
        'cmbTypes
        '
        Me.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTypes.FormattingEnabled = True
        Me.cmbTypes.Items.AddRange(New Object() {"All Types", "Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbTypes.Location = New System.Drawing.Point(46, 41)
        Me.cmbTypes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbTypes.Name = "cmbTypes"
        Me.cmbTypes.Size = New System.Drawing.Size(126, 21)
        Me.cmbTypes.TabIndex = 2
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 44)
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
        Me.cmbModel.Location = New System.Drawing.Point(46, 88)
        Me.cmbModel.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(126, 21)
        Me.cmbModel.TabIndex = 4
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(9, 307)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 30
        Me.lblResults.Text = "Results (Filtered):"
        '
        'TextBox2
        '
        Me.TextBox2.Enabled = False
        Me.TextBox2.Location = New System.Drawing.Point(101, 305)
        Me.TextBox2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(84, 20)
        Me.TextBox2.TabIndex = 4
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'frmModels
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(735, 422)
        Me.Controls.Add(Me.lblResults)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.grpModels)
        Me.Controls.Add(Me.grpActions)
        Me.Controls.Add(Me.grpFilters)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.Name = "frmModels"
        Me.Text = "View Models"
        Me.grpModels.ResumeLayout(False)
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.grpFilters.ResumeLayout(False)
        Me.grpFilters.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents chkTypeBetter As CheckBox
    Friend WithEvents grpModels As GroupBox
    Friend WithEvents lstModels As ListView
    Friend WithEvents colIdentifier As ColumnHeader
    Friend WithEvents colBrand As ColumnHeader
    Friend WithEvents colModel As ColumnHeader
    Friend WithEvents colType As ColumnHeader
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnDelete As Button
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents TextBox1 As TextBox
    Private WithEvents lblNotes As Label
    Friend WithEvents cmbTypes As ComboBox
    Private WithEvents lblType As Label
    Friend WithEvents cmbModel As ComboBox
    Friend WithEvents cmbBrand As ComboBox
    Private WithEvents lblBrand As Label
    Private WithEvents lblModel As Label
    Friend WithEvents colName As ColumnHeader
    Friend WithEvents colNotes As ColumnHeader
    Friend WithEvents txtName As TextBox
    Private WithEvents lblName As Label
    Friend WithEvents colCode As ColumnHeader
    Friend WithEvents lblResults As Label
    Friend WithEvents TextBox2 As TextBox
End Class
