Friend Module CatalogueUiCoordinator

    Public Sub CompleteCatalogueMutation(source As Form)
        My.Application.Composition.Workspace.RecordUntrackedMutation()
        Dim mainWindow As frmMain = FindMainWindow(source)
        If mainWindow IsNot Nothing Then
            mainWindow.RefreshAfterCatalogueMutation()
        End If
    End Sub

    Public Function FindMainWindow(source As Form) As frmMain
        Dim current As Form = source
        While current IsNot Nothing
            Dim mainWindow As frmMain = TryCast(current, frmMain)
            If mainWindow IsNot Nothing Then
                Return mainWindow
            End If
            current = current.Owner
        End While

        For Each window As Form In Application.OpenForms
            Dim mainWindow As frmMain = TryCast(window, frmMain)
            If mainWindow IsNot Nothing Then
                Return mainWindow
            End If
        Next
        Return Nothing
    End Function

End Module
