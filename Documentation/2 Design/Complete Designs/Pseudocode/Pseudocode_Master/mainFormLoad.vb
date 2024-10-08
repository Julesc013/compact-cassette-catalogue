'Load employee and shift data and enable/disable functions and UI elements depending on what user is logged-on
Private Sub when main form loads

	sendAbility = true
	rosterSent = false

	'show/hide neccessary elements
	if userGroup = 2

		hide function buttons that shouldnt be accessed by this user

		if roster not archived within this week then
			
			hide send roster button
			sendAbility = false
			
		end if

	else
		if ROSTERSUGGESTED exists then
			
			enable ability to load suggested roster

		end if

	end if

	add combo-boxes to 2D collection of objects

	if EMPLOYEEDATA exists then
			
		Open Excel Application
		Open EMPLOYEEDATA Workbook
		Open primary Worksheet
		
		Unprotect workbook using MASTERKEY

		'load in data from database
		for each record in file

			add data to internal record structure

		end for

		'leave file open for use in data-editing forms
		
		close files

	else

		Create Workbook
		Open primary Worksheet

	end if

	if SHIFTDATA exists then

		Open Excel Application
		Open SHIFTDATA Workbook
		Open primary Worksheet

		'load in data from database
		for each record in file

			add to shifts record structure

		end for
		
		'leave file open for use in Shift form

	end if

End Sub