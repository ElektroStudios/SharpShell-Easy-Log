#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " ThemeManager "

Namespace SharpShellEasyLog.Theming

    ''' <summary>
    ''' Applies a <see cref="ThemePalette"/> to a tree of Windows Forms controls.
    ''' </summary>
    Public NotInheritable Class ThemeManager

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ThemeManager"/> class from being created.
        ''' </summary>
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Applies the specified palette to the specified control and all its descendant controls.
        ''' <para></para>
        ''' The control tree is traversed iteratively (no recursion).
        ''' <para></para>
        ''' Applying <see cref="ThemePalette.Light"/> restores the default system appearance of the controls.
        ''' </summary>
        ''' <param name="root">
        ''' The root control (typically a <see cref="Form"/>).
        ''' </param>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="root"/> or <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        <DebuggerStepperBoundary>
        Public Shared Sub ApplyTheme(root As Control, palette As ThemePalette)

            If root Is Nothing Then
                Throw New ArgumentNullException(NameOf(root))
            End If

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            root.SuspendLayout()
            Try
                Dim pending As New Stack(Of Control)
                pending.Push(root)

                While pending.Count > 0
                    Dim ctrl As Control = pending.Pop()
                    ThemeManager.ApplyThemeToControl(ctrl, palette)

                    For Each child As Control In ctrl.Controls
                        pending.Push(child)
                    Next
                End While

            Finally
                root.ResumeLayout(performLayout:=True)

            End Try

            Dim form As Form = TryCast(root, Form)
            If (form IsNot Nothing) AndAlso form.IsHandleCreated Then
                WindowThemeHelper.SetDarkTitleBar(form.Handle, palette.IsDark, redrawNonClientArea:=form.Visible)
            End If

            root.Invalidate(invalidateChildren:=True)

        End Sub

        ''' <summary>
        ''' Applies the specified palette to the specified tool strip (or menu strip),
        ''' including the drop-down menus of its items.
        ''' </summary>
        ''' <param name="toolStrip">
        ''' The tool strip.
        ''' </param>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="toolStrip"/> or <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        <DebuggerStepperBoundary>
        Public Shared Sub ApplyTheme(toolStrip As ToolStrip, palette As ThemePalette)

            If toolStrip Is Nothing Then
                Throw New ArgumentNullException(NameOf(toolStrip))
            End If

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Dim renderer As ToolStripRenderer = Nothing
            If palette.IsDark Then
                renderer = New ThemedToolStripRenderer(palette)
                toolStrip.Renderer = renderer
                toolStrip.BackColor = palette.WindowBackColor
                toolStrip.ForeColor = palette.WindowForeColor
            Else
                toolStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode
                toolStrip.ResetBackColor()
                toolStrip.ResetForeColor()
            End If

            Dim pendingItems As New Stack(Of ToolStripItem)
            For Each item As ToolStripItem In toolStrip.Items
                pendingItems.Push(item)
            Next

            While pendingItems.Count > 0
                Dim item As ToolStripItem = pendingItems.Pop()

                Dim dropDownItem As ToolStripDropDownItem = TryCast(item, ToolStripDropDownItem)
                If (dropDownItem Is Nothing) OrElse Not dropDownItem.HasDropDownItems Then
                    Continue While
                End If

                If palette.IsDark Then
                    dropDownItem.DropDown.Renderer = renderer
                Else
                    dropDownItem.DropDown.RenderMode = ToolStripRenderMode.ManagerRenderMode
                End If

                For Each subItem As ToolStripItem In dropDownItem.DropDownItems
                    pendingItems.Push(subItem)
                Next
            End While

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Applies the specified palette to a single control (not to its children).
        ''' </summary>
        ''' <param name="ctrl">
        ''' The control.
        ''' </param>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        <DebuggerStepperBoundary>
        Private Shared Sub ApplyThemeToControl(ctrl As Control, palette As ThemePalette)

            Dim themeable As IThemeableControl = TryCast(ctrl, IThemeableControl)
            If themeable IsNot Nothing Then
                themeable.ApplyTheme(palette)
                Return
            End If

            Select Case True

                Case TypeOf ctrl Is Form
                    ctrl.BackColor = palette.WindowBackColor
                    ctrl.ForeColor = palette.WindowForeColor

                Case TypeOf ctrl Is ToolStrip
                    ThemeManager.ApplyTheme(DirectCast(ctrl, ToolStrip), palette)

                Case TypeOf ctrl Is TabPage
                    Dim page As TabPage = DirectCast(ctrl, TabPage)
                    If palette.IsDark Then
                        page.UseVisualStyleBackColor = False
                        page.BackColor = palette.SurfaceBackColor
                        page.ForeColor = palette.WindowForeColor
                    Else
                        page.ResetBackColor()
                        page.ResetForeColor()
                        page.UseVisualStyleBackColor = True
                    End If

                Case TypeOf ctrl Is TextBoxBase
                    ThemeManager.ApplyThemeToTextBox(DirectCast(ctrl, TextBoxBase), palette)

                Case TypeOf ctrl Is Button
                    ThemeManager.ApplyThemeToButton(DirectCast(ctrl, Button), palette)

                Case Else
                    ' Labels, radio buttons, check boxes, group boxes, panels, etc.
                    ' Their background is transparent or inherited from the parent (ambient property).
                    If palette.IsDark Then
                        ctrl.ForeColor = palette.WindowForeColor
                    Else
                        ctrl.ResetForeColor()
                    End If

            End Select

        End Sub

        ''' <summary>
        ''' Applies the specified palette to a <see cref="TextBoxBase"/> control.
        ''' </summary>
        ''' <param name="textBox">
        ''' The text box.
        ''' </param>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        <DebuggerStepperBoundary>
        Private Shared Sub ApplyThemeToTextBox(textBox As TextBoxBase, palette As ThemePalette)

            If palette.IsDark Then
                textBox.BackColor = If(textBox.ReadOnly AndAlso Not textBox.Multiline, palette.ReadOnlyInputBackColor, palette.InputBackColor)
                textBox.ForeColor = palette.InputForeColor
            Else
                textBox.ResetBackColor()
                textBox.ResetForeColor()
            End If

            ' The scroll bars are native parts that only follow the dark theme through SetWindowTheme.
            ' The handler is (re)attached once, and evaluates the current back color each time the handle is created.
            If textBox.Multiline Then
                RemoveHandler textBox.HandleCreated, AddressOf ThemeManager.TextBox_HandleCreated
                AddHandler textBox.HandleCreated, AddressOf ThemeManager.TextBox_HandleCreated

                If textBox.IsHandleCreated Then
                    WindowThemeHelper.SetScrollBarsTheme(textBox.Handle, palette.IsDark)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Applies the specified palette to a <see cref="Button"/> control.
        ''' </summary>
        ''' <param name="button">
        ''' The button.
        ''' </param>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        <DebuggerStepperBoundary>
        Friend Shared Sub ApplyThemeToButton(button As Button, palette As ThemePalette)

            If palette.IsDark Then
                button.FlatStyle = FlatStyle.Flat
                button.UseVisualStyleBackColor = False
                button.BackColor = palette.ButtonBackColor
                button.ForeColor = palette.WindowForeColor
                button.FlatAppearance.BorderColor = palette.ButtonBorderColor
                button.FlatAppearance.MouseOverBackColor = palette.ButtonHoverBackColor
                button.FlatAppearance.MouseDownBackColor = palette.ButtonPressedBackColor
            Else
                button.FlatStyle = FlatStyle.Standard
                button.ResetBackColor()
                button.ResetForeColor()
                button.FlatAppearance.BorderColor = Color.Empty
                button.FlatAppearance.MouseOverBackColor = Color.Empty
                button.FlatAppearance.MouseDownBackColor = Color.Empty
                button.UseVisualStyleBackColor = True
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Control.HandleCreated"/> event of multiline text boxes,
        ''' to apply the scroll bars theme that matches their current back color.
        ''' </summary>
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Private Shared Sub TextBox_HandleCreated(sender As Object, e As EventArgs)

            Dim textBox As TextBoxBase = DirectCast(sender, TextBoxBase)
            WindowThemeHelper.SetScrollBarsTheme(textBox.Handle, ThemePalette.IsDarkColor(textBox.BackColor))

        End Sub

#End Region

    End Class

End Namespace

#End Region
