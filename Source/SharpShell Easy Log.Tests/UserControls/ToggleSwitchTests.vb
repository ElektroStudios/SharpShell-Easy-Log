#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming
Imports SharpShellEasyLog.UserControls

#End Region

#Region " ToggleSwitchTests "

Namespace SharpShellEasyLog.Tests.UserControls

    ''' <summary>
    ''' Tests for <see cref="ToggleSwitch"/> and <see cref="ToolStripToggleSwitch"/>.
    ''' </summary>
    <STATestClass>
    Public NotInheritable Class ToggleSwitchTests

#Region " Test Doubles "

        ''' <summary>
        ''' Exposes the protected input methods of <see cref="ToggleSwitch"/> to simulate user interaction.
        ''' </summary>
        Private NotInheritable Class TestableToggleSwitch : Inherits ToggleSwitch

            Friend Sub SimulateKeyDown(key As Keys)
                Me.OnKeyDown(New KeyEventArgs(key))
            End Sub

            Friend Sub SimulateClick(button As MouseButtons)
                Me.OnMouseClick(New MouseEventArgs(button, 1, 5, 5, 0))
            End Sub

            Friend Function GetAccessibleObject() As AccessibleObject
                Return Me.AccessibilityObject
            End Function

        End Class

#End Region

#Region " ToggleSwitch "

        <TestMethod>
        Public Sub NewInstance_HasExpectedDefaults()

            Using toggle As New ToggleSwitch()
                Assert.IsFalse(toggle.Checked)
                Assert.IsTrue(toggle.TabStop)
                Assert.AreSame(ThemePalette.Light, toggle.Palette)
                Assert.AreEqual(New Size(40, 20), toggle.Size)
                Assert.AreEqual(AccessibleRole.CheckButton, toggle.AccessibleRole)
            End Using

        End Sub

        <TestMethod>
        Public Sub Checked_Changed_RaisesCheckedChangedOnce()

            Using toggle As New ToggleSwitch()
                Dim raisedCount As Integer = 0
                AddHandler toggle.CheckedChanged, Sub(s As Object, e As EventArgs) raisedCount += 1

                toggle.Checked = True

                Assert.AreEqual(1, raisedCount)
                Assert.IsTrue(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub Checked_SetToSameValue_DoesNotRaiseEvent()

            Using toggle As New ToggleSwitch()
                Dim raisedCount As Integer = 0
                AddHandler toggle.CheckedChanged, Sub(s As Object, e As EventArgs) raisedCount += 1

                toggle.Checked = False

                Assert.AreEqual(0, raisedCount)
            End Using

        End Sub

        <TestMethod>
        Public Sub CheckedChanged_SenderIsTheToggle()

            Using toggle As New ToggleSwitch()
                Dim eventSender As Object = Nothing
                AddHandler toggle.CheckedChanged, Sub(s As Object, e As EventArgs) eventSender = s

                toggle.Toggle()

                Assert.AreSame(toggle, eventSender)
            End Using

        End Sub

        <TestMethod>
        Public Sub Toggle_InvertsCheckedState()

            Using toggle As New ToggleSwitch()
                toggle.Toggle()
                Assert.IsTrue(toggle.Checked)

                toggle.Toggle()
                Assert.IsFalse(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        <DataRow(Keys.Space)>
        <DataRow(Keys.Enter)>
        Public Sub KeyDown_ToggleKeys_Toggle(key As Keys)

            Using toggle As New TestableToggleSwitch()
                toggle.SimulateKeyDown(key)
                Assert.IsTrue(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        <DataRow(Keys.A)>
        <DataRow(Keys.Tab)>
        <DataRow(Keys.Escape)>
        Public Sub KeyDown_OtherKeys_DoNotToggle(key As Keys)

            Using toggle As New TestableToggleSwitch()
                toggle.SimulateKeyDown(key)
                Assert.IsFalse(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub MouseClick_LeftButton_Toggles()

            Using toggle As New TestableToggleSwitch()
                toggle.SimulateClick(MouseButtons.Left)
                Assert.IsTrue(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub MouseClick_RightButton_DoesNotToggle()

            Using toggle As New TestableToggleSwitch()
                toggle.SimulateClick(MouseButtons.Right)
                Assert.IsFalse(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub AccessibleObject_ReportsRoleStateAndDefaultAction()

            Using toggle As New TestableToggleSwitch()
                Dim acc As AccessibleObject = toggle.GetAccessibleObject()

                Assert.AreEqual(AccessibleRole.CheckButton, acc.Role)
                Assert.IsFalse(acc.State.HasFlag(AccessibleStates.Checked))
                Assert.AreEqual("Toggle", acc.DefaultAction)

                acc.DoDefaultAction()

                Assert.IsTrue(toggle.Checked)
                Assert.IsTrue(acc.State.HasFlag(AccessibleStates.Checked))
            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_NullPalette_Throws()

            Using toggle As New ToggleSwitch()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() toggle.ApplyTheme(Nothing))
            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_StoresPalette()

            Using toggle As New ToggleSwitch()
                toggle.ApplyTheme(ThemePalette.Dark)
                Assert.AreSame(ThemePalette.Dark, toggle.Palette)
            End Using

        End Sub

        ''' <summary>
        ''' Paints every visual state into a bitmap, to verify that the drawing code does not throw.
        ''' </summary>
        <TestMethod>
        <DataRow(False, False, True)>
        <DataRow(True, False, True)>
        <DataRow(False, True, True)>
        <DataRow(True, True, False)>
        Public Sub Paint_AllStates_DoesNotThrow(isChecked As Boolean, isDark As Boolean, isEnabled As Boolean)

            Using toggle As New ToggleSwitch() With {.Checked = isChecked, .Enabled = isEnabled},
                  bmp As New Bitmap(toggle.Width, toggle.Height)

                toggle.ApplyTheme(If(isDark, ThemePalette.Dark, ThemePalette.Light))
                toggle.DrawToBitmap(bmp, New Rectangle(Point.Empty, toggle.Size))
            End Using

        End Sub

        <TestMethod>
        Public Sub Paint_TinySize_DoesNotThrow()

            Using toggle As New ToggleSwitch() With {.Size = New Size(2, 2)},
                  bmp As New Bitmap(2, 2)

                toggle.DrawToBitmap(bmp, New Rectangle(0, 0, 2, 2))
            End Using

        End Sub

        ''' <summary>
        ''' When checked, the track is filled with the accent color.
        ''' </summary>
        <TestMethod>
        Public Sub Paint_Checked_UsesAccentColorForTrack()

            Using toggle As New ToggleSwitch() With {.Size = New Size(40, 20), .Checked = True},
                  bmp As New Bitmap(40, 20)

                toggle.ApplyTheme(ThemePalette.Dark)
                toggle.DrawToBitmap(bmp, New Rectangle(0, 0, 40, 20))

                ' A pixel inside the track, left of the thumb (which is on the right when checked).
                Dim pixel As Color = bmp.GetPixel(10, 10)
                Assert.AreEqual(ThemePalette.Dark.AccentColor.ToArgb(), pixel.ToArgb())
            End Using

        End Sub

#End Region

#Region " ToolStripToggleSwitch "

        <TestMethod>
        Public Sub ToolStripHost_HostsToggleSwitch()

            Using host As New ToolStripToggleSwitch()
                Assert.IsNotNull(host.ToggleSwitch)
                Assert.AreSame(host.Control, host.ToggleSwitch)
                Assert.AreEqual(New Size(38, 18), host.Size)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToolStripHost_Checked_ForwardsToHostedControl()

            Using host As New ToolStripToggleSwitch()
                host.Checked = True
                Assert.IsTrue(host.ToggleSwitch.Checked)

                host.ToggleSwitch.Checked = False
                Assert.IsFalse(host.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToolStripHost_HostedControlChange_RaisesCheckedChangedFromHost()

            Using host As New ToolStripToggleSwitch()
                Dim eventSender As Object = Nothing
                Dim raisedCount As Integer = 0
                AddHandler host.CheckedChanged, Sub(s As Object, e As EventArgs)
                                                    eventSender = s
                                                    raisedCount += 1
                                                End Sub

                host.ToggleSwitch.Toggle()

                Assert.AreEqual(1, raisedCount)
                Assert.AreSame(host, eventSender)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToolStripHost_InMenuStrip_IsLaidOutWithoutGrowingTheStrip()

            Using referenceMenu As New MenuStrip(),
                  menu As New MenuStrip(),
                  host As New ToolStripToggleSwitch() With {.Alignment = ToolStripItemAlignment.Right}

                referenceMenu.Items.Add(New ToolStripMenuItem("Tools"))
                referenceMenu.PerformLayout()

                Dim item As New ToolStripMenuItem("Tools")
                menu.Items.AddRange(New ToolStripItem() {item, host})
                menu.PerformLayout()

                Assert.AreEqual(referenceMenu.Height, menu.Height, "The toggle must not make the menu strip taller.")
                Assert.IsTrue(host.Bounds.Left > item.Bounds.Right, "The right-aligned toggle must be placed after the menu items.")
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
