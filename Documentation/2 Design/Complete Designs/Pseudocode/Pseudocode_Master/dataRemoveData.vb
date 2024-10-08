'Remove employee from memory and file/database (and decrement employee count)
Private Sub when remove button is pressed

    Show message box asking for confirmation of deletion

    if messagebox result = yes then

        delete from record structure
        delete from spreadsheet
        remove from all list boxes

        decrement dataCount

    end if

End Sub