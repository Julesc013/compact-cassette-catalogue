'Save results of search (shifts and statistics) to a text file at path specified by user
Private Sub save search results button is clicked

	show dlgSaveResuts and get file path

    create/overwrite text file

    write header to file
    write employees name to file
    
    for each results

        write result to file

    end for

    write "Employee statistics:" header to file
    write employee statistics to file

    close text file

End Sub