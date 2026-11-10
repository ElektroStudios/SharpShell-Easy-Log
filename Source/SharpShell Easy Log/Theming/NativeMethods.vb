#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices
Imports System.Security

#End Region

#Region " NativeMethods "

Namespace SharpShellEasyLog.Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Platform Invocation (P/Invoke) declarations used by the theming infrastructure.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <SuppressUnmanagedCodeSecurity>
    Friend NotInheritable Class NativeMethods

#Region " Constants "

        ''' <summary>
        ''' DWM attribute that enables the immersive dark mode of the non-client area (title bar),
        ''' on Windows 10 version 20H1 (build 19041) and later.
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

        ''' <summary>
        ''' Undocumented DWM attribute that enables the immersive dark mode of the non-client area (title bar),
        ''' on Windows 10 versions 1809 (build 17763) to 1909 (build 18363).
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19

        ''' <summary>Retains the current size.</summary>
        Friend Const SWP_NOSIZE As UInteger = &H1UI

        ''' <summary>Retains the current position.</summary>
        Friend Const SWP_NOMOVE As UInteger = &H2UI

        ''' <summary>Retains the current Z order.</summary>
        Friend Const SWP_NOZORDER As UInteger = &H4UI

        ''' <summary>Does not activate the window.</summary>
        Friend Const SWP_NOACTIVATE As UInteger = &H10UI

        ''' <summary>Sends a WM_NCCALCSIZE message to the window, so the non-client area is recalculated and redrawn.</summary>
        Friend Const SWP_FRAMECHANGED As UInteger = &H20UI

        ''' <summary>Sent to a window when its non-client area needs to be changed to indicate an active or inactive state.</summary>
        Friend Const WM_NCACTIVATE As Integer = &H86

        ''' <summary>The HRESULT value for success.</summary>
        Friend Const S_OK As Integer = 0

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " DwmApi.dll "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the value of Desktop Window Manager (DWM) non-client rendering attributes for a window.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        <DllImport("DwmApi.dll", ExactSpelling:=True, PreserveSig:=True)>
        Friend Shared Function DwmSetWindowAttribute(hwnd As IntPtr,
                                                     dwAttribute As Integer,
                                                     ByRef pvAttribute As Integer,
                                                     cbAttribute As Integer
        ) As Integer
        End Function

#End Region

#Region " UxTheme.dll "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Causes a window to use a different set of visual style information than its class normally uses.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/uxtheme/nf-uxtheme-setwindowtheme"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        <DllImport("UxTheme.dll", ExactSpelling:=True, CharSet:=CharSet.Unicode, PreserveSig:=True)>
        Friend Shared Function SetWindowTheme(hwnd As IntPtr,
                                              pszSubAppName As String,
                                              pszSubIdList As String
        ) As Integer
        End Function

#End Region

#Region " User32.dll "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Changes the size, position, and Z order of a window.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        <DllImport("User32.dll", ExactSpelling:=True, SetLastError:=True)>
        Friend Shared Function SetWindowPos(hWnd As IntPtr,
                                            hWndInsertAfter As IntPtr,
                                            x As Integer,
                                            y As Integer,
                                            cx As Integer,
                                            cy As Integer,
                                            uFlags As UInteger
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Sends the specified message to a window.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessagew"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        <DllImport("User32.dll", EntryPoint:="SendMessageW", ExactSpelling:=True)>
        Friend Shared Function SendMessage(hWnd As IntPtr,
                                           msg As Integer,
                                           wParam As IntPtr,
                                           lParam As IntPtr
        ) As IntPtr
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves a handle to the foreground window (the window with which the user is currently working).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <remarks>
        ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getforegroundwindow"/>
        ''' </remarks>
        ''' ----------------------------------------------------------------------------------------------------
        <DllImport("User32.dll", ExactSpelling:=True)>
        Friend Shared Function GetForegroundWindow() As IntPtr
        End Function

#End Region

    End Class

End Namespace

#End Region
