<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModelNew
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmModelNew))
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.cmbType = New System.Windows.Forms.ComboBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.cmbBrand = New System.Windows.Forms.ComboBox()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.grpExtra = New System.Windows.Forms.GroupBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.grpBasic.SuspendLayout()
        Me.grpNotes.SuspendLayout()
        Me.grpExtra.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtModel
        '
        Me.txtModel.Location = New System.Drawing.Point(62, 81)
        Me.txtModel.MaxLength = 100
        Me.txtModel.Name = "txtModel"
        Me.txtModel.Size = New System.Drawing.Size(144, 22)
        Me.txtModel.TabIndex = 3
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(6, 84)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(50, 17)
        Me.lblModel.TabIndex = 24
        Me.lblModel.Text = "Model:"
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.lblCode)
        Me.grpBasic.Controls.Add(Me.txtCode)
        Me.grpBasic.Controls.Add(Me.cmbType)
        Me.grpBasic.Controls.Add(Me.lblType)
        Me.grpBasic.Controls.Add(Me.cmbBrand)
        Me.grpBasic.Controls.Add(Me.lblModel)
        Me.grpBasic.Controls.Add(Me.lblBrand)
        Me.grpBasic.Controls.Add(Me.txtModel)
        Me.grpBasic.Location = New System.Drawing.Point(12, 12)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Size = New System.Drawing.Size(212, 138)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(6, 112)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(45, 17)
        Me.lblCode.TabIndex = 29
        Me.lblCode.Text = "Code:"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(62, 109)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(144, 22)
        Me.txtCode.TabIndex = 4
        '
        'cmbType
        '
        Me.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbType.FormattingEnabled = True
        Me.cmbType.Items.AddRange(New Object() {"Type I (Ferric)", "Type II (Chrome)", "Type III (Ferro-Chrome)", "Type IV (Metal)"})
        Me.cmbType.Location = New System.Drawing.Point(62, 51)
        Me.cmbType.Name = "cmbType"
        Me.cmbType.Size = New System.Drawing.Size(144, 24)
        Me.cmbType.TabIndex = 2
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(6, 54)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(44, 17)
        Me.lblType.TabIndex = 27
        Me.lblType.Text = "Type:"
        '
        'cmbBrand
        '
        Me.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBrand.FormattingEnabled = True
        Me.cmbBrand.Location = New System.Drawing.Point(62, 21)
        Me.cmbBrand.Name = "cmbBrand"
        Me.cmbBrand.Size = New System.Drawing.Size(144, 24)
        Me.cmbBrand.Sorted = True
        Me.cmbBrand.TabIndex = 1
        '
        'lblBrand
        '
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(6, 24)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(50, 17)
        Me.lblBrand.TabIndex = 23
        Me.lblBrand.Text = "Brand:"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(6, 21)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(200, 54)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(230, 68)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Size = New System.Drawing.Size(212, 82)
        Me.grpNotes.TabIndex = 3
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(12, 156)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(212, 25)
        Me.btnAdd.TabIndex = 4
        Me.btnAdd.Text = "Add Model"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'lblAdd
        '
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(227, 160)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(210, 17)
        Me.lblAdd.TabIndex = 45
        Me.lblAdd.Text = "Save new models via main form."
        '
        'grpExtra
        '
        Me.grpExtra.Controls.Add(Me.lblName)
        Me.grpExtra.Controls.Add(Me.txtName)
        Me.grpExtra.Location = New System.Drawing.Point(230, 12)
        Me.grpExtra.Name = "grpExtra"
        Me.grpExtra.Size = New System.Drawing.Size(212, 50)
        Me.grpExtra.TabIndex = 2
        Me.grpExtra.TabStop = False
        Me.grpExtra.Text = "Extra"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(6, 24)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(49, 17)
        Me.lblName.TabIndex = 29
        Me.lblName.Text = "Name:"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(62, 21)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(144, 22)
        Me.txtName.TabIndex = 1
        '
        'frmModelNew
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(454, 192)
        Me.Controls.Add(Me.grpExtra)
        Me.Controls.Add(Me.grpBasic)
        Me.Controls.Add(Me.grpNotes)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.lblAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmModelNew"
        Me.Text = "Add New Model"
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.grpExtra.ResumeLayout(False)
        Me.grpExtra.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtModel As TextBox
    Friend WithEvents lblModel As Label
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents lblBrand As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents btnAdd As Button
    Friend WithEvents lblAdd As Label
    Friend WithEvents cmbBrand As ComboBox
    Friend WithEvents cmbType As ComboBox
    Private WithEvents lblType As Label
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Friend WithEvents grpExtra As GroupBox
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
End Class
