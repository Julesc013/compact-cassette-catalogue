Namespace CassetteModels

    Public Interface ICassetteModelRepository

        Function GetAll() As IList(Of CassetteModel)
        Function FindByIdentifier(identifier As String) As CassetteModel
        Function BrandExists(code As String) As Boolean
        Function IdentifierExists(identifier As String) As Boolean
        Function IsReferencedByTape(identifier As String) As Boolean
        Sub Add(value As CassetteModel)
        Sub Update(value As CassetteModel)
        Sub Delete(identifier As String)

    End Interface

End Namespace
