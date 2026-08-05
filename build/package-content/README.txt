Compact Cassette Catalogue 1.3.0 portable release
==================================================

Keep "Compact Cassette Catalogue.exe" and
"Compact Cassette Catalogue.exe.config" together in the same directory, then
run the executable. C3 itself does not need installation or administrator
rights for ordinary use.

Release lanes:

* win-x86-net40: Windows XP SP3 x86, .NET Framework 4.0 Full.
* win-x64-net48: Windows 7 SP1 x64 or later, .NET Framework 4.8.
* win-arm64-net481: Windows 11 RTM/21H2 ARM64, separately installed
  .NET Framework 4.8.1.

The x64 compatibility statement is a directly tested C3 claim, not a claim
that Microsoft currently supports Windows 7. The ARM64 prerequisite may require
administrator rights to install, but C3 remains portable.

This package contains no installer, uninstaller, updater, service, scheduled
task, self-contained runtime, or new application DLL.

Catalogues are local XML files. Back them up like other documents. Automatic
update checking is disabled by default, and failed network checks must remain
nonfatal.
