Compact Cassette Catalogue 1.3.0 Alpha 5 test preview
======================================================

This is a retained, intentionally unpublished engineering preview for owner
acceptance testing. It is not Beta or stable and is not advertised through the
update feed.

Keep "Compact Cassette Catalogue.exe" and
"Compact Cassette Catalogue.exe.config" together in the same directory, then
run the executable. C3 itself does not need installation or administrator
rights for ordinary use.

Preview lanes and intended qualification targets:

* win-x86-net40: Windows XP SP3 x86, .NET Framework 4.0 Full.
* win-x64-net48: Windows 7 SP1 x64 or later, .NET Framework 4.8.
* win-arm64-net481: Windows 11 RTM/21H2 ARM64, separately installed
  .NET Framework 4.8.1.

These exact package bytes still require owner testing and the applicable target
rows before Beta promotion. An Alpha package does not by itself establish a
minimum-OS claim. Windows 7 is not currently supported by Microsoft. The ARM64
framework prerequisite may require administrator rights to install, but C3
remains portable.

This portable package contains no installer, uninstaller, updater, service,
scheduled task, self-contained runtime, or new application DLL. Optional
offline classic setup is distributed separately and consumes these exact bytes.

Catalogues are local XML files. Back them up like other documents. Automatic
update checking is disabled by default, and failed network checks must remain
nonfatal.

Known preview boundary: source regressions cover the application repairs and
setup journal, but owner acceptance, historical, and exact target-machine
qualification remain independent gates. Retention does not imply Beta status,
publication, or release approval.
