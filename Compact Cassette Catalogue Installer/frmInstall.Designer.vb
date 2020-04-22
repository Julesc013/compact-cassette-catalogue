<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInstall
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInstall))
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.pnlInstall = New System.Windows.Forms.Panel()
        Me.barInstallProgress = New System.Windows.Forms.ProgressBar()
        Me.lblStatusProcess = New System.Windows.Forms.Label()
        Me.lblStatusHeader = New System.Windows.Forms.Label()
        Me.pnlHeaderInstall = New System.Windows.Forms.Panel()
        Me.lblInstall = New System.Windows.Forms.Label()
        Me.lblInstallInstructions = New System.Windows.Forms.Label()
        Me.pnlButtons.SuspendLayout()
        Me.pnlInstall.SuspendLayout()
        Me.pnlHeaderInstall.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlButtons
        '
        Me.pnlButtons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlButtons.BackColor = System.Drawing.SystemColors.Control
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnBack)
        Me.pnlButtons.Controls.Add(Me.btnNext)
        Me.pnlButtons.Location = New System.Drawing.Point(0, 388)
        Me.pnlButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(701, 70)
        Me.pnlButtons.TabIndex = 6
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
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
        'btnNext
        '
        Me.btnNext.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNext.Enabled = False
        Me.btnNext.Location = New System.Drawing.Point(461, 20)
        Me.btnNext.Margin = New System.Windows.Forms.Padding(0, 20, 0, 20)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(100, 30)
        Me.btnNext.TabIndex = 0
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'pnlInstall
        '
        Me.pnlInstall.Controls.Add(Me.barInstallProgress)
        Me.pnlInstall.Controls.Add(Me.lblStatusProcess)
        Me.pnlInstall.Controls.Add(Me.lblStatusHeader)
        Me.pnlInstall.Controls.Add(Me.pnlHeaderInstall)
        Me.pnlInstall.Controls.Add(Me.lblInstallInstructions)
        Me.pnlInstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlInstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlInstall.Name = "pnlInstall"
        Me.pnlInstall.Size = New System.Drawing.Size(701, 388)
        Me.pnlInstall.TabIndex = 9
        '
        'barInstallProgress
        '
        Me.barInstallProgress.Location = New System.Drawing.Point(35, 186)
        Me.barInstallProgress.Margin = New System.Windows.Forms.Padding(30, 0, 30, 10)
        Me.barInstallProgress.Name = "barInstallProgress"
        Me.barInstallProgress.Size = New System.Drawing.Size(627, 23)
        Me.barInstallProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.barInstallProgress.TabIndex = 6
        '
        'lblStatusProcess
        '
        Me.lblStatusProcess.Location = New System.Drawing.Point(102, 153)
        Me.lblStatusProcess.Margin = New System.Windows.Forms.Padding(0, 10, 30, 10)
        Me.lblStatusProcess.Name = "lblStatusProcess"
        Me.lblStatusProcess.Size = New System.Drawing.Size(560, 23)
        Me.lblStatusProcess.TabIndex = 5
        '
        'lblStatusHeader
        '
        Me.lblStatusHeader.Location = New System.Drawing.Point(32, 153)
        Me.lblStatusHeader.Margin = New System.Windows.Forms.Padding(30, 10, 30, 10)
        Me.lblStatusHeader.Name = "lblStatusHeader"
        Me.lblStatusHeader.Size = New System.Drawing.Size(52, 23)
        Me.lblStatusHeader.TabIndex = 4
        Me.lblStatusHeader.Text = "Status:"
        '
        'pnlHeaderInstall
        '
        Me.pnlHeaderInstall.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlHeaderInstall.BackColor = System.Drawing.SystemColors.Control
        Me.pnlHeaderInstall.Controls.Add(Me.lblInstall)
        Me.pnlHeaderInstall.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeaderInstall.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeaderInstall.Name = "pnlHeaderInstall"
        Me.pnlHeaderInstall.Size = New System.Drawing.Size(701, 70)
        Me.pnlHeaderInstall.TabIndex = 3
        '
        'lblInstall
        '
        Me.lblInstall.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInstall.Location = New System.Drawing.Point(30, 22)
        Me.lblInstall.Margin = New System.Windows.Forms.Padding(30, 20, 30, 20)
        Me.lblInstall.Name = "lblInstall"
        Me.lblInstall.Size = New System.Drawing.Size(641, 30)
        Me.lblInstall.TabIndex = 2
        Me.lblInstall.Text = "Installing Compact Cassette Catalogue"
        '
        'lblInstallInstructions
        '
        Me.lblInstallInstructions.Location = New System.Drawing.Point(32, 100)
        Me.lblInstallInstructions.Margin = New System.Windows.Forms.Padding(30, 30, 30, 20)
        Me.lblInstallInstructions.Name = "lblInstallInstructions"
        Me.lblInstallInstructions.Size = New System.Drawing.Size(630, 23)
        Me.lblInstallInstructions.TabIndex = 3
        Me.lblInstallInstructions.Text = "Please wait while the Setup Wizard installs Compact Cassette Catalogue."
        '
        'frmInstall
        '
        Me.AcceptButton = Me.btnNext
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(701, 458)
        Me.Controls.Add(Me.pnlInstall)
        Me.Controls.Add(Me.pnlButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmInstall"
        Me.Text = "Compact Cassette Catalogue Setup"
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlInstall.ResumeLayout(False)
        Me.pnlHeaderInstall.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnBack As Button
    Friend WithEvents btnNext As Button
    Friend WithEvents pnlInstall As Panel
    Friend WithEvents lblStatusProcess As Label
    Friend WithEvents lblStatusHeader As Label
    Friend WithEvents pnlHeaderInstall As Panel
    Friend WithEvents lblInstall As Label
    Friend WithEvents lblInstallInstructions As Label
    Friend WithEvents barInstallProgress As ProgressBar
End Class
