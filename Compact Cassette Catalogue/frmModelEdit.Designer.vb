<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModelEdit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmModelEdit))
        Me.tlpDialogRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.tlpBasicFields = New System.Windows.Forms.TableLayoutPanel()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.txtType = New System.Windows.Forms.TextBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.grpExtra = New System.Windows.Forms.GroupBox()
        Me.tlpExtraFields = New System.Windows.Forms.TableLayoutPanel()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.tlpDialogFooter = New System.Windows.Forms.TableLayoutPanel()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.flpDialogCommands = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tlpDialogRoot.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        Me.tlpBasicFields.SuspendLayout()
        Me.grpExtra.SuspendLayout()
        Me.tlpExtraFields.SuspendLayout()
        Me.grpNotes.SuspendLayout()
        Me.tlpDialogFooter.SuspendLayout()
        Me.flpDialogCommands.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpDialogRoot
        '
        Me.tlpDialogRoot.AutoSize = True
        Me.tlpDialogRoot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpDialogRoot.ColumnCount = 1
        Me.tlpDialogRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDialogRoot.Controls.Add(Me.grpBasic, 0, 0)
        Me.tlpDialogRoot.Controls.Add(Me.grpExtra, 0, 1)
        Me.tlpDialogRoot.Controls.Add(Me.grpNotes, 0, 2)
        Me.tlpDialogRoot.Controls.Add(Me.tlpDialogFooter, 0, 3)
        Me.tlpDialogRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDialogRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpDialogRoot.Name = "tlpDialogRoot"
        Me.tlpDialogRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpDialogRoot.RowCount = 4
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118.0!))
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogRoot.Size = New System.Drawing.Size(560, 397)
        Me.tlpDialogRoot.TabIndex = 0
        '
        'grpBasic
        '
        Me.grpBasic.AutoSize = True
        Me.grpBasic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.grpBasic.Controls.Add(Me.tlpBasicFields)
        Me.grpBasic.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(8)
        Me.grpBasic.TabIndex = 0
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'tlpBasicFields
        '
        Me.tlpBasicFields.AutoSize = True
        Me.tlpBasicFields.ColumnCount = 2
        Me.tlpBasicFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpBasicFields.Controls.Add(Me.lblBrand, 0, 0)
        Me.tlpBasicFields.Controls.Add(Me.txtBrand, 1, 0)
        Me.tlpBasicFields.Controls.Add(Me.lblType, 0, 1)
        Me.tlpBasicFields.Controls.Add(Me.txtType, 1, 1)
        Me.tlpBasicFields.Controls.Add(Me.lblModel, 0, 2)
        Me.tlpBasicFields.Controls.Add(Me.txtModel, 1, 2)
        Me.tlpBasicFields.Controls.Add(Me.lblCode, 0, 3)
        Me.tlpBasicFields.Controls.Add(Me.txtCode, 1, 3)
        Me.tlpBasicFields.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpBasicFields.Name = "tlpBasicFields"
        Me.tlpBasicFields.RowCount = 4
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.TabIndex = 0
        '
        'lblBrand
        '
        Me.lblBrand.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(0, 0, 6, 6)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.TabIndex = 0
        Me.lblBrand.Text = "Brand:"
        '
        'txtBrand
        '
        Me.txtBrand.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBrand.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.txtBrand.MaxLength = 100
        Me.txtBrand.MinimumSize = New System.Drawing.Size(300, 20)
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.ReadOnly = True
        Me.txtBrand.TabIndex = 0
        '
        'lblType
        '
        Me.lblType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblType.AutoSize = True
        Me.lblType.Margin = New System.Windows.Forms.Padding(0, 0, 6, 6)
        Me.lblType.Name = "lblType"
        Me.lblType.TabIndex = 1
        Me.lblType.Text = "Type:"
        '
        'txtType
        '
        Me.txtType.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtType.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.txtType.MaxLength = 100
        Me.txtType.Name = "txtType"
        Me.txtType.ReadOnly = True
        Me.txtType.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblModel.AutoSize = True
        Me.lblModel.Margin = New System.Windows.Forms.Padding(0, 0, 6, 6)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.TabIndex = 2
        Me.lblModel.Text = "Model:"
        '
        'txtModel
        '
        Me.txtModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtModel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.txtModel.MaxLength = 100
        Me.txtModel.Name = "txtModel"
        Me.txtModel.ReadOnly = True
        Me.txtModel.TabIndex = 2
        '
        'lblCode
        '
        Me.lblCode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblCode.AutoSize = True
        Me.lblCode.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.TabIndex = 3
        Me.lblCode.Text = "Code:"
        '
        'txtCode
        '
        Me.txtCode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.txtCode.Margin = New System.Windows.Forms.Padding(0)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.ReadOnly = True
        Me.txtCode.Size = New System.Drawing.Size(64, 20)
        Me.txtCode.TabIndex = 3
        '
        'grpExtra
        '
        Me.grpExtra.AutoSize = True
        Me.grpExtra.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.grpExtra.Controls.Add(Me.tlpExtraFields)
        Me.grpExtra.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpExtra.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.grpExtra.Name = "grpExtra"
        Me.grpExtra.Padding = New System.Windows.Forms.Padding(8)
        Me.grpExtra.TabIndex = 1
        Me.grpExtra.TabStop = False
        Me.grpExtra.Text = "Extra"
        '
        'tlpExtraFields
        '
        Me.tlpExtraFields.AutoSize = True
        Me.tlpExtraFields.ColumnCount = 2
        Me.tlpExtraFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpExtraFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpExtraFields.Controls.Add(Me.lblName, 0, 0)
        Me.tlpExtraFields.Controls.Add(Me.txtName, 1, 0)
        Me.tlpExtraFields.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpExtraFields.Name = "tlpExtraFields"
        Me.tlpExtraFields.RowCount = 1
        Me.tlpExtraFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpExtraFields.TabIndex = 0
        '
        'lblName
        '
        Me.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblName.AutoSize = True
        Me.lblName.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.TabIndex = 0
        Me.lblName.Text = "Name:"
        '
        'txtName
        '
        Me.txtName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtName.Margin = New System.Windows.Forms.Padding(0)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.TabIndex = 0
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(8)
        Me.grpNotes.TabIndex = 2
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'txtNotes
        '
        Me.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtNotes.TabIndex = 0
        '
        'tlpDialogFooter
        '
        Me.tlpDialogFooter.AutoSize = True
        Me.tlpDialogFooter.ColumnCount = 2
        Me.tlpDialogFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDialogFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogFooter.Controls.Add(Me.lblAdd, 0, 0)
        Me.tlpDialogFooter.Controls.Add(Me.flpDialogCommands, 1, 0)
        Me.tlpDialogFooter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDialogFooter.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpDialogFooter.Name = "tlpDialogFooter"
        Me.tlpDialogFooter.RowCount = 1
        Me.tlpDialogFooter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogFooter.TabIndex = 3
        '
        'lblAdd
        '
        Me.lblAdd.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.TabIndex = 0
        Me.lblAdd.Text = "Changes are saved with the catalogue."
        '
        'flpDialogCommands
        '
        Me.flpDialogCommands.AutoSize = True
        Me.flpDialogCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpDialogCommands.Controls.Add(Me.btnUpdate)
        Me.flpDialogCommands.Controls.Add(Me.btnCancel)
        Me.flpDialogCommands.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpDialogCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.flpDialogCommands.Name = "flpDialogCommands"
        Me.flpDialogCommands.TabIndex = 0
        Me.flpDialogCommands.WrapContents = False
        '
        'btnUpdate
        '
        Me.btnUpdate.AutoSize = True
        Me.btnUpdate.MinimumSize = New System.Drawing.Size(90, 27)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.TabIndex = 0
        Me.btnUpdate.Text = "&Update Model"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleDescription = "Cancel editing and return without applying further changes."
        Me.btnCancel.AccessibleName = "Cancel"
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmModelEdit
        '
        Me.AcceptButton = Me.btnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(560, 397)
        Me.Controls.Add(Me.tlpDialogRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmModelEdit"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Edit Model"
        Me.tlpDialogRoot.ResumeLayout(False)
        Me.tlpDialogRoot.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.tlpBasicFields.ResumeLayout(False)
        Me.tlpBasicFields.PerformLayout()
        Me.grpExtra.ResumeLayout(False)
        Me.grpExtra.PerformLayout()
        Me.tlpExtraFields.ResumeLayout(False)
        Me.tlpExtraFields.PerformLayout()
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.tlpDialogFooter.ResumeLayout(False)
        Me.tlpDialogFooter.PerformLayout()
        Me.flpDialogCommands.ResumeLayout(False)
        Me.flpDialogCommands.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents tlpDialogRoot As TableLayoutPanel
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents tlpBasicFields As TableLayoutPanel
    Friend WithEvents lblBrand As Label
    Friend WithEvents txtBrand As TextBox
    Private WithEvents lblType As Label
    Friend WithEvents txtType As TextBox
    Friend WithEvents lblModel As Label
    Friend WithEvents txtModel As TextBox
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents grpExtra As GroupBox
    Friend WithEvents tlpExtraFields As TableLayoutPanel
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents tlpDialogFooter As TableLayoutPanel
    Friend WithEvents lblAdd As Label
    Friend WithEvents flpDialogCommands As FlowLayoutPanel
    Friend WithEvents btnUpdate As Button
    Friend WithEvents btnCancel As Button
End Class
