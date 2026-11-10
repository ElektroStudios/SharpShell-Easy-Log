#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " ThemePalette "

Namespace SharpShellEasyLog.Theming

    ''' <summary>
    ''' Defines the set of colors used to paint the user interface for a specific <see cref="AppTheme"/>.
    ''' <para></para>
    ''' Use the <see cref="ThemePalette.Light"/> and <see cref="ThemePalette.Dark"/> shared instances,
    ''' or the <see cref="ThemePalette.FromTheme"/> method.
    ''' </summary>
    Public NotInheritable Class ThemePalette

#Region " Shared Instances "

        ''' <summary>
        ''' Gets the light theme palette, based on the default Windows system colors.
        ''' </summary>
        Public Shared ReadOnly Property Light As ThemePalette = ThemePalette.CreateLightPalette()

        ''' <summary>
        ''' Gets the dark theme palette.
        ''' </summary>
        Public Shared ReadOnly Property Dark As ThemePalette = ThemePalette.CreateDarkPalette()

#End Region

#Region " Properties "

        ''' <summary>Gets the theme that this palette represents.</summary>
        Public ReadOnly Property Theme As AppTheme

        ''' <summary>Gets a value indicating whether this is a dark palette.</summary>
        Public ReadOnly Property IsDark As Boolean
            <DebuggerStepThrough>
            Get
                Return Me.Theme = AppTheme.Dark
            End Get
        End Property

        ''' <summary>Gets the background color of windows and menu bars.</summary>
        Public ReadOnly Property WindowBackColor As Color

        ''' <summary>Gets the default foreground (text) color.</summary>
        Public ReadOnly Property WindowForeColor As Color

        ''' <summary>Gets the background color of content surfaces, such as tab pages.</summary>
        Public ReadOnly Property SurfaceBackColor As Color

        ''' <summary>Gets the background color of editable input controls.</summary>
        Public ReadOnly Property InputBackColor As Color

        ''' <summary>Gets the foreground color of input controls.</summary>
        Public ReadOnly Property InputForeColor As Color

        ''' <summary>Gets the background color of read-only input controls.</summary>
        Public ReadOnly Property ReadOnlyInputBackColor As Color

        ''' <summary>Gets the background color of buttons.</summary>
        Public ReadOnly Property ButtonBackColor As Color

        ''' <summary>Gets the background color of buttons when the mouse pointer is over them.</summary>
        Public ReadOnly Property ButtonHoverBackColor As Color

        ''' <summary>Gets the background color of buttons when they are pressed.</summary>
        Public ReadOnly Property ButtonPressedBackColor As Color

        ''' <summary>Gets the border color of buttons.</summary>
        Public ReadOnly Property ButtonBorderColor As Color

        ''' <summary>Gets the color of borders and frames (group boxes, tab pages, etc).</summary>
        Public ReadOnly Property BorderColor As Color

        ''' <summary>Gets the foreground color of disabled controls.</summary>
        Public ReadOnly Property DisabledForeColor As Color

        ''' <summary>Gets the foreground color of secondary text, such as unselected tab headers.</summary>
        Public ReadOnly Property SecondaryForeColor As Color

        ''' <summary>Gets the accent color.</summary>
        Public ReadOnly Property AccentColor As Color

        ''' <summary>Gets the foreground color drawn over <see cref="AccentColor"/>.</summary>
        Public ReadOnly Property AccentForeColor As Color

        ''' <summary>Gets the background color of highlighted menu items.</summary>
        Public ReadOnly Property MenuHoverBackColor As Color

        ''' <summary>Gets the background color of drop-down menus.</summary>
        Public ReadOnly Property MenuDropDownBackColor As Color

        ''' <summary>Gets the background color of unselected tab headers.</summary>
        Public ReadOnly Property TabHeaderBackColor As Color

        ''' <summary>Gets the track color of a toggle switch in the off state.</summary>
        Public ReadOnly Property ToggleOffTrackColor As Color

        ''' <summary>Gets the border and thumb color of a toggle switch in the off state.</summary>
        Public ReadOnly Property ToggleOffThumbColor As Color

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemePalette"/> class.
        ''' </summary>
        ''' <param name="theme">
        ''' The theme that this palette represents.
        ''' </param>
        <DebuggerNonUserCode>
        Private Sub New(theme As AppTheme)

            Me.Theme = theme

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the palette for the specified theme.
        ''' </summary>
        ''' <param name="theme">
        ''' The theme.
        ''' </param>
        ''' <returns>
        ''' The <see cref="ThemePalette"/> for <paramref name="theme"/>.
        ''' </returns>
        ''' <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        ''' Thrown when <paramref name="theme"/> is not a defined <see cref="AppTheme"/> value.
        ''' </exception>
        <DebuggerStepThrough>
        Public Shared Function FromTheme(theme As AppTheme) As ThemePalette

            Select Case theme
                Case AppTheme.Light
                    Return ThemePalette.Light

                Case AppTheme.Dark
                    Return ThemePalette.Dark

                Case Else
                    Throw New System.ComponentModel.InvalidEnumArgumentException(NameOf(theme), CInt(theme), GetType(AppTheme))
            End Select

        End Function

        ''' <summary>
        ''' Gets the relative luminance of the specified color, as defined by the WCAG 2.x specification.
        ''' </summary>
        ''' <param name="color">
        ''' The color.
        ''' </param>
        ''' <returns>
        ''' A value between <c>0.0</c> (darkest black) and <c>1.0</c> (lightest white).
        ''' </returns>
        <DebuggerStepThrough>
        Public Shared Function GetRelativeLuminance(color As Color) As Double

            Dim r As Double = ThemePalette.LinearizeChannel(color.R)
            Dim g As Double = ThemePalette.LinearizeChannel(color.G)
            Dim b As Double = ThemePalette.LinearizeChannel(color.B)

            Return (0.2126 * r) + (0.7152 * g) + (0.0722 * b)

        End Function

        ''' <summary>
        ''' Gets the contrast ratio between two colors, as defined by the WCAG 2.x specification.
        ''' </summary>
        ''' <param name="color1">
        ''' The first color.
        ''' </param>
        ''' <param name="color2">
        ''' The second color.
        ''' </param>
        ''' <returns>
        ''' A value between <c>1.0</c> (no contrast) and <c>21.0</c> (black on white).
        ''' </returns>
        <DebuggerStepThrough>
        Public Shared Function GetContrastRatio(color1 As Color, color2 As Color) As Double

            Dim l1 As Double = ThemePalette.GetRelativeLuminance(color1)
            Dim l2 As Double = ThemePalette.GetRelativeLuminance(color2)

            Return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05)

        End Function

        ''' <summary>
        ''' Determines whether the specified color is a dark color
        ''' (that is, white text is more readable than black text over it).
        ''' </summary>
        ''' <param name="color">
        ''' The color.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="color"/> is a dark color; otherwise, <see langword="False"/>.
        ''' </returns>
        <DebuggerStepThrough>
        Public Shared Function IsDarkColor(color As Color) As Boolean

            Return ThemePalette.GetContrastRatio(color, Color.White) > ThemePalette.GetContrastRatio(color, Color.Black)

        End Function

        ''' <summary>
        ''' Returns a <see cref="String"/> that represents this palette.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="String"/> that represents this palette.
        ''' </returns>
        <DebuggerStepThrough>
        Public Overrides Function ToString() As String

            Return $"{NameOf(ThemePalette)} ({Me.Theme})"

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Converts a gamma-encoded sRGB channel value to its linear value.
        ''' </summary>
        ''' <param name="channel">
        ''' The 8-bit channel value.
        ''' </param>
        ''' <returns>
        ''' The linear channel value, between <c>0.0</c> and <c>1.0</c>.
        ''' </returns>
        <DebuggerStepThrough>
        Private Shared Function LinearizeChannel(channel As Byte) As Double

            Dim c As Double = channel / 255.0R
            Return If(c <= 0.04045R, c / 12.92R, Math.Pow((c + 0.055R) / 1.055R, 2.4R))

        End Function

        ''' <summary>
        ''' Creates the light theme palette, based on the default Windows system colors.
        ''' </summary>
        ''' <returns>
        ''' The light theme palette.
        ''' </returns>
        <DebuggerStepThrough>
        Private Shared Function CreateLightPalette() As ThemePalette

            Return New ThemePalette(AppTheme.Light) With {
                ._WindowBackColor = SystemColors.Window,
                ._WindowForeColor = SystemColors.ControlText,
                ._SurfaceBackColor = SystemColors.Window,
                ._InputBackColor = SystemColors.Window,
                ._InputForeColor = SystemColors.WindowText,
                ._ReadOnlyInputBackColor = SystemColors.Control,
                ._ButtonBackColor = SystemColors.Control,
                ._ButtonHoverBackColor = SystemColors.ControlLight,
                ._ButtonPressedBackColor = SystemColors.ControlDark,
                ._ButtonBorderColor = SystemColors.ControlDark,
                ._BorderColor = SystemColors.ControlDark,
                ._DisabledForeColor = SystemColors.GrayText,
                ._SecondaryForeColor = SystemColors.ControlText,
                ._AccentColor = SystemColors.Highlight,
                ._AccentForeColor = SystemColors.HighlightText,
                ._MenuHoverBackColor = SystemColors.MenuHighlight,
                ._MenuDropDownBackColor = SystemColors.Menu,
                ._TabHeaderBackColor = SystemColors.Control,
                ._ToggleOffTrackColor = SystemColors.Window,
                ._ToggleOffThumbColor = SystemColors.ControlDarkDark
            }

        End Function

        ''' <summary>
        ''' Creates the dark theme palette.
        ''' </summary>
        ''' <returns>
        ''' The dark theme palette.
        ''' </returns>
        <DebuggerStepThrough>
        Private Shared Function CreateDarkPalette() As ThemePalette

            Return New ThemePalette(AppTheme.Dark) With {
                ._WindowBackColor = Color.FromArgb(32, 32, 32),
                ._WindowForeColor = Color.FromArgb(241, 241, 241),
                ._SurfaceBackColor = Color.FromArgb(43, 43, 43),
                ._InputBackColor = Color.FromArgb(25, 25, 25),
                ._InputForeColor = Color.FromArgb(230, 230, 230),
                ._ReadOnlyInputBackColor = Color.FromArgb(55, 55, 55),
                ._ButtonBackColor = Color.FromArgb(60, 60, 60),
                ._ButtonHoverBackColor = Color.FromArgb(75, 75, 75),
                ._ButtonPressedBackColor = Color.FromArgb(90, 90, 90),
                ._ButtonBorderColor = Color.FromArgb(100, 100, 100),
                ._BorderColor = Color.FromArgb(75, 75, 75),
                ._DisabledForeColor = Color.FromArgb(135, 135, 135),
                ._SecondaryForeColor = Color.FromArgb(185, 185, 185),
                ._AccentColor = Color.FromArgb(0, 103, 192),
                ._AccentForeColor = Color.White,
                ._MenuHoverBackColor = Color.FromArgb(65, 65, 65),
                ._MenuDropDownBackColor = Color.FromArgb(43, 43, 43),
                ._TabHeaderBackColor = Color.FromArgb(32, 32, 32),
                ._ToggleOffTrackColor = Color.FromArgb(45, 45, 45),
                ._ToggleOffThumbColor = Color.FromArgb(205, 205, 205)
            }

        End Function

#End Region

    End Class

End Namespace

#End Region
