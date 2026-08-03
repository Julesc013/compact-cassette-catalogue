Imports C3.Catalogue.Catalogues
Imports System.Data

Namespace CatalogueFiles.Xml.V1_1

    Public Enum LegacyCatalogueFileFailure
        None = 0
        FileNotFound
        FileTooLarge
        InvalidXml
        MissingVersion
        UnsupportedVersion
        InvalidStructure
        ConstraintViolation
        ExternalModification
        AccessDenied
        IoFailure
        VerificationFailure
    End Enum

    Public NotInheritable Class LegacyCatalogueLoadResult

        Private Sub New()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Document As DataSet
        Public Property Revision As CatalogueRevision
        Public Property FileVersion As String
        Public Property Failure As LegacyCatalogueFileFailure
        Public Property Message As String

        Public Shared Function Success(
                document As DataSet,
                revision As CatalogueRevision,
                fileVersion As String) As LegacyCatalogueLoadResult

            Return New LegacyCatalogueLoadResult() With {
                .IsSuccess = True,
                .Document = document,
                .Revision = revision,
                .FileVersion = fileVersion,
                .Failure = LegacyCatalogueFileFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(
                failure As LegacyCatalogueFileFailure,
                message As String) As LegacyCatalogueLoadResult

            Return New LegacyCatalogueLoadResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

    Public NotInheritable Class LegacyCatalogueSaveResult

        Private Sub New()
        End Sub

        Public Property IsSuccess As Boolean
        Public Property Revision As CatalogueRevision
        Public Property BackupPath As String
        Public Property Failure As LegacyCatalogueFileFailure
        Public Property Message As String

        Public Shared Function Success(
                revision As CatalogueRevision,
                backupPath As String) As LegacyCatalogueSaveResult

            Return New LegacyCatalogueSaveResult() With {
                .IsSuccess = True,
                .Revision = revision,
                .BackupPath = backupPath,
                .Failure = LegacyCatalogueFileFailure.None,
                .Message = String.Empty
            }
        End Function

        Public Shared Function Failed(
                failure As LegacyCatalogueFileFailure,
                message As String) As LegacyCatalogueSaveResult

            Return New LegacyCatalogueSaveResult() With {
                .IsSuccess = False,
                .Failure = failure,
                .Message = message
            }
        End Function

    End Class

End Namespace

