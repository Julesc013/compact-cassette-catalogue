Namespace Brands

    Public NotInheritable Class BrandDraft

        Public Sub New(name As String, code As String, notes As String)
            Me.Name = name
            Me.Code = code
            Me.Notes = notes
        End Sub

        Public ReadOnly Property Name As String
        Public ReadOnly Property Code As String
        Public ReadOnly Property Notes As String

    End Class

End Namespace

