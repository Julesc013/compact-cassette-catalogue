# Compact Cassette Catalogue



## Releases



### Version 1.0 – 7 December 2019

- 
- *List views* replaced with *data grid views*.



## Betas



### Beta 0.6.2 – 7 December 2019

- Rebuilt UI to fix element alignment.
- Added console output header.
- Fixed tape updates detection bug.



### Beta 0.6.1 – 4 December 2019

- Can now write console output to a log file.
- Improved console *help* command.
- Added close and *kill* commands.
- Bug fixes.



### Beta 0.6 – 2 December 2019

- Can now update information of existing tapes.
- Can now delete existing tapes.
- File version checker now supports *'x.x.xbx'* version numbers.
- .NET Framework changed to version 4.6.
  - Added Windows Vista support.



### Beta 0.5.4 – 26 October 2019

- Fixed bug where program reads wrong combination box selections (specifically models).



### Beta 0.5.3 – 26 October 2019

- Cannot add new recording to existing tape if no decks have been added.
- When adding new recordings to existing tapes, default values are now loaded.
- Combination boxes are sorted alphabetically (brands and models only).



### Beta 0.5.2 – 25 October 2019

- If no recording on a side, the objects now display their defaults (not user definable).
- Bug fixes.



### Beta 0.5.1 – 25 October 2019

- File version updated to *1.1.0* (see below for changes).
- Modification date updated at time of save.



### Beta 0.5 – 25 October 2019

- Added ability to load saved catalogues (file format version *1.0.2* only).
- Optimised console output function (included date and time with logs).
- Loading now writes updated program data to the file.
- Loading files checks against list of supported file versions.
- Disabled enter-to-accept functionality where multi-line notes can be written (temporarily).



### Beta 0.4.3 – 19 October 2019

- Added application icon.
- Updated default values for new decks.
- Added tutorial and feedback buttons (does nothing).
- Added default filename to Save As dialog.
- Included copyright symbol.
- Modified assembly information.
- Form name changes.
- Other minor changes.



### Beta 0.4.2 – 19 October 2019

- Added keyboard shortcuts to menu.



### Beta 0.4.1 – 19 October 2019

- Title bar now displays file name and save status.



### Beta 0.4 – 18 October 2019

- Ability to add a new catalogue (restarts program).
- Added save functionality.
- Added save-as functionality.
- Partially added checks for unsaved changes to tapes.
- Added new settings to console.
- Added query functionality to settings command.
- Fixed help command formatting.
- Bug fixes.



### Beta 0.3 – 5 October 2019

- Date and time is now stored with each new item added.
- Added help command to console.
- Menu bar restructure.
- Set up code for save/load functionality.
- Other minor changes.
- Short identifier is now the primary key for tapes.



### Beta 0.2 – 4 October 2019

- Added rudimentary console to avoid too many confirmation dialogs.
- All fields reset when toggling a new tape's sides.
- New tape's number is now non-editable.
- Program now records version's release time of day.



### Beta 0.1 – 3 October 2019

- Big restructure — a crap tonne of reengineering.
- Moved new tapes to separate form.
- Major additions to functionality.
- Added Global Variables module.
- Added code to store entered data.
- Tape numbers are automatically assigned.
- Cannot change a tape's model/year/length/region once it has been added.
- Restructured data record format.
- Many bug fixes.



## Alphas

Note: Creation/design iterations displayed as alpha versions.

Eg: "Iteration 10" reported as "Alpha 10.0"



### Alpha 10 – 5 September 2019

- Added file open/close dialogs.
- Added input and peak level options when adding new tape.
- Minor changes.



### Alpha 9 – 2 September 2019

- Added add new model form.



### Alpha 8 – 2 September 2019

- Added add new brand form.
- Small changes.
- Added more dummy code.



### Alpha 7 – 2 September 2019

- Added add new deck form.



### Alpha 6 – 31 August 2019

- Added view decks form.
- Fixed tab indexes.



### Alpha 5 – 30 August 2019

- Added view statistics form.



### Alpha 4 – 30 August 2019

- Changed main form layout.
- Added tape decks.
- Added more options to tape data.



### Alpha 3 – 30 August 2019

- Created View Models and View Brands forms.



### Alpha 2 – 29 August 2019

- Created View Tapes form.
- Included basic form opening code.



### Alpha 1 – 22 August 2019

- Created main form.
- Basic version code and about information.





# Catalogue File Format



### Version 1.1 – 25 October 2019

- Data tables now have names.
- New date-times stored:
  - Creation date-time.
  - Modification/save date-time.
  - File format update date-time.



### Version 1.0.2 – 5 October 2019

- Removed indexes as identifiers.
- Short identifier is now primary key for tapes.
- Date and time stored with each new item added.



### Version 1.0.1 – 4 October 2019

- Included time of day in stored dates.



### Version 1.0 – 3 October 2019

- Stores decks, brands, models, and tapes.
