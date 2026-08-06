<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSuccess
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSuccess))
        Me.tlpWizardRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlSuccess = New System.Windows.Forms.Panel()
        Me.tlpSuccessCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.tlpSuccessContent = New System.Windows.Forms.TableLayoutPanel()
        Me.lblHeadingSuccess = New System.Windows.Forms.Label()
        Me.lblSuccess = New System.Windows.Forms.Label()
        Me.chkStartProgram = New System.Windows.Forms.CheckBox()
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.tlpWizardRoot.SuspendLayout()
        Me.pnlSuccess.SuspendLayout()
        Me.tlpSuccessCanvas.SuspendLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpSuccessContent.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpWizardRoot
        '
        Me.tlpWizardRoot.ColumnCount = 1
        Me.tlpWizardRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.Controls.Add(Me.pnlSuccess, 0, 0)
        Me.tlpWizardRoot.Controls.Add(Me.pnlButtons, 0, 1)
        Me.tlpWizardRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpWizardRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpWizardRoot.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpWizardRoot.Name = "tlpWizardRoot"
        Me.tlpWizardRoot.RowCount = 2
        Me.tlpWizardRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpWizardRoot.Size = New System.Drawing.Size(701, 458)
        Me.tlpWizardRoot.TabIndex = 0
        '
        'pnlSuccess
        '
        Me.pnlSuccess.AutoScroll = True
        Me.pnlSuccess.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlSuccess.Controls.Add(Me.tlpSuccessCanvas)
        Me.pnlSuccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlSuccess.Location = New System.Drawing.Point(0, 0)
        Me.pnlSuccess.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlSuccess.Name = "pnlSuccess"
        Me.pnlSuccess.Size = New System.Drawing.Size(701, 408)
        Me.pnlSuccess.TabIndex = 0
        '
        'tlpSuccessCanvas
        '
        Me.tlpSuccessCanvas.AutoSize = True
        Me.tlpSuccessCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpSuccessCanvas.ColumnCount = 2
        Me.tlpSuccessCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 223.0!))
        Me.tlpSuccessCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpSuccessCanvas.Controls.Add(Me.picSideBanner, 0, 0)
        Me.tlpSuccessCanvas.Controls.Add(Me.tlpSuccessContent, 1, 0)
        Me.tlpSuccessCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpSuccessCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpSuccessCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpSuccessCanvas.MinimumSize = New System.Drawing.Size(677, 388)
        Me.tlpSuccessCanvas.Name = "tlpSuccessCanvas"
        Me.tlpSuccessCanvas.RowCount = 1
        Me.tlpSuccessCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpSuccessCanvas.Size = New System.Drawing.Size(701, 388)
        Me.tlpSuccessCanvas.TabIndex = 0
        '
        'picSideBanner
        '
        Me.picSideBanner.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picSideBanner.Image = Global.Compact_Cassette_Catalogue_Installer.My.Resources.Resources.cassette_tapes_transparent_jpg
        Me.picSideBanner.Location = New System.Drawing.Point(0, 0)
        Me.picSideBanner.Margin = New System.Windows.Forms.Padding(0)
        Me.picSideBanner.Name = "picSideBanner"
        Me.picSideBanner.Size = New System.Drawing.Size(223, 388)
        Me.picSideBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSideBanner.TabIndex = 0
        Me.picSideBanner.TabStop = False
        '
        'tlpSuccessContent
        '
        Me.tlpSuccessContent.ColumnCount = 1
        Me.tlpSuccessContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpSuccessContent.Controls.Add(Me.lblHeadingSuccess, 0, 0)
        Me.tlpSuccessContent.Controls.Add(Me.lblSuccess, 0, 1)
        Me.tlpSuccessContent.Controls.Add(Me.chkStartProgram, 0, 3)
        Me.tlpSuccessContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpSuccessContent.Location = New System.Drawing.Point(223, 0)
        Me.tlpSuccessContent.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpSuccessContent.Name = "tlpSuccessContent"
        Me.tlpSuccessContent.Padding = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.tlpSuccessContent.RowCount = 4
        Me.tlpSuccessContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpSuccessContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpSuccessContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpSuccessContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpSuccessContent.Size = New System.Drawing.Size(478, 388)
        Me.tlpSuccessContent.TabIndex = 1
        '
        'lblHeadingSuccess
        '
        Me.lblHeadingSuccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHeadingSuccess.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingSuccess.Location = New System.Drawing.Point(23, 30)
        Me.lblHeadingSuccess.Name = "lblHeadingSuccess"
        Me.lblHeadingSuccess.Size = New System.Drawing.Size(432, 70)
        Me.lblHeadingSuccess.TabIndex = 0
        Me.lblHeadingSuccess.Text = "Completed the Compact Cassette Catalogue Setup Wizard"
        '
        'lblSuccess
        '
        Me.lblSuccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSuccess.Location = New System.Drawing.Point(23, 103)
        Me.lblSuccess.Name = "lblSuccess"
        Me.lblSuccess.Size = New System.Drawing.Size(432, 67)
        Me.lblSuccess.TabIndex = 1
        Me.lblSuccess.Text = "Click the Finish button to exit the Setup Wizard."
        '
        'chkStartProgram
        '
        Me.chkStartProgram.AutoSize = True
        Me.chkStartProgram.Checked = True
        Me.chkStartProgram.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStartProgram.Location = New System.Drawing.Point(23, 344)
        Me.chkStartProgram.Name = "chkStartProgram"
        Me.chkStartProgram.Size = New System.Drawing.Size(408, 21)
        Me.chkStartProgram.TabIndex = 2
        Me.chkStartProgram.Text = "Start Compact Cassette Catalogue after closing the installer."
        Me.chkStartProgram.UseVisualStyleBackColor = True
        '
        'pnlButtons
        '
        Me.pnlButtons.AutoSize = True
        Me.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnFinish)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.pnlButtons.Location = New System.Drawing.Point(0, 408)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(701, 50)
        Me.pnlButtons.TabIndex = 1
        Me.pnlButtons.WrapContents = True
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(587, 13)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnFinish
        '
        Me.btnFinish.AutoSize = True
        Me.btnFinish.Location = New System.Drawing.Point(481, 13)
        Me.btnFinish.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(100, 30)
        Me.btnFinish.TabIndex = 0
        Me.btnFinish.Text = "Finish"
        Me.btnFinish.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.AutoSize = True
        Me.btnBack.Enabled = False
        Me.btnBack.Location = New System.Drawing.Point(375, 13)
        Me.btnBack.MinimumSize = New System.Drawing.Size(100, 30)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(100, 30)
        Me.btnBack.TabIndex = 1
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'frmSuccess
        '
        Me.AcceptButton = Me.btnFinish
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.tlpWizardRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmSuccess"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.tlpWizardRoot.ResumeLayout(False)
        Me.tlpWizardRoot.PerformLayout()
        Me.pnlSuccess.ResumeLayout(False)
        Me.pnlSuccess.PerformLayout()
        Me.tlpSuccessCanvas.ResumeLayout(False)
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpSuccessContent.ResumeLayout(False)
        Me.tlpSuccessContent.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlButtons.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpWizardRoot As TableLayoutPanel
    Friend WithEvents pnlSuccess As Panel
    Friend WithEvents tlpSuccessCanvas As TableLayoutPanel
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents tlpSuccessContent As TableLayoutPanel
    Friend WithEvents lblSuccess As Label
    Friend WithEvents lblHeadingSuccess As Label
    Friend WithEvents chkStartProgram As CheckBox
    Friend WithEvents pnlButtons As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnFinish As Button
End Class
