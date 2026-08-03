Imports System.Collections.ObjectModel

Public NotInheritable Class BufferedLogger

    Private Const MaximumEntries As Integer = 200
    Private Shared ReadOnly SyncRoot As New Object()
    Private Shared ReadOnly Entries As New Queue(Of String)()
    Private Shared _lastAction As String = "Application initialization"

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property LastAction As String
        Get
            SyncLock SyncRoot
                Return _lastAction
            End SyncLock
        End Get
    End Property

    Public Shared Sub RecordAction(action As String)
        If String.IsNullOrWhiteSpace(action) Then
            Return
        End If

        SyncLock SyncRoot
            _lastAction = action.Trim()
        End SyncLock
        Information("Action: " & action.Trim())
    End Sub

    Public Shared Sub Information(message As String)
        Add("INFO", message)
    End Sub

    Public Shared Sub Warning(message As String)
        Add("WARN", message)
    End Sub

    Public Shared Sub [Error](message As String)
        Add("ERROR", message)
    End Sub

    Public Shared Function Tail() As ReadOnlyCollection(Of String)
        SyncLock SyncRoot
            Return New ReadOnlyCollection(Of String)(Entries.ToArray())
        End SyncLock
    End Function

    Private Shared Sub Add(level As String, message As String)
        Dim safeMessage As String = If(message, String.Empty)
        Dim entry As String = String.Format(
            Globalization.CultureInfo.InvariantCulture,
            "{0:O} [{1}] {2}",
            DateTime.UtcNow,
            level,
            safeMessage)

        SyncLock SyncRoot
            Entries.Enqueue(entry)
            While Entries.Count > MaximumEntries
                Entries.Dequeue()
            End While
        End SyncLock
    End Sub

End Class

