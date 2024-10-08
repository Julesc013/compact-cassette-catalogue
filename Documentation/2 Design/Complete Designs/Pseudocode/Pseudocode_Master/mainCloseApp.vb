'Close the entire application after checking user wants to quit without sending
Private Sub when Close All button is pressed

    if rosterSent = false then
        
        ask user if they want to quit without sending

        if they do then

            Close SHIFTDATA file
            protect EMPLOYEEDATA workbook using MASTERKEY
            close EMPLOYEEDATA file

            close entire application

        end if

    else

        close entire application

    end if

End Sub