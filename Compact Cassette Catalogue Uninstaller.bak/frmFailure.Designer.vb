<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFailure
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFailure))
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.pnlFailure = New System.Windows.Forms.Panel()
        Me.picSideBanner = New System.Windows.Forms.PictureBox()
        Me.lblFailure = New System.Windows.Forms.Label()
        Me.lblHeadingFailure = New System.Windows.Forms.Label()
        Me.pnlButtons.SuspendLayout()
        Me.pnlFailure.SuspendLayout()
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
        'pnlFailure
        '
        Me.pnlFailure.Controls.Add(Me.picSideBanner)
        Me.pnlFailure.Controls.Add(Me.lblFailure)
        Me.pnlFailure.Controls.Add(Me.lblHeadingFailure)
        Me.pnlFailure.Location = New System.Drawing.Point(0, 0)
        Me.pnlFailure.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlFailure.Name = "pnlFailure"
        Me.pnlFailure.Size = New System.Drawing.Size(701, 388)
        Me.pnlFailure.TabIndex = 5
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
        'lblFailure
        '
        Me.lblFailure.Location = New System.Drawing.Point(245, 126)
        Me.lblFailure.Margin = New System.Windows.Forms.Padding(20, 10, 20, 10)
        Me.lblFailure.Name = "lblFailure"
        Me.lblFailure.Size = New System.Drawing.Size(427, 78)
        Me.lblFailure.TabIndex = 3
        Me.lblFailure.Text = "Compact Cassette Catalogue uninstallation was interrupted. The program may not ha" &
    "ve been fully uninstalled. Please run the uninstallation again. Click the finish" &
    " button to exit the uninstaller."
        '
        'lblHeadingFailure
        '
        Me.lblHeadingFailure.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeadingFailure.Location = New System.Drawing.Point(243, 39)
        Me.lblHeadingFailure.Margin = New System.Windows.Forms.Padding(20, 30, 20, 20)
        Me.lblHeadingFailure.Name = "lblHeadingFailure"
        Me.lblHeadingFailure.Size = New System.Drawing.Size(429, 57)
        Me.lblHeadingFailure.TabIndex = 2
        Me.lblHeadingFailure.Text = "Compact Cassette Catalogue Uninstaller was interrupted" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'frmFailure
        '
        Me.AcceptButton = Me.btnFinish
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.pnlFailure)
        Me.Controls.Add(Me.pnlButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmFailure"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlFailure.ResumeLayout(False)
        CType(Me.picSideBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnFinish As Button
    Friend WithEvents pnlFailure As Panel
    Friend WithEvents picSideBanner As PictureBox
    Friend WithEvents lblFailure As Label
    Friend WithEvents lblHeadingFailure As Label
End Class
