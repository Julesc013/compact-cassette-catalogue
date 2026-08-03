Imports C3.Catalogue.Catalogues
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Xml

Namespace CatalogueFiles.Xml.V1_1

    Public NotInheritable Class LegacyXmlCatalogueStore

        Private Const MaximumCatalogueBytes As Long = 64L * 1024L * 1024L

        Public Function Load(
                path As String,
                schema As DataSet,
                supportedVersions As IEnumerable(Of String)) As LegacyCatalogueLoadResult

            If String.IsNullOrWhiteSpace(path) OrElse Not File.Exists(path) Then
                Return LegacyCatalogueLoadResult.Failed(
                    LegacyCatalogueFileFailure.FileNotFound,
                    "The selected catalogue file does not exist.")
            End If
            If schema Is Nothing OrElse schema.Tables.Count = 0 Then
                Return LegacyCatalogueLoadResult.Failed(
                    LegacyCatalogueFileFailure.InvalidStructure,
                    "A catalogue schema is required before loading.")
            End If

            Try
                Dim fileInfo As New FileInfo(path)
                If fileInfo.Length > MaximumCatalogueBytes Then
                    Return LegacyCatalogueLoadResult.Failed(
                        LegacyCatalogueFileFailure.FileTooLarge,
                        "The catalogue exceeds the 64 MiB safety limit.")
                End If

                Dim document As XmlDocument = LoadSecureDocument(path)
                Dim structureFailure As String = ValidateStructure(document, schema)
                If structureFailure IsNot Nothing Then
                    Return LegacyCatalogueLoadResult.Failed(
                        LegacyCatalogueFileFailure.InvalidStructure,
                        structureFailure)
                End If

                Dim fileVersion As String = ReadFileVersion(document)
                If String.IsNullOrWhiteSpace(fileVersion) Then
                    Return LegacyCatalogueLoadResult.Failed(
                        LegacyCatalogueFileFailure.MissingVersion,
                        "The catalogue does not contain a file-format version.")
                End If
                If Not supportedVersions.Contains(fileVersion, StringComparer.Ordinal) Then
                    Return LegacyCatalogueLoadResult.Failed(
                        LegacyCatalogueFileFailure.UnsupportedVersion,
                        "Catalogue format " & fileVersion & " is not supported by this version of C3.")
                End If

                Dim loaded As DataSet = schema.Clone()
                loaded.DataSetName = schema.DataSetName
                loaded.EnforceConstraints = False
                Using reader As XmlReader = New XmlNodeReader(document)
                    loaded.ReadXml(reader, XmlReadMode.IgnoreSchema)
                End Using
                NormalizeCounters(loaded)
                loaded.EnforceConstraints = True

                Return LegacyCatalogueLoadResult.Success(
                    loaded,
                    CalculateRevision(path),
                    fileVersion)
            Catch ex As XmlException
                Return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidXml, ex.Message)
            Catch ex As ConstraintException
                Return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.ConstraintViolation, ex.Message)
            Catch ex As UnauthorizedAccessException
                Return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.AccessDenied, ex.Message)
            Catch ex As IOException
                Return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.IoFailure, ex.Message)
            Catch ex As Exception
                Return LegacyCatalogueLoadResult.Failed(LegacyCatalogueFileFailure.InvalidStructure, ex.Message)
            End Try
        End Function

        Public Function Save(
                path As String,
                document As DataSet,
                expectedRevision As CatalogueRevision,
                supportedVersions As IEnumerable(Of String)) As LegacyCatalogueSaveResult

            If String.IsNullOrWhiteSpace(path) Then
                Return LegacyCatalogueSaveResult.Failed(
                    LegacyCatalogueFileFailure.IoFailure,
                    "A destination path is required.")
            End If
            If document Is Nothing OrElse document.Tables.Count = 0 Then
                Return LegacyCatalogueSaveResult.Failed(
                    LegacyCatalogueFileFailure.InvalidStructure,
                    "There is no catalogue document to save.")
            End If

            Dim fullPath As String = System.IO.Path.GetFullPath(path)
            Dim directoryPath As String = System.IO.Path.GetDirectoryName(fullPath)
            If String.IsNullOrWhiteSpace(directoryPath) OrElse Not Directory.Exists(directoryPath) Then
                Return LegacyCatalogueSaveResult.Failed(
                    LegacyCatalogueFileFailure.IoFailure,
                    "The destination directory does not exist.")
            End If

            Dim temporaryPath As String = System.IO.Path.Combine(
                directoryPath,
                "." & System.IO.Path.GetFileName(fullPath) & "." & Guid.NewGuid().ToString("N") & ".tmp")
            Dim backupPath As String = fullPath & ".bak"

            Try
                If expectedRevision IsNot Nothing Then
                    If Not File.Exists(fullPath) OrElse Not expectedRevision.Equals(CalculateRevision(fullPath)) Then
                        Return LegacyCatalogueSaveResult.Failed(
                            LegacyCatalogueFileFailure.ExternalModification,
                            "The catalogue changed on disk after it was opened. Save As a new file or reopen it before overwriting.")
                    End If
                End If

                Dim snapshot As DataSet = document.Copy()
                NormalizeCounters(snapshot)

                Using stream As New FileStream(
                        temporaryPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None)
                    snapshot.WriteXml(stream, XmlWriteMode.IgnoreSchema)
                    stream.Flush()
                End Using

                Dim verification As LegacyCatalogueLoadResult = Load(
                    temporaryPath,
                    snapshot.Clone(),
                    supportedVersions)
                If Not verification.IsSuccess OrElse Not AreEquivalent(snapshot, verification.Document) Then
                    Dim details As String = If(
                        verification.IsSuccess,
                        "The saved snapshot did not round-trip without changes.",
                        verification.Message)
                    Return LegacyCatalogueSaveResult.Failed(
                        LegacyCatalogueFileFailure.VerificationFailure,
                        "C3 verified the temporary output before replacement and rejected it. " & details)
                End If

                If File.Exists(fullPath) Then
                    File.Replace(temporaryPath, fullPath, backupPath, True)
                Else
                    File.Move(temporaryPath, fullPath)
                    backupPath = Nothing
                End If

                Return LegacyCatalogueSaveResult.Success(CalculateRevision(fullPath), backupPath)
            Catch ex As UnauthorizedAccessException
                Return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.AccessDenied, ex.Message)
            Catch ex As IOException
                Return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.IoFailure, ex.Message)
            Catch ex As Exception
                Return LegacyCatalogueSaveResult.Failed(LegacyCatalogueFileFailure.VerificationFailure, ex.Message)
            Finally
                Try
                    If File.Exists(temporaryPath) Then
                        File.Delete(temporaryPath)
                    End If
                Catch
                End Try
            End Try
        End Function

        Private Shared Function LoadSecureDocument(path As String) As XmlDocument
            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Prohibit
            settings.XmlResolver = Nothing
            settings.MaxCharactersInDocument = MaximumCatalogueBytes
            settings.MaxCharactersFromEntities = 0L

            Dim document As New XmlDocument()
            document.XmlResolver = Nothing
            Using reader As XmlReader = XmlReader.Create(path, settings)
                document.Load(reader)
            End Using
            Return document
        End Function

        Private Shared Function ValidateStructure(document As XmlDocument, schema As DataSet) As String
            If document.DocumentElement Is Nothing OrElse
                    Not String.Equals(document.DocumentElement.Name, "Catalogue", StringComparison.Ordinal) OrElse
                    Not String.IsNullOrEmpty(document.DocumentElement.NamespaceURI) Then
                Return "The document root must be an unqualified Catalogue element."
            End If

            For Each rowNode As XmlNode In document.DocumentElement.ChildNodes
                If rowNode.NodeType <> XmlNodeType.Element Then
                    Continue For
                End If

                If Not String.IsNullOrEmpty(rowNode.NamespaceURI) Then
                    Return "Catalogue row '" & rowNode.Name & "' must be unqualified."
                End If

                Dim table As DataTable = schema.Tables(rowNode.Name)
                If table Is Nothing Then
                    Return "Unknown catalogue table '" & rowNode.Name & "'."
                End If

                For Each fieldNode As XmlNode In rowNode.ChildNodes
                    If fieldNode.NodeType <> XmlNodeType.Element Then
                        Continue For
                    End If
                    If Not String.IsNullOrEmpty(fieldNode.NamespaceURI) OrElse
                            table.Columns(fieldNode.Name) Is Nothing Then
                        Return "Unknown field '" & fieldNode.Name & "' in table '" & rowNode.Name & "'."
                    End If
                    For Each contentNode As XmlNode In fieldNode.ChildNodes
                        If contentNode.NodeType = XmlNodeType.Element Then
                            Return "Field '" & fieldNode.Name & "' in table '" &
                                rowNode.Name & "' must contain a scalar value."
                        End If
                    Next
                Next
            Next

            Return Nothing
        End Function

        Private Shared Function ReadFileVersion(document As XmlDocument) As String
            Dim node As XmlNode = document.SelectSingleNode(
                "/Catalogue/Information[normalize-space(Information)='File Version']/Value")
            If node Is Nothing Then
                Return Nothing
            End If

            Dim value As String = node.InnerText.Trim()
            Dim match As Text.RegularExpressions.Match = Text.RegularExpressions.Regex.Match(
                value,
                "^(\d+)\.(\d+)\.(\d+)")
            If match.Success Then
                Return match.Groups(1).Value & "." & match.Groups(2).Value & "." & match.Groups(3).Value
            End If
            Return value
        End Function

        Private Shared Sub NormalizeCounters(document As DataSet)
            Dim counters As DataTable = document.Tables("Counters")
            If counters Is Nothing Then
                Return
            End If

            SetCounter(counters, "Decks", RowCount(document, "Decks"))
            SetCounter(counters, "Brands", RowCount(document, "Brands"))
            SetCounter(counters, "Models", RowCount(document, "Models"))
            SetCounter(counters, "Tapes", RowCount(document, "Tapes"))
        End Sub

        Private Shared Function RowCount(document As DataSet, tableName As String) As Integer
            Dim table As DataTable = document.Tables(tableName)
            Return If(table Is Nothing, 0, table.Rows.Count)
        End Function

        Private Shared Sub SetCounter(table As DataTable, name As String, value As Integer)
            Dim row As DataRow = Nothing
            If table.PrimaryKey.Length > 0 Then
                row = table.Rows.Find(name)
            Else
                For Each candidate As DataRow In table.Rows
                    If String.Equals(CStr(candidate("Counter")), name, StringComparison.Ordinal) Then
                        row = candidate
                        Exit For
                    End If
                Next
            End If
            If row Is Nothing Then
                row = table.NewRow()
                row("Counter") = name
                table.Rows.Add(row)
            End If
            row("Number") = value
        End Sub

        Private Shared Function CalculateRevision(path As String) As CatalogueRevision
            Using stream As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using hashAlgorithm As SHA256 = SHA256.Create()
                    Dim hash As Byte() = hashAlgorithm.ComputeHash(stream)
                    Dim token As New Text.StringBuilder(hash.Length * 2)
                    For Each value As Byte In hash
                        token.Append(value.ToString("x2", CultureInfo.InvariantCulture))
                    Next
                    Return New CatalogueRevision(token.ToString())
                End Using
            End Using
        End Function

        Private Shared Function AreEquivalent(expected As DataSet, actual As DataSet) As Boolean
            If expected.Tables.Count <> actual.Tables.Count Then
                Return False
            End If

            For Each expectedTable As DataTable In expected.Tables
                Dim actualTable As DataTable = actual.Tables(expectedTable.TableName)
                If actualTable Is Nothing OrElse
                        expectedTable.Columns.Count <> actualTable.Columns.Count OrElse
                        expectedTable.Rows.Count <> actualTable.Rows.Count Then
                    Return False
                End If

                For rowIndex As Integer = 0 To expectedTable.Rows.Count - 1
                    For Each column As DataColumn In expectedTable.Columns
                        If Not Object.Equals(
                                expectedTable.Rows(rowIndex)(column.ColumnName),
                                actualTable.Rows(rowIndex)(column.ColumnName)) Then
                            Return False
                        End If
                    Next
                Next
            Next

            Return True
        End Function

    End Class

End Namespace
