#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedButton "

Namespace SharpShellEasyLog.UserControls

    ''' <summary>
    ''' A <see cref="Button"/> that supports the colors of a <see cref="ThemePalette"/>,
    ''' including a readable disabled state over dark backgrounds.
    ''' <para></para>
    ''' A standard flat <see cref="Button"/> paints its disabled text with a darker shade of its back color,
    ''' which is practically invisible over a dark back color.
    ''' </summary>
    <DesignerCategory("Code")>
    <ToolboxBitmap(GetType(Button))>
    Public Class ThemedButton : Inherits Button : Implements IThemeableControl

#Region " Properties "

        ''' <summary>
        ''' Gets the palette currently applied to the control,
        ''' or <see langword="Nothing"/> if no palette was applied yet.
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette Implements IThemeableControl.Palette

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Applies the specified palette to the control.
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
            ThemeManager.ApplyThemeToButton(Me, palette)
            Me.Invalidate()

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

            If Me.Enabled OrElse (Me.Palette Is Nothing) OrElse Not Me.Palette.IsDark Then
                MyBase.OnPaint(e)
                Return
            End If

            Dim g As Graphics = e.Graphics
            Dim bounds As Rectangle = Me.ClientRectangle

            Using backBrush As New SolidBrush(Me.BackColor)
                g.FillRectangle(backBrush, bounds)
            End Using

            Dim borderSize As Integer = Me.FlatAppearance.BorderSize
            If borderSize > 0 Then
                Using borderPen As New Pen(Me.Palette.BorderColor, borderSize)
                    Dim half As Integer = borderSize \ 2
                    g.DrawRectangle(borderPen, half, half, bounds.Width - borderSize, bounds.Height - borderSize)
                End Using
            End If

            Dim textBounds As Rectangle = Rectangle.Inflate(bounds, -(borderSize + 2), -(borderSize + 1))
            Dim flags As TextFormatFlags = ThemedPaintHelper.GetTextFormatFlags(Me.TextAlign, Me.RightToLeft, Me.UseMnemonic,
                                                                                Me.ShowKeyboardCues, wordWrap:=False)
            TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds, Me.Palette.DisabledForeColor, flags)

        End Sub

#End Region

    End Class

End Namespace

#End Region
