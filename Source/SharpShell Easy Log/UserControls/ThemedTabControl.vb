#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedTabControl "

Namespace SharpShellEasyLog.UserControls

    ''' <summary>
    ''' A <see cref="TabControl"/> that paints its tab headers and its border using the colors of a <see cref="ThemePalette"/>.
    ''' <para></para>
    ''' The native <see cref="TabControl"/> ignores <see cref="Control.BackColor"/> and always paints
    ''' light tab headers and borders, so this control owner-draws them when a dark palette is applied.
    ''' With a light palette (or when no palette is applied) the native rendering is used.
    ''' <para></para>
    ''' Custom painting is only supported for <see cref="TabAlignment.Top"/> and <see cref="TabAppearance.Normal"/>;
    ''' other layouts always use the native rendering.
    ''' </summary>
    <DesignerCategory("Code")>
    <ToolboxBitmap(GetType(TabControl))>
    Public Class ThemedTabControl : Inherits TabControl : Implements IThemeableControl

#Region " Private Fields "

        ''' <summary>
        ''' The index of the tab header under the mouse pointer, or <c>-1</c> if none.
        ''' </summary>
        Private hotTabIndex As Integer = -1

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the palette currently applied to the control,
        ''' or <see langword="Nothing"/> if no palette was applied yet.
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette Implements IThemeableControl.Palette

        ''' <summary>
        ''' Gets a value indicating whether the control is currently owner-drawn
        ''' (that is, a dark palette is applied and the tab layout supports custom painting).
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property IsOwnerDrawn As Boolean
            <DebuggerStepThrough>
            Get
                Return Me.GetStyle(ControlStyles.UserPaint)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemedTabControl"/> class.
        ''' </summary>
        <DebuggerNonUserCode>
        Public Sub New()

            MyBase.New()

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' <para></para>
        ''' The tab pages are not themed by this method;
        ''' use <see cref="ThemeManager.ApplyTheme(Control, ThemePalette)"/> to theme the whole control tree.
        ''' </summary>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        <DebuggerStepperBoundary>
        Public Sub ApplyTheme(palette As ThemePalette) Implements IThemeableControl.ApplyTheme

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Me._Palette = palette
            Me.UpdateOwnerDrawStyle()

        End Sub

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' A <see cref="PaintEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            If Not Me.IsOwnerDrawn Then
                MyBase.OnPaint(e)
                Return
            End If

            Dim g As Graphics = e.Graphics
            Dim palette As ThemePalette = Me.Palette

            ' Background behind the tab headers.
            Using backBrush As New SolidBrush(palette.WindowBackColor)
                g.FillRectangle(backBrush, Me.ClientRectangle)
            End Using

            ' Page area and its border.
            Dim headersBottom As Integer = If(Me.TabCount > 0, Me.GetTabRect(0).Bottom, 0)
            Dim pageRect As New Rectangle(0, headersBottom, Me.Width - 1, Me.Height - headersBottom - 1)
            Using surfaceBrush As New SolidBrush(palette.SurfaceBackColor),
                  borderPen As New Pen(palette.BorderColor)
                g.FillRectangle(surfaceBrush, pageRect)
                g.DrawRectangle(borderPen, pageRect)
            End Using

            ' Tab headers. The selected one is painted last, so it overlaps its neighbors.
            For i As Integer = 0 To Me.TabCount - 1
                If i <> Me.SelectedIndex Then
                    Me.PaintTabHeader(g, i, isSelected:=False)
                End If
            Next

            If (Me.SelectedIndex >= 0) AndAlso (Me.SelectedIndex < Me.TabCount) Then
                Me.PaintTabHeader(g, Me.SelectedIndex, isSelected:=True)
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseMove"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' A <see cref="MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

            MyBase.OnMouseMove(e)

            Dim newHotIndex As Integer = -1
            For i As Integer = 0 To Me.TabCount - 1
                If Me.GetTabRect(i).Contains(e.Location) Then
                    newHotIndex = i
                    Exit For
                End If
            Next

            If newHotIndex <> Me.hotTabIndex Then
                Me.hotTabIndex = newHotIndex
                If Me.IsOwnerDrawn Then
                    Me.Invalidate()
                End If
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseLeave"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseLeave(e As EventArgs)

            MyBase.OnMouseLeave(e)

            If Me.hotTabIndex <> -1 Then
                Me.hotTabIndex = -1
                If Me.IsOwnerDrawn Then
                    Me.Invalidate()
                End If
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="TabControl.SelectedIndexChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnSelectedIndexChanged(e As EventArgs)

            MyBase.OnSelectedIndexChanged(e)

            If Me.IsOwnerDrawn Then
                Me.Invalidate()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnGotFocus(e As EventArgs)

            MyBase.OnGotFocus(e)

            If Me.IsOwnerDrawn Then
                Me.Invalidate()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnLostFocus(e As EventArgs)

            MyBase.OnLostFocus(e)

            If Me.IsOwnerDrawn Then
                Me.Invalidate()
            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Enables or disables the owner-draw control styles, according to the current palette and tab layout.
        ''' </summary>
        <DebuggerStepperBoundary>
        Private Sub UpdateOwnerDrawStyle()

            Dim ownerDraw As Boolean = (Me.Palette IsNot Nothing) AndAlso Me.Palette.IsDark AndAlso
                                       (Me.Alignment = TabAlignment.Top) AndAlso (Me.Appearance = TabAppearance.Normal)

            Me.SetStyle(ControlStyles.UserPaint Or
                        ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer Or
                        ControlStyles.ResizeRedraw, ownerDraw)

            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Paints the header of the specified tab.
        ''' </summary>
        ''' <param name="g">
        ''' The graphics surface.
        ''' </param>
        ''' <param name="index">
        ''' The tab index.
        ''' </param>
        ''' <param name="isSelected">
        ''' A value indicating whether the tab is the selected tab.
        ''' </param>
        <DebuggerStepperBoundary>
        Private Sub PaintTabHeader(g As Graphics, index As Integer, isSelected As Boolean)

            Dim palette As ThemePalette = Me.Palette
            Dim tabRect As Rectangle = Me.GetTabRect(index)

            Dim headerBackColor As Color
            Dim headerForeColor As Color
            If isSelected Then
                ' The selected header is slightly bigger and merges with the page area.
                tabRect = New Rectangle(tabRect.X - 2, tabRect.Y - 2, tabRect.Width + 4, tabRect.Height + 3)
                headerBackColor = palette.SurfaceBackColor
                headerForeColor = palette.WindowForeColor
            ElseIf index = Me.hotTabIndex Then
                headerBackColor = palette.MenuHoverBackColor
                headerForeColor = palette.WindowForeColor
            Else
                headerBackColor = palette.TabHeaderBackColor
                headerForeColor = palette.SecondaryForeColor
            End If

            If Not Me.Enabled OrElse Not Me.TabPages(index).Enabled Then
                headerForeColor = palette.DisabledForeColor
            End If

            Using backBrush As New SolidBrush(headerBackColor),
                  borderPen As New Pen(palette.BorderColor)
                g.FillRectangle(backBrush, tabRect)

                ' Left, top and right edges. The bottom edge is left open for the selected tab.
                g.DrawLine(borderPen, tabRect.Left, tabRect.Bottom - 1, tabRect.Left, tabRect.Top)
                g.DrawLine(borderPen, tabRect.Left, tabRect.Top, tabRect.Right - 1, tabRect.Top)
                g.DrawLine(borderPen, tabRect.Right - 1, tabRect.Top, tabRect.Right - 1, tabRect.Bottom - 1)
            End Using

            If isSelected Then
                ' Accent line on top of the selected tab.
                Using accentBrush As New SolidBrush(palette.AccentColor)
                    g.FillRectangle(accentBrush, tabRect.Left + 1, tabRect.Top + 1, tabRect.Width - 2, 2)
                End Using
            End If

            Dim flags As TextFormatFlags = TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                                           TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis
            If Not Me.ShowKeyboardCues Then
                flags = flags Or TextFormatFlags.HidePrefix
            End If

            TextRenderer.DrawText(g, Me.TabPages(index).Text, Me.Font, tabRect, headerForeColor, flags)

            If isSelected AndAlso Me.Focused AndAlso Me.ShowFocusCues Then
                Dim focusRect As Rectangle = tabRect
                focusRect.Inflate(-4, -4)
                ControlPaint.DrawFocusRectangle(g, focusRect, headerForeColor, headerBackColor)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
