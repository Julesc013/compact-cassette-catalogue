Namespace Tapes

    Public NotInheritable Class TapeDraft

        Public Sub New(
                modelIdentifier As String,
                year As Integer,
                lengthMinutes As Decimal,
                region As String,
                condition As Integer,
                packaged As Boolean,
                sideA As TapeSide,
                sideB As TapeSide,
                notes As String)

            Me.ModelIdentifier = modelIdentifier
            Me.Year = year
            Me.LengthMinutes = lengthMinutes
            Me.Region = region
            Me.Condition = condition
            Me.Packaged = packaged
            Me.SideA = sideA
            Me.SideB = sideB
            Me.Notes = notes
        End Sub

        Public ReadOnly Property ModelIdentifier As String
        Public ReadOnly Property Year As Integer
        Public ReadOnly Property LengthMinutes As Decimal
        Public ReadOnly Property Region As String
        Public ReadOnly Property Condition As Integer
        Public ReadOnly Property Packaged As Boolean
        Public ReadOnly Property SideA As TapeSide
        Public ReadOnly Property SideB As TapeSide
        Public ReadOnly Property Notes As String

    End Class

End Namespace
