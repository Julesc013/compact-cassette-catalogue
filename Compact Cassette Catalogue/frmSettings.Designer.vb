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
        Me.lblMessages = New System.Windows.Forms.Label()
        Me.cmbMessages = New System.Windows.Forms.ComboBox()
        Me.lblProgressNote = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblMessages
        '
        Me.lblMessages.AutoSize = True
        Me.lblMessages.Location = New System.Drawing.Point(12, 15)
        Me.lblMessages.Name = "lblMessages"
        Me.lblMessages.Size = New System.Drawing.Size(87, 13)
        Me.lblMessages.TabIndex = 0
        Me.lblMessages.Text = "Show messages:"
        '
        'cmbMessages
        '
        Me.cmbMessages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMessages.FormattingEnabled = True
        Me.cmbMessages.Items.AddRange(New Object() {"All messages", "Warnings only"})
        Me.cmbMessages.Location = New System.Drawing.Point(105, 12)
        Me.cmbMessages.Name = "cmbMessages"
        Me.cmbMessages.Size = New System.Drawing.Size(126, 21)
        Me.cmbMessages.TabIndex = 1
        '
        'lblProgressNote
        '
        Me.lblProgressNote.Location = New System.Drawing.Point(12, 90)
        Me.lblProgressNote.Name = "lblProgressNote"
        Me.lblProgressNote.Size = New System.Drawing.Size(219, 16)
        Me.lblProgressNote.TabIndex = 2
        Me.lblProgressNote.Text = "More detailed settings will be added in future."
        Me.lblProgressNote.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(15, 66)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(216, 21)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save Settings"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(243, 115)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblProgressNote)
        Me.Controls.Add(Me.cmbMessages)
        Me.Controls.Add(Me.lblMessages)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmSettings"
        Me.Text = "Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblMessages As Label
    Friend WithEvents cmbMessages As ComboBox
    Friend WithEvents lblProgressNote As Label
    Friend WithEvents btnSave As Button
End Class
