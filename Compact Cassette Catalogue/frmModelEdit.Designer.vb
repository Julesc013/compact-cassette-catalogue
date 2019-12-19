<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModelEdit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmModelEdit))
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.txtModel = New System.Windows.Forms.TextBox()
        Me.grpBasic = New System.Windows.Forms.GroupBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.grpExtra = New System.Windows.Forms.GroupBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.grpNotes = New System.Windows.Forms.GroupBox()
        Me.lblAdd = New System.Windows.Forms.Label()
        Me.txtType = New System.Windows.Forms.TextBox()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.grpBasic.SuspendLayout()
        Me.grpExtra.SuspendLayout()
        Me.grpNotes.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(11, 132)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(160, 21)
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "Update Model"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Location = New System.Drawing.Point(4, 70)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(39, 13)
        Me.lblModel.TabIndex = 24
        Me.lblModel.Text = "Model:"
        '
        'txtModel
        '
        Me.txtModel.Location = New System.Drawing.Point(46, 67)
        Me.txtModel.Margin = New System.Windows.Forms.Padding(2)
        Me.txtModel.MaxLength = 100
        Me.txtModel.Name = "txtModel"
        Me.txtModel.ReadOnly = True
        Me.txtModel.Size = New System.Drawing.Size(109, 20)
        Me.txtModel.TabIndex = 1
        '
        'grpBasic
        '
        Me.grpBasic.Controls.Add(Me.txtBrand)
        Me.grpBasic.Controls.Add(Me.txtType)
        Me.grpBasic.Controls.Add(Me.lblCode)
        Me.grpBasic.Controls.Add(Me.txtCode)
        Me.grpBasic.Controls.Add(Me.lblType)
        Me.grpBasic.Controls.Add(Me.lblModel)
        Me.grpBasic.Controls.Add(Me.lblBrand)
        Me.grpBasic.Controls.Add(Me.txtModel)
        Me.grpBasic.Location = New System.Drawing.Point(11, 11)
        Me.grpBasic.Margin = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Name = "grpBasic"
        Me.grpBasic.Padding = New System.Windows.Forms.Padding(2)
        Me.grpBasic.Size = New System.Drawing.Size(160, 117)
        Me.grpBasic.TabIndex = 5
        Me.grpBasic.TabStop = False
        Me.grpBasic.Text = "Basic"
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(4, 94)
        Me.lblCode.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(35, 13)
        Me.lblCode.TabIndex = 29
        Me.lblCode.Text = "Code:"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(46, 91)
        Me.txtCode.Margin = New System.Windows.Forms.Padding(2)
        Me.txtCode.MaxLength = 2
        Me.txtCode.Name = "txtCode"
        Me.txtCode.ReadOnly = True
        Me.txtCode.Size = New System.Drawing.Size(109, 20)
        Me.txtCode.TabIndex = 5
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 45)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 27
        Me.lblType.Text = "Type:"
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
        Me.txtNotes.Location = New System.Drawing.Point(5, 17)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(150, 47)
        Me.txtNotes.TabIndex = 1
        Me.txtNotes.WordWrap = False
        '
        'grpExtra
        '
        Me.grpExtra.Controls.Add(Me.lblName)
        Me.grpExtra.Controls.Add(Me.txtName)
        Me.grpExtra.Location = New System.Drawing.Point(175, 11)
        Me.grpExtra.Margin = New System.Windows.Forms.Padding(2)
        Me.grpExtra.Name = "grpExtra"
        Me.grpExtra.Padding = New System.Windows.Forms.Padding(2)
        Me.grpExtra.Size = New System.Drawing.Size(160, 43)
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
        Me.txtName.Margin = New System.Windows.Forms.Padding(2)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(109, 20)
        Me.txtName.TabIndex = 1
        '
        'grpNotes
        '
        Me.grpNotes.Controls.Add(Me.txtNotes)
        Me.grpNotes.Location = New System.Drawing.Point(175, 58)
        Me.grpNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.grpNotes.Name = "grpNotes"
        Me.grpNotes.Padding = New System.Windows.Forms.Padding(2)
        Me.grpNotes.Size = New System.Drawing.Size(160, 70)
        Me.grpNotes.TabIndex = 3
        Me.grpNotes.TabStop = False
        Me.grpNotes.Text = "Notes"
        '
        'lblAdd
        '
        Me.lblAdd.AutoSize = True
        Me.lblAdd.Location = New System.Drawing.Point(179, 136)
        Me.lblAdd.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblAdd.Name = "lblAdd"
        Me.lblAdd.Size = New System.Drawing.Size(144, 13)
        Me.lblAdd.TabIndex = 50
        Me.lblAdd.Text = "Save changes via main form."
        '
        'txtType
        '
        Me.txtType.Location = New System.Drawing.Point(46, 42)
        Me.txtType.Margin = New System.Windows.Forms.Padding(2)
        Me.txtType.MaxLength = 100
        Me.txtType.Name = "txtType"
        Me.txtType.ReadOnly = True
        Me.txtType.Size = New System.Drawing.Size(109, 20)
        Me.txtType.TabIndex = 7
        '
        'txtBrand
        '
        Me.txtBrand.Location = New System.Drawing.Point(46, 17)
        Me.txtBrand.Margin = New System.Windows.Forms.Padding(2)
        Me.txtBrand.MaxLength = 100
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.ReadOnly = True
        Me.txtBrand.Size = New System.Drawing.Size(109, 20)
        Me.txtBrand.TabIndex = 6
        '
        'frmModelEdit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(347, 163)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.grpBasic)
        Me.Controls.Add(Me.grpExtra)
        Me.Controls.Add(Me.grpNotes)
        Me.Controls.Add(Me.lblAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmModelEdit"
        Me.Text = "Edit Model"
        Me.grpBasic.ResumeLayout(False)
        Me.grpBasic.PerformLayout()
        Me.grpExtra.ResumeLayout(False)
        Me.grpExtra.PerformLayout()
        Me.grpNotes.ResumeLayout(False)
        Me.grpNotes.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnUpdate As Button
    Friend WithEvents lblModel As Label
    Friend WithEvents txtModel As TextBox
    Friend WithEvents grpBasic As GroupBox
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCode As TextBox
    Private WithEvents lblType As Label
    Friend WithEvents lblBrand As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents grpExtra As GroupBox
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents grpNotes As GroupBox
    Friend WithEvents lblAdd As Label
    Friend WithEvents txtBrand As TextBox
    Friend WithEvents txtType As TextBox
End Class
