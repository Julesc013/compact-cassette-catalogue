Public NotInheritable Class frmAbout

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Load data into text labels.

        lblProgramName.Text = "Compact Cassette Catalogue (C3)"
        lblCopyright.Text = "© Jules Carboni, " & VERSIONDATE.Year.ToString

        lblProgramVersion.Text = "Program Version: " & VERSIONSTAGE & " " & VERSION
        lblCatalogueVersion.Text = "Catalogue Version: " & VERSIONFILE
        lblProgramDate.Text = VERSIONDATE.ToLongDateString

        'lblWebsite.Text = "Website: " & WEBSITE
        'lblContactEmail.Text = "Contact: " & CONTACTEMAIL
        lnkWebsite.Text = "Compact Cassette Catalogue"
        lnkContactEmail.Text = CONTACTEMAIL

    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub

    Private Sub PnlInformation_Paint(sender As Object, e As PaintEventArgs) Handles pnlInformation.Paint

    End Sub

    Private Sub LblWebsiteLink_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkWebsite.LinkClicked

        Process.Start(WEBSITEMAIN)

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkContactEmail.LinkClicked

        Process.Start("mailto:" & CONTACTEMAIL)

    End Sub
End Class
