#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices

#End Region

#Region " WindowThemeHelper "

Namespace SharpShellEasyLog.Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides methods to apply dark or light rendering to native window parts that
    ''' cannot be customized through Windows Forms properties
    ''' (the title bar of a window, and the scroll bars of a control).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class WindowThemeHelper

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="WindowThemeHelper"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Enables or disables the dark mode of the title bar of the specified window,
        ''' by calling the <c>DwmSetWindowAttribute</c> function with the
        ''' <c>DWMWA_USE_IMMERSIVE_DARK_MODE</c> attribute.
        ''' <para></para>
        ''' For a <see cref="Form"/>, call this method from an override of <see cref="Form.OnHandleCreated"/>:
        ''' calling it from the <see cref="Form.Load"/> event has no visible effect on the first paint.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hwnd">
        ''' The window handle.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="enable">
        ''' <see langword="True"/> to enable the dark title bar; <see langword="False"/> to restore the default light title bar.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="redrawNonClientArea">
        ''' <see langword="True"/> to force an immediate redraw of the title bar.
        ''' Use it when the window is already visible; otherwise the title bar keeps its
        ''' previous colors until the next activation change.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the attribute was applied;
        ''' <see langword="False"/> if the operating system does not support it
        ''' (Windows versions older than Windows 10 version 1809).
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="hwnd"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Function SetDarkTitleBar(hwnd As IntPtr, enable As Boolean, redrawNonClientArea As Boolean) As Boolean

            If hwnd = IntPtr.Zero Then
                Throw New ArgumentException("The window handle cannot be zero.", NameOf(hwnd))
            End If

            If Not WindowThemeHelper.IsWindowsNT() Then
                Return False
            End If

            Dim value As Integer = If(enable, 1, 0)
            Dim size As Integer = Marshal.SizeOf(GetType(Integer))
            Dim hr As Integer
            Try
                hr = NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, value, size)
                If hr <> NativeMethods.S_OK Then
                    ' Windows 10 versions prior to 20H1 use the undocumented attribute value 19.
                    ' On Windows versions older than Windows 10 version 1809 both calls fail with E_INVALIDARG.
                    hr = NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, value, size)
                End If

            Catch ex As Exception When (TypeOf ex Is DllNotFoundException) OrElse (TypeOf ex Is EntryPointNotFoundException)
                ' DWM is not available on this platform.
                Return False

            End Try

            If hr <> NativeMethods.S_OK Then
                Return False
            End If

            If redrawNonClientArea Then
                WindowThemeHelper.RedrawNonClientArea(hwnd)
            End If

            Return True

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the dark or the light "Explorer" visual style to the specified window,
        ''' which affects the rendering of its scroll bars.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hwnd">
        ''' The window handle.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="dark">
        ''' <see langword="True"/> to apply the dark visual style (<c>DarkMode_Explorer</c>);
        ''' <see langword="False"/> to apply the light visual style (<c>Explorer</c>).
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the visual style was applied;
        ''' <see langword="False"/> if the operating system does not support it.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="hwnd"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Function SetScrollBarsTheme(hwnd As IntPtr, dark As Boolean) As Boolean

            If hwnd = IntPtr.Zero Then
                Throw New ArgumentException("The window handle cannot be zero.", NameOf(hwnd))
            End If

            If Not WindowThemeHelper.IsWindowsNT() Then
                Return False
            End If

            Try
                Dim hr As Integer = NativeMethods.SetWindowTheme(hwnd, If(dark, "DarkMode_Explorer", "Explorer"), Nothing)
                Return hr = NativeMethods.S_OK

            Catch ex As Exception When (TypeOf ex Is DllNotFoundException) OrElse (TypeOf ex Is EntryPointNotFoundException)
                ' Visual styles are not available on this platform.
                Return False

            End Try

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the current operating system is Windows NT based.
        ''' <para></para>
        ''' The Windows version is not checked on purpose: <see cref="Environment.OSVersion"/> is affected by
        ''' the compatibility section of the host application manifest, so the native functions are called
        ''' directly and their result is evaluated instead.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the current operating system is Windows NT based; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function IsWindowsNT() As Boolean

            Return Environment.OSVersion.Platform = PlatformID.Win32NT

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Forces the immediate redraw of the non-client area (title bar and borders) of the specified window.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="hwnd">
        ''' The window handle.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Private Shared Sub RedrawNonClientArea(hwnd As IntPtr)

            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                                       NativeMethods.SWP_FRAMECHANGED Or NativeMethods.SWP_NOMOVE Or
                                       NativeMethods.SWP_NOSIZE Or NativeMethods.SWP_NOZORDER Or NativeMethods.SWP_NOACTIVATE)

            ' On Windows 10 the title bar of the active window is not repainted by SWP_FRAMECHANGED alone,
            ' so the activation state of the non-client area is toggled to force the repaint.
            Dim isActive As Boolean = (NativeMethods.GetForegroundWindow() = hwnd)
            NativeMethods.SendMessage(hwnd, NativeMethods.WM_NCACTIVATE, New IntPtr(If(isActive, 0, 1)), IntPtr.Zero)
            NativeMethods.SendMessage(hwnd, NativeMethods.WM_NCACTIVATE, New IntPtr(If(isActive, 1, 0)), IntPtr.Zero)

        End Sub

#End Region

    End Class

End Namespace

#End Region
