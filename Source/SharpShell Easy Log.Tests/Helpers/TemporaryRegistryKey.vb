#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32

#End Region

#Region " TemporaryRegistryKey "

Namespace SharpShellEasyLog.Tests.Helpers

    ''' <summary>
    ''' Provides a unique, isolated registry location under <c>HKEY_CURRENT_USER</c>
    ''' that is deleted (with all its content) when disposed.
    ''' <para></para>
    ''' The tests never touch <c>HKEY_LOCAL_MACHINE</c>, so they do not require administrative privileges
    ''' and do not alter the real SharpShell configuration of the machine.
    ''' </summary>
    Friend NotInheritable Class TemporaryRegistryKey : Implements IDisposable

        ''' <summary>
        ''' The path of the parent key of all the temporary keys created by the tests.
        ''' </summary>
        Private Const TestsRootPath As String = "Software\ElektroStudios\SharpShellEasyLog.Tests"

        ''' <summary>
        ''' Gets the base key (<c>HKEY_CURRENT_USER</c>).
        ''' </summary>
        Friend ReadOnly Property BaseKey As RegistryKey = Registry.CurrentUser

        ''' <summary>
        ''' Gets the path of the unique temporary key, relative to <see cref="TemporaryRegistryKey.BaseKey"/>.
        ''' </summary>
        Friend ReadOnly Property RootPath As String

        ''' <summary>
        ''' Gets the path of the emulated SharpShell key (<c>[RootPath]\SharpShell</c>),
        ''' relative to <see cref="TemporaryRegistryKey.BaseKey"/>. The key is not created.
        ''' </summary>
        Friend ReadOnly Property SharpShellPath As String

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TemporaryRegistryKey"/> class.
        ''' </summary>
        Friend Sub New()

            Me.RootPath = $"{TemporaryRegistryKey.TestsRootPath}\{Guid.NewGuid():N}"
            Me.SharpShellPath = $"{Me.RootPath}\SharpShell"

            Using key As RegistryKey = Me.BaseKey.CreateSubKey(Me.RootPath, writable:=True)
                If key Is Nothing Then
                    Throw New InvalidOperationException($"Unable to create the test registry key '{Me.RootPath}'.")
                End If
            End Using

        End Sub

        ''' <summary>
        ''' Opens the emulated SharpShell key for reading.
        ''' </summary>
        ''' <returns>
        ''' The opened key, or <see langword="Nothing"/> if it does not exist.
        ''' </returns>
        Friend Function OpenSharpShellKey() As RegistryKey

            Return Me.BaseKey.OpenSubKey(Me.SharpShellPath, writable:=False)

        End Function

        ''' <summary>
        ''' Deletes the unique temporary key and all its content.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            Me.BaseKey.DeleteSubKeyTree(Me.RootPath, throwOnMissingSubKey:=False)

            ' Remove the parent key too, if no other test is using it.
            Using parent As RegistryKey = Me.BaseKey.OpenSubKey(TemporaryRegistryKey.TestsRootPath, writable:=False)
                If (parent IsNot Nothing) AndAlso (parent.SubKeyCount = 0) AndAlso (parent.ValueCount = 0) Then
                    parent.Close()
                    Me.BaseKey.DeleteSubKey(TemporaryRegistryKey.TestsRootPath, throwOnMissingSubKey:=False)
                End If
            End Using

        End Sub

    End Class

End Namespace

#End Region
