'Store new employee data entered into the system
Private Sub when Add Data button is clicked

    'get data from user

    new first name = inputbox result 'data that user entered
    while new first name fails validation
        get new first name again
    end while

    new last name = inputbox result
    while new last name fails validation
        get new last name again
    end while

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

    availabilityStored = "Missing" 'by default... availability has not yet been entered


    add all new data to internal record structure
    add all new records to external sheet
    add all new data to list boxes
    'except: no listbox for colour codes

    increment dataCount by 1

End Sub