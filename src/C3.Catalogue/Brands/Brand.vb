Namespace Brands

    Public NotInheritable Class Brand

        Public Sub New(name As String, code As String, addedAt As DateTime, notes As String)
            Me.Name = name
            Me.Code = code
            Me.AddedAt = addedAt
            Me.Notes = notes
        End Sub

        Public ReadOnly Property Name As String
        Public ReadOnly Property Code As String
        Public ReadOnly Property AddedAt As DateTime
        Public ReadOnly Property Notes As String

    End Class

End Namespace

