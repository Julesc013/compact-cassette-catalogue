'Load in shift data upon form load (and count how many shift-blocks)
Private Sub when shifts form is loaded

	shiftCount = 0 'set to 0 each time form is loaded

	for each record in shifts structure
		add record to list box
		increment shiftCount
	end for

	'leave file open for use in the form

End Sub