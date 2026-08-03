Namespace Decks

    Public Interface IDeckRepository

        Function GetAll() As IList(Of Deck)
        Function FindByName(name As String) As Deck
        Function NameExists(name As String) As Boolean
        Function IsReferencedByTape(name As String) As Boolean
        Sub Add(value As Deck)
        Sub Update(value As Deck)
        Sub Delete(name As String)

    End Interface

End Namespace
