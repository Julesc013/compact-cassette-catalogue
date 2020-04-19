<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
        Me.btnOK = New System.Windows.Forms.Button()
        Me.lblProgramName = New System.Windows.Forms.Label()
        Me.lblCopyright = New System.Windows.Forms.Label()
        Me.lblProgramVersion = New System.Windows.Forms.Label()
        Me.lblCatalogueVersion = New System.Windows.Forms.Label()
        Me.lblProgramDate = New System.Windows.Forms.Label()
        Me.pnlInformation = New System.Windows.Forms.Panel()
        Me.lnkWebsite = New System.Windows.Forms.LinkLabel()
        Me.lblContactEmail = New System.Windows.Forms.Label()
        Me.lblWebsite = New System.Windows.Forms.Label()
        Me.lnkContactEmail = New System.Windows.Forms.LinkLabel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.pnlInformation.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(376, 333)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(120, 29)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'lblProgramName
        '
        Me.lblProgramName.AutoSize = True
        Me.lblProgramName.Location = New System.Drawing.Point(3, 9)
        Me.lblProgramName.Name = "lblProgramName"
        Me.lblProgramName.Size = New System.Drawing.Size(221, 17)
        Me.lblProgramName.TabIndex = 3
        Me.lblProgramName.Text = "Compact Cassette Catalogue (C3)"
        '
        'lblCopyright
        '
        Me.lblCopyright.AutoSize = True
        Me.lblCopyright.Location = New System.Drawing.Point(3, 30)
        Me.lblCopyright.Name = "lblCopyright"
        Me.lblCopyright.Size = New System.Drawing.Size(116, 17)
        Me.lblCopyright.TabIndex = 4
        Me.lblCopyright.Text = "© Jules Carboni, "
        '
        'lblProgramVersion
        '
        Me.lblProgramVersion.AutoSize = True
        Me.lblProgramVersion.Location = New System.Drawing.Point(3, 62)
        Me.lblProgramVersion.Name = "lblProgramVersion"
        Me.lblProgramVersion.Size = New System.Drawing.Size(122, 17)
        Me.lblProgramVersion.TabIndex = 5
        Me.lblProgramVersion.Text = "Program Version: "
        '
        'lblCatalogueVersion
        '
        Me.lblCatalogueVersion.AutoSize = True
        Me.lblCatalogueVersion.Location = New System.Drawing.Point(3, 83)
        Me.lblCatalogueVersion.Name = "lblCatalogueVersion"
        Me.lblCatalogueVersion.Size = New System.Drawing.Size(128, 17)
        Me.lblCatalogueVersion.TabIndex = 6
        Me.lblCatalogueVersion.Text = "Catalogue Version:"
        '
        'lblProgramDate
        '
        Me.lblProgramDate.AutoSize = True
        Me.lblProgramDate.Location = New System.Drawing.Point(3, 104)
        Me.lblProgramDate.Name = "lblProgramDate"
        Me.lblProgramDate.Size = New System.Drawing.Size(0, 17)
        Me.lblProgramDate.TabIndex = 7
        '
        'pnlInformation
        '
        Me.pnlInformation.Controls.Add(Me.lnkContactEmail)
        Me.pnlInformation.Controls.Add(Me.lnkWebsite)
        Me.pnlInformation.Controls.Add(Me.lblContactEmail)
        Me.pnlInformation.Controls.Add(Me.lblWebsite)
        Me.pnlInformation.Controls.Add(Me.lblProgramDate)
        Me.pnlInformation.Controls.Add(Me.lblProgramName)
        Me.pnlInformation.Controls.Add(Me.lblCatalogueVersion)
        Me.pnlInformation.Controls.Add(Me.lblCopyright)
        Me.pnlInformation.Controls.Add(Me.lblProgramVersion)
        Me.pnlInformation.Location = New System.Drawing.Point(146, 142)
        Me.pnlInformation.Name = "pnlInformation"
        Me.pnlInformation.Size = New System.Drawing.Size(350, 185)
        Me.pnlInformation.TabIndex = 8
        '
        'lnkWebsite
        '
        Me.lnkWebsite.AutoSize = True
        Me.lnkWebsite.Location = New System.Drawing.Point(64, 136)
        Me.lnkWebsite.Name = "lnkWebsite"
        Me.lnkWebsite.Size = New System.Drawing.Size(0, 17)
        Me.lnkWebsite.TabIndex = 11
        '
        'lblContactEmail
        '
        Me.lblContactEmail.AutoSize = True
        Me.lblContactEmail.Location = New System.Drawing.Point(3, 157)
        Me.lblContactEmail.Name = "lblContactEmail"
        Me.lblContactEmail.Size = New System.Drawing.Size(64, 17)
        Me.lblContactEmail.TabIndex = 10
        Me.lblContactEmail.Text = "Contact: "
        '
        'lblWebsite
        '
        Me.lblWebsite.AutoSize = True
        Me.lblWebsite.Location = New System.Drawing.Point(3, 136)
        Me.lblWebsite.Name = "lblWebsite"
        Me.lblWebsite.Size = New System.Drawing.Size(67, 17)
        Me.lblWebsite.TabIndex = 9
        Me.lblWebsite.Text = "Website: "
        '
        'lnkContactEmail
        '
        Me.lnkContactEmail.AutoSize = True
        Me.lnkContactEmail.Location = New System.Drawing.Point(64, 157)
        Me.lnkContactEmail.Name = "lnkContactEmail"
        Me.lnkContactEmail.Size = New System.Drawing.Size(0, 17)
        Me.lnkContactEmail.TabIndex = 12
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.Compact_Cassette_Catalogue.My.Resources.Resources.cassette_icon
        Me.PictureBox2.Location = New System.Drawing.Point(90, 142)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(50, 58)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 9
        Me.PictureBox2.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Compact_Cassette_Catalogue.My.Resources.Resources.banner_wide
        Me.PictureBox1.Location = New System.Drawing.Point(15, 14)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(481, 122)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'frmAbout
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(511, 376)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.pnlInformation)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAbout"
        Me.Padding = New System.Windows.Forms.Padding(12, 11, 12, 11)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About Compact Cassette Catalogue"
        Me.TopMost = True
        Me.pnlInformation.ResumeLayout(False)
        Me.pnlInformation.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnOK As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents lblProgramName As Label
    Friend WithEvents lblCopyright As Label
    Friend WithEvents lblProgramVersion As Label
    Friend WithEvents lblCatalogueVersion As Label
    Friend WithEvents lblProgramDate As Label
    Friend WithEvents pnlInformation As Panel
    Friend WithEvents lblContactEmail As Label
    Friend WithEvents lblWebsite As Label
    Friend WithEvents lnkWebsite As LinkLabel
    Friend WithEvents lnkContactEmail As LinkLabel
    Friend WithEvents PictureBox2 As PictureBox
End Class
