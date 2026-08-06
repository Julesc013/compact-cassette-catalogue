# v1.3.0a5 visual qualification set

This directory is reserved for immutable native screenshots accepted during
Alpha 5 qualification. It is not evidence that Alpha 5 has already passed.

Required capture families include:

- canonical Windows 7/96-DPI default and maximized views;
- 100%, 125%, 150%, and 200% real display scaling;
- XP-compatible font/theme rendering;
- High Contrast Black and White;
- 800 x 552 effective minimum work area;
- maximum representative text and validation content;
- keyboard focus/command reachability; and
- installer/uninstaller pages only when a conditional setup defect is changed.

Use names of the form:

```text
<lane>--<os-build>--<scale>--<form>--<state>.png
<same-stem>.json
```

The adjacent JSON records the exact package/source/lock identity and capture
environment. Do not commit screenshots until their package bytes and native
environment are known.

