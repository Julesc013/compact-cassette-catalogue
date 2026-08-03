Imports C3.Catalogue.CassetteModels
Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyCassetteModelRepository
        Implements ICassetteModelRepository

        Private ReadOnly _documentProvider As Func(Of DataSet)

        Public Sub New(documentProvider As Func(Of DataSet))
            If documentProvider Is Nothing Then
                Throw New ArgumentNullException("documentProvider")
            End If
            _documentProvider = documentProvider
        End Sub

        Public Function GetAll() As IList(Of CassetteModel) Implements ICassetteModelRepository.GetAll
            Dim values As New List(Of CassetteModel)()
            For Each row As DataRow In ModelsTable().Rows
                If row.RowState <> DataRowState.Deleted Then
                    values.Add(Map(row))
                End If
            Next
            Return values
        End Function

        Public Function FindByIdentifier(identifier As String) As CassetteModel _
                Implements ICassetteModelRepository.FindByIdentifier

            Dim row As DataRow = FindRow(identifier)
            If row Is Nothing Then
                Return Nothing
            End If
            Return Map(row)
        End Function

        Public Function BrandExists(code As String) As Boolean _
                Implements ICassetteModelRepository.BrandExists

            For Each row As DataRow In RequireTable("Brands").Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(Convert.ToString(row("Code")), code, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Function IdentifierExists(identifier As String) As Boolean _
                Implements ICassetteModelRepository.IdentifierExists

            Return FindRow(identifier) IsNot Nothing
        End Function

        Public Function IsReferencedByTape(identifier As String) As Boolean _
                Implements ICassetteModelRepository.IsReferencedByTape

            For Each row As DataRow In RequireTable("Tapes").Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(Convert.ToString(row("Model")), identifier, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Sub Add(value As CassetteModel) Implements ICassetteModelRepository.Add
            ModelsTable().Rows.Add(
                value.BrandCode,
                value.TypeNumber,
                value.ModelName,
                value.Code,
                value.Identifier,
                value.DisplayName,
                value.TapeCount,
                value.AddedAt,
                value.Notes)
            SynchronizeModelCounter()
        End Sub

        Public Sub Update(value As CassetteModel) Implements ICassetteModelRepository.Update
            Dim row As DataRow = FindRow(value.Identifier)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected cassette model no longer exists.")
            End If

            row("Model") = value.ModelName
            row("Name") = value.DisplayName
            row("Notes") = value.Notes
        End Sub

        Public Sub Delete(identifier As String) Implements ICassetteModelRepository.Delete
            Dim row As DataRow = FindRow(identifier)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected cassette model no longer exists.")
            End If

            ModelsTable().Rows.Remove(row)
            SynchronizeModelCounter()
        End Sub

        Private Function Document() As DataSet
            Dim value As DataSet = _documentProvider()
            If value Is Nothing Then
                Throw New InvalidOperationException("No active catalogue document is available.")
            End If
            Return value
        End Function

        Private Function ModelsTable() As DataTable
            Return RequireTable("Models")
        End Function

        Private Function RequireTable(name As String) As DataTable
            Dim table As DataTable = Document().Tables(name)
            If table Is Nothing Then
                Throw New InvalidOperationException("Catalogue table '" & name & "' is missing.")
            End If
            Return table
        End Function

        Private Function FindRow(identifier As String) As DataRow
            If String.IsNullOrWhiteSpace(identifier) Then
                Return Nothing
            End If

            For Each row As DataRow In ModelsTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(
                            Convert.ToString(row("Identifier")),
                            identifier,
                            StringComparison.OrdinalIgnoreCase) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Function Map(row As DataRow) As CassetteModel
            Dim addedAt As DateTime = DateTime.MinValue
            If Not row.IsNull("Date") Then
                addedAt = Convert.ToDateTime(row("Date"))
            End If

            Return New CassetteModel(
                ResolveBrandCode(Convert.ToString(row("Brand"))),
                ReadInteger(row, "Type"),
                Convert.ToString(row("Model")),
                Convert.ToString(row("Code")),
                Convert.ToString(row("Identifier")),
                Convert.ToString(row("Name")),
                ReadInteger(row, "Number"),
                addedAt,
                Convert.ToString(row("Notes")))
        End Function

        Private Function ResolveBrandCode(storedValue As String) As String
            For Each row As DataRow In RequireTable("Brands").Rows
                If row.RowState <> DataRowState.Deleted Then
                    Dim code As String = Convert.ToString(row("Code"))
                    Dim name As String = Convert.ToString(row("Brand"))
                    If String.Equals(storedValue, code, StringComparison.OrdinalIgnoreCase) OrElse
                            String.Equals(storedValue, name, StringComparison.OrdinalIgnoreCase) Then
                        Return code
                    End If
                End If
            Next
            Return storedValue
        End Function

        Private Shared Function ReadInteger(row As DataRow, columnName As String) As Integer
            If row.IsNull(columnName) Then
                Return 0
            End If
            Return Convert.ToInt32(row(columnName))
        End Function

        Private Sub SynchronizeModelCounter()
            Dim counter As DataRow = RequireTable("Counters").Rows.Find("Models")
            If counter IsNot Nothing Then
                counter("Number") = ModelsTable().Rows.Count
            End If
        End Sub

    End Class

End Namespace
