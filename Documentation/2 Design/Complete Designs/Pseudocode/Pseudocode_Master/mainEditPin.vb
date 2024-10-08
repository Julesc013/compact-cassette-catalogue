'Change PIN (if can verify old PIN) and save new PIN to memory and file
Private Sub when edit PIN button is pressed
	
	pinTry = inputBox 'verify existing password
	pinReal = user PIN in spreadsheet

	If pinTry = pinReal Then

		pinNew = inputBox 'get new password
		pinRedo = inputBox 're-enter new password
		
		While pinNew <> pinRedo 'passwords dont match
		
			pinNew = inputBox 'get new password
			pinRedo = inputBox 're-enter new password
		
		End While

		If pinNew = pinRedo
			
			set new password (write it to sheet)
			show success message

		Else
			
			show failed message
			
		End If

	Else

		show incorrect message

	End If

End Sub