Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    ' Owns writes to the metadata rows in the version 1.1 XML compatibility model.
    ' Presentation code must not depend on the physical Information table layout.
    Public NotInheritable Class LegacyCatalogueMetadataWriter

        Private ReadOnly _documentProvider As Func(Of DataSet)

        Public Sub New(documentProvider As Func(Of DataSet))
            If documentProvider Is Nothing Then
                Throw New ArgumentNullException("documentProvider")
            End If
            _documentProvider = documentProvider
        End Sub

        Public Sub MarkModified(modifiedAt As DateTime)
            SetValue("File Modified", modifiedAt.ToString())
        End Sub

        Public Sub RefreshProductMetadata(
                productVersion As String,
                productStage As String,
                productDate As DateTime)

            SetValue("Program Version", productVersion)
            SetValue("Program Stage", productStage)
            SetValue("Program Date", productDate.ToString())
        End Sub

        Private Sub SetValue(name As String, value As String)
            Dim table As DataTable = InformationTable()
            Dim row As DataRow = table.Rows.Find(name)
            If row Is Nothing Then
                Throw New InvalidOperationException(
                    "Catalogue information row '" & name & "' is missing.")
            End If
            row("Value") = If(value, String.Empty)
        End Sub

        Private Function InformationTable() As DataTable
            Dim document As DataSet = _documentProvider()
            If document Is Nothing Then
                Throw New InvalidOperationException("No active catalogue document is available.")
            End If

            Dim table As DataTable = document.Tables("Information")
            If table Is Nothing Then
                Throw New InvalidOperationException("Catalogue table 'Information' is missing.")
            End If
            Return table
        End Function

    End Class

End Namespace
