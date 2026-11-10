#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " IThemeableControl "

Namespace SharpShellEasyLog.Theming

    ''' <summary>
    ''' Defines a control that knows how to paint itself using a <see cref="ThemePalette"/>.
    ''' <para></para>
    ''' <see cref="ThemeManager.ApplyTheme"/> delegates the theming of controls that implement this interface
    ''' to the control itself, instead of applying the generic color assignments.
    ''' </summary>
    Public Interface IThemeableControl

        ''' <summary>
        ''' Gets the palette currently applied to the control,
        ''' or <see langword="Nothing"/> if no palette was applied yet.
        ''' </summary>
        ReadOnly Property Palette As ThemePalette

        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        Sub ApplyTheme(palette As ThemePalette)

    End Interface

End Namespace

#End Region
