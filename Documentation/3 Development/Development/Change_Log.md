# Blue Dog Café Rostering System Change Log

## Releases



### To Do 

- Fix bug where cancelling pin creation can allow logon.
- Remove unused code.
- Set email SMTP server according to Rachele's requirements (SMTP server throws error often).
- Cannot save new shifts or delete shifts.



- Allow user to cancel changing PIN or adding Data/Shifts.
- Quick information (empty shift combination boxes, shifts not updating).
- Add message to availability/shifts that the user can enter "Close".
- When adding shifts "12:00 am' should autocorrect to "close" in all cases and "0" should not translate to "12:00 am".
- Data records should restore when pressing cancel (make button visible again).
- Address lingering compiler warnings.



### Version 1.5 (August 6)

- Moved button strip to the top of the main form.
- Replaced time-entry text-boxes with drop-down lists.
- When trying to enter search-term into result text-box, a guiding prompt is issued.
- Changed button and object labels.
- About Information can be accessed by clicking version information on log-on screen.



### Version 1.4 (July 27)

- Fixed bug in loading and saving employee availability.



### Version 1.3 (July 21 – 23)

- Cannot add availability if the finish time is earlier than the start time.
- Fixed bug in which Availability start time is written to the finish time cells of the Excel spreadsheet (start and finish became the same time).
- Fixed bug in which Search button is hidden instead of Send button (for Chefs/Managers).
- Fixed bug in which calendar dates were returned with one week added (made weeks 1-indexed).
- Fixed bug in which the Remove Shift button remains enabled without a shift-block selected.
- Fixed bug in which shifts can be added even if their finish time is earlier than their start time.



### Version 1.2 (July 17)

- Fixed bug where new rosters fail to overwrite old ones.
- Fixed bug caused by not deleting and recreating suggested roster file.
- Fixed bug where overwriting a roster would not clear the old data.
- Cannot add a shift if the finish time is earlier than the start time.
- Fixed bug where chefs/managers could not log-on.
- Changed minor aesthetic details.
- Added more comprehensive code comments.



### Version 1.1 (July 17)

- Added Regular Expression validation to all complex user inputs.
- Owner email address and password now stored in settings (no longer constants).
- Added check to see if staff can be put on a closing shift.
- Updated data entry procedures (PINs, employee data, availability, and shifts).



### Version 1.0 (July 17)

- Excel data workbooks (employee data and availability) are now encrypted.
- Restored start-up form to log-on screen.
- Made program a single instance application.
- Added location of settings file to about program information.



## Betas



### Version 1.15 (July 16)

- Availability can now be changed and stored.
- Data records (all three) are cloned prior to editing so they can be restored when pressing cancel (not working).
- Shift-block combination boxes are now disabled upon exiting the data editing form.
- Time formatting functions are now case insensitive to "Close".



### Version 1.14 (July 16)

- Added colour code column to data editing form.
- Data form now performs basic functions.
- Can now add employee and data.
- Can now remove employee and data.
- Can now change data associated with existing employee.
- Can now save availability to file (but cannot change availability yet).
- Added cancel buttons to employee data and shift editing forms.
- Fixed combination box duplicate population bug.
- Fixed bug caused by not resizing availability after each addition/removal of an employee.
- Fixed bug where phone numbers are saved as integers by formatting all cells as 'text'.
- Forced all child forms to appear in top-left to better show input boxes beneath.
- Various other bug fixes.
- Made a few bugs too, rosters won't convert to PDF and still won't send.



### Version 1.13 (July 16)

- Shifts can now be added.
- Shifts can now be removed.
- Shifts are now saved to file upon accepting changes.
- Added progress bar to shift saving procedure.
- Changed all record structures to 'lists of T'.
- Updated all data loading procedures to handle 'list of T' behaviour.



### Version 1.12 (July 16)

- Search results can now be sorted.
- Made search results record structure public to class.
- Minor changes in displaying and saving results.
- Fixed more minor search bugs.



### Version 1.11 (July 16)

- Added progress bar to search procedure.
- Search results can now be saved to a text file.
- Fixed some minor search bugs.



### Version 1.10 (July 15)

- Fixed a lot of archive searching bugs (basically redid the whole thing).
- Separated search results into two list boxes.
- Some forms now reload when child forms have changed data that they use.
- Fixed bug in time formatting subroutines.



### Version 1.9 (July 15)

- Can now suggest rosters.
- Moved roster saving functionality to a subroutine of its own.



### Version 1.8 (July 15)

- Fixed a stupid amount of roster loading bugs (names and shifts not appearing).
- Added progress bar to roster loading.



### Version 1.7 (July 15)

- Suggested rosters can be loaded.
- Rosters sent within the week and the week prior can be loaded.



### Version 1.6 (July 14)

- Custom colour codes are now stored in the records.
- Can now change user PIN once logged on.
- Roster can now be saved to an Excel spreadsheet (including formatting).
- Roster can now be archived and sent via email.
- Added progress bar for user-friendliness.



### Version 1.5 (July 14)

- Ability to add corresponding shifts to selected staff members in roster.
- Restructured availability storage system (fixed loading bug).
- Lots of bug fixes.



### Version 1.4 (July 13)

- Added global structures to store user data.
- Added code to load data from file upon logon.
- Creates missing files upon first load.
- Added collections of all combination boxes.
- Restructured declarations of variables.



### Version 1.3 (July 12)

- Implemented Linear Search function (and associated methods).
- Modified roster naming and storage methods.
- Can now identify if a roster was sent within the week.



### Version 1.2 (June 1)

- Revised PIN creation prompt text.
- Fixed combo-box names in main form.
- Setup more global variables.
- Declared all variables in main form.
- Added code to show/hide user-specific objects/functions.
- Roster naming and referencing code.
- Added unused code for getting day index of the week.



### Version 1.1 (May 31)

- Added PIN creation code.
- Added logon code (PIN checking).
- Enter key mapped to Log-on button.



### Version 1.0 (May 29)

- Moved global variables and subroutines from forms to a module.
- Added internal settings for storage of user PINs.



### Version 0.6 (May 24)

- Added screen resolution check and warning.



### Version 0.1 – 0.5 (May 16 – May 22)

- Iteratively designed all forms.
- Added placeholder code for opening/closing forms.
- Added program and version information.

