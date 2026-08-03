Namespace Tapes

    Public NotInheritable Class TapeSide

        Public Sub New(
                isRecorded As Boolean,
                name As String,
                recordedAt As DateTime,
                deckName As String,
                inputName As String,
                peakLevel As Integer,
                noiseReduction As String,
                hx As Boolean,
                mpx As Boolean,
                dubbed As Boolean,
                speed As String,
                bias As Integer,
                biasCalibration As Integer,
                equalization As String,
                level As Decimal,
                levelCalibration As Decimal,
                contents As String,
                artist As String,
                title As String)

            Me.IsRecorded = isRecorded
            Me.Name = name
            Me.RecordedAt = recordedAt
            Me.DeckName = deckName
            Me.InputName = inputName
            Me.PeakLevel = peakLevel
            Me.NoiseReduction = noiseReduction
            Me.Hx = hx
            Me.Mpx = mpx
            Me.Dubbed = dubbed
            Me.Speed = speed
            Me.Bias = bias
            Me.BiasCalibration = biasCalibration
            Me.Equalization = equalization
            Me.Level = level
            Me.LevelCalibration = levelCalibration
            Me.Contents = contents
            Me.Artist = artist
            Me.Title = title
        End Sub

        Public ReadOnly Property IsRecorded As Boolean
        Public ReadOnly Property Name As String
        Public ReadOnly Property RecordedAt As DateTime
        Public ReadOnly Property DeckName As String
        Public ReadOnly Property InputName As String
        Public ReadOnly Property PeakLevel As Integer
        Public ReadOnly Property NoiseReduction As String
        Public ReadOnly Property Hx As Boolean
        Public ReadOnly Property Mpx As Boolean
        Public ReadOnly Property Dubbed As Boolean
        Public ReadOnly Property Speed As String
        Public ReadOnly Property Bias As Integer
        Public ReadOnly Property BiasCalibration As Integer
        Public ReadOnly Property Equalization As String
        Public ReadOnly Property Level As Decimal
        Public ReadOnly Property LevelCalibration As Decimal
        Public ReadOnly Property Contents As String
        Public ReadOnly Property Artist As String
        Public ReadOnly Property Title As String

        Public Shared Function Empty() As TapeSide
            Return New TapeSide(
                False,
                String.Empty,
                DateTime.MinValue,
                String.Empty,
                String.Empty,
                0,
                String.Empty,
                False,
                False,
                False,
                String.Empty,
                0,
                0,
                String.Empty,
                Decimal.Zero,
                Decimal.Zero,
                String.Empty,
                String.Empty,
                String.Empty)
        End Function

    End Class

End Namespace
