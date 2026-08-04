Namespace Catalogues

    Public NotInheritable Class CatalogueRevision
        Implements IEquatable(Of CatalogueRevision)

        Private ReadOnly _value As C3.Domain.Catalogues.CatalogueRevision

        Public Sub New(token As String)
            _value = New C3.Domain.Catalogues.CatalogueRevision(token)
        End Sub

        Public ReadOnly Property Token As String
            Get
                Return _value.Token
            End Get
        End Property

        Public Overloads Function Equals(other As CatalogueRevision) As Boolean _
                Implements IEquatable(Of CatalogueRevision).Equals
            Return other IsNot Nothing AndAlso _value.Equals(other._value)
        End Function

        Public Overrides Function Equals(value As Object) As Boolean
            Return Equals(TryCast(value, CatalogueRevision))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            Return _value.ToString()
        End Function
    End Class

End Namespace
