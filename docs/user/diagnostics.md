# Diagnostics and crash reports

C3 keeps a bounded in-memory log of recent actions and diagnostic messages. The
buffer is not a permanent activity history and does not contain catalogue XML.

If an unexpected unhandled error terminates the application, C3 makes a
best-effort attempt to write a report below the current user's local application
data directory:

```text
%LOCALAPPDATA%\C3\CrashReports\C3-error-YYYYMMDD-HHMMSS-fff.log
```

The report contains the C3 version and build lane, operating-system and CLR
versions, process bitness, active catalogue path, last recorded action, exception
details, and the bounded recent log. It does not contain the catalogue contents.

Catalogue paths and exception messages can still reveal private information.
Review a report before attaching it to a public issue. Crash-report creation is
best effort: failure to create the directory or file must never replace or hide
the original application failure.

