'Save new availability internally and to file
Private Sub when ok button is clicked

    populate employeeData internal datastore from start/finish textboxes

    if validaton of every item succeeds then

        write all items to EMPLOYEEDATA sheet cells

        availabilityStored = "Submitted"

        close this form

    else
        show error message

    end if

End Sub