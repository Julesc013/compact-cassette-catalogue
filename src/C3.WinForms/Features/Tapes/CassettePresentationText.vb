Friend Module CassettePresentationText

    Public Function ConditionValue(displayIndex As Integer) As Integer
        Dim values = New Dictionary(Of Integer, Integer) From {
            {0, 8}, {1, 7}, {2, 6}, {3, 5}, {4, 4}, {5, 3}, {6, 2}, {7, 1}, {8, 0}
        }
        If Not values.ContainsKey(displayIndex) Then
            Return -1
        End If
        Return values(displayIndex)
    End Function

    Public Function ConditionLabel(value As Integer) As String
        Dim values = New Dictionary(Of Integer, String) From {
            {0, "Broken"}, {1, "Poor"}, {2, "Fair"}, {3, "Good"}, {4, "Good Plus"},
            {5, "Very Good"}, {6, "Very Good Plus"}, {7, "Near Mint"}, {8, "Mint"}
        }
        Return values(value)
    End Function

    Public Function TypeLabel(value As Integer, includeName As Boolean) As String
        Dim numerals = New Dictionary(Of Integer, String) From {
            {1, "I"}, {2, "II"}, {3, "III"}, {4, "IV"}
        }
        If Not includeName Then
            Return numerals(value)
        End If

        Dim names = New Dictionary(Of Integer, String) From {
            {1, "Ferric"}, {2, "Chrome"}, {3, "Ferrichrome"}, {4, "Metal"}
        }
        Return numerals(value) & " - " & names(value)
    End Function

End Module
