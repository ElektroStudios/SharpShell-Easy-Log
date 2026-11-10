#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

#End Region

#Region " SharpShellLoggingMode "

Namespace SharpShellEasyLog.Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies the logging modes supported by SharpShell.
    ''' <para></para>
    ''' The numeric values match the <c>LoggingMode</c> DWORD value that SharpShell reads from
    ''' the <c>HKLM\SOFTWARE\SharpShell</c> registry key.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Flags>
    Public Enum SharpShellLoggingMode As Integer

        ''' <summary>
        ''' Logging is disabled.
        ''' </summary>
        <Description("Disabled")>
        Disabled = 0

        ''' <summary>
        ''' Log messages are sent to the debugger output (e.g. DebugView).
        ''' </summary>
        <Description("Debug")>
        Debug = 1 << 0

        ''' <summary>
        ''' Log messages are written to the Windows Event Log.
        ''' </summary>
        <Description("Windows Event Log")>
        EventLog = 1 << 1

        ''' <summary>
        ''' Log messages are written to a log file.
        ''' </summary>
        <Description("Log File")>
        File = 1 << 2

        ''' <summary>
        ''' All the logging modes are enabled.
        ''' </summary>
        <Description("All")>
        All = Debug Or EventLog Or File

    End Enum

End Namespace

#End Region
