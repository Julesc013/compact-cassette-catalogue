<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSuccess
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSuccess))
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.pnlSuccess = New System.Windows.Forms.Panel()
        Me.chkStartProgram = New System.Windows.Forms.CheckBox()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.lblSuccess = New System.Windows.Forms.Label()
        Me.lblHeadingSuccess = New System.Windows.Forms.Label()
        Me.pnlButtons.SuspendLayout()
        Me.pnlSuccess.SuspendLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlButtons
        '
        Me.pnlButtons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Controls.Add(Me.btnFinish)
        Me.pnlButtons.Location = New System.Drawing.Point(0, 388)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(701, 70)
        Me.pnlButtons.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(581, 20)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(20)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBack.Enabled = False
        Me.btnBack.Location = New System.Drawing.Point(361, 20)
        Me.btnBack.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(100, 30)
        Me.btnBack.TabIndex = 1
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'btnFinish
        '
        Me.btnFinish.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFinish.Location = New System.Drawing.Point(461, 20)
        Me.btnFinish.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(100, 30)
        Me.btnFinish.TabIndex = 0
        Me.btnFinish.Text = "Finish"
        Me.btnFinish.UseVisualStyleBackColor = True
        '
        'pnlSuccess
        '
        Me.pnlSuccess.Controls.Add(Me.chkStartProgram)
        Me.pnlSuccess.Controls.Add(Me.picSideBanner)
        Me.pnlSuccess.Controls.Add(Me.lblSuccess)
        Me.pnlSuccess.Controls.Add(Me.lblHeadingSuccess)
        Me.pnlSuccess.Location = New System.Drawing.Point(0, 0)
        Me.pnlSuccess.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlSuccess.Name = "pnlSuccess"
        Me.pnlSuccess.Size = New System.Drawing.Size(701, 388)
        Me.pnlSuccess.TabIndex = 6
        '
        'chkStartProgram
        '
        Me.chkStartProgram.AutoSize = True
        Me.chkStartProgram.Checked = True
        Me.chkStartProgram.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStartProgram.Location = New System.Drawing.Point(248, 337)
        Me.chkStartProgram.Margin = New System.Windows.Forms.Padding(20, 30, 20, 30)
        Me.chkStartProgram.Name = "chkStartProgram"
        Me.chkStartProgram.Size = New System.Drawing.Size(408, 21)
        Me.chkStartProgram.TabIndex = 4
        Me.chkStartProgram.Text = "Start Compact Cassette Catalogue after closing the installer."
        Me.chkStartProgram.UseVisualStyleBackColor = True
        '
        'picSideBanner
        '
        Me.picSideBanner.Image = CType(resources.GetObject("picSideBanner.Image"), System.Drawing.Image)
        Me.picSideBanner.Location = New System.Drawing.Point(0, 0)
        Me.picSideBanner.Margin = New System.Windows.Forms.Padding(0)
        Me.picSideBanner.Name = "picSideBanner"
        Me.picSideBanner.Size = New System.Drawing.Size(223, 388)
        Me.picSideBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSideBanner.TabIndex = 1
        Me.picSideBanner.TabStop = False
        '
        'lblSuccess
        '
        Me.lblSuccess.Location = New System.Drawing.Point(245, 126)
        Me.lblSuccess.Margin = New System.Windows.Forms.Padding(20, 10, 20, 10)
        Me.lblSuccess.Name = "lblSuccess"
        Me.lblSuccess.Size = New System.Drawing.Size(427, 25)
        Me.lblSuccess.TabIndex = 3
        Me.lblSuccess.Text = "Click the Finish button to exit the Setup Wizard."
        '
        'lblHeadingSuccess
        '
        Me.lblHeadingSuccess.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingSuccess.Location = New System.Drawing.Point(243, 39)
        Me.lblHeadingSuccess.Margin = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.lblHeadingSuccess.Name = "lblHeadingSuccess"
        Me.lblHeadingSuccess.Size = New System.Drawing.Size(429, 57)
        Me.lblHeadingSuccess.TabIndex = 2
        Me.lblHeadingSuccess.Text = "Completed the Compact Cassette Catalogue Setup Wizard" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'frmSuccess
        '
        Me.AcceptButton = Me.btnFinish
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.pnlSuccess)
        Me.Controls.Add(Me.pnlButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmSuccess"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlSuccess.ResumeLayout(False)
        Me.pnlSuccess.PerformLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnFinish As Button
    Friend WithEvents pnlSuccess As Panel
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents lblSuccess As Label
    Friend WithEvents lblHeadingSuccess As Label
    Friend WithEvents chkStartProgram As CheckBox
End Class
