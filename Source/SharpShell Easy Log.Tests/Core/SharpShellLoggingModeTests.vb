#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Core

#End Region

#Region " SharpShellLoggingModeTests "

Namespace SharpShellEasyLog.Tests.Core

    ''' <summary>
    ''' Tests for <see cref="SharpShellLoggingMode"/> and <see cref="SharpShellLoggingModeExtensions"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class SharpShellLoggingModeTests

#Region " Enum Values "

        ''' <summary>
        ''' The enum values must match the values that SharpShell reads from the <c>LoggingMode</c> registry value.
        ''' </summary>
        <TestMethod>
        Public Sub EnumValues_MatchSharpShellLoggingModeValues()

            Assert.AreEqual(0, CInt(SharpShellLoggingMode.Disabled))
            Assert.AreEqual(1, CInt(SharpShellLoggingMode.Debug))
            Assert.AreEqual(2, CInt(SharpShellLoggingMode.EventLog))
            Assert.AreEqual(4, CInt(SharpShellLoggingMode.File))
            Assert.AreEqual(7, CInt(SharpShellLoggingMode.All))

        End Sub

        ''' <summary>
        ''' The enumeration is a combination of flags (e.g. <see cref="SharpShellLoggingMode.All"/>).
        ''' </summary>
        <TestMethod>
        Public Sub EnumType_HasFlagsAttribute()

            Assert.IsTrue(GetType(SharpShellLoggingMode).IsDefined(GetType(FlagsAttribute), inherit:=False))

        End Sub

#End Region

#Region " IsValid "

        <TestMethod>
        <DataRow(0)>
        <DataRow(1)>
        <DataRow(2)>
        <DataRow(3)>
        <DataRow(4)>
        <DataRow(5)>
        <DataRow(6)>
        <DataRow(7)>
        Public Sub IsValid_KnownFlagCombinations_ReturnsTrue(value As Integer)

            Assert.IsTrue(DirectCast(value, SharpShellLoggingMode).IsValid())

        End Sub

        <TestMethod>
        <DataRow(8)>
        <DataRow(9)>
        <DataRow(16)>
        <DataRow(-1)>
        <DataRow(Integer.MinValue)>
        Public Sub IsValid_UnknownFlags_ReturnsFalse(value As Integer)

            Assert.IsFalse(DirectCast(value, SharpShellLoggingMode).IsValid())

        End Sub

#End Region

#Region " UsesLogFile "

        <TestMethod>
        <DataRow(SharpShellLoggingMode.File)>
        <DataRow(SharpShellLoggingMode.All)>
        <DataRow(SharpShellLoggingMode.Debug Or SharpShellLoggingMode.File)>
        <DataRow(SharpShellLoggingMode.EventLog Or SharpShellLoggingMode.File)>
        Public Sub UsesLogFile_ModesWithFileFlag_ReturnsTrue(mode As SharpShellLoggingMode)

            Assert.IsTrue(mode.UsesLogFile())

        End Sub

        <TestMethod>
        <DataRow(SharpShellLoggingMode.Disabled)>
        <DataRow(SharpShellLoggingMode.Debug)>
        <DataRow(SharpShellLoggingMode.EventLog)>
        <DataRow(SharpShellLoggingMode.Debug Or SharpShellLoggingMode.EventLog)>
        Public Sub UsesLogFile_ModesWithoutFileFlag_ReturnsFalse(mode As SharpShellLoggingMode)

            Assert.IsFalse(mode.UsesLogFile())

        End Sub

        <TestMethod>
        Public Sub UsesLogFile_InvalidMode_Throws()

            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Sub() DirectCast(12, SharpShellLoggingMode).UsesLogFile())

        End Sub

#End Region

#Region " ResolveLoggingMode "

        <TestMethod>
        <DataRow(1)>
        <DataRow(2)>
        <DataRow(4)>
        <DataRow(7)>
        <DataRow(0)>
        <DataRow(99)>
        Public Sub ResolveLoggingMode_LoggingDisabled_AlwaysReturnsDisabled(modeValue As Integer)

            Assert.AreEqual(SharpShellLoggingMode.Disabled, SharpShellLoggingModeExtensions.ResolveLoggingMode(False, modeValue))

        End Sub

        <TestMethod>
        <DataRow(1, SharpShellLoggingMode.Debug)>
        <DataRow(2, SharpShellLoggingMode.EventLog)>
        <DataRow(4, SharpShellLoggingMode.File)>
        <DataRow(7, SharpShellLoggingMode.All)>
        Public Sub ResolveLoggingMode_LoggingEnabled_ReturnsMode(modeValue As Integer, expected As SharpShellLoggingMode)

            Assert.AreEqual(expected, SharpShellLoggingModeExtensions.ResolveLoggingMode(True, modeValue))

        End Sub

        <TestMethod>
        <DataRow(0)>
        <DataRow(8)>
        <DataRow(-1)>
        Public Sub ResolveLoggingMode_LoggingEnabledWithInvalidMode_Throws(modeValue As Integer)

            Dim ex As ArgumentOutOfRangeException = Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Sub() SharpShellLoggingModeExtensions.ResolveLoggingMode(True, modeValue))

            Assert.AreEqual("modeValue", ex.ParamName)

        End Sub

#End Region

    End Class

End Namespace

#End Region
