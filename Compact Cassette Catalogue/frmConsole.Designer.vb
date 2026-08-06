<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConsole
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

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConsole))
        Me.tlpConsoleRoot = New System.Windows.Forms.TableLayoutPanel()
        Me.lstConsole = New System.Windows.Forms.ListBox()
        Me.tlpConsoleCommands = New System.Windows.Forms.TableLayoutPanel()
        Me.txtCommand = New System.Windows.Forms.TextBox()
        Me.btnEnter = New System.Windows.Forms.Button()
        Me.tlpConsoleRoot.SuspendLayout()
        Me.tlpConsoleCommands.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpConsoleRoot
        '
        Me.tlpConsoleRoot.ColumnCount = 1
        Me.tlpConsoleRoot.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpConsoleRoot.Controls.Add(Me.lstConsole, 0, 0)
        Me.tlpConsoleRoot.Controls.Add(Me.tlpConsoleCommands, 0, 1)
        Me.tlpConsoleRoot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpConsoleRoot.Location = New System.Drawing.Point(0, 0)
        Me.tlpConsoleRoot.Name = "tlpConsoleRoot"
        Me.tlpConsoleRoot.Padding = New System.Windows.Forms.Padding(12)
        Me.tlpConsoleRoot.RowCount = 2
        Me.tlpConsoleRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpConsoleRoot.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpConsoleRoot.Size = New System.Drawing.Size(500, 300)
        Me.tlpConsoleRoot.TabIndex = 0
        '
        'lstConsole
        '
        Me.lstConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstConsole.FormattingEnabled = True
        Me.lstConsole.HorizontalScrollbar = True
        Me.lstConsole.IntegralHeight = False
        Me.lstConsole.Location = New System.Drawing.Point(12, 12)
        Me.lstConsole.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.lstConsole.Name = "lstConsole"
        Me.lstConsole.Size = New System.Drawing.Size(476, 241)
        Me.lstConsole.TabIndex = 0
        '
        'tlpConsoleCommands
        '
        Me.tlpConsoleCommands.AutoSize = True
        Me.tlpConsoleCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpConsoleCommands.ColumnCount = 2
        Me.tlpConsoleCommands.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpConsoleCommands.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpConsoleCommands.Controls.Add(Me.txtCommand, 0, 0)
        Me.tlpConsoleCommands.Controls.Add(Me.btnEnter, 1, 0)
        Me.tlpConsoleCommands.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpConsoleCommands.Location = New System.Drawing.Point(12, 261)
        Me.tlpConsoleCommands.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpConsoleCommands.Name = "tlpConsoleCommands"
        Me.tlpConsoleCommands.RowCount = 1
        Me.tlpConsoleCommands.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.tlpConsoleCommands.Size = New System.Drawing.Size(476, 27)
        Me.tlpConsoleCommands.TabIndex = 1
        '
        'txtCommand
        '
        Me.txtCommand.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCommand.Location = New System.Drawing.Point(0, 3)
        Me.txtCommand.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.txtCommand.Name = "txtCommand"
        Me.txtCommand.Size = New System.Drawing.Size(393, 20)
        Me.txtCommand.TabIndex = 0
        '
        'btnEnter
        '
        Me.btnEnter.AutoSize = True
        Me.btnEnter.Location = New System.Drawing.Point(401, 0)
        Me.btnEnter.Margin = New System.Windows.Forms.Padding(0)
        Me.btnEnter.MinimumSize = New System.Drawing.Size(75, 27)
        Me.btnEnter.Name = "btnEnter"
        Me.btnEnter.Size = New System.Drawing.Size(75, 27)
        Me.btnEnter.TabIndex = 1
        Me.btnEnter.Text = "&Enter"
        Me.btnEnter.UseVisualStyleBackColor = True
        '
        'frmConsole
        '
        Me.AcceptButton = Me.btnEnter
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(500, 300)
        Me.Controls.Add(Me.tlpConsoleRoot)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(390, 240)
        Me.Name = "frmConsole"
        Me.Text = "Log Console"
        Me.tlpConsoleRoot.ResumeLayout(False)
        Me.tlpConsoleRoot.PerformLayout()
        Me.tlpConsoleCommands.ResumeLayout(False)
        Me.tlpConsoleCommands.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpConsoleRoot As TableLayoutPanel
    Friend WithEvents lstConsole As ListBox
    Friend WithEvents tlpConsoleCommands As TableLayoutPanel
    Friend WithEvents txtCommand As TextBox
    Friend WithEvents btnEnter As Button
End Class
