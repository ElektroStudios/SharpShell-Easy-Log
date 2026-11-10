#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32

#End Region

#Region " SharpShellRegistrySettings "

Namespace SharpShellEasyLog.Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Reads and writes the SharpShell logging configuration stored in the Windows registry.
    ''' <para></para>
    ''' SharpShell reads its logging configuration from the <c>HKLM\SOFTWARE\SharpShell</c> registry key:
    ''' <list type="bullet">
    '''   <item><description><c>LoggingMode</c> (DWORD): A <see cref="SharpShellLoggingMode"/> value.</description></item>
    '''   <item><description><c>LogPath</c> (String): The log file path, used when the
    '''   <see cref="SharpShellLoggingMode.File"/> flag is set.</description></item>
    ''' </list>
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class SharpShellRegistrySettings

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The path of the SharpShell registry key, relative to <see cref="RegistryHive.LocalMachine"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Const DefaultSubKeyPath As String = "SOFTWARE\SharpShell"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The name of the registry value that holds the SharpShell logging mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Const LoggingModeValueName As String = "LoggingMode"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The name of the registry value that holds the SharpShell log file path.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Const LogPathValueName As String = "LogPath"

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="SharpShellRegistrySettings"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the registry views where the SharpShell configuration must be written.
        ''' <para></para>
        ''' On 64-bit Windows the <c>HKLM\SOFTWARE</c> key is redirected for 32-bit processes
        ''' (<c>WOW6432Node</c>), so both the 64-bit and the 32-bit registry views must be updated
        ''' for the configuration to take effect in 32-bit and 64-bit processes that host SharpShell servers.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="is64BitOperatingSystem">
        ''' A value indicating whether the operating system is 64-bit.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The registry views to update.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function GetTargetRegistryViews(is64BitOperatingSystem As Boolean) As RegistryView()

            If is64BitOperatingSystem Then
                Return {RegistryView.Registry64, RegistryView.Registry32}
            End If

            Return {RegistryView.Default}

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the specified SharpShell logging configuration to <c>HKLM\SOFTWARE\SharpShell</c>,
        ''' in every registry view returned by <see cref="GetTargetRegistryViews"/>.
        ''' <para></para>
        ''' This operation requires administrative privileges.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="mode">
        ''' The logging mode to apply.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="logFilePath">
        ''' The log file path. It is only written when <paramref name="mode"/> includes
        ''' the <see cref="SharpShellLoggingMode.File"/> flag. Can be <see langword="Nothing"/> or empty.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="UnauthorizedAccessException">
        ''' Thrown when the current process does not have administrative privileges.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Sub ApplyToLocalMachine(mode As SharpShellLoggingMode, logFilePath As String)

            For Each view As RegistryView In SharpShellRegistrySettings.GetTargetRegistryViews(Environment.Is64BitOperatingSystem)
                Using baseKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view)
                    SharpShellRegistrySettings.Apply(baseKey, SharpShellRegistrySettings.DefaultSubKeyPath, mode, logFilePath)
                End Using
            Next

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the specified SharpShell logging configuration to the specified registry key.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="baseKey">
        ''' The base registry key (e.g. <c>HKEY_LOCAL_MACHINE</c>) where <paramref name="subKeyPath"/> is located.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="subKeyPath">
        ''' The path of the SharpShell registry subkey, relative to <paramref name="baseKey"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="mode">
        ''' The logging mode to apply.
        ''' <para></para>
        ''' When the value is <see cref="SharpShellLoggingMode.Disabled"/>, the SharpShell values are removed,
        ''' and the subkey is deleted only if it becomes empty (values not owned by this application are preserved).
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="logFilePath">
        ''' The log file path. It is only written when <paramref name="mode"/> includes
        ''' the <see cref="SharpShellLoggingMode.File"/> flag. Can be <see langword="Nothing"/> or empty.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="baseKey"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="subKeyPath"/> is <see langword="Nothing"/>, empty or white space.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="mode"/> is not a valid <see cref="SharpShellLoggingMode"/> value.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Sub Apply(baseKey As RegistryKey, subKeyPath As String, mode As SharpShellLoggingMode, logFilePath As String)

            SharpShellRegistrySettings.ValidateKeyArguments(baseKey, subKeyPath)
            SharpShellLoggingModeExtensions.ThrowIfInvalid(mode, NameOf(mode))

            If mode = SharpShellLoggingMode.Disabled Then
                SharpShellRegistrySettings.Remove(baseKey, subKeyPath)
                Return
            End If

            Using subKey As RegistryKey = baseKey.CreateSubKey(subKeyPath, writable:=True)
                If subKey Is Nothing Then
                    Throw New InvalidOperationException($"Unable to create or open registry key '{baseKey.Name}\{subKeyPath}'.")
                End If

                subKey.SetValue(SharpShellRegistrySettings.LoggingModeValueName, CInt(mode), RegistryValueKind.DWord)

                If mode.UsesLogFile() AndAlso Not String.IsNullOrWhiteSpace(logFilePath) Then
                    subKey.SetValue(SharpShellRegistrySettings.LogPathValueName, logFilePath, RegistryValueKind.String)
                Else
                    subKey.DeleteValue(SharpShellRegistrySettings.LogPathValueName, throwOnMissingValue:=False)
                End If
            End Using

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Removes the SharpShell logging values from the specified registry key,
        ''' and deletes the key if it becomes empty.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="baseKey">
        ''' The base registry key where <paramref name="subKeyPath"/> is located.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="subKeyPath">
        ''' The path of the SharpShell registry subkey, relative to <paramref name="baseKey"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="baseKey"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="subKeyPath"/> is <see langword="Nothing"/>, empty or white space.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Sub Remove(baseKey As RegistryKey, subKeyPath As String)

            SharpShellRegistrySettings.ValidateKeyArguments(baseKey, subKeyPath)

            Dim isEmpty As Boolean
            Using subKey As RegistryKey = baseKey.OpenSubKey(subKeyPath, writable:=True)
                If subKey Is Nothing Then
                    ' Nothing to remove: the key does not exist.
                    Return
                End If

                subKey.DeleteValue(SharpShellRegistrySettings.LoggingModeValueName, throwOnMissingValue:=False)
                subKey.DeleteValue(SharpShellRegistrySettings.LogPathValueName, throwOnMissingValue:=False)

                isEmpty = (subKey.ValueCount = 0) AndAlso (subKey.SubKeyCount = 0)
            End Using

            If isEmpty Then
                baseKey.DeleteSubKey(subKeyPath, throwOnMissingSubKey:=False)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Reads the SharpShell logging mode from the specified registry key.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="baseKey">
        ''' The base registry key where <paramref name="subKeyPath"/> is located.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="subKeyPath">
        ''' The path of the SharpShell registry subkey, relative to <paramref name="baseKey"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The stored <see cref="SharpShellLoggingMode"/>,
        ''' or <see cref="SharpShellLoggingMode.Disabled"/> if the key or the value does not exist.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="baseKey"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="subKeyPath"/> is <see langword="Nothing"/>, empty or white space.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidDataException">
        ''' Thrown when the stored value is not a DWORD, or it is not a valid <see cref="SharpShellLoggingMode"/> value.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Function ReadLoggingMode(baseKey As RegistryKey, subKeyPath As String) As SharpShellLoggingMode

            SharpShellRegistrySettings.ValidateKeyArguments(baseKey, subKeyPath)

            Using subKey As RegistryKey = baseKey.OpenSubKey(subKeyPath, writable:=False)
                If subKey Is Nothing Then
                    Return SharpShellLoggingMode.Disabled
                End If

                Dim value As Object = subKey.GetValue(SharpShellRegistrySettings.LoggingModeValueName, Nothing)
                If value Is Nothing Then
                    Return SharpShellLoggingMode.Disabled
                End If

                If Not (TypeOf value Is Integer) Then
                    Throw New InvalidDataException($"The '{SharpShellRegistrySettings.LoggingModeValueName}' registry value is not a DWORD.")
                End If

                Dim mode As SharpShellLoggingMode = DirectCast(DirectCast(value, Integer), SharpShellLoggingMode)
                If Not mode.IsValid() Then
                    Throw New InvalidDataException($"The '{SharpShellRegistrySettings.LoggingModeValueName}' registry value ({CInt(mode)}) is not a valid logging mode.")
                End If

                Return mode
            End Using

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Reads the SharpShell log file path from the specified registry key.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="baseKey">
        ''' The base registry key where <paramref name="subKeyPath"/> is located.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="subKeyPath">
        ''' The path of the SharpShell registry subkey, relative to <paramref name="baseKey"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The stored log file path, or <see langword="Nothing"/> if the key or the value does not exist.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="baseKey"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="subKeyPath"/> is <see langword="Nothing"/>, empty or white space.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Shared Function ReadLogPath(baseKey As RegistryKey, subKeyPath As String) As String

            SharpShellRegistrySettings.ValidateKeyArguments(baseKey, subKeyPath)

            Using subKey As RegistryKey = baseKey.OpenSubKey(subKeyPath, writable:=False)
                If subKey Is Nothing Then
                    Return Nothing
                End If

                Return TryCast(subKey.GetValue(SharpShellRegistrySettings.LogPathValueName, Nothing), String)
            End Using

        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Validates the registry key arguments shared by the public methods.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="baseKey">
        ''' The base registry key.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="subKeyPath">
        ''' The subkey path.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="baseKey"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="subKeyPath"/> is <see langword="Nothing"/>, empty or white space.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Sub ValidateKeyArguments(baseKey As RegistryKey, subKeyPath As String)

            If baseKey Is Nothing Then
                Throw New ArgumentNullException(NameOf(baseKey))
            End If

            If String.IsNullOrWhiteSpace(subKeyPath) Then
                Throw New ArgumentException("The registry subkey path cannot be null, empty or white space.", NameOf(subKeyPath))
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
