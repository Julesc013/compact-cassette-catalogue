<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBrandNew
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBrandNew))
        Me.tlpDialogRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.tlpBasicFields = New System.Windows.Forms.TableLayoutPanel()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.tlpDialogFooter = New System.Windows.Forms.TableLayoutPanel()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.flpDialogCommands = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tlpDialogRoot.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        Me.tlpBasicFields.SuspendLayout()
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
        Me.tlpDialogRoot.Controls.Add(Me.grpNotes, 0, 1)
        Me.tlpDialogRoot.Controls.Add(Me.tlpDialogFooter, 0, 2)
        Me.tlpDialogRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDialogRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpDialogRoot.Name = "tlpDialogRoot"
        Me.tlpDialogRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpDialogRoot.RowCount = 3
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118.0!))
        Me.tlpDialogRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogRoot.Size = New System.Drawing.Size(420, 264)
        Me.tlpDialogRoot.TabIndex = 0
        '
        'grpBasic
        '
        Me.grpBasic.AutoSize = True
        Me.grpBasic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.grpBasic.Controls.Add(Me.tlpBasicFields)
        Me.grpBasic.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpBasic.Location = New System.Drawing.Point(12, 12)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(8)
        Me.grpBasic.Size = New System.Drawing.Size(396, 81)
        Me.grpBasic.TabIndex = 0
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'tlpBasicFields
        '
        Me.tlpBasicFields.AutoSize = True
        Me.tlpBasicFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpBasicFields.ColumnCount = 2
        Me.tlpBasicFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpBasicFields.Controls.Add(Me.lblBrand, 0, 0)
        Me.tlpBasicFields.Controls.Add(Me.txtBrand, 1, 0)
        Me.tlpBasicFields.Controls.Add(Me.lblCode, 0, 1)
        Me.tlpBasicFields.Controls.Add(Me.txtCode, 1, 1)
        Me.tlpBasicFields.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpBasicFields.Location = New System.Drawing.Point(8, 21)
        Me.tlpBasicFields.Name = "tlpBasicFields"
        Me.tlpBasicFields.RowCount = 2
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBasicFields.Size = New System.Drawing.Size(380, 52)
        Me.tlpBasicFields.TabIndex = 0
        '
        'lblBrand
        '
        Me.lblBrand.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(0, 6)
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(0, 0, 6, 6)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(38, 13)
        Me.lblBrand.TabIndex = 0
        Me.lblBrand.Text = "Brand:"
        '
        'txtBrand
        '
        Me.txtBrand.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBrand.Location = New System.Drawing.Point(44, 0)
        Me.txtBrand.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.txtBrand.MaxLength = 100
        Me.txtBrand.MinimumSize = New System.Drawing.Size(260, 20)
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.Size = New System.Drawing.Size(336, 20)
        Me.txtBrand.TabIndex = 0
        '
        'lblCode
        '
        Me.lblCode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(0, 33)
        Me.lblCode.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(35, 13)
        Me.lblCode.TabIndex = 1
        Me.lblCode.Text = "Code:"
        '
        'txtCode
        '
        Me.txtCode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.txtCode.Location = New System.Drawing.Point(44, 32)
        Me.txtCode.Margin = New System.Windows.Forms.Padding(0)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(64, 20)
        Me.txtCode.TabIndex = 1
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpNotes.Location = New System.Drawing.Point(12, 101)
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(8)
        Me.grpNotes.Size = New System.Drawing.Size(396, 110)
        Me.grpNotes.TabIndex = 1
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'txtNotes
        '
        Me.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtNotes.Location = New System.Drawing.Point(8, 21)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtNotes.Size = New System.Drawing.Size(380, 81)
        Me.txtNotes.TabIndex = 0
        '
        'tlpDialogFooter
        '
        Me.tlpDialogFooter.AutoSize = True
        Me.tlpDialogFooter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpDialogFooter.ColumnCount = 2
        Me.tlpDialogFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDialogFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogFooter.Controls.Add(Me.lblAdd, 0, 0)
        Me.tlpDialogFooter.Controls.Add(Me.flpDialogCommands, 1, 0)
        Me.tlpDialogFooter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDialogFooter.Location = New System.Drawing.Point(12, 219)
        Me.tlpDialogFooter.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpDialogFooter.Name = "tlpDialogFooter"
        Me.tlpDialogFooter.RowCount = 1
        Me.tlpDialogFooter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpDialogFooter.Size = New System.Drawing.Size(396, 33)
        Me.tlpDialogFooter.TabIndex = 2
        '
        'lblAdd
        '
        Me.lblAdd.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(0, 10)
        Me.lblAdd.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(222, 13)
        Me.lblAdd.TabIndex = 0
        Me.lblAdd.Text = "Changes are saved with the catalogue."
        '
        'flpDialogCommands
        '
        Me.flpDialogCommands.AutoSize = True
        Me.flpDialogCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpDialogCommands.Controls.Add(Me.btnAdd)
        Me.flpDialogCommands.Controls.Add(Me.btnCancel)
        Me.flpDialogCommands.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpDialogCommands.Location = New System.Drawing.Point(230, 0)
        Me.flpDialogCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.flpDialogCommands.Name = "flpDialogCommands"
        Me.flpDialogCommands.Size = New System.Drawing.Size(166, 33)
        Me.flpDialogCommands.TabIndex = 0
        Me.flpDialogCommands.WrapContents = False
        '
        'btnAdd
        '
        Me.btnAdd.AutoSize = True
        Me.btnAdd.Location = New System.Drawing.Point(3, 3)
        Me.btnAdd.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(75, 27)
        Me.btnAdd.TabIndex = 0
        Me.btnAdd.Text = "&Add Brand"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleDescription = "Cancel this creation step and return without adding an item."
        Me.btnCancel.AccessibleName = "Cancel"
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(84, 3)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 27)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmBrandNew
        '
        Me.AcceptButton = Me.btnAdd
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(420, 264)
        Me.Controls.Add(Me.tlpDialogRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmBrandNew"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add New Brand"
        Me.tlpDialogRoot.ResumeLayout(False)
        Me.tlpDialogRoot.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.tlpBasicFields.ResumeLayout(False)
        Me.tlpBasicFields.PerformLayout()
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
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents tlpDialogFooter As TableLayoutPanel
    Friend WithEvents lblAdd As Label
    Friend WithEvents flpDialogCommands As FlowLayoutPanel
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnCancel As Button
End Class
