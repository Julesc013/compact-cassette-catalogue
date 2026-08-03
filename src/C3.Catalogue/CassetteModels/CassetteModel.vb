Namespace CassetteModels

    Public NotInheritable Class CassetteModel

        Public Sub New(
                brandCode As String,
                typeNumber As Integer,
                modelName As String,
                code As String,
                identifier As String,
                displayName As String,
                tapeCount As Integer,
                addedAt As DateTime,
                notes As String)

            Me.BrandCode = brandCode
            Me.TypeNumber = typeNumber
            Me.ModelName = modelName
            Me.Code = code
            Me.Identifier = identifier
            Me.DisplayName = displayName
            Me.TapeCount = tapeCount
            Me.AddedAt = addedAt
            Me.Notes = notes
        End Sub

        Public ReadOnly Property BrandCode As String
        Public ReadOnly Property TypeNumber As Integer
        Public ReadOnly Property ModelName As String
        Public ReadOnly Property Code As String
        Public ReadOnly Property Identifier As String
        Public ReadOnly Property DisplayName As String
        Public ReadOnly Property TapeCount As Integer
        Public ReadOnly Property AddedAt As DateTime
        Public ReadOnly Property Notes As String

    End Class

End Namespace
