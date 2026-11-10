#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " ThemedToolStripRendererTests "

Namespace SharpShellEasyLog.Tests.Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Tests for <see cref="ThemedToolStripRenderer"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <STATestClass>
    Public NotInheritable Class ThemedToolStripRendererTests

        <TestMethod>
        Public Sub Constructor_NullPalette_Throws()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemedToolStripRendererTests.CreateRenderer(Nothing))

        End Sub

        <TestMethod>
        Public Sub Constructor_StoresPaletteAndUsesSquareEdges()

            Dim renderer As New ThemedToolStripRenderer(ThemePalette.Dark)

            Assert.AreSame(ThemePalette.Dark, renderer.Palette)
            Assert.IsFalse(renderer.RoundedEdges)

        End Sub

        <TestMethod>
        Public Sub ColorTable_ReturnsPaletteColors()

            Dim palette As ThemePalette = ThemePalette.Dark
            Dim table As ProfessionalColorTable = New ThemedToolStripRenderer(palette).ColorTable

            Assert.AreEqual(palette.WindowBackColor, table.MenuStripGradientBegin)
            Assert.AreEqual(palette.WindowBackColor, table.MenuStripGradientEnd)
            Assert.AreEqual(palette.MenuDropDownBackColor, table.ToolStripDropDownBackground)
            Assert.AreEqual(palette.MenuDropDownBackColor, table.ImageMarginGradientBegin)
            Assert.AreEqual(palette.MenuHoverBackColor, table.MenuItemSelected)
            Assert.AreEqual(palette.BorderColor, table.MenuBorder)
            Assert.AreEqual(palette.BorderColor, table.SeparatorDark)

        End Sub

        ''' <summary>
        ''' Paints a menu strip (with a checked item, a separator and a sub-menu arrow) into a bitmap,
        ''' to verify that the custom rendering code paths do not throw.
        ''' </summary>
        <TestMethod>
        Public Sub Render_MenuWithCheckedItemsAndSubMenus_DoesNotThrow()

            Using menu As New ContextMenuStrip(),
                  bmp As New Bitmap(300, 200)

                Dim parentItem As New ToolStripMenuItem("Parent")
                parentItem.DropDownItems.Add(New ToolStripMenuItem("Child"))
                menu.Items.Add(New ToolStripMenuItem("Checked") With {.Checked = True})
                menu.Items.Add(New ToolStripSeparator())
                menu.Items.Add(parentItem)
                menu.Items.Add(New ToolStripMenuItem("Disabled") With {.Enabled = False})
                menu.Renderer = New ThemedToolStripRenderer(ThemePalette.Dark)
                menu.Size = New Size(300, 200)
                menu.PerformLayout()

                menu.DrawToBitmap(bmp, New Rectangle(Point.Empty, bmp.Size))
            End Using

        End Sub

        ''' <summary>Creates a renderer (used to test constructor validation).</summary>
        Private Shared Sub CreateRenderer(palette As ThemePalette)

            Dim renderer As New ThemedToolStripRenderer(palette)
            GC.KeepAlive(renderer)

        End Sub

    End Class

End Namespace

#End Region
