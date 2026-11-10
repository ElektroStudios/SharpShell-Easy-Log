#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming
Imports SharpShellEasyLog.UserControls

#End Region

#Region " ThemeManagerTests "

Namespace SharpShellEasyLog.Tests.Theming

    ''' <summary>
    ''' Tests for <see cref="ThemeManager"/>.
    ''' </summary>
    <STATestClass>
    Public NotInheritable Class ThemeManagerTests

#Region " Test Fixture "

        ''' <summary>A sample form with one control of each supported kind.</summary>
        Private form As Form
        Private tabControl As ThemedTabControl
        Private tabPage As TabPage
        Private groupBox As ThemedGroupBox
        Private plainGroupBox As GroupBox
        Private label As Label
        Private radioButton As RadioButton
        Private button As Button
        Private editableTextBox As TextBox
        Private readOnlyTextBox As TextBox
        Private multilineTextBox As TextBox
        Private menuStrip As MenuStrip
        Private menuItem As ToolStripMenuItem
        Private subMenuItem As ToolStripMenuItem
        Private toggle As ToolStripToggleSwitch

        <TestInitialize>
        Public Sub TestInitialize()

            Me.form = New Form() With {.BackColor = SystemColors.Window} ' Same as FormMain.
            Me.tabControl = New ThemedTabControl() With {.Dock = DockStyle.Fill}
            Me.tabPage = New TabPage("Page") With {.UseVisualStyleBackColor = True}
            Me.groupBox = New ThemedGroupBox() With {.Text = "Themed"}
            Me.plainGroupBox = New GroupBox() With {.Text = "Plain"}
            Me.label = New Label() With {.Text = "Label"}
            Me.radioButton = New RadioButton() With {.Text = "Radio"}
            Me.button = New Button() With {.Text = "Button", .UseVisualStyleBackColor = True}
            Me.editableTextBox = New TextBox()
            Me.readOnlyTextBox = New TextBox() With {.ReadOnly = True}
            Me.multilineTextBox = New TextBox() With {.Multiline = True, .ReadOnly = True}
            Me.menuStrip = New MenuStrip()
            Me.menuItem = New ToolStripMenuItem("Menu")
            Me.subMenuItem = New ToolStripMenuItem("Sub menu")
            Me.toggle = New ToolStripToggleSwitch()

            Me.subMenuItem.DropDownItems.Add(New ToolStripMenuItem("Nested"))
            Me.menuItem.DropDownItems.Add(Me.subMenuItem)
            Me.menuStrip.Items.AddRange(New ToolStripItem() {Me.menuItem, Me.toggle})

            Me.groupBox.Controls.AddRange(New Control() {Me.radioButton, Me.button, Me.label})
            Me.plainGroupBox.Controls.Add(Me.readOnlyTextBox)
            Me.tabPage.Controls.AddRange(New Control() {Me.groupBox, Me.plainGroupBox, Me.editableTextBox, Me.multilineTextBox})
            Me.tabControl.TabPages.Add(Me.tabPage)
            Me.form.Controls.Add(Me.tabControl)
            Me.form.Controls.Add(Me.menuStrip)

        End Sub

        <TestCleanup>
        Public Sub TestCleanup()

            Me.form.Dispose()

        End Sub

#End Region

#Region " Argument Validation "

        <TestMethod>
        Public Sub ApplyTheme_NullRoot_Throws()

            Dim ex As ArgumentNullException = Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() ThemeManager.ApplyTheme(DirectCast(Nothing, Control), ThemePalette.Dark))

            Assert.AreEqual("root", ex.ParamName)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_NullPalette_Throws()

            Dim ex As ArgumentNullException = Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() ThemeManager.ApplyTheme(Me.form, Nothing))

            Assert.AreEqual("palette", ex.ParamName)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_NullToolStrip_Throws()

            Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() ThemeManager.ApplyTheme(DirectCast(Nothing, ToolStrip), ThemePalette.Dark))

        End Sub

#End Region

#Region " Dark Theme "

        <TestMethod>
        Public Sub ApplyDark_Form_UsesWindowColors()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.WindowBackColor, Me.form.BackColor)
            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.form.ForeColor)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_TabPage_UsesSurfaceColorAndDisablesVisualStyleBackColor()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.IsFalse(Me.tabPage.UseVisualStyleBackColor)
            Assert.AreEqual(ThemePalette.Dark.SurfaceBackColor, Me.tabPage.BackColor)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_ThemeableControls_ReceiveThePalette()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreSame(ThemePalette.Dark, Me.tabControl.Palette)
            Assert.AreSame(ThemePalette.Dark, Me.groupBox.Palette)
            Assert.AreSame(ThemePalette.Dark, Me.toggle.ToggleSwitch.Palette, "Controls hosted in a tool strip must be themed too.")
            Assert.IsTrue(Me.tabControl.IsOwnerDrawn)
            Assert.IsTrue(Me.groupBox.IsCustomPainted)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_NestedControls_UseDarkForeColor()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.label.ForeColor)
            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.radioButton.ForeColor)
            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.plainGroupBox.ForeColor)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_Button_IsFlatWithDarkColors()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreEqual(FlatStyle.Flat, Me.button.FlatStyle)
            Assert.IsFalse(Me.button.UseVisualStyleBackColor)
            Assert.AreEqual(ThemePalette.Dark.ButtonBackColor, Me.button.BackColor)
            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.button.ForeColor)
            Assert.AreEqual(ThemePalette.Dark.ButtonBorderColor, Me.button.FlatAppearance.BorderColor)
            Assert.AreEqual(ThemePalette.Dark.ButtonHoverBackColor, Me.button.FlatAppearance.MouseOverBackColor)
            Assert.AreEqual(ThemePalette.Dark.ButtonPressedBackColor, Me.button.FlatAppearance.MouseDownBackColor)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_TextBoxes_UseInputColors()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.InputBackColor, Me.editableTextBox.BackColor)
            Assert.AreEqual(ThemePalette.Dark.InputForeColor, Me.editableTextBox.ForeColor)
            Assert.AreEqual(ThemePalette.Dark.ReadOnlyInputBackColor, Me.readOnlyTextBox.BackColor)
            Assert.AreEqual(ThemePalette.Dark.InputBackColor, Me.multilineTextBox.BackColor, "The read-only log viewer keeps the input background.")

        End Sub

        <TestMethod>
        Public Sub ApplyDark_MenuStrip_UsesThemedRendererInDropDownsToo()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(Me.menuStrip.Renderer)
            Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(Me.menuItem.DropDown.Renderer)
            Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(Me.subMenuItem.DropDown.Renderer, "Nested drop-downs must be themed too.")
            Assert.AreSame(ThemePalette.Dark, DirectCast(Me.menuStrip.Renderer, ThemedToolStripRenderer).Palette)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_DeeplyNestedTree_IsFullyThemedWithoutRecursion()

            Dim parent As Control = Me.tabPage
            Dim deepestLabel As Label = Nothing
            For i As Integer = 1 To 300
                Dim panel As New Panel()
                parent.Controls.Add(panel)
                parent = panel
            Next
            deepestLabel = New Label()
            parent.Controls.Add(deepestLabel)

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, deepestLabel.ForeColor)

        End Sub

        <TestMethod>
        Public Sub ApplyDark_ToSubtree_DoesNotAffectSiblings()

            ThemeManager.ApplyTheme(Me.groupBox, ThemePalette.Dark)

            Assert.AreEqual(ThemePalette.Dark.WindowForeColor, Me.label.ForeColor)
            Assert.AreNotEqual(ThemePalette.Dark.InputBackColor, Me.editableTextBox.BackColor)
            Assert.AreNotEqual(ThemePalette.Dark.WindowBackColor, Me.form.BackColor)

        End Sub

#End Region

#Region " Light Theme (restore) "

        <TestMethod>
        Public Sub ApplyLightAfterDark_RestoresDefaultAppearance()

            ' Capture the default values.
            Dim defaultEditableBack As Color = Me.editableTextBox.BackColor
            Dim defaultReadOnlyBack As Color = Me.readOnlyTextBox.BackColor
            Dim defaultButtonBack As Color = Me.button.BackColor
            Dim defaultLabelFore As Color = Me.label.ForeColor

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(Me.form, ThemePalette.Light)

            Assert.AreEqual(defaultEditableBack, Me.editableTextBox.BackColor)
            Assert.AreEqual(defaultReadOnlyBack, Me.readOnlyTextBox.BackColor)
            Assert.AreEqual(defaultButtonBack, Me.button.BackColor)
            Assert.AreEqual(defaultLabelFore, Me.label.ForeColor)
            Assert.AreEqual(FlatStyle.Standard, Me.button.FlatStyle)
            Assert.IsTrue(Me.button.UseVisualStyleBackColor)
            Assert.IsTrue(Me.tabPage.UseVisualStyleBackColor)
            Assert.AreEqual(SystemColors.Window, Me.form.BackColor)

        End Sub

        <TestMethod>
        Public Sub ApplyLightAfterDark_ReadOnlyTextBox_UsesSystemControlColor()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(Me.form, ThemePalette.Light)

            Assert.AreEqual(SystemColors.Control, Me.readOnlyTextBox.BackColor)
            Assert.AreEqual(SystemColors.Window, Me.editableTextBox.BackColor)

        End Sub

        <TestMethod>
        Public Sub ApplyLightAfterDark_DisablesOwnerDrawAndRestoresSystemRenderer()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(Me.form, ThemePalette.Light)

            Assert.IsFalse(Me.tabControl.IsOwnerDrawn)
            Assert.IsFalse(Me.groupBox.IsCustomPainted)
            Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, Me.menuStrip.RenderMode)
            Assert.IsNotInstanceOfType(Of ThemedToolStripRenderer)(Me.menuStrip.Renderer)
            Assert.IsNotInstanceOfType(Of ThemedToolStripRenderer)(Me.subMenuItem.DropDown.Renderer)

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_Repeatedly_IsIdempotent()

            For i As Integer = 1 To 5
                ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)
            Next

            Assert.AreEqual(ThemePalette.Dark.ButtonBackColor, Me.button.BackColor)

            For i As Integer = 1 To 5
                ThemeManager.ApplyTheme(Me.form, ThemePalette.Light)
            Next

            Assert.AreEqual(FlatStyle.Standard, Me.button.FlatStyle)

        End Sub

#End Region

#Region " Handles "

        <TestMethod>
        Public Sub ApplyTheme_WithCreatedHandles_DoesNotThrow()

            Me.form.CreateControl()
            Dim forceHandle As IntPtr = Me.multilineTextBox.Handle
            Assert.AreNotEqual(IntPtr.Zero, forceHandle)

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)
            ThemeManager.ApplyTheme(Me.form, ThemePalette.Light)

        End Sub

        <TestMethod>
        Public Sub DarkTheme_TextBoxHandleCreatedLater_DoesNotThrow()

            ThemeManager.ApplyTheme(Me.form, ThemePalette.Dark)

            ' The scroll bars theme is applied from the HandleCreated event.
            Dim handle As IntPtr = Me.multilineTextBox.Handle
            Assert.AreNotEqual(IntPtr.Zero, handle)

            ' Re-creating the handle must re-apply it without errors.
            Me.multilineTextBox.WordWrap = Not Me.multilineTextBox.WordWrap

        End Sub

#End Region

    End Class

End Namespace

#End Region
