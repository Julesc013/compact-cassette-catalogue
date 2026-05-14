# To Do

## 1.2.x Maintenance

- Complete and record a clean Windows XP SP3 32-bit runtime test with .NET Framework 4.0 installed.
- Manually smoke-test the 1.2.0 portable x86 build on modern Windows after each release candidate.
- Verify list windows before release: tapes, models, brands, decks, filtering, selecting results, editing, and deletion.
- Confirm settings persistence across close/reopen, especially update-check policy and message display preference.
- Decide whether the old network installer remains archived only or gets a separate repair/test pass.
- Review update version comparison for pre-release, beta, and four-part version strings.
- Review all browser-opening paths after release packaging so failures remain non-crashing.
- Keep the wiki and quick-start documentation synchronized with the portable x86 release path.

## 1.3 Usability

- Add popular brands and models by default if first-use data behavior is intentionally changed.
- Improve search and filtering behavior.
- Add sorting for list-view items.
- Save column reorderings for list views.
- Add more tape filters, including speed and recording-related fields.
- Add more deck filters.
- Improve console logging and optional log-file output.
- Fix console scrolling behavior.
- Fix notes textbox Enter-key behavior where accept buttons interfere with multiline input.
- Improve validation, including regex validation and case-sensitive searching options.

## 2.0 Catalogue Management

- Add a catalogue version upgrade/conversion tool.
- Allow saving and editing as older catalogue file versions where practical.
- Add file associations if OS integration is safe on supported systems.
- Add optional managed catalogue folders for users who do not want manual XML file handling.
- Add an advanced file-management setting for users who prefer manual XML save/open behavior.
- Add a backup function that exports catalogue XML files into a C3 backup archive.
- Store brands and models by stable codes so names can be edited safely.
- Give decks stable hidden identifiers so deck names and manufacturers can be edited safely.
- Support adding multiple instances of the same deck model.

## 3.0 UI Rebuild

- Rebuild the UI for resizing.
- Add resizable and fullscreen layouts.
- Show tape lists in a side pane in fullscreen layouts.
- Show console output in a non-obtrusive area.
- Populate combo boxes programmatically.
- Allow users to add and remove fields from view forms.
- Add optional tape images.
- Clearly mark brand/model codes that cannot be changed after creation.

## Deferred Native Updating

- Do not make startup depend on network access.
- Do not add HTTP downgrade fallback.
- Revisit native update behavior only if it can be made offline-safe and old-Windows-safe.
