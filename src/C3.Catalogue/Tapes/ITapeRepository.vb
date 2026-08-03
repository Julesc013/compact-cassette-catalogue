Namespace Tapes

    Public Interface ITapeRepository

        Function GetAll() As IList(Of Tape)
        Function FindByShortIdentifier(identifier As String) As Tape
        Function ModelExists(identifier As String) As Boolean
        Function NextNumberForModel(identifier As String) As Integer
        Function IdentifierExists(identifier As String, shortIdentifier As String) As Boolean
        Sub AddRange(values As IList(Of Tape))
        Sub Update(value As Tape)
        Sub Delete(shortIdentifier As String)

    End Interface

End Namespace
