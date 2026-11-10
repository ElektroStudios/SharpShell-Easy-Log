#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedLabel "

Namespace SharpShellEasyLog.UserControls

    ''' <summary>
    ''' A <see cref="Label"/> that supports the colors of a <see cref="ThemePalette"/>,
    ''' including a readable disabled state over dark backgrounds.
    ''' <para></para>
    ''' A standard <see cref="Label"/> paints its disabled text with a darker shade of its back color
    ''' when the back color is darker than <see cref="SystemColors.Control"/>,
    ''' which is practically invisible over a dark back color.
    ''' </summary>
    <DesignerCategory("Code")>
    <ToolboxBitmap(GetType(Label))>
    Public Class ThemedLabel : Inherits Label : Implements IThemeableControl

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

            If palette.IsDark Then
                Me.ForeColor = palette.WindowForeColor
            Else
                Me.ResetForeColor()
            End If

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

            If Me.Enabled OrElse (Me.Palette Is Nothing) OrElse Not Me.Palette.IsDark OrElse (Me.Image IsNot Nothing) Then
                MyBase.OnPaint(e)
                Return
            End If

            Dim flags As TextFormatFlags = ThemedPaintHelper.GetTextFormatFlags(Me.TextAlign, Me.RightToLeft, Me.UseMnemonic,
                                                                                Me.ShowKeyboardCues, wordWrap:=Not Me.AutoSize)
            If Not Me.AutoEllipsis Then
                flags = flags And Not TextFormatFlags.EndEllipsis
            End If

            Dim textBounds As Rectangle = Me.ClientRectangle
            textBounds = New Rectangle(textBounds.X + Me.Padding.Left, textBounds.Y + Me.Padding.Top,
                                       textBounds.Width - Me.Padding.Horizontal, textBounds.Height - Me.Padding.Vertical)

            TextRenderer.DrawText(e.Graphics, Me.Text, Me.Font, textBounds, Me.Palette.DisabledForeColor, flags)

        End Sub

#End Region

    End Class

End Namespace

#End Region
