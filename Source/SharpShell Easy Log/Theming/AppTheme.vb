#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " AppTheme "

Namespace SharpShellEasyLog.Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies the visual theme of the application.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Enum AppTheme As Integer

        ''' <summary>
        ''' Light theme, based on the default Windows system colors.
        ''' </summary>
        Light = 0

        ''' <summary>
        ''' Dark theme.
        ''' </summary>
        Dark = 1

    End Enum

End Namespace

#End Region
