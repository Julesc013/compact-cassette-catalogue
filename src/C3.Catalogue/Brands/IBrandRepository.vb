Namespace Brands

    Public Interface IBrandRepository

        Function GetAll() As IList(Of Brand)
        Function FindByCode(code As String) As Brand
        Function IsCodeInUse(code As String) As Boolean
        Function IsReferencedByModel(code As String) As Boolean
        Sub Add(value As Brand)
        Sub Update(value As Brand)
        Sub Delete(code As String)

    End Interface

End Namespace

