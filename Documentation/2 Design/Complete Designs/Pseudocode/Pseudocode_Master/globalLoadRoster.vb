'Load data from stored roster (excel) into roster editor window's combo-boxes
Private Sub loadRoster

	If fileName excel file exists

		Open Excel Application
		Open fileName Workbook
		Open primary Worksheet
		
		for each day of the week
			for each staff box in form 'six staff boxes
				
				load data into combination boxes
				
			End For
		End For
		
		close files
		
	Else

		Display "no roster to load" message
		
	End If

End Sub