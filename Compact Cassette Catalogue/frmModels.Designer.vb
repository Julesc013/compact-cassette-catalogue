<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModels
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmModels))
        Me.splitBrowseRoot = New System.Windows.Forms.SplitContainer()
        Me.tlpBrowseRight = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpBrowseFooter = New System.Windows.Forms.TableLayoutPanel()
        Me.flpBrowseStatus = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkTypeBetter = New System.Windows.Forms.CheckBox()
        Me.grpModels = New System.Windows.Forms.GroupBox()
        Me.lstModels = New System.Windows.Forms.ListView()
        Me.colIdentifier = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colBrand = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colModel = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCode = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNumber = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNotes = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.grpActions = New System.Windows.Forms.GroupBox()
        Me.btnAddModel = New System.Windows.Forms.Button()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.grpFilters = New System.Windows.Forms.GroupBox()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.cmbBrand = New System.Windows.Forms.ComboBox()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.cmbTypes = New System.Windows.Forms.ComboBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.lblResults = New System.Windows.Forms.Label()
        Me.txtResults = New System.Windows.Forms.TextBox()
        CType(Me.splitBrowseRoot, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitBrowseRoot.Panel1.SuspendLayout()
        Me.splitBrowseRoot.Panel2.SuspendLayout()
        Me.splitBrowseRoot.SuspendLayout()
        Me.tlpBrowseRight.SuspendLayout()
        Me.tlpBrowseFooter.SuspendLayout()
        Me.flpBrowseStatus.SuspendLayout()
        Me.grpModels.SuspendLayout()
        Me.grpActions.SuspendLayout()
        Me.grpFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitBrowseRoot
        '
        Me.splitBrowseRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitBrowseRoot.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitBrowseRoot.Location = New System.Drawing.Point(0, 0)
        Me.splitBrowseRoot.Name = "splitBrowseRoot"
        Me.splitBrowseRoot.Panel1.AutoScroll = True
        Me.splitBrowseRoot.Panel1.Controls.Add(Me.grpFilters)
        Me.splitBrowseRoot.Panel1.Padding = New System.Windows.Forms.Padding(11, 11, 4, 8)
        Me.splitBrowseRoot.Panel2.Controls.Add(Me.tlpBrowseRight)
        Me.splitBrowseRoot.Panel2.Padding = New System.Windows.Forms.Padding(4, 11, 11, 8)
        Me.splitBrowseRoot.Size = New System.Drawing.Size(900, 560)
        Me.splitBrowseRoot.SplitterDistance = 189
        Me.splitBrowseRoot.SplitterWidth = 4
        Me.splitBrowseRoot.TabIndex = 0
        '
        'tlpBrowseRight
        '
        Me.tlpBrowseRight.ColumnCount = 1
        Me.tlpBrowseRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpBrowseRight.Controls.Add(Me.grpModels, 0, 0)
        Me.tlpBrowseRight.Controls.Add(Me.tlpBrowseFooter, 0, 1)
        Me.tlpBrowseRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpBrowseRight.Location = New System.Drawing.Point(4, 11)
        Me.tlpBrowseRight.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpBrowseRight.Name = "tlpBrowseRight"
        Me.tlpBrowseRight.RowCount = 2
        Me.tlpBrowseRight.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpBrowseRight.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBrowseRight.Size = New System.Drawing.Size(692, 541)
        Me.tlpBrowseRight.TabIndex = 0
        '
        'tlpBrowseFooter
        '
        Me.tlpBrowseFooter.AutoSize = True
        Me.tlpBrowseFooter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpBrowseFooter.ColumnCount = 2
        Me.tlpBrowseFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpBrowseFooter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpBrowseFooter.Controls.Add(Me.flpBrowseStatus, 0, 0)
        Me.tlpBrowseFooter.Controls.Add(Me.grpActions, 1, 0)
        Me.tlpBrowseFooter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpBrowseFooter.Location = New System.Drawing.Point(0, 427)
        Me.tlpBrowseFooter.Margin = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.tlpBrowseFooter.Name = "tlpBrowseFooter"
        Me.tlpBrowseFooter.RowCount = 1
        Me.tlpBrowseFooter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpBrowseFooter.Size = New System.Drawing.Size(692, 114)
        Me.tlpBrowseFooter.TabIndex = 1
        '
        'flpBrowseStatus
        '
        Me.flpBrowseStatus.AutoSize = True
        Me.flpBrowseStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpBrowseStatus.Controls.Add(Me.lblResults)
        Me.flpBrowseStatus.Controls.Add(Me.txtResults)
        Me.flpBrowseStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpBrowseStatus.Location = New System.Drawing.Point(0, 0)
        Me.flpBrowseStatus.Margin = New System.Windows.Forms.Padding(0, 0, 6, 0)
        Me.flpBrowseStatus.Name = "flpBrowseStatus"
        Me.flpBrowseStatus.Padding = New System.Windows.Forms.Padding(0, 4, 0, 0)
        Me.flpBrowseStatus.Size = New System.Drawing.Size(508, 114)
        Me.flpBrowseStatus.TabIndex = 0
        Me.flpBrowseStatus.WrapContents = False
        '
        'chkTypeBetter
        '
        Me.chkTypeBetter.AutoSize = True
        Me.chkTypeBetter.Enabled = False
        Me.chkTypeBetter.Location = New System.Drawing.Point(46, 69)
        Me.chkTypeBetter.Margin = New System.Windows.Forms.Padding(2)
        Me.chkTypeBetter.Name = "chkTypeBetter"
        Me.chkTypeBetter.Size = New System.Drawing.Size(101, 17)
        Me.chkTypeBetter.TabIndex = 3
        Me.chkTypeBetter.Text = "Type I or better."
        Me.chkTypeBetter.UseVisualStyleBackColor = True
        '
        'grpModels
        '
        Me.grpModels.Controls.Add(Me.lstModels)
        Me.grpModels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpModels.Location = New System.Drawing.Point(0, 0)
        Me.grpModels.Margin = New System.Windows.Forms.Padding(0)
        Me.grpModels.Name = "grpModels"
        Me.grpModels.Padding = New System.Windows.Forms.Padding(2)
        Me.grpModels.Size = New System.Drawing.Size(692, 422)
        Me.grpModels.TabIndex = 2
        Me.grpModels.TabStop = False
        Me.grpModels.Text = "Models"
        '
        'lstModels
        '
        Me.lstModels.AllowColumnReorder = True
        Me.lstModels.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colIdentifier, Me.colBrand, Me.colType, Me.colModel, Me.colCode, Me.colName, Me.colNumber, Me.colNotes})
        Me.lstModels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstModels.HideSelection = False
        Me.lstModels.Location = New System.Drawing.Point(2, 15)
        Me.lstModels.Margin = New System.Windows.Forms.Padding(0)
        Me.lstModels.Name = "lstModels"
        Me.lstModels.Size = New System.Drawing.Size(688, 405)
        Me.lstModels.TabIndex = 43
        Me.lstModels.UseCompatibleStateImageBehavior = False
        Me.lstModels.View = System.Windows.Forms.View.Details
        '
        'colIdentifier
        '
        Me.colIdentifier.Text = "Identifier"
        Me.colIdentifier.Width = 68
        '
        'colBrand
        '
        Me.colBrand.Text = "Brand"
        Me.colBrand.Width = 80
        '
        'colType
        '
        Me.colType.Text = "Type"
        Me.colType.Width = 74
        '
        'colModel
        '
        Me.colModel.Text = "Model"
        Me.colModel.Width = 98
        '
        'colCode
        '
        Me.colCode.Text = "Code"
        Me.colCode.Width = 40
        '
        'colName
        '
        Me.colName.Text = "Full Name"
        Me.colName.Width = 102
        '
        'colNumber
        '
        Me.colNumber.Text = "Number"
        '
        'colNotes
        '
        Me.colNotes.Text = "Notes"
        Me.colNotes.Width = 140
        '
        'btnAddModel
        '
        Me.btnAddModel.AccessibleDescription = "Add a model and create a missing brand when required."
        Me.btnAddModel.AccessibleName = "Add Model"
        Me.btnAddModel.Location = New System.Drawing.Point(4, 17)
        Me.btnAddModel.Margin = New System.Windows.Forms.Padding(2)
        Me.btnAddModel.Name = "btnAddModel"
        Me.btnAddModel.Size = New System.Drawing.Size(170, 21)
        Me.btnAddModel.TabIndex = 1
        Me.btnAddModel.Text = "Add &Model…"
        Me.btnAddModel.UseVisualStyleBackColor = True
        '
        'grpActions
        '
        Me.grpActions.Controls.Add(Me.btnAddModel)
        Me.grpActions.Controls.Add(Me.btnEdit)
        Me.grpActions.Controls.Add(Me.btnRefresh)
        Me.grpActions.Controls.Add(Me.Label1)
        Me.grpActions.Controls.Add(Me.btnDelete)
        Me.grpActions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpActions.Location = New System.Drawing.Point(514, 2)
        Me.grpActions.Margin = New System.Windows.Forms.Padding(0, 2, 0, 2)
        Me.grpActions.Name = "grpActions"
        Me.grpActions.Padding = New System.Windows.Forms.Padding(2)
        Me.grpActions.Size = New System.Drawing.Size(178, 110)
        Me.grpActions.TabIndex = 3
        Me.grpActions.TabStop = False
        Me.grpActions.Text = "Actions"
        '
        'btnEdit
        '
        Me.btnEdit.Enabled = False
        Me.btnEdit.Location = New System.Drawing.Point(4, 67)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(83, 21)
        Me.btnEdit.TabIndex = 3
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(4, 42)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(2)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(170, 21)
        Me.btnRefresh.TabIndex = 2
        Me.btnRefresh.Text = "Refresh List"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 90)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(144, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Changes are saved with the catalogue."
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(91, 67)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(83, 21)
        Me.btnDelete.TabIndex = 4
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'grpFilters
        '
        Me.grpFilters.Controls.Add(Me.txtName)
        Me.grpFilters.Controls.Add(Me.lblName)
        Me.grpFilters.Controls.Add(Me.chkTypeBetter)
        Me.grpFilters.Controls.Add(Me.txtNotes)
        Me.grpFilters.Controls.Add(Me.lblNotes)
        Me.grpFilters.Controls.Add(Me.cmbBrand)
        Me.grpFilters.Controls.Add(Me.lblBrand)
        Me.grpFilters.Controls.Add(Me.cmbTypes)
        Me.grpFilters.Controls.Add(Me.lblType)
        Me.grpFilters.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpFilters.Location = New System.Drawing.Point(11, 11)
        Me.grpFilters.Margin = New System.Windows.Forms.Padding(0)
        Me.grpFilters.Name = "grpFilters"
        Me.grpFilters.Padding = New System.Windows.Forms.Padding(2)
        Me.grpFilters.Size = New System.Drawing.Size(178, 140)
        Me.grpFilters.TabIndex = 1
        Me.grpFilters.TabStop = False
        Me.grpFilters.Text = "Filters"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(46, 90)
        Me.txtName.Margin = New System.Windows.Forms.Padding(2)
        Me.txtName.MaxLength = 100
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(127, 20)
        Me.txtName.TabIndex = 5
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(3, 93)
        Me.lblName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 27
        Me.lblName.Text = "Name:"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(46, 114)
        Me.txtNotes.Margin = New System.Windows.Forms.Padding(2)
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(127, 20)
        Me.txtNotes.TabIndex = 6
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(3, 117)
        Me.lblNotes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(38, 13)
        Me.lblNotes.TabIndex = 8
        Me.lblNotes.Text = "Notes:"
        '
        'cmbBrand
        '
        Me.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBrand.FormattingEnabled = True
        Me.cmbBrand.Items.AddRange(New Object() {"All Brands"})
        Me.cmbBrand.Location = New System.Drawing.Point(46, 17)
        Me.cmbBrand.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbBrand.Name = "cmbBrand"
        Me.cmbBrand.Size = New System.Drawing.Size(127, 21)
        Me.cmbBrand.TabIndex = 1
        '
        'lblBrand
        '
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Location = New System.Drawing.Point(4, 20)
        Me.lblBrand.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(38, 13)
        Me.lblBrand.TabIndex = 1
        Me.lblBrand.Text = "Brand:"
        '
        'cmbTypes
        '
        Me.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTypes.FormattingEnabled = True
        Me.cmbTypes.Items.AddRange(New Object() {"All Types", "Type I (Ferric)", "Type II (Chrome)", "Type III (Ferrichrome)", "Type IV (Metal)"})
        Me.cmbTypes.Location = New System.Drawing.Point(46, 42)
        Me.cmbTypes.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbTypes.Name = "cmbTypes"
        Me.cmbTypes.Size = New System.Drawing.Size(127, 21)
        Me.cmbTypes.TabIndex = 2
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(4, 45)
        Me.lblType.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 8
        Me.lblType.Text = "Type:"
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblResults.Location = New System.Drawing.Point(2, 10)
        Me.lblResults.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(88, 13)
        Me.lblResults.TabIndex = 30
        Me.lblResults.Text = "Results (Filtered):"
        '
        'txtResults
        '
        Me.txtResults.Location = New System.Drawing.Point(96, 6)
        Me.txtResults.Margin = New System.Windows.Forms.Padding(2)
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ReadOnly = True
        Me.txtResults.Size = New System.Drawing.Size(87, 20)
        Me.txtResults.TabIndex = 4
        Me.txtResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'frmModels
        '
        Me.AcceptButton = Me.btnRefresh
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = False
        Me.ClientSize = New System.Drawing.Size(900, 560)
        Me.Controls.Add(Me.splitBrowseRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = True
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "frmModels"
        Me.Text = "View Models"
        Me.splitBrowseRoot.Panel1.ResumeLayout(False)
        Me.splitBrowseRoot.Panel2.ResumeLayout(False)
        CType(Me.splitBrowseRoot, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitBrowseRoot.ResumeLayout(False)
        Me.tlpBrowseRight.ResumeLayout(False)
        Me.tlpBrowseRight.PerformLayout()
        Me.tlpBrowseFooter.ResumeLayout(False)
        Me.tlpBrowseFooter.PerformLayout()
        Me.flpBrowseStatus.ResumeLayout(False)
        Me.flpBrowseStatus.PerformLayout()
        Me.grpModels.ResumeLayout(False)
        Me.grpActions.ResumeLayout(False)
        Me.grpActions.PerformLayout()
        Me.grpFilters.ResumeLayout(False)
        Me.grpFilters.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents splitBrowseRoot As SplitContainer
    Friend WithEvents tlpBrowseRight As TableLayoutPanel
    Friend WithEvents tlpBrowseFooter As TableLayoutPanel
    Friend WithEvents flpBrowseStatus As FlowLayoutPanel

    Friend WithEvents chkTypeBetter As CheckBox
    Friend WithEvents grpModels As GroupBox
    Friend WithEvents grpActions As GroupBox
    Friend WithEvents btnAddModel As Button
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnDelete As Button
    Friend WithEvents grpFilters As GroupBox
    Friend WithEvents txtNotes As TextBox
    Private WithEvents lblNotes As Label
    Friend WithEvents cmbTypes As ComboBox
    Private WithEvents lblType As Label
    Friend WithEvents cmbBrand As ComboBox
    Private WithEvents lblBrand As Label
    Friend WithEvents txtName As TextBox
    Private WithEvents lblName As Label
    Friend WithEvents lblResults As Label
    Friend WithEvents txtResults As TextBox
    Friend WithEvents btnEdit As Button
    Friend WithEvents lstModels As ListView
    Friend WithEvents colIdentifier As ColumnHeader
    Friend WithEvents colBrand As ColumnHeader
    Friend WithEvents colType As ColumnHeader
    Friend WithEvents colModel As ColumnHeader
    Friend WithEvents colCode As ColumnHeader
    Friend WithEvents colName As ColumnHeader
    Friend WithEvents colNotes As ColumnHeader
    Friend WithEvents colNumber As ColumnHeader
End Class
