#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Theming

#End Region

#Region " WindowThemeHelperTests "

Namespace SharpShellEasyLog.Tests.Theming

    ''' <summary>
    ''' Tests for <see cref="WindowThemeHelper"/>.
    ''' </summary>
    <STATestClass>
    Public NotInheritable Class WindowThemeHelperTests

        <TestMethod>
        Public Sub SetDarkTitleBar_ZeroHandle_Throws()

            Dim ex As ArgumentException = Assert.ThrowsExactly(Of ArgumentException)(
                Sub() WindowThemeHelper.SetDarkTitleBar(IntPtr.Zero, enable:=True, redrawNonClientArea:=False))

            Assert.AreEqual("hwnd", ex.ParamName)

        End Sub

        <TestMethod>
        Public Sub SetScrollBarsTheme_ZeroHandle_Throws()

            Dim ex As ArgumentException = Assert.ThrowsExactly(Of ArgumentException)(
                Sub() WindowThemeHelper.SetScrollBarsTheme(IntPtr.Zero, dark:=True))

            Assert.AreEqual("hwnd", ex.ParamName)

        End Sub

        ''' <summary>
        ''' On Windows 10 version 1809 or later the attribute must be applied;
        ''' on any OS the call must never throw for a valid window handle.
        ''' </summary>
        <TestMethod>
        Public Sub SetDarkTitleBar_ValidHandle_TogglesWithoutThrowing()

            Using form As New Form()
                Dim hwnd As IntPtr = form.Handle

                Dim darkResult As Boolean = WindowThemeHelper.SetDarkTitleBar(hwnd, enable:=True, redrawNonClientArea:=True)
                Dim lightResult As Boolean = WindowThemeHelper.SetDarkTitleBar(hwnd, enable:=False, redrawNonClientArea:=True)

                Assert.AreEqual(darkResult, lightResult, "Enabling and disabling must be equally supported.")

                If WindowThemeHelperTests.IsWindows10Build17763OrGreater() Then
                    Assert.IsTrue(darkResult, "DWMWA_USE_IMMERSIVE_DARK_MODE should be supported on this OS.")
                End If
            End Using

        End Sub

        <TestMethod>
        Public Sub SetScrollBarsTheme_ValidHandle_DoesNotThrow()

            Using textBox As New TextBox() With {.Multiline = True, .ScrollBars = ScrollBars.Both}
                Dim hwnd As IntPtr = textBox.Handle

                WindowThemeHelper.SetScrollBarsTheme(hwnd, dark:=True)
                WindowThemeHelper.SetScrollBarsTheme(hwnd, dark:=False)
            End Using

        End Sub

        ''' <summary>
        ''' Determines whether the OS is Windows 10 version 1809 (build 17763) or greater,
        ''' reading the real build number from the registry (not affected by the host manifest).
        ''' </summary>
        Private Shared Function IsWindows10Build17763OrGreater() As Boolean

            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Return False
            End If

            Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", writable:=False)
                If key Is Nothing Then
                    Return False
                End If

                Dim buildValue As String = TryCast(key.GetValue("CurrentBuildNumber"), String)
                Dim build As Integer
                Return Integer.TryParse(buildValue, build) AndAlso (build >= 17763)
            End Using

        End Function

    End Class

End Namespace

#End Region
