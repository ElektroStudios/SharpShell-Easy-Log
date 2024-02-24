<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain : Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
        Me.RadioButtonLoggingEnable = New System.Windows.Forms.RadioButton()
        Me.RadioButtonLoggingDisable = New System.Windows.Forms.RadioButton()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPageSettings = New System.Windows.Forms.TabPage()
        Me.GroupBoxLogFile = New System.Windows.Forms.GroupBox()
        Me.ButtonLogFilePath = New System.Windows.Forms.Button()
        Me.TextBoxLogFilePath = New System.Windows.Forms.TextBox()
        Me.LabelLogFilePath = New System.Windows.Forms.Label()
        Me.GroupBoxMode = New System.Windows.Forms.GroupBox()
        Me.RadioButtonModeAll = New System.Windows.Forms.RadioButton()
        Me.RadioButtonModeLogFile = New System.Windows.Forms.RadioButton()
        Me.RadioButtonModeWindowsEventLog = New System.Windows.Forms.RadioButton()
        Me.RadioButtonModeDebug = New System.Windows.Forms.RadioButton()
        Me.GroupBoxLogging = New System.Windows.Forms.GroupBox()
        Me.TabPageLogOutput = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBoxActions = New System.Windows.Forms.GroupBox()
        Me.ButtonOpenFile = New System.Windows.Forms.Button()
        Me.ButtonClearText = New System.Windows.Forms.Button()
        Me.ButtonCopyText = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExplorerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RestartToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WindowsEventViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DebugViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControlMain.SuspendLayout()
        Me.TabPageSettings.SuspendLayout()
        Me.GroupBoxLogFile.SuspendLayout()
        Me.GroupBoxMode.SuspendLayout()
        Me.GroupBoxLogging.SuspendLayout()
        Me.TabPageLogOutput.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBoxActions.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBoxLogFile
        '
        Me.TextBoxLogFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxLogFile.Location = New System.Drawing.Point(3, 63)
        Me.TextBoxLogFile.MaxLength = 2147483647
        Me.TextBoxLogFile.Multiline = True
        Me.TextBoxLogFile.Name = "TextBoxLogFile"
        Me.TextBoxLogFile.ReadOnly = True
        Me.TextBoxLogFile.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxLogFile.Size = New System.Drawing.Size(652, 307)
        Me.TextBoxLogFile.TabIndex = 2
        '
        'RadioButtonLoggingEnable
        '
        Me.RadioButtonLoggingEnable.AutoSize = True
        Me.RadioButtonLoggingEnable.Location = New System.Drawing.Point(6, 42)
        Me.RadioButtonLoggingEnable.Name = "RadioButtonLoggingEnable"
        Me.RadioButtonLoggingEnable.Size = New System.Drawing.Size(58, 17)
        Me.RadioButtonLoggingEnable.TabIndex = 3
        Me.RadioButtonLoggingEnable.Tag = "1"
        Me.RadioButtonLoggingEnable.Text = "Enable"
        Me.RadioButtonLoggingEnable.UseVisualStyleBackColor = True
        '
        'RadioButtonLoggingDisable
        '
        Me.RadioButtonLoggingDisable.AutoSize = True
        Me.RadioButtonLoggingDisable.Checked = True
        Me.RadioButtonLoggingDisable.Location = New System.Drawing.Point(6, 19)
        Me.RadioButtonLoggingDisable.Name = "RadioButtonLoggingDisable"
        Me.RadioButtonLoggingDisable.Size = New System.Drawing.Size(60, 17)
        Me.RadioButtonLoggingDisable.TabIndex = 4
        Me.RadioButtonLoggingDisable.TabStop = True
        Me.RadioButtonLoggingDisable.Tag = "0"
        Me.RadioButtonLoggingDisable.Text = "Disable"
        Me.RadioButtonLoggingDisable.UseVisualStyleBackColor = True
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPageSettings)
        Me.TabControlMain.Controls.Add(Me.TabPageLogOutput)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 24)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(672, 405)
        Me.TabControlMain.TabIndex = 7
        '
        'TabPageSettings
        '
        Me.TabPageSettings.Controls.Add(Me.GroupBoxLogFile)
        Me.TabPageSettings.Controls.Add(Me.GroupBoxMode)
        Me.TabPageSettings.Controls.Add(Me.GroupBoxLogging)
        Me.TabPageSettings.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSettings.Name = "TabPageSettings"
        Me.TabPageSettings.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSettings.Size = New System.Drawing.Size(664, 379)
        Me.TabPageSettings.TabIndex = 0
        Me.TabPageSettings.Text = "Log Settings"
        Me.TabPageSettings.UseVisualStyleBackColor = True
        '
        'GroupBoxLogFile
        '
        Me.GroupBoxLogFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxLogFile.Controls.Add(Me.ButtonLogFilePath)
        Me.GroupBoxLogFile.Controls.Add(Me.TextBoxLogFilePath)
        Me.GroupBoxLogFile.Controls.Add(Me.LabelLogFilePath)
        Me.GroupBoxLogFile.Enabled = False
        Me.GroupBoxLogFile.Location = New System.Drawing.Point(8, 193)
        Me.GroupBoxLogFile.Name = "GroupBoxLogFile"
        Me.GroupBoxLogFile.Size = New System.Drawing.Size(648, 65)
        Me.GroupBoxLogFile.TabIndex = 9
        Me.GroupBoxLogFile.TabStop = False
        Me.GroupBoxLogFile.Text = "Log File"
        '
        'ButtonLogFilePath
        '
        Me.ButtonLogFilePath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonLogFilePath.Location = New System.Drawing.Point(567, 25)
        Me.ButtonLogFilePath.Name = "ButtonLogFilePath"
        Me.ButtonLogFilePath.Size = New System.Drawing.Size(75, 23)
        Me.ButtonLogFilePath.TabIndex = 2
        Me.ButtonLogFilePath.Text = "Open..."
        Me.ButtonLogFilePath.UseVisualStyleBackColor = True
        '
        'TextBoxLogFilePath
        '
        Me.TextBoxLogFilePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxLogFilePath.Location = New System.Drawing.Point(60, 27)
        Me.TextBoxLogFilePath.Name = "TextBoxLogFilePath"
        Me.TextBoxLogFilePath.ReadOnly = True
        Me.TextBoxLogFilePath.Size = New System.Drawing.Size(501, 20)
        Me.TextBoxLogFilePath.TabIndex = 1
        '
        'LabelLogFilePath
        '
        Me.LabelLogFilePath.AutoSize = True
        Me.LabelLogFilePath.Location = New System.Drawing.Point(6, 31)
        Me.LabelLogFilePath.Name = "LabelLogFilePath"
        Me.LabelLogFilePath.Size = New System.Drawing.Size(48, 13)
        Me.LabelLogFilePath.TabIndex = 0
        Me.LabelLogFilePath.Text = "File Path"
        '
        'GroupBoxMode
        '
        Me.GroupBoxMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxMode.Controls.Add(Me.RadioButtonModeAll)
        Me.GroupBoxMode.Controls.Add(Me.RadioButtonModeLogFile)
        Me.GroupBoxMode.Controls.Add(Me.RadioButtonModeWindowsEventLog)
        Me.GroupBoxMode.Controls.Add(Me.RadioButtonModeDebug)
        Me.GroupBoxMode.Enabled = False
        Me.GroupBoxMode.Location = New System.Drawing.Point(8, 77)
        Me.GroupBoxMode.Name = "GroupBoxMode"
        Me.GroupBoxMode.Size = New System.Drawing.Size(648, 110)
        Me.GroupBoxMode.TabIndex = 8
        Me.GroupBoxMode.TabStop = False
        Me.GroupBoxMode.Text = "Mode"
        '
        'RadioButtonModeAll
        '
        Me.RadioButtonModeAll.AutoSize = True
        Me.RadioButtonModeAll.Location = New System.Drawing.Point(6, 88)
        Me.RadioButtonModeAll.Name = "RadioButtonModeAll"
        Me.RadioButtonModeAll.Size = New System.Drawing.Size(36, 17)
        Me.RadioButtonModeAll.TabIndex = 6
        Me.RadioButtonModeAll.Tag = "7"
        Me.RadioButtonModeAll.Text = "All"
        Me.RadioButtonModeAll.UseVisualStyleBackColor = True
        '
        'RadioButtonModeLogFile
        '
        Me.RadioButtonModeLogFile.AutoSize = True
        Me.RadioButtonModeLogFile.Location = New System.Drawing.Point(6, 65)
        Me.RadioButtonModeLogFile.Name = "RadioButtonModeLogFile"
        Me.RadioButtonModeLogFile.Size = New System.Drawing.Size(62, 17)
        Me.RadioButtonModeLogFile.TabIndex = 5
        Me.RadioButtonModeLogFile.Tag = "4"
        Me.RadioButtonModeLogFile.Text = "Log File"
        Me.RadioButtonModeLogFile.UseVisualStyleBackColor = True
        '
        'RadioButtonModeWindowsEventLog
        '
        Me.RadioButtonModeWindowsEventLog.AutoSize = True
        Me.RadioButtonModeWindowsEventLog.Location = New System.Drawing.Point(6, 42)
        Me.RadioButtonModeWindowsEventLog.Name = "RadioButtonModeWindowsEventLog"
        Me.RadioButtonModeWindowsEventLog.Size = New System.Drawing.Size(121, 17)
        Me.RadioButtonModeWindowsEventLog.TabIndex = 4
        Me.RadioButtonModeWindowsEventLog.Tag = "2"
        Me.RadioButtonModeWindowsEventLog.Text = "Windows Event Log"
        Me.RadioButtonModeWindowsEventLog.UseVisualStyleBackColor = True
        '
        'RadioButtonModeDebug
        '
        Me.RadioButtonModeDebug.AutoSize = True
        Me.RadioButtonModeDebug.Checked = True
        Me.RadioButtonModeDebug.Location = New System.Drawing.Point(6, 19)
        Me.RadioButtonModeDebug.Name = "RadioButtonModeDebug"
        Me.RadioButtonModeDebug.Size = New System.Drawing.Size(57, 17)
        Me.RadioButtonModeDebug.TabIndex = 3
        Me.RadioButtonModeDebug.TabStop = True
        Me.RadioButtonModeDebug.Tag = "1"
        Me.RadioButtonModeDebug.Text = "Debug"
        Me.RadioButtonModeDebug.UseVisualStyleBackColor = True
        '
        'GroupBoxLogging
        '
        Me.GroupBoxLogging.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxLogging.Controls.Add(Me.RadioButtonLoggingDisable)
        Me.GroupBoxLogging.Controls.Add(Me.RadioButtonLoggingEnable)
        Me.GroupBoxLogging.Location = New System.Drawing.Point(8, 6)
        Me.GroupBoxLogging.Name = "GroupBoxLogging"
        Me.GroupBoxLogging.Size = New System.Drawing.Size(648, 65)
        Me.GroupBoxLogging.TabIndex = 7
        Me.GroupBoxLogging.TabStop = False
        Me.GroupBoxLogging.Text = "Logging"
        '
        'TabPageLogOutput
        '
        Me.TabPageLogOutput.Controls.Add(Me.TableLayoutPanel1)
        Me.TabPageLogOutput.Location = New System.Drawing.Point(4, 22)
        Me.TabPageLogOutput.Name = "TabPageLogOutput"
        Me.TabPageLogOutput.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageLogOutput.Size = New System.Drawing.Size(664, 379)
        Me.TabPageLogOutput.TabIndex = 1
        Me.TabPageLogOutput.Text = "Log File Output"
        Me.TabPageLogOutput.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBoxActions, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(658, 373)
        Me.TableLayoutPanel1.TabIndex = 6
        '
        'GroupBoxActions
        '
        Me.GroupBoxActions.Controls.Add(Me.ButtonOpenFile)
        Me.GroupBoxActions.Controls.Add(Me.ButtonClearText)
        Me.GroupBoxActions.Controls.Add(Me.ButtonCopyText)
        Me.GroupBoxActions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxActions.Location = New System.Drawing.Point(3, 3)
        Me.GroupBoxActions.Name = "GroupBoxActions"
        Me.GroupBoxActions.Size = New System.Drawing.Size(652, 54)
        Me.GroupBoxActions.TabIndex = 7
        Me.GroupBoxActions.TabStop = False
        Me.GroupBoxActions.Text = "Actions"
        '
        'ButtonOpenFile
        '
        Me.ButtonOpenFile.Enabled = False
        Me.ButtonOpenFile.Location = New System.Drawing.Point(6, 19)
        Me.ButtonOpenFile.Name = "ButtonOpenFile"
        Me.ButtonOpenFile.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOpenFile.TabIndex = 5
        Me.ButtonOpenFile.Text = "Open File"
        Me.ButtonOpenFile.UseVisualStyleBackColor = True
        '
        'ButtonClearText
        '
        Me.ButtonClearText.Enabled = False
        Me.ButtonClearText.Location = New System.Drawing.Point(168, 19)
        Me.ButtonClearText.Name = "ButtonClearText"
        Me.ButtonClearText.Size = New System.Drawing.Size(75, 23)
        Me.ButtonClearText.TabIndex = 3
        Me.ButtonClearText.Text = "Clear Text"
        Me.ButtonClearText.UseVisualStyleBackColor = True
        '
        'ButtonCopyText
        '
        Me.ButtonCopyText.Enabled = False
        Me.ButtonCopyText.Location = New System.Drawing.Point(87, 19)
        Me.ButtonCopyText.Name = "ButtonCopyText"
        Me.ButtonCopyText.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCopyText.TabIndex = 4
        Me.ButtonCopyText.Text = "Copy Text"
        Me.ButtonCopyText.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(672, 24)
        Me.MenuStrip1.TabIndex = 8
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DebugViewToolStripMenuItem, Me.WindowsEventViewerToolStripMenuItem, Me.ExplorerToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(47, 20)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'ExplorerToolStripMenuItem
        '
        Me.ExplorerToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RestartToolStripMenuItem})
        Me.ExplorerToolStripMenuItem.Image = Global.My.Resources.Resources.ExplorerIcon
        Me.ExplorerToolStripMenuItem.Name = "ExplorerToolStripMenuItem"
        Me.ExplorerToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.ExplorerToolStripMenuItem.Text = "Explorer"
        '
        'RestartToolStripMenuItem
        '
        Me.RestartToolStripMenuItem.Name = "RestartToolStripMenuItem"
        Me.RestartToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.RestartToolStripMenuItem.Text = "Restart"
        '
        'WindowsEventViewerToolStripMenuItem
        '
        Me.WindowsEventViewerToolStripMenuItem.Image = Global.My.Resources.Resources.EventVwrIcon
        Me.WindowsEventViewerToolStripMenuItem.Name = "WindowsEventViewerToolStripMenuItem"
        Me.WindowsEventViewerToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.WindowsEventViewerToolStripMenuItem.Text = "Windows Event Viewer"
        '
        'DebugViewToolStripMenuItem
        '
        Me.DebugViewToolStripMenuItem.Image = Global.My.Resources.Resources.DbgviewIcon
        Me.DebugViewToolStripMenuItem.Name = "DebugViewToolStripMenuItem"
        Me.DebugViewToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.DebugViewToolStripMenuItem.Text = "DebugView"
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(672, 429)
        Me.Controls.Add(Me.TabControlMain)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SharpShell Easy Log"
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPageSettings.ResumeLayout(False)
        Me.GroupBoxLogFile.ResumeLayout(False)
        Me.GroupBoxLogFile.PerformLayout()
        Me.GroupBoxMode.ResumeLayout(False)
        Me.GroupBoxMode.PerformLayout()
        Me.GroupBoxLogging.ResumeLayout(False)
        Me.GroupBoxLogging.PerformLayout()
        Me.TabPageLogOutput.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBoxActions.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxLogFile As TextBox
    Friend WithEvents RadioButtonLoggingEnable As RadioButton
    Friend WithEvents RadioButtonLoggingDisable As RadioButton
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents TabPageSettings As TabPage
    Friend WithEvents GroupBoxLogFile As GroupBox
    Friend WithEvents ButtonLogFilePath As Button
    Friend WithEvents TextBoxLogFilePath As TextBox
    Friend WithEvents LabelLogFilePath As Label
    Friend WithEvents GroupBoxMode As GroupBox
    Friend WithEvents RadioButtonModeAll As RadioButton
    Friend WithEvents RadioButtonModeLogFile As RadioButton
    Friend WithEvents RadioButtonModeWindowsEventLog As RadioButton
    Friend WithEvents RadioButtonModeDebug As RadioButton
    Friend WithEvents GroupBoxLogging As GroupBox
    Friend WithEvents TabPageLogOutput As TabPage
    Friend WithEvents ButtonOpenFile As Button
    Friend WithEvents ButtonCopyText As Button
    Friend WithEvents ButtonClearText As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents GroupBoxActions As GroupBox
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DebugViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExplorerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RestartToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents WindowsEventViewerToolStripMenuItem As ToolStripMenuItem
End Class
