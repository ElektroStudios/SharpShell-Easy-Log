#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Drawing2D

#End Region

#Region " ThemedToolStripRenderer "

Namespace SharpShellEasyLog.Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="ToolStripProfessionalRenderer"/> that paints menus and tool strips
    ''' using the colors of a <see cref="ThemePalette"/>.
    ''' <para></para>
    ''' It is intended for the dark theme; the light theme uses the default system renderer.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class ThemedToolStripRenderer : Inherits ToolStripProfessionalRenderer

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the palette used by this renderer.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public ReadOnly Property Palette As ThemePalette

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemedToolStripRenderer"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="palette">
        ''' The palette used to paint the tool strips.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Sub New(palette As ThemePalette)

            MyBase.New(New ThemedColorTable(palette))
            Me.Palette = palette
            Me.RoundedEdges = False

        End Sub

#End Region

#Region " Overridden Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderItemText"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="ToolStripItemTextRenderEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)

            e.TextColor = If(e.Item.Enabled, Me.Palette.WindowForeColor, Me.Palette.DisabledForeColor)
            MyBase.OnRenderItemText(e)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderArrow"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="ToolStripArrowRenderEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)

            e.ArrowColor = If(e.Item Is Nothing OrElse e.Item.Enabled, Me.Palette.WindowForeColor, Me.Palette.DisabledForeColor)
            MyBase.OnRenderArrow(e)

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderItemCheck"/> event.
        ''' <para></para>
        ''' The default check mark glyph is black, so it is drawn manually to remain visible over dark backgrounds.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="ToolStripItemImageRenderEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)

            Dim rect As Rectangle = e.ImageRectangle
            rect.Inflate(2, 2)

            Using backBrush As New SolidBrush(Me.Palette.MenuHoverBackColor),
                  borderPen As New Pen(Me.Palette.BorderColor)
                e.Graphics.FillRectangle(backBrush, rect)
                e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1)
            End Using

            Dim oldMode As SmoothingMode = e.Graphics.SmoothingMode
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            Using checkPen As New Pen(Me.Palette.WindowForeColor, 1.8F)
                Dim x As Single = rect.X
                Dim y As Single = rect.Y
                Dim w As Single = rect.Width
                Dim h As Single = rect.Height
                e.Graphics.DrawLines(checkPen, New PointF() {
                    New PointF(x + (w * 0.25F), y + (h * 0.52F)),
                    New PointF(x + (w * 0.43F), y + (h * 0.7F)),
                    New PointF(x + (w * 0.76F), y + (h * 0.32F))
                })
            End Using
            e.Graphics.SmoothingMode = oldMode

        End Sub

#End Region

#Region " ThemedColorTable "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A <see cref="ProfessionalColorTable"/> that returns the colors of a <see cref="ThemePalette"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private NotInheritable Class ThemedColorTable : Inherits ProfessionalColorTable

            ''' <summary>The palette.</summary>
            Private ReadOnly palette As ThemePalette

            ''' ----------------------------------------------------------------------------------------------------
            ''' <summary>
            ''' Initializes a new instance of the <see cref="ThemedColorTable"/> class.
            ''' </summary>
            ''' ----------------------------------------------------------------------------------------------------
            ''' <param name="palette">
            ''' The palette.
            ''' </param>
            ''' ----------------------------------------------------------------------------------------------------
            ''' <exception cref="ArgumentNullException">
            ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
            ''' </exception>
            ''' ----------------------------------------------------------------------------------------------------
            <DebuggerStepThrough>
            Friend Sub New(palette As ThemePalette)

                If palette Is Nothing Then
                    Throw New ArgumentNullException(NameOf(palette))
                End If

                Me.palette = palette
                Me.UseSystemColors = False

            End Sub

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuStripGradientBegin As Color
                Get
                    Return Me.palette.WindowBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuStripGradientEnd As Color
                Get
                    Return Me.palette.WindowBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ToolStripGradientBegin As Color
                Get
                    Return Me.palette.WindowBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
                Get
                    Return Me.palette.WindowBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ToolStripGradientEnd As Color
                Get
                    Return Me.palette.WindowBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ToolStripBorder As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuBorder As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemBorder As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemSelected As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemPressedGradientMiddle As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
                Get
                    Return Me.palette.MenuDropDownBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property SeparatorDark As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property SeparatorLight As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property CheckBackground As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property CheckSelectedBackground As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property CheckPressedBackground As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ButtonSelectedHighlight As Color
                Get
                    Return Me.palette.MenuHoverBackColor
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property ButtonSelectedBorder As Color
                Get
                    Return Me.palette.BorderColor
                End Get
            End Property

        End Class

#End Region

    End Class

End Namespace

#End Region
