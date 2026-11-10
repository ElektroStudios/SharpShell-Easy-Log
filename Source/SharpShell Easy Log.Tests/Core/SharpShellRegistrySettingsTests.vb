#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32

Imports SharpShellEasyLog.Core
Imports SharpShellEasyLog.Tests.Helpers

#End Region

#Region " SharpShellRegistrySettingsTests "

Namespace SharpShellEasyLog.Tests.Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Tests for <see cref="SharpShellRegistrySettings"/>.
    ''' <para></para>
    ''' The tests use an isolated key under <c>HKEY_CURRENT_USER</c>,
    ''' so they never modify the real SharpShell configuration in <c>HKEY_LOCAL_MACHINE</c>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class SharpShellRegistrySettingsTests

        ''' <summary>The temporary registry key used by the current test.</summary>
        Private tempKey As TemporaryRegistryKey

        ''' <summary>A sample log file path.</summary>
        Private Const SampleLogPath As String = "C:\Logs\SharpShell.log"

        <TestInitialize>
        Public Sub TestInitialize()

            Me.tempKey = New TemporaryRegistryKey()

        End Sub

        <TestCleanup>
        Public Sub TestCleanup()

            Me.tempKey.Dispose()

        End Sub

#Region " Constants "

        <TestMethod>
        Public Sub Constants_MatchSharpShellRegistryNames()

            Assert.AreEqual("SOFTWARE\SharpShell", SharpShellRegistrySettings.DefaultSubKeyPath)
            Assert.AreEqual("LoggingMode", SharpShellRegistrySettings.LoggingModeValueName)
            Assert.AreEqual("LogPath", SharpShellRegistrySettings.LogPathValueName)

        End Sub

#End Region

#Region " GetTargetRegistryViews "

        <TestMethod>
        Public Sub GetTargetRegistryViews_64BitOS_ReturnsBothViews()

            CollectionAssert.AreEqual({RegistryView.Registry64, RegistryView.Registry32},
                                      SharpShellRegistrySettings.GetTargetRegistryViews(is64BitOperatingSystem:=True))

        End Sub

        <TestMethod>
        Public Sub GetTargetRegistryViews_32BitOS_ReturnsDefaultViewOnly()

            CollectionAssert.AreEqual({RegistryView.Default},
                                      SharpShellRegistrySettings.GetTargetRegistryViews(is64BitOperatingSystem:=False))

        End Sub

#End Region

#Region " Apply "

        <TestMethod>
        <DataRow(SharpShellLoggingMode.Debug)>
        <DataRow(SharpShellLoggingMode.EventLog)>
        <DataRow(SharpShellLoggingMode.File)>
        <DataRow(SharpShellLoggingMode.All)>
        Public Sub Apply_EnabledMode_WritesLoggingModeAsDword(mode As SharpShellLoggingMode)

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, mode, SharpShellRegistrySettingsTests.SampleLogPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNotNull(key)
                Assert.AreEqual(RegistryValueKind.DWord, key.GetValueKind(SharpShellRegistrySettings.LoggingModeValueName))
                Assert.AreEqual(CInt(mode), CInt(key.GetValue(SharpShellRegistrySettings.LoggingModeValueName)))
            End Using

        End Sub

        <TestMethod>
        <DataRow(SharpShellLoggingMode.File)>
        <DataRow(SharpShellLoggingMode.All)>
        Public Sub Apply_ModeWithFileFlagAndPath_WritesLogPath(mode As SharpShellLoggingMode)

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, mode, SharpShellRegistrySettingsTests.SampleLogPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.AreEqual(RegistryValueKind.String, key.GetValueKind(SharpShellRegistrySettings.LogPathValueName))
                Assert.AreEqual(SharpShellRegistrySettingsTests.SampleLogPath, CStr(key.GetValue(SharpShellRegistrySettings.LogPathValueName)))
            End Using

        End Sub

        <TestMethod>
        <DataRow(SharpShellLoggingMode.Debug)>
        <DataRow(SharpShellLoggingMode.EventLog)>
        Public Sub Apply_ModeWithoutFileFlag_DoesNotWriteLogPath(mode As SharpShellLoggingMode)

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, mode, SharpShellRegistrySettingsTests.SampleLogPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNull(key.GetValue(SharpShellRegistrySettings.LogPathValueName))
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_FileModeWithNullPath_WritesModeButNoLogPath()

            Me.Apply_FileModeWithoutPath_WritesModeButNoLogPath(Nothing)

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("   ")>
        Public Sub Apply_FileModeWithoutPath_WritesModeButNoLogPath(logPath As String)

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.File, logPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.AreEqual(CInt(SharpShellLoggingMode.File), CInt(key.GetValue(SharpShellRegistrySettings.LoggingModeValueName)))
                Assert.IsNull(key.GetValue(SharpShellRegistrySettings.LogPathValueName))
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_SwitchFromFileToDebug_RemovesStaleLogPath()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.File, SharpShellRegistrySettingsTests.SampleLogPath)
            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Debug, SharpShellRegistrySettingsTests.SampleLogPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.AreEqual(CInt(SharpShellLoggingMode.Debug), CInt(key.GetValue(SharpShellRegistrySettings.LoggingModeValueName)))
                Assert.IsNull(key.GetValue(SharpShellRegistrySettings.LogPathValueName))
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_CalledTwice_OverwritesValues()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.File, "C:\old.log")
            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.All, "C:\new.log")

            Assert.AreEqual(SharpShellLoggingMode.All, SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))
            Assert.AreEqual("C:\new.log", SharpShellRegistrySettings.ReadLogPath(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub Apply_Disabled_DeletesKeyWhenItBecomesEmpty()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.All, SharpShellRegistrySettingsTests.SampleLogPath)
            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Disabled, SharpShellRegistrySettingsTests.SampleLogPath)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNull(key, "The empty SharpShell key should have been deleted.")
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_Disabled_PreservesForeignValuesAndKey()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.All, SharpShellRegistrySettingsTests.SampleLogPath)
            Using key As RegistryKey = Me.tempKey.BaseKey.OpenSubKey(Me.tempKey.SharpShellPath, writable:=True)
                key.SetValue("SomeOtherValue", "keep me", RegistryValueKind.String)
            End Using

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Disabled, Nothing)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNotNull(key, "The key contains other values, so it must not be deleted.")
                Assert.AreEqual("keep me", CStr(key.GetValue("SomeOtherValue")))
                Assert.IsNull(key.GetValue(SharpShellRegistrySettings.LoggingModeValueName))
                Assert.IsNull(key.GetValue(SharpShellRegistrySettings.LogPathValueName))
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_Disabled_PreservesKeyWithSubKeys()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Debug, Nothing)
            Me.tempKey.BaseKey.CreateSubKey($"{Me.tempKey.SharpShellPath}\Child").Dispose()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Disabled, Nothing)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNotNull(key)
                Assert.AreEqual(1, key.SubKeyCount)
                Assert.AreEqual(0, key.ValueCount)
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_DisabledWhenKeyDoesNotExist_DoesNotThrow()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Disabled, Nothing)

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNull(key)
            End Using

        End Sub

        <TestMethod>
        Public Sub Apply_NullBaseKey_Throws()

            Dim ex As ArgumentNullException = Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() SharpShellRegistrySettings.Apply(Nothing, Me.tempKey.SharpShellPath, SharpShellLoggingMode.Debug, Nothing))

            Assert.AreEqual("baseKey", ex.ParamName)

        End Sub

        <TestMethod>
        Public Sub Apply_NullSubKeyPath_Throws()

            Me.Apply_InvalidSubKeyPath_Throws(Nothing)

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("  ")>
        Public Sub Apply_InvalidSubKeyPath_Throws(subKeyPath As String)

            Dim ex As ArgumentException = Assert.ThrowsExactly(Of ArgumentException)(
                Sub() SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, subKeyPath, SharpShellLoggingMode.Debug, Nothing))

            Assert.AreEqual("subKeyPath", ex.ParamName)

        End Sub

        <TestMethod>
        Public Sub Apply_InvalidMode_ThrowsAndWritesNothing()

            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Sub() SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, DirectCast(64, SharpShellLoggingMode), Nothing))

            Using key As RegistryKey = Me.tempKey.OpenSharpShellKey()
                Assert.IsNull(key)
            End Using

        End Sub

#End Region

#Region " Remove "

        <TestMethod>
        Public Sub Remove_ExistingConfiguration_DeletesKey()

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.File, SharpShellRegistrySettingsTests.SampleLogPath)

            SharpShellRegistrySettings.Remove(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath)

            Assert.AreEqual(SharpShellLoggingMode.Disabled, SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))
            Assert.IsNull(SharpShellRegistrySettings.ReadLogPath(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub Remove_NullBaseKey_Throws()

            Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() SharpShellRegistrySettings.Remove(Nothing, Me.tempKey.SharpShellPath))

        End Sub

#End Region

#Region " ReadLoggingMode / ReadLogPath "

        <TestMethod>
        Public Sub ReadLoggingMode_MissingKey_ReturnsDisabled()

            Assert.AreEqual(SharpShellLoggingMode.Disabled, SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub ReadLoggingMode_MissingValue_ReturnsDisabled()

            Me.tempKey.BaseKey.CreateSubKey(Me.tempKey.SharpShellPath).Dispose()

            Assert.AreEqual(SharpShellLoggingMode.Disabled, SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        <DataRow(SharpShellLoggingMode.Debug)>
        <DataRow(SharpShellLoggingMode.EventLog)>
        <DataRow(SharpShellLoggingMode.File)>
        <DataRow(SharpShellLoggingMode.All)>
        Public Sub ReadLoggingMode_AfterApply_RoundTrips(mode As SharpShellLoggingMode)

            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, mode, SharpShellRegistrySettingsTests.SampleLogPath)

            Assert.AreEqual(mode, SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub ReadLoggingMode_NonDwordValue_ThrowsInvalidData()

            Using key As RegistryKey = Me.tempKey.BaseKey.CreateSubKey(Me.tempKey.SharpShellPath, writable:=True)
                key.SetValue(SharpShellRegistrySettings.LoggingModeValueName, "1", RegistryValueKind.String)
            End Using

            Assert.ThrowsExactly(Of InvalidDataException)(
                Sub() SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub ReadLoggingMode_OutOfRangeDword_ThrowsInvalidData()

            Using key As RegistryKey = Me.tempKey.BaseKey.CreateSubKey(Me.tempKey.SharpShellPath, writable:=True)
                key.SetValue(SharpShellRegistrySettings.LoggingModeValueName, 255, RegistryValueKind.DWord)
            End Using

            Assert.ThrowsExactly(Of InvalidDataException)(
                Sub() SharpShellRegistrySettings.ReadLoggingMode(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub ReadLogPath_MissingKey_ReturnsNothing()

            Assert.IsNull(SharpShellRegistrySettings.ReadLogPath(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

        <TestMethod>
        Public Sub ReadLogPath_PathWithUnicodeCharacters_RoundTrips()

            Const unicodePath As String = "C:\Registro\ñandú — ログ.log"
            SharpShellRegistrySettings.Apply(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath, SharpShellLoggingMode.File, unicodePath)

            Assert.AreEqual(unicodePath, SharpShellRegistrySettings.ReadLogPath(Me.tempKey.BaseKey, Me.tempKey.SharpShellPath))

        End Sub

#End Region

    End Class

End Namespace

#End Region
