Namespace Tapes

    Public NotInheritable Class Tape

        Public Sub New(
                modelIdentifier As String,
                year As Integer,
                lengthMinutes As Decimal,
                region As String,
                number As Integer,
                identifier As String,
                shortIdentifier As String,
                condition As Integer,
                packaged As Boolean,
                sideA As TapeSide,
                sideB As TapeSide,
                addedAt As DateTime,
                notes As String)

            Me.ModelIdentifier = modelIdentifier
            Me.Year = year
            Me.LengthMinutes = lengthMinutes
            Me.Region = region
            Me.Number = number
            Me.Identifier = identifier
            Me.ShortIdentifier = shortIdentifier
            Me.Condition = condition
            Me.Packaged = packaged
            Me.SideA = sideA
            Me.SideB = sideB
            Me.AddedAt = addedAt
            Me.Notes = notes
        End Sub

        Public ReadOnly Property ModelIdentifier As String
        Public ReadOnly Property Year As Integer
        Public ReadOnly Property LengthMinutes As Decimal
        Public ReadOnly Property Region As String
        Public ReadOnly Property Number As Integer
        Public ReadOnly Property Identifier As String
        Public ReadOnly Property ShortIdentifier As String
        Public ReadOnly Property Condition As Integer
        Public ReadOnly Property Packaged As Boolean
        Public ReadOnly Property SideA As TapeSide
        Public ReadOnly Property SideB As TapeSide
        Public ReadOnly Property AddedAt As DateTime
        Public ReadOnly Property Notes As String

    End Class

End Namespace
