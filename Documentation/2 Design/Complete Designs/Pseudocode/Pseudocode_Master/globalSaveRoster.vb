'store data from roster editor into excel file for sending/suggesting
Private Sub saveRoster

	initialise Excel object

	If excel is installed

		Create Workbook
		Open primary Worksheet

		for each day of the week
			for each staff box in roster 'MAXSTAFF
				
				transfer data between combination boxes and file

			End For
		End For

		set font size = 14

		if this file is to be sent and archived

			'add header
			insert two new rows at the top
			add header to first row
			add days of the week to second rows

			find all names in cells and colour the text to match

			save file to archive folder in home directory

			if roster within same week exists then
				delete old copy
			end If

			export a PDF copy to home directory

		Else

			save excel file to home directory

		end if

		close files

	Else

		Display "excel not installed" message
		
	End If

End Sub