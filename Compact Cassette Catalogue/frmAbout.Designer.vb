<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
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

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
        Me.tlpAboutRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.tlpAboutBody = New System.Windows.Forms.TableLayoutPanel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.pnlInformation = New System.Windows.Forms.Panel()
        Me.tlpAboutDetails = New System.Windows.Forms.TableLayoutPanel()
        Me.lblProgramName = New System.Windows.Forms.Label()
        Me.lblCopyright = New System.Windows.Forms.Label()
        Me.lblProgramVersion = New System.Windows.Forms.Label()
        Me.lblCatalogueVersion = New System.Windows.Forms.Label()
        Me.lblProgramDate = New System.Windows.Forms.Label()
        Me.lblWebsite = New System.Windows.Forms.Label()
        Me.lnkWebsite = New System.Windows.Forms.LinkLabel()
        Me.lblContactEmail = New System.Windows.Forms.Label()
        Me.lnkContactEmail = New System.Windows.Forms.LinkLabel()
        Me.flpAboutCommands = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.tlpAboutRoot.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpAboutBody.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlInformation.SuspendLayout()
        Me.tlpAboutDetails.SuspendLayout()
        Me.flpAboutCommands.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpAboutRoot
        '
        Me.tlpAboutRoot.AutoSize = True
        Me.tlpAboutRoot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpAboutRoot.ColumnCount = 1
        Me.tlpAboutRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpAboutRoot.Controls.Add(Me.PictureBox1, 0, 0)
        Me.tlpAboutRoot.Controls.Add(Me.tlpAboutBody, 0, 1)
        Me.tlpAboutRoot.Controls.Add(Me.flpAboutCommands, 0, 2)
        Me.tlpAboutRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpAboutRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpAboutRoot.Name = "tlpAboutRoot"
        Me.tlpAboutRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpAboutRoot.RowCount = 3
        Me.tlpAboutRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122.0!))
        Me.tlpAboutRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutRoot.Size = New System.Drawing.Size(540, 430)
        Me.tlpAboutRoot.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Image = Global.Compact_Cassette_Catalogue.My.Resources.Resources.banner_wide
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.PictureBox1.MinimumSize = New System.Drawing.Size(481, 110)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(516, 110)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'tlpAboutBody
        '
        Me.tlpAboutBody.AutoSize = True
        Me.tlpAboutBody.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpAboutBody.ColumnCount = 2
        Me.tlpAboutBody.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutBody.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpAboutBody.Controls.Add(Me.PictureBox2, 0, 0)
        Me.tlpAboutBody.Controls.Add(Me.pnlInformation, 1, 0)
        Me.tlpAboutBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpAboutBody.Location = New System.Drawing.Point(12, 134)
        Me.tlpAboutBody.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.tlpAboutBody.Name = "tlpAboutBody"
        Me.tlpAboutBody.RowCount = 1
        Me.tlpAboutBody.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutBody.Size = New System.Drawing.Size(516, 239)
        Me.tlpAboutBody.TabIndex = 1
        '
        'PictureBox2
        '
        Me.PictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.PictureBox2.Image = Global.Compact_Cassette_Catalogue.My.Resources.Resources.cassette_icon
        Me.PictureBox2.Location = New System.Drawing.Point(8, 8)
        Me.PictureBox2.Margin = New System.Windows.Forms.Padding(8, 8, 16, 0)
        Me.PictureBox2.MinimumSize = New System.Drawing.Size(58, 58)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(58, 58)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 0
        Me.PictureBox2.TabStop = False
        '
        'pnlInformation
        '
        Me.pnlInformation.AutoSize = True
        Me.pnlInformation.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlInformation.Controls.Add(Me.tlpAboutDetails)
        Me.pnlInformation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlInformation.Location = New System.Drawing.Point(82, 0)
        Me.pnlInformation.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlInformation.Name = "pnlInformation"
        Me.pnlInformation.Size = New System.Drawing.Size(434, 239)
        Me.pnlInformation.TabIndex = 0
        '
        'tlpAboutDetails
        '
        Me.tlpAboutDetails.AutoSize = True
        Me.tlpAboutDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpAboutDetails.ColumnCount = 2
        Me.tlpAboutDetails.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpAboutDetails.Controls.Add(Me.lblProgramName, 0, 0)
        Me.tlpAboutDetails.Controls.Add(Me.lblCopyright, 0, 1)
        Me.tlpAboutDetails.Controls.Add(Me.lblProgramVersion, 0, 2)
        Me.tlpAboutDetails.Controls.Add(Me.lblCatalogueVersion, 0, 3)
        Me.tlpAboutDetails.Controls.Add(Me.lblProgramDate, 0, 4)
        Me.tlpAboutDetails.Controls.Add(Me.lblWebsite, 0, 5)
        Me.tlpAboutDetails.Controls.Add(Me.lnkWebsite, 1, 5)
        Me.tlpAboutDetails.Controls.Add(Me.lblContactEmail, 0, 6)
        Me.tlpAboutDetails.Controls.Add(Me.lnkContactEmail, 1, 6)
        Me.tlpAboutDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpAboutDetails.Location = New System.Drawing.Point(0, 0)
        Me.tlpAboutDetails.Name = "tlpAboutDetails"
        Me.tlpAboutDetails.Padding = New System.Windows.Forms.Padding(0, 8, 0, 0)
        Me.tlpAboutDetails.RowCount = 7
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpAboutDetails.SetColumnSpan(Me.lblProgramName, 2)
        Me.tlpAboutDetails.SetColumnSpan(Me.lblCopyright, 2)
        Me.tlpAboutDetails.SetColumnSpan(Me.lblProgramVersion, 2)
        Me.tlpAboutDetails.SetColumnSpan(Me.lblCatalogueVersion, 2)
        Me.tlpAboutDetails.SetColumnSpan(Me.lblProgramDate, 2)
        Me.tlpAboutDetails.Size = New System.Drawing.Size(434, 239)
        Me.tlpAboutDetails.TabIndex = 0
        '
        'lblProgramName
        '
        Me.lblProgramName.AutoSize = True
        Me.lblProgramName.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.lblProgramName.Name = "lblProgramName"
        Me.lblProgramName.Size = New System.Drawing.Size(221, 17)
        Me.lblProgramName.TabIndex = 0
        Me.lblProgramName.Text = "Compact Cassette Catalogue (C3)"
        '
        'lblCopyright
        '
        Me.lblCopyright.AutoSize = True
        Me.lblCopyright.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.lblCopyright.Name = "lblCopyright"
        Me.lblCopyright.Size = New System.Drawing.Size(116, 17)
        Me.lblCopyright.TabIndex = 1
        Me.lblCopyright.Text = "© Jules Carboni, "
        '
        'lblProgramVersion
        '
        Me.lblProgramVersion.AutoSize = True
        Me.lblProgramVersion.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.lblProgramVersion.Name = "lblProgramVersion"
        Me.lblProgramVersion.Size = New System.Drawing.Size(122, 17)
        Me.lblProgramVersion.TabIndex = 2
        Me.lblProgramVersion.Text = "Program Version: "
        '
        'lblCatalogueVersion
        '
        Me.lblCatalogueVersion.AutoSize = True
        Me.lblCatalogueVersion.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.lblCatalogueVersion.Name = "lblCatalogueVersion"
        Me.lblCatalogueVersion.Size = New System.Drawing.Size(128, 17)
        Me.lblCatalogueVersion.TabIndex = 3
        Me.lblCatalogueVersion.Text = "Catalogue Version:"
        '
        'lblProgramDate
        '
        Me.lblProgramDate.AutoSize = True
        Me.lblProgramDate.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.lblProgramDate.Name = "lblProgramDate"
        Me.lblProgramDate.Size = New System.Drawing.Size(0, 17)
        Me.lblProgramDate.TabIndex = 4
        '
        'lblWebsite
        '
        Me.lblWebsite.AutoSize = True
        Me.lblWebsite.Margin = New System.Windows.Forms.Padding(0, 0, 6, 6)
        Me.lblWebsite.Name = "lblWebsite"
        Me.lblWebsite.Size = New System.Drawing.Size(67, 17)
        Me.lblWebsite.TabIndex = 5
        Me.lblWebsite.Text = "Website: "
        '
        'lnkWebsite
        '
        Me.lnkWebsite.AutoSize = True
        Me.lnkWebsite.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.lnkWebsite.Name = "lnkWebsite"
        Me.lnkWebsite.Size = New System.Drawing.Size(0, 17)
        Me.lnkWebsite.TabIndex = 0
        '
        'lblContactEmail
        '
        Me.lblContactEmail.AutoSize = True
        Me.lblContactEmail.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.lblContactEmail.Name = "lblContactEmail"
        Me.lblContactEmail.Size = New System.Drawing.Size(64, 17)
        Me.lblContactEmail.TabIndex = 6
        Me.lblContactEmail.Text = "Contact: "
        '
        'lnkContactEmail
        '
        Me.lnkContactEmail.AutoSize = True
        Me.lnkContactEmail.Margin = New System.Windows.Forms.Padding(0)
        Me.lnkContactEmail.Name = "lnkContactEmail"
        Me.lnkContactEmail.Size = New System.Drawing.Size(0, 17)
        Me.lnkContactEmail.TabIndex = 1
        '
        'flpAboutCommands
        '
        Me.flpAboutCommands.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpAboutCommands.AutoSize = True
        Me.flpAboutCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpAboutCommands.Controls.Add(Me.btnOK)
        Me.flpAboutCommands.Location = New System.Drawing.Point(425, 385)
        Me.flpAboutCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.flpAboutCommands.Name = "flpAboutCommands"
        Me.flpAboutCommands.Size = New System.Drawing.Size(103, 33)
        Me.flpAboutCommands.TabIndex = 2
        Me.flpAboutCommands.WrapContents = False
        '
        'btnOK
        '
        Me.btnOK.AutoSize = True
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(3, 3)
        Me.btnOK.MinimumSize = New System.Drawing.Size(96, 27)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(96, 27)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmAbout
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.btnOK
        Me.ClientSize = New System.Drawing.Size(540, 430)
        Me.Controls.Add(Me.tlpAboutRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAbout"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About Compact Cassette Catalogue"
        Me.TopMost = True
        Me.tlpAboutRoot.ResumeLayout(False)
        Me.tlpAboutRoot.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpAboutBody.ResumeLayout(False)
        Me.tlpAboutBody.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlInformation.ResumeLayout(False)
        Me.pnlInformation.PerformLayout()
        Me.tlpAboutDetails.ResumeLayout(False)
        Me.tlpAboutDetails.PerformLayout()
        Me.flpAboutCommands.ResumeLayout(False)
        Me.flpAboutCommands.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents tlpAboutRoot As TableLayoutPanel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents tlpAboutBody As TableLayoutPanel
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents pnlInformation As Panel
    Friend WithEvents tlpAboutDetails As TableLayoutPanel
    Friend WithEvents lblProgramName As Label
    Friend WithEvents lblCopyright As Label
    Friend WithEvents lblProgramVersion As Label
    Friend WithEvents lblCatalogueVersion As Label
    Friend WithEvents lblProgramDate As Label
    Friend WithEvents lblWebsite As Label
    Friend WithEvents lnkWebsite As LinkLabel
    Friend WithEvents lblContactEmail As Label
    Friend WithEvents lnkContactEmail As LinkLabel
    Friend WithEvents flpAboutCommands As FlowLayoutPanel
    Friend WithEvents btnOK As Button
End Class
