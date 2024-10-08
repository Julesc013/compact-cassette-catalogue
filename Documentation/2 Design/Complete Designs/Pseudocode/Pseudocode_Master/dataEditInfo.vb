'Store new data entered to existing employee's record
Private Sub when edit info button is clicked

    'get data from user
    new phone number = inputbox result
    while new phone number fails validation
        get new phone number again
    end while

    new email address = inputbox result
    while new email address fails validation
        get new email address again
    end while

    new can close cafe = inputbox result
    while new can close cafe fails validation
        get new can close cafe again
    end while

    get new colour code = inputbox result
    while new colour code fails validation
        get new colour code again
    end while


    overwrite all data to internal record structure
    overwrite all to external sheet
    overwrite all to list box
    'except: no listbox for colour codes

End Sub