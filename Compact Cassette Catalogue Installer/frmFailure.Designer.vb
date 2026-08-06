<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFailure
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFailure))
        Me.tlpWizardRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlFailure = New System.Windows.Forms.Panel()
        Me.tlpFailureCanvas = New System.Windows.Forms.TableLayoutPanel()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.tlpFailureContent = New System.Windows.Forms.TableLayoutPanel()
        Me.lblHeadingFailure = New System.Windows.Forms.Label()
        Me.lblFailure = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.tlpWizardRoot.SuspendLayout()
        Me.pnlFailure.SuspendLayout()
        Me.tlpFailureCanvas.SuspendLayout()
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpFailureContent.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpWizardRoot
        '
        Me.tlpWizardRoot.ColumnCount = 1
        Me.tlpWizardRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpWizardRoot.Controls.Add(Me.pnlFailure, 0, 0)
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
        'pnlFailure
        '
        Me.pnlFailure.AutoScroll = True
        Me.pnlFailure.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.pnlFailure.Controls.Add(Me.tlpFailureCanvas)
        Me.pnlFailure.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlFailure.Location = New System.Drawing.Point(0, 0)
        Me.pnlFailure.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlFailure.Name = "pnlFailure"
        Me.pnlFailure.Size = New System.Drawing.Size(701, 408)
        Me.pnlFailure.TabIndex = 0
        '
        'tlpFailureCanvas
        '
        Me.tlpFailureCanvas.AutoSize = True
        Me.tlpFailureCanvas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpFailureCanvas.ColumnCount = 2
        Me.tlpFailureCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 223.0!))
        Me.tlpFailureCanvas.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFailureCanvas.Controls.Add(Me.picSideBanner, 0, 0)
        Me.tlpFailureCanvas.Controls.Add(Me.tlpFailureContent, 1, 0)
        Me.tlpFailureCanvas.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpFailureCanvas.Location = New System.Drawing.Point(0, 0)
        Me.tlpFailureCanvas.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpFailureCanvas.MinimumSize = New System.Drawing.Size(677, 388)
        Me.tlpFailureCanvas.Name = "tlpFailureCanvas"
        Me.tlpFailureCanvas.RowCount = 1
        Me.tlpFailureCanvas.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFailureCanvas.Size = New System.Drawing.Size(701, 388)
        Me.tlpFailureCanvas.TabIndex = 0
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
        'tlpFailureContent
        '
        Me.tlpFailureContent.ColumnCount = 1
        Me.tlpFailureContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFailureContent.Controls.Add(Me.lblHeadingFailure, 0, 0)
        Me.tlpFailureContent.Controls.Add(Me.lblFailure, 0, 1)
        Me.tlpFailureContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpFailureContent.Location = New System.Drawing.Point(223, 0)
        Me.tlpFailureContent.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpFailureContent.Name = "tlpFailureContent"
        Me.tlpFailureContent.Padding = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.tlpFailureContent.RowCount = 3
        Me.tlpFailureContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.tlpFailureContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170.0!))
        Me.tlpFailureContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpFailureContent.Size = New System.Drawing.Size(478, 388)
        Me.tlpFailureContent.TabIndex = 1
        '
        'lblHeadingFailure
        '
        Me.lblHeadingFailure.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHeadingFailure.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingFailure.Location = New System.Drawing.Point(23, 30)
        Me.lblHeadingFailure.Name = "lblHeadingFailure"
        Me.lblHeadingFailure.Size = New System.Drawing.Size(432, 80)
        Me.lblHeadingFailure.TabIndex = 0
        Me.lblHeadingFailure.Text = "Compact Cassette Catalogue Setup Wizard was interrupted"
        '
        'lblFailure
        '
        Me.lblFailure.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblFailure.Location = New System.Drawing.Point(23, 113)
        Me.lblFailure.Name = "lblFailure"
        Me.lblFailure.Size = New System.Drawing.Size(432, 167)
        Me.lblFailure.TabIndex = 1
        Me.lblFailure.Text = resources.GetString("lblFailure.Text")
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
        'frmFailure
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
        Me.Name = "frmFailure"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.tlpWizardRoot.ResumeLayout(False)
        Me.tlpWizardRoot.PerformLayout()
        Me.pnlFailure.ResumeLayout(False)
        Me.pnlFailure.PerformLayout()
        Me.tlpFailureCanvas.ResumeLayout(False)
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpFailureContent.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlButtons.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpWizardRoot As TableLayoutPanel
    Friend WithEvents pnlFailure As Panel
    Friend WithEvents tlpFailureCanvas As TableLayoutPanel
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents tlpFailureContent As TableLayoutPanel
    Friend WithEvents lblFailure As Label
    Friend WithEvents lblHeadingFailure As Label
    Friend WithEvents pnlButtons As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnFinish As Button
End Class
