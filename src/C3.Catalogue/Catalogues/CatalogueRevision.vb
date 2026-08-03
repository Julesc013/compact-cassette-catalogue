Namespace Catalogues

    Public NotInheritable Class CatalogueRevision
        Implements IEquatable(Of CatalogueRevision)

        Private ReadOnly _token As String

        Public Sub New(token As String)
            If String.IsNullOrWhiteSpace(token) Then
                Throw New ArgumentException("A catalogue revision token is required.", "token")
            End If
            _token = token
        End Sub

        Public ReadOnly Property Token As String
            Get
                Return _token
            End Get
        End Property

        Public Overloads Function Equals(other As CatalogueRevision) As Boolean _
                Implements IEquatable(Of CatalogueRevision).Equals
            Return other IsNot Nothing AndAlso String.Equals(_token, other._token, StringComparison.Ordinal)
        End Function

        Public Overrides Function Equals(value As Object) As Boolean
            Return Equals(TryCast(value, CatalogueRevision))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return StringComparer.Ordinal.GetHashCode(_token)
        End Function

        Public Overrides Function ToString() As String
            Return _token
        End Function
    End Class

End Namespace

