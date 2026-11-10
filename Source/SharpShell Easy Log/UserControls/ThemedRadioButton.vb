#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Windows.Forms.VisualStyles

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedRadioButton "

Namespace SharpShellEasyLog.UserControls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="RadioButton"/> that supports the colors of a <see cref="ThemePalette"/>,
    ''' including a readable disabled state over dark backgrounds.
    ''' <para></para>
    ''' A standard <see cref="RadioButton"/> paints its disabled text with a darker shade of its back color,
    ''' which is practically invisible over a dark back color.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <DesignerCategory("Code")>
    <ToolboxBitmap(GetType(RadioButton))>
    Public Class ThemedRadioButton : Inherits RadioButton : Implements IThemeableControl

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The gap, in pixels, between the radio glyph and the text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const GlyphTextGap As Integer = 3

#End Region

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the palette currently applied to the control,
        ''' or <see langword="Nothing"/> if no palette was applied yet.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette Implements IThemeableControl.Palette

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
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

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="PaintEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            ' Custom painting is only needed for the disabled state of the standard (non-button) appearance.
            If Me.Enabled OrElse (Me.Palette Is Nothing) OrElse Not Me.Palette.IsDark OrElse
               (Me.Appearance <> System.Windows.Forms.Appearance.Normal) OrElse (Me.FlatStyle = FlatStyle.System) OrElse
               (Me.Image IsNot Nothing) OrElse (Me.CheckAlign <> Drawing.ContentAlignment.MiddleLeft) Then
                MyBase.OnPaint(e)
                Return
            End If

            Dim g As Graphics = e.Graphics

            Using backBrush As New SolidBrush(Me.BackColor)
                g.FillRectangle(backBrush, Me.ClientRectangle)
            End Using

            Dim state As RadioButtonState = If(Me.Checked, RadioButtonState.CheckedDisabled, RadioButtonState.UncheckedDisabled)
            Dim glyphSize As Size = RadioButtonRenderer.GetGlyphSize(g, state)
            Dim glyphLocation As New Point(Me.Padding.Left, (Me.Height - glyphSize.Height) \ 2)
            RadioButtonRenderer.DrawRadioButton(g, glyphLocation, state)

            Dim textLeft As Integer = glyphLocation.X + glyphSize.Width + ThemedRadioButton.GlyphTextGap
            Dim textBounds As New Rectangle(textLeft, 0, Math.Max(0, Me.Width - textLeft - Me.Padding.Right), Me.Height)
            Dim flags As TextFormatFlags = ThemedPaintHelper.GetTextFormatFlags(Me.TextAlign, Me.RightToLeft, Me.UseMnemonic,
                                                                                Me.ShowKeyboardCues, wordWrap:=False)

            TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds, Me.Palette.DisabledForeColor, flags)

        End Sub

#End Region

    End Class

End Namespace

#End Region
