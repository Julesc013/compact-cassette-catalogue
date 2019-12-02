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
        Me.txtModel.Location = New System.Drawing.Point(46, 66)
        Me.txtModel.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtModel.MaxLength = 100
        Me.txtModel.Name = "txtModel"
        Me.txtModel.Size = New System.Drawing.Size(109, 20)
        Me.txtModel.TabIndex = 3
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 68)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
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
        Me.grpBasic.Location = New System.Drawing.Point(9, 10)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpBasic.Size = New System.Drawing.Size(159, 112)
        Me.grpBasic.TabIndex = 1
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(4, 91)
        Me.lblCode.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(35, 13)
        Me.lblCode.TabIndex = 29
        Me.lblCode.Text = "Code:"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(46, 89)
        Me.txtCode.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(109, 20)
        Me.txtCode.TabIndex = 4
        '
        'cmbType
        '
        Me.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbType.FormattingEnabled = True
        Me.cmbType.Items.AddRange(New Object() {"Type I (Ferric)", "Type II (Chrome)", "Type III (Ferro-Chrome)", "Type IV (Metal)"})
        Me.cmbType.Location = New System.Drawing.Point(46, 41)
        Me.cmbType.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbType.Name = "cmbType"
        Me.cmbType.Size = New System.Drawing.Size(109, 21)
        Me.cmbType.TabIndex = 2
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 44)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 27
        Me.lblType.Text = "Type:"
        '
        'cmbBrand
        '
        Me.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBrand.FormattingEnabled = True
        Me.cmbBrand.Location = New System.Drawing.Point(46, 17)
        Me.cmbBrand.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbBrand.Name = "cmbBrand"
        Me.cmbBrand.Size = New System.Drawing.Size(109, 21)
        Me.cmbBrand.Sorted = True
        Me.cmbBrand.TabIndex = 1
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
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(4, 17)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(151, 45)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(172, 55)
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpNotes.Size = New System.Drawing.Size(159, 67)
        Me.grpNotes.TabIndex = 3
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(9, 127)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(159, 20)
        Me.btnAdd.TabIndex = 4
        Me.btnAdd.Text = "Add Model"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'lblAdd
        '
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(170, 130)
        Me.lblAdd.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(144, 13)
        Me.lblAdd.TabIndex = 45
        Me.lblAdd.Text = "Save changes via main form."
        '
        'grpExtra
        '
        Me.grpExtra.Controls.Add(Me.lblName)
        Me.grpExtra.Controls.Add(Me.txtName)
        Me.grpExtra.Location = New System.Drawing.Point(172, 10)
        Me.grpExtra.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpExtra.Name = "grpExtra"
        Me.grpExtra.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grpExtra.Size = New System.Drawing.Size(159, 41)
        Me.grpExtra.TabIndex = 2
        Me.grpExtra.TabStop = False
        Me.grpExtra.Text = "Extra"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(4, 20)
        Me.lblName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 29
        Me.lblName.Text = "Name:"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(46, 17)
        Me.txtName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(109, 20)
        Me.txtName.TabIndex = 1
        '
        'frmModelNew
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(340, 156)
        Me.Controls.Add(Me.grpExtra)
        Me.Controls.Add(Me.grpBasic)
        Me.Controls.Add(Me.grpNotes)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.lblAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
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
