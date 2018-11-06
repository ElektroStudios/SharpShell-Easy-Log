
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32

#End Region

Public NotInheritable Class FormMain

#Region " Private Fields "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The <see cref="FileSystemWatcher"/> used to monitor new entries added in the log file.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private WithEvents Watcher As FileSystemWatcher

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determine whether <see cref="FormMain"/> is fully loaded.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isFormInitialized As Boolean

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Keep track of the log file path.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private logFilePath As String

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Keep track of the last position in the <see cref="FileStream"/> used to read the log file.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private lastStreamPos As Long

#End Region

#Region " Event Handlers "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Form.Load"/> event of the <see cref="FormMain"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub FormMain_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.MinimumSize = Me.Size
        Me.isFormInitialized = True

        Me.LoadUserSettings()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="RadioButton.CheckedChanged"/> event of the 
    ''' <see cref="RadioButtonLoggingEnable"/> and
    ''' <see cref="RadioButtonLoggingDisable"/> controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub RadioButtonsLogging_CheckedChanged(sender As Object, e As EventArgs) _
    Handles RadioButtonLoggingEnable.CheckedChanged,
            RadioButtonLoggingDisable.CheckedChanged

        If Not (Me.isFormInitialized) Then
            Exit Sub
        End If

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If (rb.Checked) Then
            My.Settings.LoggingIndex = CInt(rb.Tag)

            Select Case My.Settings.LoggingIndex
                Case CInt(Me.RadioButtonLoggingEnable.Tag)
                    Me.GroupBoxMode.Enabled = True
                    Me.GroupBoxLogFile.Enabled = ({4, 7}.Contains(My.Settings.ModeIndex))

                Case CInt(Me.RadioButtonLoggingDisable.Tag)
                    Me.GroupBoxMode.Enabled = False
                    Me.GroupBoxLogFile.Enabled = False
            End Select

            Me.UpdateRegistryValues()
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="RadioButton.CheckedChanged"/> event of the 
    ''' <see cref="RadioButtonModeDebug"/>, 
    ''' <see cref="RadioButtonModeWindowsEventLog"/>, 
    ''' <see cref="RadioButtonModeLogFile"/> and
    ''' <see cref="RadioButtonModeAll"/> controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub RadioButtonsMode_CheckedChanged(sender As Object, e As EventArgs) _
    Handles RadioButtonModeDebug.CheckedChanged,
            RadioButtonModeWindowsEventLog.CheckedChanged,
            RadioButtonModeLogFile.CheckedChanged,
            RadioButtonModeAll.CheckedChanged

        If Not (Me.isFormInitialized) Then
            Exit Sub
        End If

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If (rb.Checked) Then
            My.Settings.ModeIndex = CInt(rb.Tag)

            Me.GroupBoxLogFile.Enabled = ({4, 7}.Contains(My.Settings.ModeIndex)) AndAlso (My.Settings.LoggingIndex = 1)
        End If

        Me.UpdateRegistryValues()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="ButtonLogFilePath"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonLogFilePath_Click(sender As Object, e As EventArgs) Handles ButtonLogFilePath.Click

        Using sfd As New SaveFileDialog() With {
            .AddExtension = True,
            .CheckPathExists = True,
            .DefaultExt = "log",
            .InitialDirectory = Path.GetTempPath(),
            .FileName = String.Format("SharpShell.{0}.{1}", Date.Now.ToString("dd-MMM-yyyy.HH_mm_ss", CultureInfo.InvariantCulture), .DefaultExt),
            .Filter = "All files (*.*)|*.*|log files (*.log)|*.log|txt files (*.txt)|*.txt",
            .FilterIndex = 2,
            .OverwritePrompt = True,
            .RestoreDirectory = True,
            .ShowHelp = False,
            .SupportMultiDottedExtensions = False,
            .Title = "Create a SharpShell Log File",
            .ValidateNames = True
        }

            If (sfd.ShowDialog = DialogResult.OK) Then
                My.Settings.LogFilePath = sfd.FileName
                Me.TextBoxLogFilePath.Text = My.Settings.LogFilePath

                Me.UpdateRegistryValues()
            End If
        End Using

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.TextChanged"/> event of the <see cref="TextBoxLogFilePath"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub TextBoxLogFilePath_TextChanged(sender As Object, e As EventArgs) Handles TextBoxLogFilePath.TextChanged

        If (Me.Watcher IsNot Nothing) Then
            Me.Watcher.Dispose()
        End If

        Me.Watcher = New FileSystemWatcher() With {
            .Path = Path.GetDirectoryName(My.Settings.LogFilePath),
            .Filter = Path.GetFileName(My.Settings.LogFilePath),
            .EnableRaisingEvents = True,
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.LastWrite
        }

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="TextBox.TextChanged"/> event of the <see cref="TextBoxLogFile"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub TextBoxLogFile_TextChanged(sender As Object, e As EventArgs) Handles TextBoxLogFile.TextChanged

        Dim tb As TextBox = DirectCast(sender, TextBox)
        Dim isEmpty As Boolean = (tb.TextLength = 0)

        Me.ButtonOpenFile.Enabled = Not isEmpty
        Me.ButtonCopyText.Enabled = Not isEmpty
        Me.ButtonClearText.Enabled = Not isEmpty

        tb.ScrollToCaret()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="ButtonClearText"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonClearText_Click(sender As Object, e As EventArgs) Handles ButtonClearText.Click

        Me.TextBoxLogFile.Clear()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="ButtonCopyText"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonCopyText_Click(sender As Object, e As EventArgs) Handles ButtonCopyText.Click

        Clipboard.SetText(Me.TextBoxLogFile.Text)

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Button.Click"/> event of the <see cref="ButtonOpenFile"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonOpenFile_Click(sender As Object, e As EventArgs) Handles ButtonOpenFile.Click

        Try
            Process.Start(My.Settings.LogFilePath)

        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, Me.Name, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripMenuItem.Click"/> event of the <see cref="DebugViewToolStripMenuItem"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub DebugViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DebugViewToolStripMenuItem.Click

        Dim tmpName As String = Path.Combine(Path.GetTempPath, "DebugView.exe")

        If Not File.Exists(tmpName) Then
            File.WriteAllBytes(tmpName, My.Resources.Dbgview)
        End If

        Try
            Using pr As New Process
                pr.StartInfo.FileName = tmpName
                pr.StartInfo.ErrorDialog = True
                pr.StartInfo.ErrorDialogParentHandle = Me.Handle
                pr.StartInfo.UseShellExecute = True
                pr.StartInfo.WorkingDirectory = Path.GetDirectoryName(tmpName)

                pr.Start()
            End Using

        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, Me.Name, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripMenuItem.Click"/> event of the <see cref="DebugViewToolStripMenuItem"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub RestartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartToolStripMenuItem.Click

        Try
            Using pr As New Process
                pr.StartInfo.FileName = "Taskkill.exe"
                pr.StartInfo.Arguments = "/F /IM Explorer.exe"
                pr.StartInfo.ErrorDialog = True
                pr.StartInfo.ErrorDialogParentHandle = Me.Handle
                pr.StartInfo.UseShellExecute = True
                pr.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)

                pr.Start()
                pr.WaitForExit(Timeout.Infinite)
            End Using

            Using pr As New Process
                pr.StartInfo.FileName = "Explorer.exe"
                pr.StartInfo.ErrorDialog = True
                pr.StartInfo.ErrorDialogParentHandle = Me.Handle
                pr.StartInfo.UseShellExecute = True
                pr.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)

                pr.Start()
            End Using

        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, Me.Name, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripMenuItem.Click"/> event of the <see cref="WindowsEventViewerToolStripMenuItem"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub WindowsEventViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WindowsEventViewerToolStripMenuItem.Click

        Try
            Using pr As New Process
                pr.StartInfo.FileName = "EventVwr.exe"
                pr.StartInfo.ErrorDialog = True
                pr.StartInfo.ErrorDialogParentHandle = Me.Handle
                pr.StartInfo.UseShellExecute = True
                pr.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)

                pr.Start()
            End Using

        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, Me.Name, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="FileSystemWatcher.Changed"/> event of the <see cref="Watcher"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="FileSystemEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub Watcher_Changed(sender As Object, e As FileSystemEventArgs) Handles Watcher.Changed

        If (My.Settings.LoggingIndex = 0) Then
            Exit Sub
        End If

        Select Case e.ChangeType

            Case WatcherChangeTypes.Changed

                Using fs As New FileStream(My.Settings.LogFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
                    fs.Seek(Me.lastStreamPos, SeekOrigin.Begin)

                    Using sr As New StreamReader(fs)

                        Dim logContent As String = sr.ReadToEnd()
                        Me.lastStreamPos = fs.Position

                        Me.TextBoxLogFile.Invoke(
                            Sub()
                                Me.TextBoxLogFile.AppendText(logContent)
                                Me.TextBoxLogFile.ScrollToCaret()
                            End Sub)

                    End Using

                End Using

        End Select

    End Sub

#End Region

#Region " Private Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads the user settings into the UI controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub LoadUserSettings()

        Dim rbLogging As RadioButton =
            (From rb As RadioButton In Me.GroupBoxLogging.Controls.OfType(Of RadioButton)
             Where CInt(rb.Tag) = My.Settings.LoggingIndex).
             DefaultIfEmpty(Me.RadioButtonLoggingDisable).
             SingleOrDefault()

        rbLogging.Checked = True

        Dim rbMode As RadioButton =
            (From rb As RadioButton In Me.GroupBoxMode.Controls.OfType(Of RadioButton)
             Where CInt(rb.Tag) = My.Settings.ModeIndex).
             DefaultIfEmpty(Me.RadioButtonModeDebug).
             SingleOrDefault()

        rbMode.Checked = True

        If Not String.IsNullOrEmpty(My.Settings.LogFilePath) Then
            Me.TextBoxLogFilePath.Text = My.Settings.LogFilePath
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the values of <c>HKLM\SOFTWARE\SharpShell</c> registry key.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateRegistryValues()

        If (My.Settings.LoggingIndex = 0) Then
            Using key As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                key.DeleteSubKey("SOFTWARE\SharpShell", throwOnMissingSubKey:=False)
            End Using

        Else
            Using key As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default),
                  subkey As RegistryKey = key.CreateSubKey("SOFTWARE\SharpShell", writable:=True)

                subkey.SetValue("LoggingMode", My.Settings.ModeIndex, RegistryValueKind.DWord)

                If ({4, 7}.Contains(My.Settings.ModeIndex)) AndAlso Not String.IsNullOrEmpty(My.Settings.LogFilePath) Then
                    subkey.SetValue("LogPath", My.Settings.LogFilePath, RegistryValueKind.String)
                Else
                    subkey.DeleteValue("LogPath", throwOnMissingValue:=False)
                End If
            End Using

        End If

    End Sub

#End Region

End Class
