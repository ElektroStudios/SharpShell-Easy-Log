#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Windows.Forms.Design

#End Region

#Region " ToolStripToggleSwitch "

Namespace SharpShellEasyLog.UserControls

    ''' <summary>
    ''' A <see cref="ToolStripControlHost"/> that hosts a <see cref="UserControls.ToggleSwitch"/>,
    ''' so it can be placed in a <see cref="MenuStrip"/>, <see cref="ToolStrip"/> or <see cref="StatusStrip"/>.
    ''' </summary>
    <DesignerCategory("Code")>
    <DefaultEvent(NameOf(ToolStripToggleSwitch.CheckedChanged))>
    <DefaultProperty(NameOf(ToolStripToggleSwitch.Checked))>
    <ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip Or
                                       ToolStripItemDesignerAvailability.ToolStrip Or
                                       ToolStripItemDesignerAvailability.StatusStrip)>
    Public Class ToolStripToggleSwitch : Inherits ToolStripControlHost

#Region " Events "

        ''' <summary>
        ''' Occurs when the value of the <see cref="ToolStripToggleSwitch.Checked"/> property changes.
        ''' </summary>
        <Category("Action")>
        <Description("Occurs when the value of the Checked property changes.")>
        Public Event CheckedChanged As EventHandler

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the hosted <see cref="UserControls.ToggleSwitch"/> control.
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property ToggleSwitch As ToggleSwitch
            <DebuggerStepThrough>
            Get
                Return DirectCast(Me.Control, ToggleSwitch)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the switch is in the on state.
        ''' </summary>
        <Category("Appearance")>
        <DefaultValue(False)>
        <Description("Indicates whether the switch is in the on state.")>
        Public Property Checked As Boolean
            <DebuggerStepThrough>
            Get
                Return Me.ToggleSwitch.Checked
            End Get
            <DebuggerStepThrough>
            Set(value As Boolean)
                Me.ToggleSwitch.Checked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the default size of the item.
        ''' </summary>
        Protected Overrides ReadOnly Property DefaultSize As Size
            <DebuggerStepThrough>
            Get
                Return New Size(38, 18)
            End Get
        End Property

        ''' <summary>
        ''' Gets the default margin of the item.
        ''' </summary>
        Protected Overrides ReadOnly Property DefaultMargin As Padding
            <DebuggerStepThrough>
            Get
                Return New Padding(2, 1, 8, 1)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ToolStripToggleSwitch"/> class.
        ''' </summary>
        <DebuggerNonUserCode>
        Public Sub New()

            MyBase.New(New ToggleSwitch())
            Me.AutoSize = False
            Me.Size = Me.DefaultSize

        End Sub

#End Region

#Region " Protected Methods "

        ''' <summary>
        ''' Raises the <see cref="ToolStripToggleSwitch.CheckedChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        ''' An <see cref="EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overridable Sub OnCheckedChanged(e As EventArgs)

            RaiseEvent CheckedChanged(Me, e)

        End Sub

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Subscribes events from the hosted control.
        ''' </summary>
        ''' <param name="control">
        ''' The control from which to subscribe events.
        ''' </param>
        Protected Overrides Sub OnSubscribeControlEvents(control As Control)

            MyBase.OnSubscribeControlEvents(control)
            AddHandler DirectCast(control, ToggleSwitch).CheckedChanged, AddressOf Me.ToggleSwitch_CheckedChanged

        End Sub

        ''' <summary>
        ''' Unsubscribes events from the hosted control.
        ''' </summary>
        ''' <param name="control">
        ''' The control from which to unsubscribe events.
        ''' </param>
        Protected Overrides Sub OnUnsubscribeControlEvents(control As Control)

            MyBase.OnUnsubscribeControlEvents(control)
            RemoveHandler DirectCast(control, ToggleSwitch).CheckedChanged, AddressOf Me.ToggleSwitch_CheckedChanged

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the <see cref="UserControls.ToggleSwitch.CheckedChanged"/> event of the hosted control.
        ''' </summary>
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Private Sub ToggleSwitch_CheckedChanged(sender As Object, e As EventArgs)

            Me.OnCheckedChanged(e)

        End Sub

#End Region

    End Class

End Namespace

#End Region
