#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.CompilerServices

#End Region

#Region " SharpShellLoggingModeExtensions "

Namespace SharpShellEasyLog.Core

    ''' <summary>
    ''' Provides helper and extension methods for the <see cref="SharpShellLoggingMode"/> enumeration.
    ''' </summary>
    <HideModuleName>
    Public Module SharpShellLoggingModeExtensions

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the specified value is a valid <see cref="SharpShellLoggingMode"/> value
        ''' (any combination of the defined flags).
        ''' </summary>
        ''' <param name="mode">
        ''' The logging mode to evaluate.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="mode"/> is a valid value; otherwise, <see langword="False"/>.
        ''' </returns>
        <Extension>
        <DebuggerStepThrough>
        Public Function IsValid(mode As SharpShellLoggingMode) As Boolean

            Return (mode And Not SharpShellLoggingMode.All) = SharpShellLoggingMode.Disabled

        End Function

        ''' <summary>
        ''' Determines whether the specified logging mode writes log messages to a log file,
        ''' and therefore requires a log file path.
        ''' </summary>
        ''' <param name="mode">
        ''' The logging mode to evaluate.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="mode"/> includes the <see cref="SharpShellLoggingMode.File"/> flag;
        ''' otherwise, <see langword="False"/>.
        ''' </returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="mode"/> is not a valid <see cref="SharpShellLoggingMode"/> value.
        ''' </exception>
        <Extension>
        <DebuggerStepThrough>
        Public Function UsesLogFile(mode As SharpShellLoggingMode) As Boolean

            SharpShellLoggingModeExtensions.ThrowIfInvalid(mode, NameOf(mode))
            Return (mode And SharpShellLoggingMode.File) = SharpShellLoggingMode.File

        End Function

        ''' <summary>
        ''' Resolves the effective <see cref="SharpShellLoggingMode"/> from the values
        ''' stored in the application settings.
        ''' </summary>
        ''' <param name="isLoggingEnabled">
        ''' A value indicating whether the logging is enabled.
        ''' </param>
        ''' <param name="modeValue">
        ''' The numeric logging mode value (<c>1</c>, <c>2</c>, <c>4</c> or any combination of them).
        ''' </param>
        ''' <returns>
        ''' <see cref="SharpShellLoggingMode.Disabled"/> if <paramref name="isLoggingEnabled"/> is <see langword="False"/>;
        ''' otherwise, the <see cref="SharpShellLoggingMode"/> that corresponds to <paramref name="modeValue"/>.
        ''' </returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when logging is enabled and <paramref name="modeValue"/> is not a valid, non-zero logging mode.
        ''' </exception>
        <DebuggerStepThrough>
        Public Function ResolveLoggingMode(isLoggingEnabled As Boolean, modeValue As Integer) As SharpShellLoggingMode

            If Not isLoggingEnabled Then
                Return SharpShellLoggingMode.Disabled
            End If

            Dim mode As SharpShellLoggingMode = DirectCast(modeValue, SharpShellLoggingMode)
            If (mode = SharpShellLoggingMode.Disabled) OrElse Not mode.IsValid() Then
                Throw New ArgumentOutOfRangeException(NameOf(modeValue), modeValue,
                                                      $"The value must be a non-zero combination of {NameOf(SharpShellLoggingMode)} flags.")
            End If

            Return mode

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Throws an <see cref="ArgumentOutOfRangeException"/> if the specified logging mode is not valid.
        ''' </summary>
        ''' <param name="mode">
        ''' The logging mode to validate.
        ''' </param>
        ''' <param name="paramName">
        ''' The name of the parameter being validated.
        ''' </param>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="mode"/> is not a valid <see cref="SharpShellLoggingMode"/> value.
        ''' </exception>
        <DebuggerStepThrough>
        Friend Sub ThrowIfInvalid(mode As SharpShellLoggingMode, paramName As String)

            If Not mode.IsValid() Then
                Throw New ArgumentOutOfRangeException(paramName, mode,
                                                      $"The value is not a valid {NameOf(SharpShellLoggingMode)} value.")
            End If

        End Sub

#End Region

    End Module

End Namespace

#End Region
