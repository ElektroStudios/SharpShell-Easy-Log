#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Drawing2D

Imports SharpShellEasyLog.Theming

#End Region

#Region " ToggleSwitch "

Namespace SharpShellEasyLog.UserControls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' An on/off toggle switch control, similar to the toggle switches of the Windows settings.
    ''' <para></para>
    ''' The control is toggled with a mouse click, or with the <see cref="Keys.Space"/> or <see cref="Keys.Enter"/> keys.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <DesignerCategory("Code")>
    <DefaultEvent(NameOf(ToggleSwitch.CheckedChanged))>
    <DefaultProperty(NameOf(ToggleSwitch.Checked))>
    <ToolboxBitmap(GetType(CheckBox))>
    Public Class ToggleSwitch : Inherits Control : Implements IThemeableControl

#Region " Private Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' A value indicating whether the mouse pointer is over the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private isHovered As Boolean

#End Region

#Region " Events "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Occurs when the value of the <see cref="ToggleSwitch.Checked"/> property changes.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Category("Action")>
        <Description("Occurs when the value of the Checked property changes.")>
        Public Event CheckedChanged As EventHandler

#End Region

#Region " Properties "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value indicating whether the switch is in the on state.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Category("Appearance")>
        <DefaultValue(False)>
        <Description("Indicates whether the switch is in the on state.")>
        Public Property Checked As Boolean
            <DebuggerStepThrough>
            Get
                Return Me.checkedField
            End Get
            <DebuggerStepperBoundary>
            Set(value As Boolean)
                If value <> Me.checkedField Then
                    Me.checkedField = value
                    Me.Invalidate()
                    Me.AccessibilityNotifyClients(AccessibleEvents.StateChange, -1)
                    Me.OnCheckedChanged(EventArgs.Empty)
                End If
            End Set
        End Property
        ''' <summary>(Backing field) <see cref="ToggleSwitch.Checked"/></summary>
        Private checkedField As Boolean

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the palette currently applied to the control.
        ''' <para></para>
        ''' The default value is <see cref="ThemePalette.Light"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette = ThemePalette.Light Implements IThemeableControl.Palette

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the default size of the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides ReadOnly Property DefaultSize As Size
            <DebuggerStepThrough>
            Get
                Return New Size(40, 20)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ToggleSwitch"/> class.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerNonUserCode>
        Public Sub New()

            MyBase.New()

            Me.SetStyle(ControlStyles.UserPaint Or
                        ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer Or
                        ControlStyles.ResizeRedraw Or
                        ControlStyles.SupportsTransparentBackColor Or
                        ControlStyles.Selectable, True)

            ' Each click toggles the switch, also when the user clicks quickly twice.
            Me.SetStyle(ControlStyles.StandardDoubleClick, False)

            Me.BackColor = Color.Transparent
            Me.Cursor = Cursors.Hand
            Me.TabStop = True
            Me.AccessibleRole = AccessibleRole.CheckButton

        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Toggles the value of the <see cref="ToggleSwitch.Checked"/> property.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Sub Toggle()

            Me.Checked = Not Me.Checked

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="palette">
        ''' The palette to apply.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="palette"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepperBoundary>
        Public Sub ApplyTheme(palette As ThemePalette) Implements IThemeableControl.ApplyTheme

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Me._Palette = palette
            Me.Invalidate()

        End Sub

#End Region

#Region " Protected Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="ToggleSwitch.CheckedChanged"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overridable Sub OnCheckedChanged(e As EventArgs)

            RaiseEvent CheckedChanged(Me, e)

        End Sub

#End Region

#Region " Overridden Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new accessibility object for the control.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' A new <see cref="AccessibleObject"/> for the control.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Function CreateAccessibilityInstance() As AccessibleObject

            Return New ToggleSwitchAccessibleObject(Me)

        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="PaintEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            MyBase.OnPaint(e)

            Dim g As Graphics = e.Graphics
            Dim palette As ThemePalette = Me.Palette
            Dim oldSmoothingMode As SmoothingMode = g.SmoothingMode
            g.SmoothingMode = SmoothingMode.AntiAlias

            ' Track.
            Dim trackRect As New RectangleF(1.0F, 1.0F, Me.Width - 3.0F, Me.Height - 3.0F)
            If (trackRect.Width <= 0) OrElse (trackRect.Height <= 0) Then
                Return
            End If

            Dim alpha As Integer = If(Me.Enabled, 255, 110)
            Dim trackFillColor As Color = Color.FromArgb(alpha, If(Me.Checked, palette.AccentColor, palette.ToggleOffTrackColor))
            Dim trackBorderColor As Color = Color.FromArgb(alpha, If(Me.Checked, palette.AccentColor, palette.ToggleOffThumbColor))
            Dim thumbColor As Color = Color.FromArgb(alpha, If(Me.Checked, palette.AccentForeColor, palette.ToggleOffThumbColor))

            Using trackPath As GraphicsPath = ToggleSwitch.CreatePillPath(trackRect),
                  trackBrush As New SolidBrush(trackFillColor),
                  trackPen As New Pen(trackBorderColor, 1.0F)
                g.FillPath(trackBrush, trackPath)
                g.DrawPath(trackPen, trackPath)
            End Using

            ' Thumb.
            Dim thumbMargin As Single = If(Me.isHovered AndAlso Me.Enabled, 3.0F, 4.0F)
            Dim thumbDiameter As Single = Math.Max(1.0F, trackRect.Height - (thumbMargin * 2.0F))
            Dim thumbX As Single = If(Me.Checked,
                                      trackRect.Right - thumbMargin - thumbDiameter,
                                      trackRect.Left + thumbMargin)
            Dim thumbRect As New RectangleF(thumbX, trackRect.Top + thumbMargin, thumbDiameter, thumbDiameter)

            Using thumbBrush As New SolidBrush(thumbColor)
                g.FillEllipse(thumbBrush, thumbRect)
            End Using

            g.SmoothingMode = oldSmoothingMode

            ' Focus cues.
            If Me.Focused AndAlso Me.ShowFocusCues Then
                ControlPaint.DrawFocusRectangle(g, Me.ClientRectangle)
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.MouseClick"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="MouseEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnMouseClick(e As MouseEventArgs)

            MyBase.OnMouseClick(e)

            If e.Button = MouseButtons.Left Then
                Me.Toggle()
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.KeyDown"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' A <see cref="KeyEventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

            MyBase.OnKeyDown(e)

            If e.Handled Then
                Return
            End If

            If (e.KeyCode = Keys.Space) OrElse (e.KeyCode = Keys.Enter) Then
                Me.Toggle()
                e.Handled = True
            End If

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.MouseEnter"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnMouseEnter(e As EventArgs)

            MyBase.OnMouseEnter(e)
            Me.isHovered = True
            Me.Invalidate()

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.MouseLeave"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnMouseLeave(e As EventArgs)

            MyBase.OnMouseLeave(e)
            Me.isHovered = False
            Me.Invalidate()

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnGotFocus(e As EventArgs)

            MyBase.OnGotFocus(e)
            Me.Invalidate()

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnLostFocus(e As EventArgs)

            MyBase.OnLostFocus(e)
            Me.Invalidate()

        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="Control.EnabledChanged"/> event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Protected Overrides Sub OnEnabledChanged(e As EventArgs)

            MyBase.OnEnabledChanged(e)
            Me.Invalidate()

        End Sub

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a pill-shaped (fully rounded rectangle) path that fits the specified rectangle.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="rect">
        ''' The bounding rectangle.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resulting <see cref="GraphicsPath"/>. The caller is responsible for disposing it.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Private Shared Function CreatePillPath(rect As RectangleF) As GraphicsPath

            Dim diameter As Single = Math.Min(rect.Height, rect.Width)
            Dim path As New GraphicsPath()
            path.AddArc(rect.Left, rect.Top, diameter, diameter, 90, 180)
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 180)
            path.CloseFigure()
            Return path

        End Function

#End Region

#Region " ToggleSwitchAccessibleObject "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Provides accessibility information (check button role and checked state) for a <see cref="ToggleSwitch"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private NotInheritable Class ToggleSwitchAccessibleObject : Inherits ControlAccessibleObject

            ''' <summary>The owner control.</summary>
            Private ReadOnly ownerToggle As ToggleSwitch

            ''' ----------------------------------------------------------------------------------------------------
            ''' <summary>
            ''' Initializes a new instance of the <see cref="ToggleSwitchAccessibleObject"/> class.
            ''' </summary>
            ''' ----------------------------------------------------------------------------------------------------
            ''' <param name="owner">
            ''' The owner control.
            ''' </param>
            ''' ----------------------------------------------------------------------------------------------------
            <DebuggerStepThrough>
            Friend Sub New(owner As ToggleSwitch)

                MyBase.New(owner)
                Me.ownerToggle = owner

            End Sub

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property Role As AccessibleRole
                Get
                    Return AccessibleRole.CheckButton
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property State As AccessibleStates
                Get
                    Dim result As AccessibleStates = MyBase.State
                    If Me.ownerToggle.Checked Then
                        result = result Or AccessibleStates.Checked
                    End If
                    Return result
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides ReadOnly Property DefaultAction As String
                Get
                    Return "Toggle"
                End Get
            End Property

            ''' <inheritdoc/>
            Public Overrides Sub DoDefaultAction()

                Me.ownerToggle.Toggle()

            End Sub

        End Class

#End Region

    End Class

End Namespace

#End Region
