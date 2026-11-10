#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Text

#End Region

#Region " LogFileTailReader "

Namespace SharpShellEasyLog.Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Incrementally reads the new content appended to a (UTF-8 or ASCII) log file,
    ''' similar to the Unix <c>tail -f</c> command.
    ''' <para></para>
    ''' The file is opened with <see cref="FileShare.ReadWrite"/> and <see cref="FileShare.Delete"/> sharing,
    ''' so it can be read while another process is writing to it.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class LogFileTailReader

#Region " Private Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The UTF-8 byte order mark.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared ReadOnly Utf8Preamble As Byte() = {&HEF, &HBB, &HBF}

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The encoding used to decode the log file content.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared ReadOnly ContentEncoding As Encoding = New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False, throwOnInvalidBytes:=False)

#End Region

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the full path of the log file being read.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public ReadOnly Property FilePath As String

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the byte position in the log file up to which the content has already been read.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public ReadOnly Property Position As Long
            <DebuggerStepThrough>
            Get
                Return Me.positionField
            End Get
        End Property
        ''' <summary>(Backing field) <see cref="LogFileTailReader.Position"/></summary>
        Private positionField As Long

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a value indicating whether only complete lines (terminated by a line feed character) are returned.
        ''' <para></para>
        ''' When <see langword="True"/>, a trailing line that is still being written is kept pending
        ''' until its line terminator is written. This also guarantees that multi-byte UTF-8 characters
        ''' are never split between two reads.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public ReadOnly Property CompleteLinesOnly As Boolean

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="LogFileTailReader"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="filePath">
        ''' The path of the log file to read. The file does not need to exist yet.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="completeLinesOnly">
        ''' A value indicating whether only complete lines are returned.
        ''' See <see cref="LogFileTailReader.CompleteLinesOnly"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="filePath"/> is <see langword="Nothing"/>, empty, white space,
        ''' or it is not a valid file path.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Sub New(filePath As String, Optional completeLinesOnly As Boolean = True)

            If String.IsNullOrWhiteSpace(filePath) Then
                Throw New ArgumentException("The file path cannot be null, empty or white space.", NameOf(filePath))
            End If

            Try
                Me.FilePath = Path.GetFullPath(filePath)

            Catch ex As Exception When (TypeOf ex Is NotSupportedException) OrElse
                                       (TypeOf ex Is PathTooLongException) OrElse
                                       (TypeOf ex Is ArgumentException)
                Throw New ArgumentException($"The file path is not valid: '{filePath}'.", NameOf(filePath), ex)

            End Try

            Me.CompleteLinesOnly = completeLinesOnly

        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Reads the content appended to the log file since the last read.
        ''' <para></para>
        ''' If the file was truncated or re-created (its length is lower than <see cref="LogFileTailReader.Position"/>),
        ''' the file is read again from the beginning.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The new content, or <see cref="String.Empty"/> if there is no new content or the file does not exist.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="IOException">
        ''' Thrown when the file cannot be opened because another process locked it without read sharing.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Function ReadNewContent() As String

            If Not File.Exists(Me.FilePath) Then
                Me.positionField = 0
                Return String.Empty
            End If

            Using fs As New FileStream(Me.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)

                Dim length As Long = fs.Length
                If length < Me.positionField Then
                    ' The file was truncated or re-created.
                    Me.positionField = 0
                End If

                Dim available As Long = length - Me.positionField
                If available <= 0 Then
                    Return String.Empty
                End If

                If available > Integer.MaxValue Then
                    Throw New InvalidOperationException("The amount of new content is too large to be read at once.")
                End If

                Dim buffer(CInt(available) - 1) As Byte
                fs.Seek(Me.positionField, SeekOrigin.Begin)

                Dim totalRead As Integer = 0
                While totalRead < buffer.Length
                    Dim read As Integer = fs.Read(buffer, totalRead, buffer.Length - totalRead)
                    If read = 0 Then
                        Exit While
                    End If
                    totalRead += read
                End While

                If totalRead = 0 Then
                    Return String.Empty
                End If

                Dim startIndex As Integer = 0
                If (Me.positionField = 0) AndAlso LogFileTailReader.StartsWithUtf8Preamble(buffer, totalRead) Then
                    startIndex = LogFileTailReader.Utf8Preamble.Length
                End If

                Dim endIndex As Integer = totalRead ' Exclusive.
                If Me.CompleteLinesOnly Then
                    endIndex = Array.LastIndexOf(buffer, CByte(10), totalRead - 1, totalRead) + 1
                    If endIndex <= startIndex Then
                        ' No complete line yet. Keep the BOM (if any) pending too, so it is skipped on the next read.
                        Return String.Empty
                    End If
                End If

                Me.positionField += endIndex
                Return LogFileTailReader.ContentEncoding.GetString(buffer, startIndex, endIndex - startIndex)

            End Using

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Resets the read position, so the next call to <see cref="LogFileTailReader.ReadNewContent"/>
        ''' reads the whole file from the beginning.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Sub Reset()

            Me.positionField = 0

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Moves the read position to the current end of the log file,
        ''' so the existing content is skipped and only content appended afterwards is returned.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Sub SeekToEnd()

            Me.positionField = If(File.Exists(Me.FilePath), New FileInfo(Me.FilePath).Length, 0L)

        End Sub

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified buffer starts with the UTF-8 byte order mark.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="buffer">
        ''' The buffer.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="count">
        ''' The number of valid bytes in <paramref name="buffer"/>.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the buffer starts with the UTF-8 byte order mark; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function StartsWithUtf8Preamble(buffer As Byte(), count As Integer) As Boolean

            If count < LogFileTailReader.Utf8Preamble.Length Then
                Return False
            End If

            For i As Integer = 0 To LogFileTailReader.Utf8Preamble.Length - 1
                If buffer(i) <> LogFileTailReader.Utf8Preamble(i) Then
                    Return False
                End If
            Next

            Return True

        End Function

#End Region

    End Class

End Namespace

#End Region
