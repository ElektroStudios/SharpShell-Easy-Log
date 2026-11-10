#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

Imports SharpShellEasyLog.Core
Imports SharpShellEasyLog.Theming

#End Region

#Region " FormMainTests "

Namespace SharpShellEasyLog.Tests

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Tests for <see cref="FormMain"/>.
    ''' <para></para>
    ''' The form is never shown, so its <see cref="Form.Load"/> event is not raised
    ''' and the SharpShell registry key in <c>HKEY_LOCAL_MACHINE</c> is never modified.
    ''' The user settings are never saved to disk.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <STATestClass>
    Public NotInheritable Class FormMainTests

        ''' <summary>The form under test.</summary>
        Private form As FormMain

        <TestInitialize>
        Public Sub TestInitialize()

            Me.form = New FormMain()

        End Sub

        <TestCleanup>
        Public Sub TestCleanup()

            Me.form.Dispose()

        End Sub

#Region " Designer / Layout "

        <TestMethod>
        Public Sub LoggingRadioButtons_TagsMatchLoggingIndexSetting()

            Assert.AreEqual(0, CInt(Me.form.RadioButtonLoggingDisable.Tag))
            Assert.AreEqual(1, CInt(Me.form.RadioButtonLoggingEnable.Tag))

        End Sub

        <TestMethod>
        Public Sub ModeRadioButtons_TagsMatchSharpShellLoggingModeValues()

            Assert.AreEqual(CInt(SharpShellLoggingMode.Debug), CInt(Me.form.RadioButtonModeDebug.Tag))
            Assert.AreEqual(CInt(SharpShellLoggingMode.EventLog), CInt(Me.form.RadioButtonModeWindowsEventLog.Tag))
            Assert.AreEqual(CInt(SharpShellLoggingMode.File), CInt(Me.form.RadioButtonModeLogFile.Tag))
            Assert.AreEqual(CInt(SharpShellLoggingMode.All), CInt(Me.form.RadioButtonModeAll.Tag))

        End Sub

        <TestMethod>
        Public Sub ModeGroup_ContainsOneRadioButtonPerModeAndNoDuplicatedTags()

            Dim tags As Integer() = Me.form.GroupBoxMode.Controls.OfType(Of RadioButton)().
                                    Select(Function(rb As RadioButton) CInt(rb.Tag)).
                                    ToArray()

            Assert.AreEqual(4, tags.Length)
            Assert.AreEqual(tags.Length, tags.Distinct().Count())
            Assert.IsTrue(tags.All(Function(t As Integer) DirectCast(t, SharpShellLoggingMode).IsValid()))

        End Sub

        <TestMethod>
        Public Sub ThemeSelectors_AreRightAlignedInMenuStrip()

            Assert.IsTrue(Me.form.MenuStrip1.Items.Contains(Me.form.ToolStripThemeToggle))
            Assert.IsTrue(Me.form.MenuStrip1.Items.Contains(Me.form.ToolStripThemeLabel))
            Assert.AreEqual(ToolStripItemAlignment.Right, Me.form.ToolStripThemeToggle.Alignment)
            Assert.AreEqual(ToolStripItemAlignment.Right, Me.form.ToolStripThemeLabel.Alignment)

            ' Right-aligned items are laid out from right to left: the toggle must be the rightmost item.
            Assert.IsTrue(Me.form.MenuStrip1.Items.IndexOf(Me.form.ToolStripThemeToggle) <
                          Me.form.MenuStrip1.Items.IndexOf(Me.form.ToolStripThemeLabel))

        End Sub

        <TestMethod>
        Public Sub ViewMenu_ContainsDarkThemeItemWithShortcut()

            Assert.IsTrue(Me.form.ViewToolStripMenuItem.DropDownItems.Contains(Me.form.DarkThemeToolStripMenuItem))
            Assert.AreEqual(Keys.Control Or Keys.D, Me.form.DarkThemeToolStripMenuItem.ShortcutKeys)

        End Sub

        <TestMethod>
        Public Sub ThemedContainers_UseThemeableControls()

            Assert.IsInstanceOfType(Of IThemeableControl)(Me.form.TabControlMain)
            Assert.IsInstanceOfType(Of IThemeableControl)(Me.form.GroupBoxLogging)
            Assert.IsInstanceOfType(Of IThemeableControl)(Me.form.GroupBoxMode)
            Assert.IsInstanceOfType(Of IThemeableControl)(Me.form.GroupBoxLogFile)
            Assert.IsInstanceOfType(Of IThemeableControl)(Me.form.GroupBoxActions)

        End Sub

        <TestMethod>
        Public Sub MenuStrip_ThemeToggleFitsInMenuStrip()

            Me.form.MenuStrip1.PerformLayout()

            Assert.IsTrue(Me.form.ToolStripThemeToggle.Height <= Me.form.MenuStrip1.DisplayRectangle.Height,
                          "The toggle must fit in the menu strip.")

        End Sub

#End Region

#Region " Initial State "

        <TestMethod>
        Public Sub NewInstance_ThemeSelectorsMatchCurrentTheme()

            Dim isDark As Boolean = (Me.form.CurrentTheme = AppTheme.Dark)

            Assert.AreEqual(isDark, Me.form.ToolStripThemeToggle.Checked)
            Assert.AreEqual(isDark, Me.form.DarkThemeToolStripMenuItem.Checked)
            Assert.AreSame(ThemePalette.FromTheme(Me.form.CurrentTheme), Me.form.TabControlMain.Palette)

        End Sub

        <TestMethod>
        Public Sub NewInstance_DefaultSelection_IsLoggingDisabled()

            Assert.IsTrue(Me.form.RadioButtonLoggingDisable.Checked)
            Assert.AreEqual(SharpShellLoggingMode.Disabled, Me.form.SelectedLoggingMode)

        End Sub

#End Region

#Region " SelectedLoggingMode "

        <TestMethod>
        Public Sub SelectedLoggingMode_ReflectsRadioButtons()

            Me.form.RadioButtonLoggingEnable.Checked = True

            Me.form.RadioButtonModeDebug.Checked = True
            Assert.AreEqual(SharpShellLoggingMode.Debug, Me.form.SelectedLoggingMode)

            Me.form.RadioButtonModeWindowsEventLog.Checked = True
            Assert.AreEqual(SharpShellLoggingMode.EventLog, Me.form.SelectedLoggingMode)

            Me.form.RadioButtonModeLogFile.Checked = True
            Assert.AreEqual(SharpShellLoggingMode.File, Me.form.SelectedLoggingMode)

            Me.form.RadioButtonModeAll.Checked = True
            Assert.AreEqual(SharpShellLoggingMode.All, Me.form.SelectedLoggingMode)

        End Sub

        <TestMethod>
        Public Sub SelectedLoggingMode_LoggingDisabled_IsDisabledRegardlessOfMode()

            Me.form.RadioButtonModeAll.Checked = True
            Me.form.RadioButtonLoggingDisable.Checked = True

            Assert.AreEqual(SharpShellLoggingMode.Disabled, Me.form.SelectedLoggingMode)

        End Sub

#End Region

#Region " Theming "

        <TestMethod>
        Public Sub ApplyTheme_Dark_UpdatesFormControlsAndSelectors()

            Me.form.ApplyTheme(AppTheme.Dark)

            Assert.AreEqual(AppTheme.Dark, Me.form.CurrentTheme)
            Assert.IsTrue(Me.form.ToolStripThemeToggle.Checked)
            Assert.IsTrue(Me.form.DarkThemeToolStripMenuItem.Checked)
            Assert.IsTrue(Me.form.TabControlMain.IsOwnerDrawn)
            Assert.IsTrue(Me.form.GroupBoxMode.IsCustomPainted)
            Assert.AreEqual(ThemePalette.Dark.WindowBackColor, Me.form.BackColor)
            Assert.AreEqual(ThemePalette.Dark.SurfaceBackColor, Me.form.TabPageSettings.BackColor)
            Assert.AreEqual(ThemePalette.Dark.InputBackColor, Me.form.TextBoxLogFile.BackColor)
            Assert.AreEqual(ThemePalette.Dark.ReadOnlyInputBackColor, Me.form.TextBoxLogFilePath.BackColor)
            Assert.AreEqual(FlatStyle.Flat, Me.form.ButtonOpenFile.FlatStyle)
            Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(Me.form.MenuStrip1.Renderer)
            Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(Me.form.ExplorerToolStripMenuItem.DropDown.Renderer)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_LightAfterDark_RestoresSystemAppearance()

            Me.form.ApplyTheme(AppTheme.Dark)
            Me.form.ApplyTheme(AppTheme.Light)

            Assert.AreEqual(AppTheme.Light, Me.form.CurrentTheme)
            Assert.IsFalse(Me.form.ToolStripThemeToggle.Checked)
            Assert.IsFalse(Me.form.DarkThemeToolStripMenuItem.Checked)
            Assert.IsFalse(Me.form.TabControlMain.IsOwnerDrawn)
            Assert.AreEqual(SystemColors.Window, Me.form.BackColor)
            Assert.IsTrue(Me.form.TabPageSettings.UseVisualStyleBackColor)
            Assert.AreEqual(SystemColors.Control, Me.form.TextBoxLogFile.BackColor, "The read-only log viewer keeps its original read-only background.")
            Assert.AreEqual(SystemColors.Control, Me.form.TextBoxLogFilePath.BackColor)
            Assert.AreEqual(FlatStyle.Standard, Me.form.ButtonOpenFile.FlatStyle)
            Assert.IsTrue(Me.form.ButtonOpenFile.UseVisualStyleBackColor)
            Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, Me.form.MenuStrip1.RenderMode)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_UndefinedTheme_ThrowsAndKeepsCurrentTheme()

            Me.form.ApplyTheme(AppTheme.Dark)

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() Me.form.ApplyTheme(DirectCast(99, AppTheme)))

            Assert.AreEqual(AppTheme.Dark, Me.form.CurrentTheme)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_DoesNotRaiseThemeSelectorFeedbackLoop()

            ' Applying a theme updates the toggle; the toggle change must not re-apply (or flip) the theme.
            Me.form.ApplyTheme(AppTheme.Dark)
            Me.form.ApplyTheme(AppTheme.Dark)

            Assert.AreEqual(AppTheme.Dark, Me.form.CurrentTheme)
            Assert.IsTrue(Me.form.ToolStripThemeToggle.Checked)

        End Sub

        <TestMethod>
        Public Sub HandleCreation_WithDarkTheme_DoesNotThrow()

            Me.form.ApplyTheme(AppTheme.Dark)

            ' Accessing Handle creates the window (and calls OnHandleCreated) without raising the Load event.
            Dim hwnd As IntPtr = Me.form.Handle
            Assert.AreNotEqual(IntPtr.Zero, hwnd)

            ' Switching the theme with a created handle also updates the title bar.
            Me.form.ApplyTheme(AppTheme.Light)
            Me.form.ApplyTheme(AppTheme.Dark)

        End Sub

#End Region

    End Class

End Namespace

#End Region
