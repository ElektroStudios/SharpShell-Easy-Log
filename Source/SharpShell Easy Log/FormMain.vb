#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Threading

Imports SharpShellEasyLog.Core
Imports SharpShellEasyLog.Theming

#End Region

''' ----------------------------------------------------------------------------------------------------
''' <summary>
''' The main window of the application.
''' </summary>
''' ----------------------------------------------------------------------------------------------------
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
    ''' The <see cref="LogFileTailReader"/> used to read the new entries added in the log file.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private tailReader As LogFileTailReader

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determine whether <see cref="FormMain"/> is fully loaded.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isFormInitialized As Boolean

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determine whether the theme selector controls are being updated programmatically,
    ''' to ignore their change events.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isUpdatingThemeSelectors As Boolean

#End Region

#Region " Properties "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the visual theme currently applied to the form.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend ReadOnly Property CurrentTheme As AppTheme

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the SharpShell logging mode selected in the user interface.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend ReadOnly Property SelectedLoggingMode As SharpShellLoggingMode
        <DebuggerStepThrough>
        Get
            Return SharpShellLoggingModeExtensions.ResolveLoggingMode(
                isLoggingEnabled:=Me.RadioButtonLoggingEnable.Checked,
                modeValue:=CInt(Me.GroupBoxMode.Controls.OfType(Of RadioButton)().Single(Function(rb As RadioButton) rb.Checked).Tag))
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance of the <see cref="FormMain"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Sub New()

        Me.InitializeComponent()

        ' The theme is applied before the window handle is created, to avoid a visible light-to-dark transition.
        Me.ApplyTheme(If(My.Settings.UseDarkTheme, AppTheme.Dark, AppTheme.Light))

    End Sub

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
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Text = $"{My.Application.Info.Title} v{My.Application.Info.Version.ToString(3)}"

        Me.MinimumSize = Me.Size

        Me.LoadUserSettings()
        Me.UpdateControlsState()
        Me.isFormInitialized = True

        ' Keep the registry in sync with the saved settings when the logging is enabled.
        If Me.RadioButtonLoggingEnable.Checked Then
            Me.UpdateRegistryValues()
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Form.FormClosed"/> event of the <see cref="FormMain"/> control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' The <see cref="FormClosedEventArgs"/> instance containing the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub FormMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Me.StopLogFileMonitoring()

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

        If Not Me.isFormInitialized Then
            Exit Sub
        End If

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            My.Settings.LoggingIndex = CInt(rb.Tag)

            Me.UpdateControlsState()
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

        If Not Me.isFormInitialized Then
            Exit Sub
        End If

        Dim rb As RadioButton = DirectCast(sender, RadioButton)
        If rb.Checked Then
            My.Settings.ModeIndex = CInt(rb.Tag)

            Me.UpdateControlsState()
            Me.UpdateRegistryValues()
        End If

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
            .FileName = $"SharpShell.{Date.Now.ToString("dd-MMM-yyyy.HH_mm_ss", CultureInfo.InvariantCulture)}.log",
            .Filter = "All files (*.*)|*.*|log files (*.log)|*.log|txt files (*.txt)|*.txt",
            .FilterIndex = 2,
            .OverwritePrompt = True,
            .RestoreDirectory = True,
            .ShowHelp = False,
            .SupportMultiDottedExtensions = False,
            .Title = "Create a SharpShell Log File",
            .ValidateNames = True
        }

            If sfd.ShowDialog(Me) = DialogResult.OK Then
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

        ' The output of the previous log file (if any) does not belong to the new log file.
        Me.TextBoxLogFile.Clear()
        Me.StartLogFileMonitoring(Me.TextBoxLogFilePath.Text)

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

        Try
            Clipboard.SetText(Me.TextBoxLogFile.Text)

        Catch ex As ExternalException
            ' The clipboard is being used by another process.
            Me.ShowError(ex.Message)

        End Try

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
            Using pr As Process = Process.Start(My.Settings.LogFilePath)
            End Using

        Catch ex As Exception
            Me.ShowError(ex.Message)

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

        Dim tmpName As String = Path.Combine(Path.GetTempPath(), "DebugView.exe")

        Try
            If Not File.Exists(tmpName) Then
                File.WriteAllBytes(tmpName, My.Resources.Dbgview)
            End If

            Using pr As New Process
                pr.StartInfo.FileName = tmpName
                pr.StartInfo.ErrorDialog = True
                pr.StartInfo.ErrorDialogParentHandle = Me.Handle
                pr.StartInfo.UseShellExecute = True
                pr.StartInfo.WorkingDirectory = Path.GetDirectoryName(tmpName)

                pr.Start()
            End Using

        Catch ex As Exception
            Me.ShowError(ex.Message)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripMenuItem.Click"/> event of the <see cref="RestartToolStripMenuItem"/> control.
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
                pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
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
            Me.ShowError(ex.Message)

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
            Me.ShowError(ex.Message)

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="UserControls.ToolStripToggleSwitch.CheckedChanged"/> event
    ''' of the <see cref="ToolStripThemeToggle"/> control.
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
    Private Sub ToolStripThemeToggle_CheckedChanged(sender As Object, e As EventArgs) Handles ToolStripThemeToggle.CheckedChanged

        If Me.isUpdatingThemeSelectors Then
            Exit Sub
        End If

        Me.ChangeThemeAndSave(If(Me.ToolStripThemeToggle.Checked, AppTheme.Dark, AppTheme.Light))

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripItem.Click"/> event of the <see cref="ToolStripThemeLabel"/> control.
    ''' <para></para>
    ''' Clicking the label toggles the theme too, like the label of a check box.
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
    Private Sub ToolStripThemeLabel_Click(sender As Object, e As EventArgs) Handles ToolStripThemeLabel.Click

        Me.ToolStripThemeToggle.ToggleSwitch.Toggle()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="ToolStripMenuItem.Click"/> event of the <see cref="DarkThemeToolStripMenuItem"/> control.
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
    Private Sub DarkThemeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DarkThemeToolStripMenuItem.Click

        Me.ChangeThemeAndSave(If(Me.CurrentTheme = AppTheme.Dark, AppTheme.Light, AppTheme.Dark))

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="FileSystemWatcher.Changed"/>, <see cref="FileSystemWatcher.Created"/>
    ''' and <see cref="FileSystemWatcher.Renamed"/> events of the <see cref="Watcher"/> object.
    ''' <para></para>
    ''' The events are raised in the UI thread, because <see cref="FileSystemWatcher.SynchronizingObject"/> is set to the form.
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
    Private Sub Watcher_Changed(sender As Object, e As FileSystemEventArgs) Handles Watcher.Changed, Watcher.Created, Watcher.Renamed

        Me.ReadNewLogContent()

    End Sub

#End Region

#Region " Overridden Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event.
    ''' <para></para>
    ''' The dark title bar must be applied here: calling <c>DwmSetWindowAttribute</c>
    ''' from the <see cref="Form.Load"/> event has no effect on the first paint of the window.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="e">
    ''' An <see cref="EventArgs"/> that contains the event data.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnHandleCreated(e As EventArgs)

        MyBase.OnHandleCreated(e)
        WindowThemeHelper.SetDarkTitleBar(Me.Handle, Me.CurrentTheme = AppTheme.Dark, redrawNonClientArea:=False)

    End Sub

#End Region

#Region " Friend Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the specified visual theme to the form and all its controls,
    ''' and updates the theme selector controls and <see cref="My.MySettings.UseDarkTheme"/>.
    ''' <para></para>
    ''' The settings are not saved to disk by this method.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="theme">
    ''' The theme to apply.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="InvalidEnumArgumentException">
    ''' Thrown when <paramref name="theme"/> is not a defined <see cref="AppTheme"/> value.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Sub ApplyTheme(theme As AppTheme)

        Dim palette As ThemePalette = ThemePalette.FromTheme(theme)

        Me._CurrentTheme = theme
        ThemeManager.ApplyTheme(Me, palette)

        Me.isUpdatingThemeSelectors = True
        Try
            Me.ToolStripThemeToggle.Checked = palette.IsDark
            Me.DarkThemeToolStripMenuItem.Checked = palette.IsDark

        Finally
            Me.isUpdatingThemeSelectors = False

        End Try

        My.Settings.UseDarkTheme = palette.IsDark

    End Sub

#End Region

#Region " Private Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the specified visual theme and saves the user settings to disk.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="theme">
    ''' The theme to apply.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ChangeThemeAndSave(theme As AppTheme)

        Me.ApplyTheme(theme)
        My.Settings.Save()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads the user settings into the UI controls.
    ''' <para></para>
    ''' Invalid stored values are replaced by the default values, and the settings are normalized accordingly.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub LoadUserSettings()

        Dim rbLogging As RadioButton =
            (From rb As RadioButton In Me.GroupBoxLogging.Controls.OfType(Of RadioButton)()
             Where CInt(rb.Tag) = My.Settings.LoggingIndex).
             DefaultIfEmpty(Me.RadioButtonLoggingDisable).
             Single()

        rbLogging.Checked = True
        My.Settings.LoggingIndex = CInt(rbLogging.Tag)

        Dim rbMode As RadioButton =
            (From rb As RadioButton In Me.GroupBoxMode.Controls.OfType(Of RadioButton)()
             Where CInt(rb.Tag) = My.Settings.ModeIndex).
             DefaultIfEmpty(Me.RadioButtonModeDebug).
             Single()

        rbMode.Checked = True
        My.Settings.ModeIndex = CInt(rbMode.Tag)

        If Not String.IsNullOrEmpty(My.Settings.LogFilePath) Then
            Me.TextBoxLogFilePath.Text = My.Settings.LogFilePath
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Enables or disables the group boxes according to the selected logging mode.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateControlsState()

        Dim mode As SharpShellLoggingMode = Me.SelectedLoggingMode

        Me.GroupBoxMode.Enabled = (mode <> SharpShellLoggingMode.Disabled)
        Me.GroupBoxLogFile.Enabled = mode.UsesLogFile()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the values of <c>HKLM\SOFTWARE\SharpShell</c> registry key,
    ''' in the 64-bit and the 32-bit registry views.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateRegistryValues()

        Try
            SharpShellRegistrySettings.ApplyToLocalMachine(Me.SelectedLoggingMode, My.Settings.LogFilePath)

        Catch ex As Exception When (TypeOf ex Is UnauthorizedAccessException) OrElse
                                   (TypeOf ex Is SecurityException) OrElse
                                   (TypeOf ex Is IOException)
            Me.ShowError($"Unable to update the SharpShell registry settings. Run this application as Administrator.{Environment.NewLine}{Environment.NewLine}{ex.Message}")

        End Try

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Starts monitoring the specified log file, and reads its current content.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="logFilePath">
    ''' The log file path. If it is empty, the monitoring is only stopped.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub StartLogFileMonitoring(logFilePath As String)

        Me.StopLogFileMonitoring()

        If String.IsNullOrWhiteSpace(logFilePath) Then
            Exit Sub
        End If

        Me.tailReader = New LogFileTailReader(logFilePath)

        Dim directoryPath As String = Path.GetDirectoryName(Me.tailReader.FilePath)
        If Not Directory.Exists(directoryPath) Then
            ' FileSystemWatcher cannot monitor a non-existent directory (e.g. a cleaned temp folder).
            Exit Sub
        End If

        Me.Watcher = New FileSystemWatcher(directoryPath, Path.GetFileName(Me.tailReader.FilePath)) With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.Size Or NotifyFilters.FileName,
            .SynchronizingObject = Me
        }
        Me.Watcher.EnableRaisingEvents = True

        Me.ReadNewLogContent()

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Stops monitoring the current log file.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub StopLogFileMonitoring()

        If Me.Watcher IsNot Nothing Then
            Me.Watcher.EnableRaisingEvents = False
            Me.Watcher.Dispose()
            Me.Watcher = Nothing
        End If

        Me.tailReader = Nothing

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Reads the new content of the monitored log file and appends it to <see cref="TextBoxLogFile"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ReadNewLogContent()

        If (Me.tailReader Is Nothing) OrElse Me.IsDisposed Then
            Exit Sub
        End If

        Dim content As String
        Try
            content = Me.tailReader.ReadNewContent()

        Catch ex As IOException
            ' The file is temporarily locked by the writer process.
            ' The read position is not advanced, so the content will be read on the next change notification.
            Exit Sub

        End Try

        If content.Length <> 0 Then
            Me.TextBoxLogFile.AppendText(content)
        End If

    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Shows an error message box.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="message">
    ''' The error message.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ShowError(message As String)

        MessageBox.Show(Me, message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)

    End Sub

#End Region

End Class
