<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSettings))
        Me.lblShowMessages = New System.Windows.Forms.Label()
        Me.cmbShowMessages = New System.Windows.Forms.ComboBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.lblCheckUpdates = New System.Windows.Forms.Label()
        Me.cmbCheckUpdates = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'lblShowMessages
        '
        Me.lblShowMessages.AutoSize = True
        Me.lblShowMessages.Location = New System.Drawing.Point(13, 16)
        Me.lblShowMessages.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblShowMessages.Name = "lblShowMessages"
        Me.lblShowMessages.Size = New System.Drawing.Size(114, 17)
        Me.lblShowMessages.TabIndex = 0
        Me.lblShowMessages.Text = "Show messages:"
        '
        'cmbShowMessages
        '
        Me.cmbShowMessages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbShowMessages.FormattingEnabled = True
        Me.cmbShowMessages.Items.AddRange(New Object() {"All messages", "Warnings only"})
        Me.cmbShowMessages.Location = New System.Drawing.Point(144, 13)
        Me.cmbShowMessages.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmbShowMessages.Name = "cmbShowMessages"
        Me.cmbShowMessages.Size = New System.Drawing.Size(167, 24)
        Me.cmbShowMessages.TabIndex = 1
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(13, 83)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(298, 26)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save Settings"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'lblCheckUpdates
        '
        Me.lblCheckUpdates.AutoSize = True
        Me.lblCheckUpdates.Location = New System.Drawing.Point(13, 48)
        Me.lblCheckUpdates.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCheckUpdates.Name = "lblCheckUpdates"
        Me.lblCheckUpdates.Size = New System.Drawing.Size(127, 17)
        Me.lblCheckUpdates.TabIndex = 4
        Me.lblCheckUpdates.Text = "Check for updates:"
        '
        'cmbCheckUpdates
        '
        Me.cmbCheckUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCheckUpdates.FormattingEnabled = True
        Me.cmbCheckUpdates.Items.AddRange(New Object() {"Automatically (on startup)", "Manually only"})
        Me.cmbCheckUpdates.Location = New System.Drawing.Point(144, 45)
        Me.cmbCheckUpdates.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbCheckUpdates.Name = "cmbCheckUpdates"
        Me.cmbCheckUpdates.Size = New System.Drawing.Size(167, 24)
        Me.cmbCheckUpdates.TabIndex = 5
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(324, 122)
        Me.Controls.Add(Me.cmbCheckUpdates)
        Me.Controls.Add(Me.lblCheckUpdates)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.cmbShowMessages)
        Me.Controls.Add(Me.lblShowMessages)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.Name = "frmSettings"
        Me.Text = "Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShowMessages As Label
    Friend WithEvents cmbShowMessages As ComboBox
    Friend WithEvents btnSave As Button
    Friend WithEvents lblCheckUpdates As Label
    Friend WithEvents cmbCheckUpdates As ComboBox
End Class
