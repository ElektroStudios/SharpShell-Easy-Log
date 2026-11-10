#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " ThemedPaintHelper "

Namespace SharpShellEasyLog.UserControls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Helper methods shared by the themed controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class ThemedPaintHelper

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="ThemedPaintHelper"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Friend Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the <see cref="TextFormatFlags"/> that reproduce the specified text alignment and options.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="alignment">
        ''' The text alignment.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="rightToLeft">
        ''' The right-to-left setting of the control.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="useMnemonic">
        ''' A value indicating whether the ampersand character is interpreted as a mnemonic prefix.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="showKeyboardCues">
        ''' A value indicating whether the keyboard cues (mnemonic underlines) are visible.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="wordWrap">
        ''' A value indicating whether the text is wrapped into multiple lines.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting <see cref="TextFormatFlags"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Friend Shared Function GetTextFormatFlags(alignment As ContentAlignment,
                                                  rightToLeft As RightToLeft,
                                                  useMnemonic As Boolean,
                                                  showKeyboardCues As Boolean,
                                                  wordWrap As Boolean) As TextFormatFlags

            Dim flags As TextFormatFlags = TextFormatFlags.EndEllipsis

            Select Case alignment
                Case ContentAlignment.TopLeft, ContentAlignment.MiddleLeft, ContentAlignment.BottomLeft
                    flags = flags Or TextFormatFlags.Left
                Case ContentAlignment.TopCenter, ContentAlignment.MiddleCenter, ContentAlignment.BottomCenter
                    flags = flags Or TextFormatFlags.HorizontalCenter
                Case Else
                    flags = flags Or TextFormatFlags.Right
            End Select

            Select Case alignment
                Case ContentAlignment.TopLeft, ContentAlignment.TopCenter, ContentAlignment.TopRight
                    flags = flags Or TextFormatFlags.Top
                Case ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter, ContentAlignment.MiddleRight
                    flags = flags Or TextFormatFlags.VerticalCenter
                Case Else
                    flags = flags Or TextFormatFlags.Bottom
            End Select

            If wordWrap Then
                flags = flags Or TextFormatFlags.WordBreak
            Else
                flags = flags Or TextFormatFlags.SingleLine
            End If

            If rightToLeft = RightToLeft.Yes Then
                flags = flags Or TextFormatFlags.RightToLeft
            End If

            If Not useMnemonic Then
                flags = flags Or TextFormatFlags.NoPrefix
            ElseIf Not showKeyboardCues Then
                flags = flags Or TextFormatFlags.HidePrefix
            End If

            Return flags

        End Function

#End Region

    End Class

End Namespace

#End Region
