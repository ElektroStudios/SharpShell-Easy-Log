#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Reflection

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemePaletteTests "

Namespace SharpShellEasyLog.Tests.Theming

    ''' <summary>
    ''' Tests for <see cref="ThemePalette"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ThemePaletteTests

        ''' <summary>The minimum WCAG AA contrast ratio for normal text.</summary>
        Private Const MinimumTextContrast As Double = 4.5

#Region " Shared Instances / FromTheme "

        <TestMethod>
        Public Sub Light_HasLightTheme()

            Assert.AreEqual(AppTheme.Light, ThemePalette.Light.Theme)
            Assert.IsFalse(ThemePalette.Light.IsDark)

        End Sub

        <TestMethod>
        Public Sub Dark_HasDarkTheme()

            Assert.AreEqual(AppTheme.Dark, ThemePalette.Dark.Theme)
            Assert.IsTrue(ThemePalette.Dark.IsDark)

        End Sub

        <TestMethod>
        Public Sub FromTheme_ReturnsMatchingSharedInstance()

            Assert.AreSame(ThemePalette.Light, ThemePalette.FromTheme(AppTheme.Light))
            Assert.AreSame(ThemePalette.Dark, ThemePalette.FromTheme(AppTheme.Dark))

        End Sub

        <TestMethod>
        Public Sub FromTheme_UndefinedValue_Throws()

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() ThemePalette.FromTheme(DirectCast(42, AppTheme)))

        End Sub

        <TestMethod>
        Public Sub ToString_ContainsThemeName()

            StringAssert.Contains(ThemePalette.Dark.ToString(), "Dark")
            StringAssert.Contains(ThemePalette.Light.ToString(), "Light")

        End Sub

#End Region

#Region " Light Palette "

        ''' <summary>The light theme must use the default Windows system colors.</summary>
        <TestMethod>
        Public Sub Light_UsesSystemColors()

            Dim palette As ThemePalette = ThemePalette.Light

            Assert.AreEqual(SystemColors.Window, palette.WindowBackColor)
            Assert.AreEqual(SystemColors.ControlText, palette.WindowForeColor)
            Assert.AreEqual(SystemColors.Window, palette.InputBackColor)
            Assert.AreEqual(SystemColors.WindowText, palette.InputForeColor)
            Assert.AreEqual(SystemColors.Control, palette.ReadOnlyInputBackColor)
            Assert.AreEqual(SystemColors.GrayText, palette.DisabledForeColor)
            Assert.AreEqual(SystemColors.Highlight, palette.AccentColor)

        End Sub

        <TestMethod>
        Public Sub Light_EveryColorIsASystemColor()

            For Each prop As PropertyInfo In ThemePaletteTests.GetColorProperties()
                Dim value As Color = DirectCast(prop.GetValue(ThemePalette.Light), Color)
                Assert.IsTrue(value.IsSystemColor, $"{prop.Name} should be a system color, but it is {value}.")
            Next

        End Sub

#End Region

#Region " Dark Palette "

        <TestMethod>
        Public Sub Dark_EveryColorIsDefinedAndOpaque()

            For Each prop As PropertyInfo In ThemePaletteTests.GetColorProperties()
                Dim value As Color = DirectCast(prop.GetValue(ThemePalette.Dark), Color)
                Assert.IsFalse(value.IsEmpty, $"{prop.Name} is empty.")
                Assert.AreEqual(255, CInt(value.A), $"{prop.Name} is not opaque.")
            Next

        End Sub

        <TestMethod>
        Public Sub Dark_BackgroundColorsAreDark()

            Dim palette As ThemePalette = ThemePalette.Dark

            For Each backColor As Color In {palette.WindowBackColor, palette.SurfaceBackColor, palette.InputBackColor,
                                            palette.ReadOnlyInputBackColor, palette.ButtonBackColor, palette.MenuDropDownBackColor,
                                            palette.TabHeaderBackColor, palette.MenuHoverBackColor}
                Assert.IsTrue(ThemePalette.IsDarkColor(backColor), $"{backColor} should be a dark color.")
            Next

        End Sub

        <TestMethod>
        Public Sub Dark_TextColorsHaveEnoughContrast()

            Dim palette As ThemePalette = ThemePalette.Dark

            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.WindowBackColor, "Window text")
            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.SurfaceBackColor, "Surface text")
            ThemePaletteTests.AssertContrast(palette.InputForeColor, palette.InputBackColor, "Input text")
            ThemePaletteTests.AssertContrast(palette.InputForeColor, palette.ReadOnlyInputBackColor, "Read-only input text")
            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.ButtonBackColor, "Button text")
            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.ButtonHoverBackColor, "Hovered button text")
            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.MenuHoverBackColor, "Hovered menu text")
            ThemePaletteTests.AssertContrast(palette.WindowForeColor, palette.MenuDropDownBackColor, "Menu text")
            ThemePaletteTests.AssertContrast(palette.SecondaryForeColor, palette.TabHeaderBackColor, "Unselected tab text")
            ThemePaletteTests.AssertContrast(palette.AccentForeColor, palette.AccentColor, "Accent text")

        End Sub

        <TestMethod>
        Public Sub Dark_DisabledTextIsDistinguishableButDimmer()

            Dim palette As ThemePalette = ThemePalette.Dark

            Dim disabledContrast As Double = ThemePalette.GetContrastRatio(palette.DisabledForeColor, palette.SurfaceBackColor)
            Dim enabledContrast As Double = ThemePalette.GetContrastRatio(palette.WindowForeColor, palette.SurfaceBackColor)

            Assert.IsTrue(disabledContrast >= 3.0, $"Disabled text contrast is too low ({disabledContrast:F2}).")
            Assert.IsTrue(disabledContrast < enabledContrast, "Disabled text should be dimmer than enabled text.")

        End Sub

#End Region

#Region " Color Math "

        <TestMethod>
        Public Sub GetRelativeLuminance_BlackAndWhite()

            Assert.AreEqual(0.0, ThemePalette.GetRelativeLuminance(Color.Black), 0.0001)
            Assert.AreEqual(1.0, ThemePalette.GetRelativeLuminance(Color.White), 0.0001)

        End Sub

        <TestMethod>
        Public Sub GetContrastRatio_BlackOnWhite_Is21()

            Assert.AreEqual(21.0, ThemePalette.GetContrastRatio(Color.Black, Color.White), 0.0001)

        End Sub

        <TestMethod>
        Public Sub GetContrastRatio_IsSymmetric()

            Dim c1 As Color = Color.FromArgb(32, 32, 32)
            Dim c2 As Color = Color.FromArgb(200, 120, 40)

            Assert.AreEqual(ThemePalette.GetContrastRatio(c1, c2), ThemePalette.GetContrastRatio(c2, c1), 0.000001)

        End Sub

        <TestMethod>
        Public Sub GetContrastRatio_SameColor_IsOne()

            Assert.AreEqual(1.0, ThemePalette.GetContrastRatio(Color.Teal, Color.Teal), 0.000001)

        End Sub

        <TestMethod>
        <DataRow(0, 0, 0, True)>
        <DataRow(32, 32, 32, True)>
        <DataRow(0, 60, 120, True)>
        <DataRow(255, 255, 255, False)>
        <DataRow(240, 240, 240, False)>
        <DataRow(255, 255, 0, False)>
        Public Sub IsDarkColor_ReturnsExpectedValue(r As Integer, g As Integer, b As Integer, expected As Boolean)

            Assert.AreEqual(expected, ThemePalette.IsDarkColor(Color.FromArgb(r, g, b)))

        End Sub

#End Region

#Region " Helpers "

        ''' <summary>Gets every public <see cref="Color"/> property of <see cref="ThemePalette"/>.</summary>
        Private Shared Function GetColorProperties() As PropertyInfo()

            Dim props As PropertyInfo() = GetType(ThemePalette).GetProperties(BindingFlags.Public Or BindingFlags.Instance).
                                          Where(Function(p As PropertyInfo) p.PropertyType Is GetType(Color)).
                                          ToArray()

            Assert.IsTrue(props.Length >= 20, "Unexpected number of color properties.")
            Return props

        End Function

        ''' <summary>Asserts that two colors have at least the minimum WCAG AA text contrast.</summary>
        Private Shared Sub AssertContrast(foreColor As Color, backColor As Color, description As String)

            Dim ratio As Double = ThemePalette.GetContrastRatio(foreColor, backColor)
            Assert.IsTrue(ratio >= ThemePaletteTests.MinimumTextContrast,
                          $"{description}: contrast ratio {ratio:F2} is lower than {ThemePaletteTests.MinimumTextContrast}.")

        End Sub

#End Region

    End Class

End Namespace

#End Region
