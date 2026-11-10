#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming
Imports SharpShellEasyLog.UserControls

#End Region

#Region " ThemedControlsTests "

Namespace SharpShellEasyLog.Tests.UserControls

    ''' <summary>
    ''' Tests for <see cref="ThemedGroupBox"/> and <see cref="ThemedTabControl"/>.
    ''' </summary>
    <STATestClass>
    Public NotInheritable Class ThemedControlsTests

#Region " ThemedGroupBox "

        <TestMethod>
        Public Sub GroupBox_NewInstance_IsNotCustomPainted()

            Using box As New ThemedGroupBox()
                Assert.IsNull(box.Palette)
                Assert.IsFalse(box.IsCustomPainted)
            End Using

        End Sub

        <TestMethod>
        Public Sub GroupBox_ApplyTheme_NullPalette_Throws()

            Using box As New ThemedGroupBox()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() box.ApplyTheme(Nothing))
            End Using

        End Sub

        <TestMethod>
        Public Sub GroupBox_ApplyDarkThenLight_TogglesCustomPaintingAndForeColor()

            Using box As New ThemedGroupBox()
                Dim defaultForeColor As Color = box.ForeColor

                box.ApplyTheme(ThemePalette.Dark)
                Assert.IsTrue(box.IsCustomPainted)
                Assert.AreEqual(ThemePalette.Dark.WindowForeColor, box.ForeColor)

                box.ApplyTheme(ThemePalette.Light)
                Assert.IsFalse(box.IsCustomPainted)
                Assert.AreEqual(defaultForeColor, box.ForeColor)
            End Using

        End Sub

        <TestMethod>
        <DataRow(True, True, "Caption")>
        <DataRow(True, False, "Caption")>
        <DataRow(False, True, "Caption")>
        <DataRow(True, True, "")>
        <DataRow(True, True, "A very long caption that does not fit in the group box width at all")>
        Public Sub GroupBox_Paint_DoesNotThrow(isDark As Boolean, isEnabled As Boolean, caption As String)

            Using box As New ThemedGroupBox() With {.Text = caption, .Enabled = isEnabled, .Size = New Size(120, 60)},
                  bmp As New Bitmap(120, 60)

                box.ApplyTheme(If(isDark, ThemePalette.Dark, ThemePalette.Light))
                box.DrawToBitmap(bmp, New Rectangle(0, 0, 120, 60))
            End Using

        End Sub

        <TestMethod>
        Public Sub GroupBox_DarkPaint_DrawsBorderWithPaletteColor()

            Using parent As New Panel() With {.BackColor = ThemePalette.Dark.SurfaceBackColor, .Size = New Size(120, 60)},
                  box As New ThemedGroupBox() With {.Text = "", .Dock = DockStyle.Fill},
                  bmp As New Bitmap(120, 60)

                parent.Controls.Add(box)
                box.ApplyTheme(ThemePalette.Dark)
                box.DrawToBitmap(bmp, New Rectangle(0, 0, 120, 60))

                ' The left border, below the caption area.
                Assert.AreEqual(ThemePalette.Dark.BorderColor.ToArgb(), bmp.GetPixel(0, 40).ToArgb())
                ' The inner area keeps the parent background.
                Assert.AreEqual(ThemePalette.Dark.SurfaceBackColor.ToArgb(), bmp.GetPixel(60, 40).ToArgb())
            End Using

        End Sub

#End Region

#Region " ThemedButton / ThemedRadioButton / ThemedLabel "

        <TestMethod>
        Public Sub Button_ApplyDarkThenLight_SwitchesFlatStyle()

            Using button As New ThemedButton() With {.UseVisualStyleBackColor = True}
                button.ApplyTheme(ThemePalette.Dark)
                Assert.AreSame(ThemePalette.Dark, button.Palette)
                Assert.AreEqual(FlatStyle.Flat, button.FlatStyle)
                Assert.AreEqual(ThemePalette.Dark.ButtonBackColor, button.BackColor)

                button.ApplyTheme(ThemePalette.Light)
                Assert.AreEqual(FlatStyle.Standard, button.FlatStyle)
                Assert.IsTrue(button.UseVisualStyleBackColor)
            End Using

        End Sub

        <TestMethod>
        Public Sub RadioButtonAndLabel_ApplyDarkThenLight_SwitchForeColor()

            Using radio As New ThemedRadioButton(),
                  label As New ThemedLabel()

                Dim defaultRadioFore As Color = radio.ForeColor
                Dim defaultLabelFore As Color = label.ForeColor

                radio.ApplyTheme(ThemePalette.Dark)
                label.ApplyTheme(ThemePalette.Dark)
                Assert.AreEqual(ThemePalette.Dark.WindowForeColor, radio.ForeColor)
                Assert.AreEqual(ThemePalette.Dark.WindowForeColor, label.ForeColor)

                radio.ApplyTheme(ThemePalette.Light)
                label.ApplyTheme(ThemePalette.Light)
                Assert.AreEqual(defaultRadioFore, radio.ForeColor)
                Assert.AreEqual(defaultLabelFore, label.ForeColor)
            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedInputControls_ApplyTheme_NullPalette_Throws()

            Using button As New ThemedButton(),
                  radio As New ThemedRadioButton(),
                  label As New ThemedLabel()

                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() button.ApplyTheme(Nothing))
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() radio.ApplyTheme(Nothing))
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() label.ApplyTheme(Nothing))
            End Using

        End Sub

        ''' <summary>
        ''' The disabled text of the themed controls must use <see cref="ThemePalette.DisabledForeColor"/>,
        ''' which stays readable over dark backgrounds (the standard controls use a darker shade of the back color).
        ''' </summary>
        <TestMethod>
        Public Sub DisabledDarkControls_PaintTextWithDisabledForeColor()

            Dim palette As ThemePalette = ThemePalette.Dark
            Dim controls As Control() = {
                New ThemedButton() With {.Text = "WWWWWWWW", .Size = New Size(120, 30)},
                New ThemedRadioButton() With {.Text = "WWWWWWWW", .Size = New Size(120, 30)},
                New ThemedLabel() With {.Text = "WWWWWWWW", .AutoSize = False, .Size = New Size(120, 30)}
            }

            For Each ctrl As Control In controls
                Using parent As New Panel() With {.BackColor = palette.SurfaceBackColor, .Size = New Size(120, 30)},
                      bmp As New Bitmap(120, 30)

                    parent.Controls.Add(ctrl)
                    DirectCast(ctrl, IThemeableControl).ApplyTheme(palette)
                    ctrl.Enabled = False
                    ctrl.DrawToBitmap(bmp, New Rectangle(0, 0, 120, 30))

                    ' The text area (right of the radio glyph) must contain readable (bright) text pixels.
                    Dim found As Boolean = ThemedControlsTests.ContainsBrightPixel(bmp, New Rectangle(24, 0, 96, 30), minimumChannelValue:=110)
                    Assert.IsTrue(found, $"{ctrl.GetType().Name}: the disabled text is not readable over the dark background.")
                End Using
            Next

        End Sub

        ''' <summary>Determines whether the specified area of the bitmap contains a pixel whose RGB channels are all bright enough.</summary>
        Private Shared Function ContainsBrightPixel(bmp As Bitmap, area As Rectangle, minimumChannelValue As Integer) As Boolean

            For y As Integer = area.Top To area.Bottom - 1
                For x As Integer = area.Left To area.Right - 1
                    Dim pixel As Color = bmp.GetPixel(x, y)
                    If (pixel.R >= minimumChannelValue) AndAlso (pixel.G >= minimumChannelValue) AndAlso (pixel.B >= minimumChannelValue) Then
                        Return True
                    End If
                Next
            Next
            Return False

        End Function

#End Region

#Region " ThemedTabControl "

        <TestMethod>
        Public Sub TabControl_NewInstance_IsNotOwnerDrawn()

            Using tabs As New ThemedTabControl()
                Assert.IsNull(tabs.Palette)
                Assert.IsFalse(tabs.IsOwnerDrawn)
            End Using

        End Sub

        <TestMethod>
        Public Sub TabControl_ApplyTheme_NullPalette_Throws()

            Using tabs As New ThemedTabControl()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() tabs.ApplyTheme(Nothing))
            End Using

        End Sub

        <TestMethod>
        Public Sub TabControl_ApplyDarkThenLight_TogglesOwnerDraw()

            Using tabs As New ThemedTabControl()
                tabs.ApplyTheme(ThemePalette.Dark)
                Assert.IsTrue(tabs.IsOwnerDrawn)
                Assert.AreSame(ThemePalette.Dark, tabs.Palette)

                tabs.ApplyTheme(ThemePalette.Light)
                Assert.IsFalse(tabs.IsOwnerDrawn)
            End Using

        End Sub

        <TestMethod>
        <DataRow(TabAlignment.Bottom, TabAppearance.Normal)>
        <DataRow(TabAlignment.Left, TabAppearance.Normal)>
        <DataRow(TabAlignment.Top, TabAppearance.Buttons)>
        Public Sub TabControl_UnsupportedLayout_KeepsNativeRendering(alignment As TabAlignment, appearance As TabAppearance)

            Using tabs As New ThemedTabControl() With {.Alignment = alignment, .Appearance = appearance}
                tabs.ApplyTheme(ThemePalette.Dark)
                Assert.IsFalse(tabs.IsOwnerDrawn)
            End Using

        End Sub

        <TestMethod>
        <DataRow(0)>
        <DataRow(1)>
        <DataRow(3)>
        Public Sub TabControl_DarkPaint_DoesNotThrow(tabCount As Integer)

            Using form As New Form(),
                  tabs As New ThemedTabControl() With {.Dock = DockStyle.Fill},
                  bmp As New Bitmap(300, 200)

                For i As Integer = 1 To tabCount
                    tabs.TabPages.Add($"Tab {i}")
                Next
                form.ClientSize = New Size(300, 200)
                form.Controls.Add(tabs)
                tabs.ApplyTheme(ThemePalette.Dark)

                tabs.DrawToBitmap(bmp, New Rectangle(0, 0, 300, 200))
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
