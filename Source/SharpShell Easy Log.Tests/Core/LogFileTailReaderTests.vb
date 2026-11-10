#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports SharpShellEasyLog.Core
Imports SharpShellEasyLog.Tests.Helpers

#End Region

#Region " LogFileTailReaderTests "

Namespace SharpShellEasyLog.Tests.Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Tests for <see cref="LogFileTailReader"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class LogFileTailReaderTests

        ''' <summary>UTF-8 encoding without byte order mark.</summary>
        Private Shared ReadOnly Utf8NoBom As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

        ''' <summary>The temporary directory used by the current test.</summary>
        Private tempDir As TemporaryDirectory

        ''' <summary>The log file path used by the current test.</summary>
        Private logPath As String

        <TestInitialize>
        Public Sub TestInitialize()

            Me.tempDir = New TemporaryDirectory()
            Me.logPath = Me.tempDir.GetFilePath("SharpShell.log")

        End Sub

        <TestCleanup>
        Public Sub TestCleanup()

            Me.tempDir.Dispose()

        End Sub

#Region " Helpers "

        ''' <summary>Appends text to the log file, using UTF-8 without BOM.</summary>
        Private Sub AppendToLog(text As String)

            File.AppendAllText(Me.logPath, text, LogFileTailReaderTests.Utf8NoBom)

        End Sub

        ''' <summary>Creates a reader for the specified path (used to test constructor validation).</summary>
        Private Shared Sub CreateReader(filePath As String)

            Dim reader As New LogFileTailReader(filePath)
            GC.KeepAlive(reader)

        End Sub

        ''' <summary>Appends raw bytes to the log file.</summary>
        Private Sub AppendBytesToLog(bytes As Byte())

            Using fs As New FileStream(Me.logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                fs.Write(bytes, 0, bytes.Length)
            End Using

        End Sub

#End Region

#Region " Constructor "

        <TestMethod>
        Public Sub Constructor_ValidPath_StoresFullPathAndDefaults()

            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual(Path.GetFullPath(Me.logPath), reader.FilePath)
            Assert.AreEqual(0L, reader.Position)
            Assert.IsTrue(reader.CompleteLinesOnly)

        End Sub

        <TestMethod>
        Public Sub Constructor_RelativePath_IsResolvedToFullPath()

            Dim reader As New LogFileTailReader("relative.log")

            Assert.IsTrue(Path.IsPathRooted(reader.FilePath))
            Assert.AreEqual(Path.GetFullPath("relative.log"), reader.FilePath)

        End Sub

        <TestMethod>
        Public Sub Constructor_NullPath_Throws()

            Dim ex As ArgumentException = Assert.ThrowsExactly(Of ArgumentException)(Sub() LogFileTailReaderTests.CreateReader(Nothing))
            Assert.AreEqual("filePath", ex.ParamName)

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("   ")>
        Public Sub Constructor_EmptyPath_Throws(filePath As String)

            Assert.ThrowsExactly(Of ArgumentException)(Sub() LogFileTailReaderTests.CreateReader(filePath))

        End Sub

        <TestMethod>
        Public Sub Constructor_PathWithInvalidCharacters_Throws()

            Dim ex As ArgumentException = Assert.ThrowsExactly(Of ArgumentException)(Sub() LogFileTailReaderTests.CreateReader("C:\inva|id<>.log"))
            Assert.AreEqual("filePath", ex.ParamName)

        End Sub

#End Region

#Region " ReadNewContent "

        <TestMethod>
        Public Sub ReadNewContent_FileDoesNotExist_ReturnsEmpty()

            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual(String.Empty, reader.ReadNewContent())
            Assert.AreEqual(0L, reader.Position)
            Assert.IsFalse(File.Exists(Me.logPath), "Reading must not create the file.")

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_EmptyFile_ReturnsEmpty()

            File.WriteAllBytes(Me.logPath, Array.Empty(Of Byte)())
            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual(String.Empty, reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_ExistingContent_ReturnsWholeFile()

            Me.AppendToLog($"line 1{vbCrLf}line 2{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual($"line 1{vbCrLf}line 2{vbCrLf}", reader.ReadNewContent())
            Assert.AreEqual(New FileInfo(Me.logPath).Length, reader.Position)

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_CalledTwiceWithoutChanges_SecondReturnsEmpty()

            Me.AppendToLog($"line 1{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            reader.ReadNewContent()

            Assert.AreEqual(String.Empty, reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_AfterAppend_ReturnsOnlyNewContent()

            Me.AppendToLog($"first{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)
            reader.ReadNewContent()

            Me.AppendToLog($"second{vbCrLf}third{vbCrLf}")

            Assert.AreEqual($"second{vbCrLf}third{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_CompleteLinesOnly_KeepsPartialLinePending()

            Me.AppendToLog($"complete{vbCrLf}partial")
            Dim reader As New LogFileTailReader(Me.logPath, completeLinesOnly:=True)

            Assert.AreEqual($"complete{vbCrLf}", reader.ReadNewContent())

            Me.AppendToLog($" line{vbCrLf}")

            Assert.AreEqual($"partial line{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_CompleteLinesOnly_NoLineFeedYet_ReturnsEmpty()

            Me.AppendToLog("no line terminator")
            Dim reader As New LogFileTailReader(Me.logPath, completeLinesOnly:=True)

            Assert.AreEqual(String.Empty, reader.ReadNewContent())
            Assert.AreEqual(0L, reader.Position)

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_CompleteLinesOnlyDisabled_ReturnsPartialLine()

            Me.AppendToLog("no line terminator")
            Dim reader As New LogFileTailReader(Me.logPath, completeLinesOnly:=False)

            Assert.AreEqual("no line terminator", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_UnixLineEndings_AreSupported()

            Me.AppendToLog($"a{vbLf}b{vbLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual($"a{vbLf}b{vbLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_Utf8Bom_IsSkipped()

            File.WriteAllText(Me.logPath, $"with bom{vbCrLf}", New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))
            Dim reader As New LogFileTailReader(Me.logPath)

            Dim content As String = reader.ReadNewContent()

            Assert.AreEqual($"with bom{vbCrLf}", content)
            Assert.AreNotEqual(ChrW(&HFEFF), content(0))

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_Utf8BomWithoutCompleteLine_IsSkippedOnLaterRead()

            File.WriteAllText(Me.logPath, "pending", New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))
            Dim reader As New LogFileTailReader(Me.logPath)
            Assert.AreEqual(String.Empty, reader.ReadNewContent())

            Me.AppendToLog(vbCrLf)

            Assert.AreEqual($"pending{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_NonAsciiCharacters_AreDecodedAsUtf8()

            Me.AppendToLog($"Configuración — ログ — ñ{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            Assert.AreEqual($"Configuración — ログ — ñ{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_MultiByteCharacterSplitBetweenWrites_IsNotCorrupted()

            ' "ñ" is encoded in UTF-8 as two bytes: C3 B1.
            Dim bytes As Byte() = LogFileTailReaderTests.Utf8NoBom.GetBytes($"señal{vbLf}")
            Dim splitIndex As Integer = 3 ' Between C3 and B1.
            Assert.AreEqual(&HC3, CInt(bytes(splitIndex - 1)))

            Me.AppendBytesToLog(bytes.Take(splitIndex).ToArray())
            Dim reader As New LogFileTailReader(Me.logPath)
            Dim firstRead As String = reader.ReadNewContent()

            Me.AppendBytesToLog(bytes.Skip(splitIndex).ToArray())
            Dim secondRead As String = reader.ReadNewContent()

            Assert.AreEqual($"señal{vbLf}", firstRead & secondRead)
            Assert.IsFalse((firstRead & secondRead).Contains(ChrW(&HFFFD)), "No replacement characters expected.")

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_FileTruncated_RestartsFromBeginning()

            Me.AppendToLog($"old line 1{vbCrLf}old line 2{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)
            reader.ReadNewContent()

            File.WriteAllText(Me.logPath, $"new{vbCrLf}", LogFileTailReaderTests.Utf8NoBom)

            Assert.AreEqual($"new{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_FileDeletedAndRecreated_RestartsFromBeginning()

            Me.AppendToLog($"original content that is long{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)
            reader.ReadNewContent()

            File.Delete(Me.logPath)
            Assert.AreEqual(String.Empty, reader.ReadNewContent())
            Assert.AreEqual(0L, reader.Position)

            Me.AppendToLog($"recreated{vbCrLf}")

            Assert.AreEqual($"recreated{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_FileOpenedForWritingByAnotherStream_CanBeRead()

            Using writer As New FileStream(Me.logPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite),
                  sw As New StreamWriter(writer, LogFileTailReaderTests.Utf8NoBom)
                sw.AutoFlush = True
                sw.Write($"while open{vbCrLf}")

                Dim reader As New LogFileTailReader(Me.logPath)

                Assert.AreEqual($"while open{vbCrLf}", reader.ReadNewContent())
            End Using

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_FileLockedWithoutReadSharing_ThrowsIOExceptionAndKeepsPosition()

            Me.AppendToLog($"line{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            Using lockStream As New FileStream(Me.logPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                Assert.ThrowsExactly(Of IOException)(Sub() reader.ReadNewContent())
            End Using

            Assert.AreEqual(0L, reader.Position)
            Assert.AreEqual($"line{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_DoesNotLockTheFile()

            Me.AppendToLog($"line{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)
            reader.ReadNewContent()

            ' Exclusive access must be possible after reading.
            Using fs As New FileStream(Me.logPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                Assert.IsTrue(fs.CanWrite)
            End Using

        End Sub

        <TestMethod>
        Public Sub ReadNewContent_ManySmallAppends_ReturnsEverythingInOrder()

            Dim reader As New LogFileTailReader(Me.logPath)
            Dim collected As New StringBuilder()
            Dim expected As New StringBuilder()

            For i As Integer = 1 To 200
                Dim line As String = $"Entry #{i:D3}{vbCrLf}"
                expected.Append(line)
                Me.AppendToLog(line)
                If i Mod 7 = 0 Then
                    collected.Append(reader.ReadNewContent())
                End If
            Next
            collected.Append(reader.ReadNewContent())

            Assert.AreEqual(expected.ToString(), collected.ToString())

        End Sub

#End Region

#Region " Reset / SeekToEnd "

        <TestMethod>
        Public Sub Reset_AfterRead_ReadsWholeFileAgain()

            Me.AppendToLog($"line{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)
            reader.ReadNewContent()

            reader.Reset()

            Assert.AreEqual(0L, reader.Position)
            Assert.AreEqual($"line{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub SeekToEnd_SkipsExistingContent()

            Me.AppendToLog($"existing{vbCrLf}")
            Dim reader As New LogFileTailReader(Me.logPath)

            reader.SeekToEnd()
            Me.AppendToLog($"appended{vbCrLf}")

            Assert.AreEqual($"appended{vbCrLf}", reader.ReadNewContent())

        End Sub

        <TestMethod>
        Public Sub SeekToEnd_FileDoesNotExist_PositionIsZero()

            Dim reader As New LogFileTailReader(Me.logPath)

            reader.SeekToEnd()

            Assert.AreEqual(0L, reader.Position)

        End Sub

#End Region

    End Class

End Namespace

#End Region
