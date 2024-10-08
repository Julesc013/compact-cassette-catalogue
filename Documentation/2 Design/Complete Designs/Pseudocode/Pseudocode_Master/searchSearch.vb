'Search archived rosters for all shifts worked by specified employee. Also calculate basic statistics from these results
Private Sub when Search button clicked

	'search archive from this year back to MAXYEAR years in the past
	For each year to search

		reset local variables

		if MAXYEARs have not elapsed since 2019
			search from oldest existing archive
		end if

		if roster from year exists then

			for each week of year

				if roster from week exists then

					Open Excel Application
					Open roster Workbook
					Open primary Worksheet
					
					for each day of the week

						for each record in sheet

							if found name of employee

								copy information from spreadsheet archive to results array

								add resultant shift to listbox

								increment totalShifts by 1
								increment totalHours by length of shift
								if timeStart is earlier than earliestStart then
									earliestStart = timeStart
								end If
								if timeEnd is later than latestEnd then
									latestEnd = timeEnd
								end if

							end if

						end for

					end for
					
					close files

				end if

			end for

		end if

	end for

	populate every text-box with corresponding data

	enable sort and save results buttons

End Sub