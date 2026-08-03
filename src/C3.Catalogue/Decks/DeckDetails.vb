Namespace Decks

    Public NotInheritable Class DeckDetails

        Public Sub New(
                manufacturer As String,
                model As String,
                year As Integer,
                condition As Integer,
                type1 As Boolean,
                type2 As Boolean,
                type3 As Boolean,
                type4 As Boolean,
                hx As Boolean,
                mpx As Boolean,
                dolbyB As Boolean,
                dolbyC As Boolean,
                dolbyS As Boolean,
                dbx1 As Boolean,
                dbx2 As Boolean,
                stereo As Boolean,
                programSearch As Boolean,
                reverse As Boolean,
                calibration As Boolean,
                azimuth As Boolean,
                dubbingSlow As Boolean,
                dubbingFast As Boolean,
                frequencyLow As Integer,
                frequencyHigh As Integer,
                signalRatio As Integer,
                signalRatioNoiseReduction As String,
                wowFlutter As Decimal,
                distortion As Decimal,
                heads As Integer,
                wells As Integer,
                speedSlow As Boolean,
                speedNormal As Boolean,
                speedFast As Boolean,
                notes As String)

            Me.Manufacturer = manufacturer
            Me.Model = model
            Me.Year = year
            Me.Condition = condition
            Me.Type1 = type1
            Me.Type2 = type2
            Me.Type3 = type3
            Me.Type4 = type4
            Me.Hx = hx
            Me.Mpx = mpx
            Me.DolbyB = dolbyB
            Me.DolbyC = dolbyC
            Me.DolbyS = dolbyS
            Me.Dbx1 = dbx1
            Me.Dbx2 = dbx2
            Me.Stereo = stereo
            Me.ProgramSearch = programSearch
            Me.Reverse = reverse
            Me.Calibration = calibration
            Me.Azimuth = azimuth
            Me.DubbingSlow = dubbingSlow
            Me.DubbingFast = dubbingFast
            Me.FrequencyLow = frequencyLow
            Me.FrequencyHigh = frequencyHigh
            Me.SignalRatio = signalRatio
            Me.SignalRatioNoiseReduction = signalRatioNoiseReduction
            Me.WowFlutter = wowFlutter
            Me.Distortion = distortion
            Me.Heads = heads
            Me.Wells = wells
            Me.SpeedSlow = speedSlow
            Me.SpeedNormal = speedNormal
            Me.SpeedFast = speedFast
            Me.Notes = notes
        End Sub

        Public ReadOnly Property Manufacturer As String
        Public ReadOnly Property Model As String
        Public ReadOnly Property Year As Integer
        Public ReadOnly Property Condition As Integer
        Public ReadOnly Property Type1 As Boolean
        Public ReadOnly Property Type2 As Boolean
        Public ReadOnly Property Type3 As Boolean
        Public ReadOnly Property Type4 As Boolean
        Public ReadOnly Property Hx As Boolean
        Public ReadOnly Property Mpx As Boolean
        Public ReadOnly Property DolbyB As Boolean
        Public ReadOnly Property DolbyC As Boolean
        Public ReadOnly Property DolbyS As Boolean
        Public ReadOnly Property Dbx1 As Boolean
        Public ReadOnly Property Dbx2 As Boolean
        Public ReadOnly Property Stereo As Boolean
        Public ReadOnly Property ProgramSearch As Boolean
        Public ReadOnly Property Reverse As Boolean
        Public ReadOnly Property Calibration As Boolean
        Public ReadOnly Property Azimuth As Boolean
        Public ReadOnly Property DubbingSlow As Boolean
        Public ReadOnly Property DubbingFast As Boolean
        Public ReadOnly Property FrequencyLow As Integer
        Public ReadOnly Property FrequencyHigh As Integer
        Public ReadOnly Property SignalRatio As Integer
        Public ReadOnly Property SignalRatioNoiseReduction As String
        Public ReadOnly Property WowFlutter As Decimal
        Public ReadOnly Property Distortion As Decimal
        Public ReadOnly Property Heads As Integer
        Public ReadOnly Property Wells As Integer
        Public ReadOnly Property SpeedSlow As Boolean
        Public ReadOnly Property SpeedNormal As Boolean
        Public ReadOnly Property SpeedFast As Boolean
        Public ReadOnly Property Notes As String

    End Class

End Namespace
