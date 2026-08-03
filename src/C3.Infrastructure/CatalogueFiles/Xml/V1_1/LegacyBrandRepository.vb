Imports C3.Catalogue.Brands
Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyBrandRepository
        Implements IBrandRepository

        Private ReadOnly _documentProvider As Func(Of DataSet)

        Public Sub New(documentProvider As Func(Of DataSet))
            If documentProvider Is Nothing Then
                Throw New ArgumentNullException("documentProvider")
            End If
            _documentProvider = documentProvider
        End Sub

        Public Function GetAll() As IList(Of Brand) Implements IBrandRepository.GetAll
            Dim values As New List(Of Brand)()
            For Each row As DataRow In BrandsTable().Rows
                If row.RowState <> DataRowState.Deleted Then
                    values.Add(Map(row))
                End If
            Next
            Return values
        End Function

        Public Function FindByCode(code As String) As Brand Implements IBrandRepository.FindByCode
            If String.IsNullOrWhiteSpace(code) Then
                Return Nothing
            End If

            For Each row As DataRow In BrandsTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(CStr(row("Code")), code, StringComparison.OrdinalIgnoreCase) Then
                    Return Map(row)
                End If
            Next
            Return Nothing
        End Function

        Public Function IsCodeInUse(code As String) As Boolean Implements IBrandRepository.IsCodeInUse
            Return FindByCode(code) IsNot Nothing
        End Function

        Public Function IsReferencedByModel(code As String) As Boolean _
                Implements IBrandRepository.IsReferencedByModel

            Dim brandRow As DataRow = FindRow(code)
            Dim legacyName As String = String.Empty
            If brandRow IsNot Nothing Then
                legacyName = Convert.ToString(brandRow("Brand"))
            End If

            For Each row As DataRow In ModelsTable().Rows
                If row.RowState <> DataRowState.Deleted Then
                    Dim storedBrand As String = Convert.ToString(row("Brand"))
                    If String.Equals(storedBrand, code, StringComparison.OrdinalIgnoreCase) OrElse
                            (legacyName.Length > 0 AndAlso
                             String.Equals(storedBrand, legacyName, StringComparison.OrdinalIgnoreCase)) Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Function

        Public Sub Add(value As Brand) Implements IBrandRepository.Add
            BrandsTable().Rows.Add(value.Name, value.Code, value.AddedAt, value.Notes)
            SynchronizeBrandCounter()
        End Sub

        Public Sub Update(value As Brand) Implements IBrandRepository.Update
            Dim row As DataRow = FindRow(value.Code)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected brand no longer exists.")
            End If

            Dim previousName As String = Convert.ToString(row("Brand"))
            row("Brand") = value.Name
            row("Date") = value.AddedAt
            row("Notes") = value.Notes

            ' Older C3 files stored the display name in Models.Brand. Migrate
            ' those references to the stable code when the owning brand changes.
            For Each modelRow As DataRow In ModelsTable().Rows
                If modelRow.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(
                            Convert.ToString(modelRow("Brand")),
                            previousName,
                            StringComparison.OrdinalIgnoreCase) Then
                    modelRow("Brand") = value.Code
                End If
            Next
        End Sub

        Public Sub Delete(code As String) Implements IBrandRepository.Delete
            Dim row As DataRow = FindRow(code)
            If row Is Nothing Then
                Throw New InvalidOperationException("The selected brand no longer exists.")
            End If

            BrandsTable().Rows.Remove(row)
            SynchronizeBrandCounter()
        End Sub

        Private Function Document() As DataSet
            Dim value As DataSet = _documentProvider()
            If value Is Nothing Then
                Throw New InvalidOperationException("No active catalogue document is available.")
            End If
            Return value
        End Function

        Private Function BrandsTable() As DataTable
            Return RequireTable("Brands")
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

        Private Function FindRow(code As String) As DataRow
            For Each row As DataRow In BrandsTable().Rows
                If row.RowState <> DataRowState.Deleted AndAlso
                        String.Equals(CStr(row("Code")), code, StringComparison.OrdinalIgnoreCase) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Shared Function Map(row As DataRow) As Brand
            Dim addedAt As DateTime = DateTime.MinValue
            If Not row.IsNull("Date") Then
                addedAt = Convert.ToDateTime(row("Date"))
            End If

            Return New Brand(
                Convert.ToString(row("Brand")),
                Convert.ToString(row("Code")),
                addedAt,
                Convert.ToString(row("Notes")))
        End Function

        Private Sub SynchronizeBrandCounter()
            Dim counters As DataTable = RequireTable("Counters")
            Dim counter As DataRow = counters.Rows.Find("Brands")
            If counter IsNot Nothing Then
                counter("Number") = BrandsTable().Rows.Count
            End If
        End Sub

    End Class

End Namespace
