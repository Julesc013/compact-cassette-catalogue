Namespace Updates

    ''' <summary>
    ''' Represents the SemVer release identity used by C3 update manifests.
    ''' Build metadata is retained for display but never affects precedence.
    ''' </summary>
    Public NotInheritable Class SemanticVersion
        Implements IComparable(Of SemanticVersion)

        Public Const MaximumTextCharacters As Integer = 128

        Private ReadOnly _coreIdentifiers As String()
        Private ReadOnly _prereleaseIdentifiers As String()

        Private Sub New(
                originalText As String,
                releaseLabel As String,
                coreIdentifiers As String(),
                prereleaseIdentifiers As String())

            Me.OriginalText = originalText
            Me.ReleaseLabel = releaseLabel
            _coreIdentifiers = coreIdentifiers
            _prereleaseIdentifiers = prereleaseIdentifiers
        End Sub

        Public ReadOnly Property OriginalText As String

        ''' <summary>
        ''' Gets the canonical release label without optional build metadata.
        ''' </summary>
        Public ReadOnly Property ReleaseLabel As String

        Public ReadOnly Property CoreVersion As String
            Get
                Return String.Join(".", _coreIdentifiers)
            End Get
        End Property

        Public ReadOnly Property HasPrerelease As Boolean
            Get
                Return _prereleaseIdentifiers.Length > 0
            End Get
        End Property

        Public ReadOnly Property PrereleaseLabel As String
            Get
                Return String.Join(".", _prereleaseIdentifiers)
            End Get
        End Property

        Public ReadOnly Property FirstPrereleaseIdentifier As String
            Get
                If _prereleaseIdentifiers.Length = 0 Then
                    Return Nothing
                End If
                Return _prereleaseIdentifiers(0)
            End Get
        End Property

        Public Shared Function TryParse(
                value As String,
                ByRef parsed As SemanticVersion) As Boolean

            parsed = Nothing
            If value Is Nothing OrElse value.Length = 0 OrElse
                    value.Length > MaximumTextCharacters OrElse value <> value.Trim() Then
                Return False
            End If

            Dim plusIndex As Integer = value.IndexOf("+"c)
            If plusIndex >= 0 AndAlso value.IndexOf("+"c, plusIndex + 1) >= 0 Then
                Return False
            End If

            Dim releaseLabel As String = If(plusIndex >= 0, value.Substring(0, plusIndex), value)
            If plusIndex >= 0 Then
                Dim buildLabel As String = value.Substring(plusIndex + 1)
                If Not AreValidIdentifiers(buildLabel, False) Then
                    Return False
                End If
            End If

            Dim dashIndex As Integer = releaseLabel.IndexOf("-"c)
            Dim coreLabel As String = If(
                dashIndex >= 0,
                releaseLabel.Substring(0, dashIndex),
                releaseLabel)
            Dim prereleaseLabel As String = If(
                dashIndex >= 0,
                releaseLabel.Substring(dashIndex + 1),
                String.Empty)

            Dim coreIdentifiers As String() = coreLabel.Split("."c)
            If coreIdentifiers.Length <> 3 Then
                Return False
            End If
            For Each identifier As String In coreIdentifiers
                If Not IsCanonicalNumericIdentifier(identifier) Then
                    Return False
                End If
            Next

            Dim prereleaseIdentifiers As String() = New String() {}
            If dashIndex >= 0 Then
                If Not AreValidIdentifiers(prereleaseLabel, True) Then
                    Return False
                End If
                prereleaseIdentifiers = prereleaseLabel.Split("."c)
            End If

            parsed = New SemanticVersion(
                value,
                releaseLabel,
                coreIdentifiers,
                prereleaseIdentifiers)
            Return True
        End Function

        Public Function CompareTo(other As SemanticVersion) As Integer _
                Implements IComparable(Of SemanticVersion).CompareTo

            If other Is Nothing Then
                Return 1
            End If

            For index As Integer = 0 To _coreIdentifiers.Length - 1
                Dim comparison As Integer = CompareNumericIdentifiers(
                    _coreIdentifiers(index),
                    other._coreIdentifiers(index))
                If comparison <> 0 Then
                    Return comparison
                End If
            Next

            If Not HasPrerelease AndAlso Not other.HasPrerelease Then
                Return 0
            End If
            If Not HasPrerelease Then
                Return 1
            End If
            If Not other.HasPrerelease Then
                Return -1
            End If

            Dim sharedLength As Integer = Math.Min(
                _prereleaseIdentifiers.Length,
                other._prereleaseIdentifiers.Length)
            For index As Integer = 0 To sharedLength - 1
                Dim left As String = _prereleaseIdentifiers(index)
                Dim right As String = other._prereleaseIdentifiers(index)
                Dim leftIsNumeric As Boolean = IsNumericIdentifier(left)
                Dim rightIsNumeric As Boolean = IsNumericIdentifier(right)
                Dim comparison As Integer

                If leftIsNumeric AndAlso rightIsNumeric Then
                    comparison = CompareNumericIdentifiers(left, right)
                ElseIf leftIsNumeric Then
                    comparison = -1
                ElseIf rightIsNumeric Then
                    comparison = 1
                Else
                    comparison = String.CompareOrdinal(left, right)
                End If

                If comparison <> 0 Then
                    Return comparison
                End If
            Next

            Return _prereleaseIdentifiers.Length.CompareTo(
                other._prereleaseIdentifiers.Length)
        End Function

        Private Shared Function AreValidIdentifiers(
                value As String,
                enforceNumericCanonicalForm As Boolean) As Boolean

            If value.Length = 0 Then
                Return False
            End If

            Dim identifiers As String() = value.Split("."c)
            For Each identifier As String In identifiers
                If identifier.Length = 0 Then
                    Return False
                End If
                For Each character As Char In identifier
                    If Not IsAsciiLetterOrDigit(character) AndAlso character <> "-"c Then
                        Return False
                    End If
                Next
                If enforceNumericCanonicalForm AndAlso IsNumericIdentifier(identifier) AndAlso
                        Not IsCanonicalNumericIdentifier(identifier) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function IsCanonicalNumericIdentifier(value As String) As Boolean
            Return IsNumericIdentifier(value) AndAlso
                (value.Length = 1 OrElse value(0) <> "0"c)
        End Function

        Private Shared Function IsNumericIdentifier(value As String) As Boolean
            If value.Length = 0 Then
                Return False
            End If
            For Each character As Char In value
                If character < "0"c OrElse character > "9"c Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function IsAsciiLetterOrDigit(value As Char) As Boolean
            Return (value >= "0"c AndAlso value <= "9"c) OrElse
                (value >= "A"c AndAlso value <= "Z"c) OrElse
                (value >= "a"c AndAlso value <= "z"c)
        End Function

        Private Shared Function CompareNumericIdentifiers(
                left As String,
                right As String) As Integer

            Dim lengthComparison As Integer = left.Length.CompareTo(right.Length)
            If lengthComparison <> 0 Then
                Return lengthComparison
            End If
            Return String.CompareOrdinal(left, right)
        End Function

    End Class

End Namespace
