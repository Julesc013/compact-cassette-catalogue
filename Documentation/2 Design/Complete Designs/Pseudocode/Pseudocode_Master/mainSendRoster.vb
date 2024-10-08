'if allowed, send roster to all employees via email, present summary of this process and offer to close entire app
Private Sub when send roster button is clicked

    if sendAbility = true then
        
        save roster to archive and save as PDF

        for each employee in database
            'send email to each
            'Author: M.V.Rajasekhar
            'Source: http://www.freevbcode.com/ShowCode.asp?ID=5486
            'Modifications: Removed some parameters and modified attachment line.
            
            With email
            
                set sent from = OWNEREMAIL
                set subject and message contents
                set send to = employee email
                attach PDF roster file to email

            End With

            If smtpServer exists Then
                send email via smtp
            end if

            call send email built-in function

        end for

        rosterSent = true

        Delete file ROSTERSUGGESTED

        display summary message box (with option to close program)

        if chosen to close then

            Close SHIFTDATA file
            protect EMPLOYEEDATA workbook using MASTERKEY
            close EMPLOYEEDATA file

            close entire application
            
        end if

    else
        show error message
    
    end if

end sub