### Short Term

- write Documentation (with a quick-start guide/tutorial)

- INCLUDE POPULAR BRANDS AND MODELS BY DEFAULT



#### v2.0


- make an uninstaller!

- UPDATER IS BUILD INTO MAIN PROGRAM
    - checks if C3.bak exists in current running directory (if so deletes it)
    - Renames self to C3.bak, downloads new version to self directory at C3.exe

- tool to upgrade/convert catalogue versions
- file associations

- SAVE FILES TO A USER FOLDER AND SELECT CATALOGUES FROM A LIST LIKE FALLOUT 4.
- save as just creates a new named catalogue in the hardcoded folder.
- Setting to anable "advanced file management" for manual saving of xml files as if they were word documents.

- Backup function just zips all the xml files and lets the user save the ".c3b" (C3 backup) zip wherever.



#### v2.1

- sort list view items
- save column reorderings for next load

- search stuff

- View statistics



#### v2.2

- Complete user settings. [already done?]
- Save directory and defaults to file (new format version). [just the build in my.settings?]



#### v2.3

- Grapher to visualise
  - recordings made during all time
  - total num tapes over time
  - new tapes added each month
  - total length over time
  - etc
 
 
 
#### v2.x

- add more filters for decks!
- add more filters for tapes (eg speed and such)

- only nr/bias/speed selectable if deck supports it?

- abiity to add many instances of one model of deck


- Ability to edit brand, model, and deck NAMES (and manufacterers). [currently cannot as they are used in primary keys or in second-hand stored values in tape records.] MAKE BRANDS AND MODELS STORED ONLY AS CODES.
- Give decks a hidden primary key/code so that they can be renamed. (E.G: Marantz-SD340-020619)



#### v3.0

- fullscreen spaces all the elements out
- fullscreen shows list of all tapes on side pane
- fullscreen shows console in corner

- REBUILD UI FOR RESIZING
- add resizablility
- add and remove fields from view forms
- comboboxes populated programatically

- ability to add an image of each tape

- Add message "THIS CODE CANNOT BE CHANGED" when making a brand/model code



### Long Term



- More verbose process logging (to an output file, not console)

- ability to save as and edit as older file versions



- fix regex validation
- fix console scrolling bug
- case sensitive searching
- can press enter while in notes textbox (while btnAdd is accept_button)



- Clean up code (commented out lines)

- make all updates and changes checkings into one funciton with a case statement at the end



- support for 1, 2 and 4+ point version numbers (eg 1.3 and 1.4.2.1)
