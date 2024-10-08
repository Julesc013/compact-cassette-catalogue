'Check entered PIN is correct, then logon. If no PIN stored, create new one.
Private Sub when logon button pressed

	'figure out whose PIN to use
	set userGroup according to whose logged-on

	If PINDATA file does not exist then

		initialise Excel object

		If excel is installed

			Create Workbook
			Open primary Worksheet

			PinNew = input box 'ask for new PIN

			while not 4 charaters long and only numbers
				PinNew = input box 'ask again
			end while

			save PinNew to spreadsheet cell

			'leave PIN file open for changing PIN in Main form

			'logon now
			show main form
			close this form

		Else

		Display "excel not installed, Cannot log on" message
		
		End If

	else

		Open Excel application
		Open PINDATA Workbook
		Open primary Worksheet

		Unprotect workbook using MASTERKEY
		
		get PinReal from spreadsheet

		'leave PIN file open for changing PIN in Main form

		if PinReal = pin entered at logon then
			'logon
			show frmMain
			close this form
		else
			display PIN wrong message
		end if

	end If
	
End Sub