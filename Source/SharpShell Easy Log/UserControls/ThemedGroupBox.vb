#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedGroupBox "

Namespace SharpShellEasyLog.UserControls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="GroupBox"/> that paints its border and caption using the colors of a <see cref="ThemePalette"/>.
    ''' <para></para>
    ''' With a light palette (or when no palette is applied) the control is painted
    ''' exactly like a standard <see cref="GroupBox"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <DesignerCategory("Code")>
    <ToolboxBitmap(GetType(GroupBox))>
    Public Class ThemedGroupBox : Inherits GroupBox : Implements IThemeableControl

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

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether the control is currently custom painted
        ''' (that is, a dark palette is applied).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property IsCustomPainted As Boolean
            <DebuggerStepThrough>
            Get
                Return (Me.Palette IsNot Nothing) AndAlso Me.Palette.IsDark
            End Get
        End Property

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

            If Not Me.IsCustomPainted Then
                MyBase.OnPaint(e)
                Return
            End If

            Const captionOffsetX As Integer = 6
            Const captionPadding As Integer = 2

            Dim g As Graphics = e.Graphics
            Dim captionHeight As Integer = Me.Font.Height
            Dim borderTop As Integer = captionHeight \ 2
            Dim borderRect As New Rectangle(0, borderTop, Me.Width - 1, Me.Height - borderTop - 1)

            Using borderPen As New Pen(Me.Palette.BorderColor)
                g.DrawRectangle(borderPen, borderRect)
            End Using

            If Not String.IsNullOrEmpty(Me.Text) Then
                Dim flags As TextFormatFlags = TextFormatFlags.SingleLine Or TextFormatFlags.NoPadding Or TextFormatFlags.Left
                If Not Me.ShowKeyboardCues Then
                    flags = flags Or TextFormatFlags.HidePrefix
                End If

                Dim textSize As Size = TextRenderer.MeasureText(g, Me.Text, Me.Font, New Size(Integer.MaxValue, captionHeight), flags)
                Dim captionRect As New Rectangle(captionOffsetX, 0,
                                                 Math.Min(textSize.Width + (captionPadding * 2), Me.Width - (captionOffsetX * 2)),
                                                 captionHeight)

                ' Erase the border line behind the caption.
                Using backBrush As New SolidBrush(Me.BackColor)
                    g.FillRectangle(backBrush, captionRect)
                End Using

                Dim textRect As New Rectangle(captionRect.X + captionPadding, captionRect.Y,
                                              captionRect.Width - (captionPadding * 2), captionRect.Height)
                Dim textColor As Color = If(Me.Enabled, Me.Palette.WindowForeColor, Me.Palette.DisabledForeColor)
                TextRenderer.DrawText(g, Me.Text, Me.Font, textRect, textColor, flags Or TextFormatFlags.EndEllipsis)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.EnabledChanged"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnEnabledChanged(e As EventArgs)

            MyBase.OnEnabledChanged(e)
            Me.Invalidate()

        End Sub

#End Region

    End Class

End Namespace

#End Region
