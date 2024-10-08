'sort all elements in list of results according to time shift started
'Author: Adrian Janson
'Source: ISBN 978-0-9750764-6-0
'Modifications: Sorts record structures, not plain arrays. Allows sorting entire structure by one specific field.
Private Sub quickSort
	'sort by one specific field

	If length of List > 1 then

        pivot = first element of the array
        start = first index of array
        end = last index of array

        while start <= end

            while array(start) < pivot
                increment start by 1
            end while

            while array(end) > pivot
                decrement end by 1
            end while

            if start <= end then

                swap array(start) with array(end)

                increment start by 1
                decrement end by 1
                
            end if

        end while

		quickSort(array from first index to End)
		quickSort(array from Start to last index)

End Sub