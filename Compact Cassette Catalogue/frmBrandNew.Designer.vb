<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBrandNew
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBrandNew))
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.grpNotes.SuspendLayout()
        Me.grpBasic.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(9, 78)
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpNotes.Size = New System.Drawing.Size(207, 72)
        Me.grpNotes.TabIndex = 2
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(4, 17)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(199, 51)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.lblCode)
        Me.grpBasic.Controls.Add(Me.lblBrand)
        Me.grpBasic.Controls.Add(Me.txtCode)
        Me.grpBasic.Controls.Add(Me.txtBrand)
        Me.grpBasic.Location = New System.Drawing.Point(9, 10)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpBasic.Size = New System.Drawing.Size(207, 63)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(4, 42)
        Me.lblCode.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(35, 13)
        Me.lblCode.TabIndex = 24
        Me.lblCode.Text = "Code:"
        '
        'lblBrand
        '
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(4, 20)
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(38, 13)
        Me.lblBrand.TabIndex = 23
        Me.lblBrand.Text = "Brand:"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(46, 40)
        Me.txtCode.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(36, 20)
        Me.txtCode.TabIndex = 2
        '
        'txtBrand
        '
        Me.txtBrand.Location = New System.Drawing.Point(46, 17)
        Me.txtBrand.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBrand.MaxLength = 100
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.Size = New System.Drawing.Size(157, 20)
        Me.txtBrand.TabIndex = 1
        '
        'lblAdd
        '
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(95, 158)
        Me.lblAdd.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(100, 13)
        Me.lblAdd.TabIndex = 48
        Me.lblAdd.Text = "Save via main form."
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(9, 155)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(82, 20)
        Me.btnAdd.TabIndex = 3
        Me.btnAdd.Text = "Add Brand"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'frmBrandNew
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(226, 184)
        Me.Controls.Add(Me.grpNotes)
        Me.Controls.Add(Me.grpBasic)
        Me.Controls.Add(Me.lblAdd)
        Me.Controls.Add(Me.btnAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.Name = "frmBrandNew"
        Me.Text = "Add New Brand"
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents lblCode As Label
    Friend WithEvents lblBrand As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents txtBrand As TextBox
    Friend WithEvents lblAdd As Label
    Friend WithEvents btnAdd As Button
End Class
