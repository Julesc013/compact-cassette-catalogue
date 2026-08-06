<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSettings))
        Me.tlpSettingsRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.lblShowMessages = New System.Windows.Forms.Label()
        Me.cmbShowMessages = New System.Windows.Forms.ComboBox()
        Me.lblCheckUpdates = New System.Windows.Forms.Label()
        Me.cmbCheckUpdates = New System.Windows.Forms.ComboBox()
        Me.flpSettingsCommands = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tlpSettingsRoot.SuspendLayout()
        Me.flpSettingsCommands.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpSettingsRoot
        '
        Me.tlpSettingsRoot.AutoSize = True
        Me.tlpSettingsRoot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpSettingsRoot.ColumnCount = 2
        Me.tlpSettingsRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpSettingsRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpSettingsRoot.Controls.Add(Me.lblShowMessages, 0, 0)
        Me.tlpSettingsRoot.Controls.Add(Me.cmbShowMessages, 1, 0)
        Me.tlpSettingsRoot.Controls.Add(Me.lblCheckUpdates, 0, 1)
        Me.tlpSettingsRoot.Controls.Add(Me.cmbCheckUpdates, 1, 1)
        Me.tlpSettingsRoot.Controls.Add(Me.flpSettingsCommands, 0, 2)
        Me.tlpSettingsRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpSettingsRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpSettingsRoot.Name = "tlpSettingsRoot"
        Me.tlpSettingsRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpSettingsRoot.RowCount = 3
        Me.tlpSettingsRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpSettingsRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpSettingsRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpSettingsRoot.SetColumnSpan(Me.flpSettingsCommands, 2)
        Me.tlpSettingsRoot.Size = New System.Drawing.Size(520, 160)
        Me.tlpSettingsRoot.TabIndex = 0
        '
        'lblShowMessages
        '
        Me.lblShowMessages.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblShowMessages.AutoSize = True
        Me.lblShowMessages.Location = New System.Drawing.Point(12, 17)
        Me.lblShowMessages.Margin = New System.Windows.Forms.Padding(0, 0, 12, 8)
        Me.lblShowMessages.Name = "lblShowMessages"
        Me.lblShowMessages.Size = New System.Drawing.Size(163, 17)
        Me.lblShowMessages.TabIndex = 0
        Me.lblShowMessages.Text = "Show pop-up messages:"
        '
        'cmbShowMessages
        '
        Me.cmbShowMessages.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbShowMessages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbShowMessages.FormattingEnabled = True
        Me.cmbShowMessages.Items.AddRange(New Object() {"All messages", "Warnings only"})
        Me.cmbShowMessages.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.cmbShowMessages.MinimumSize = New System.Drawing.Size(240, 0)
        Me.cmbShowMessages.Name = "cmbShowMessages"
        Me.cmbShowMessages.TabIndex = 0
        '
        'lblCheckUpdates
        '
        Me.lblCheckUpdates.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblCheckUpdates.AutoSize = True
        Me.lblCheckUpdates.Location = New System.Drawing.Point(12, 49)
        Me.lblCheckUpdates.Margin = New System.Windows.Forms.Padding(0, 0, 12, 12)
        Me.lblCheckUpdates.Name = "lblCheckUpdates"
        Me.lblCheckUpdates.Size = New System.Drawing.Size(212, 17)
        Me.lblCheckUpdates.TabIndex = 1
        Me.lblCheckUpdates.Text = "Automatically check for updates:"
        '
        'cmbCheckUpdates
        '
        Me.cmbCheckUpdates.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbCheckUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCheckUpdates.FormattingEnabled = True
        Me.cmbCheckUpdates.Items.AddRange(New Object() {"On startup", "Weekly", "Monthly", "Never / manual only"})
        Me.cmbCheckUpdates.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.cmbCheckUpdates.MinimumSize = New System.Drawing.Size(240, 0)
        Me.cmbCheckUpdates.Name = "cmbCheckUpdates"
        Me.cmbCheckUpdates.TabIndex = 1
        '
        'flpSettingsCommands
        '
        Me.flpSettingsCommands.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpSettingsCommands.AutoSize = True
        Me.flpSettingsCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpSettingsCommands.Controls.Add(Me.btnSave)
        Me.flpSettingsCommands.Controls.Add(Me.btnCancel)
        Me.flpSettingsCommands.Location = New System.Drawing.Point(318, 90)
        Me.flpSettingsCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.flpSettingsCommands.Name = "flpSettingsCommands"
        Me.flpSettingsCommands.Size = New System.Drawing.Size(190, 58)
        Me.flpSettingsCommands.TabIndex = 2
        Me.flpSettingsCommands.WrapContents = False
        '
        'btnSave
        '
        Me.btnSave.AutoSize = True
        Me.btnSave.Location = New System.Drawing.Point(3, 3)
        Me.btnSave.MinimumSize = New System.Drawing.Size(100, 27)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 27)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "&Save Settings"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleDescription = "Close Settings without saving further changes."
        Me.btnCancel.AccessibleName = "Cancel"
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(109, 3)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 27)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(520, 160)
        Me.Controls.Add(Me.tlpSettingsRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Settings"
        Me.tlpSettingsRoot.ResumeLayout(False)
        Me.tlpSettingsRoot.PerformLayout()
        Me.flpSettingsCommands.ResumeLayout(False)
        Me.flpSettingsCommands.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents tlpSettingsRoot As TableLayoutPanel
    Friend WithEvents lblShowMessages As Label
    Friend WithEvents cmbShowMessages As ComboBox
    Friend WithEvents lblCheckUpdates As Label
    Friend WithEvents cmbCheckUpdates As ComboBox
    Friend WithEvents flpSettingsCommands As FlowLayoutPanel
    Friend WithEvents btnSave As Button
    Friend WithEvents btnCancel As Button
End Class
