'Add new shift (from times specified by user) to internal list and stored file
Private Sub when add shift button is clicked

    get startTime from user with input box
    while startTime fails validation
        get startTime again
    end while

    get endTime with input box
    while endTime fails validation or is earlier than startTime
        get endTime again
    end while

    If endTime is past midnight then
        endTime = "close"
    end If

    add times to internal record structure
    add times to external sheet
    add times to list box

    increment shiftCount by 1

End Sub