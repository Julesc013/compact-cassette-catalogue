Namespace CassetteModels

    Public NotInheritable Class CassetteModelDraft

        Public Sub New(
                brandCode As String,
                typeNumber As Integer,
                modelName As String,
                code As String,
                displayName As String,
                notes As String)

            Me.BrandCode = brandCode
            Me.TypeNumber = typeNumber
            Me.ModelName = modelName
            Me.Code = code
            Me.DisplayName = displayName
            Me.Notes = notes
        End Sub

        Public ReadOnly Property BrandCode As String
        Public ReadOnly Property TypeNumber As Integer
        Public ReadOnly Property ModelName As String
        Public ReadOnly Property Code As String
        Public ReadOnly Property DisplayName As String
        Public ReadOnly Property Notes As String

    End Class

End Namespace
