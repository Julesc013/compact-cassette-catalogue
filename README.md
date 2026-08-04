# Compact Cassette Catalogue

**Cataloguing application for home recorded compact cassette tapes.**

See [changes](CHANGELOG.md "C3 Changelog"), the [legacy maintenance workboard](TODO.md "C3 1.3 Workboard"), and the [wiki](https://github.com/Julesc013/compact-cassette-catalogue/wiki "C3 Wiki").

> **Repository status:** the available user release remains C3 1.2.0 Beta 1.
> `dev/1.x` contains the intentionally unpublished C3 1.3.0 Alpha 1 maintenance
> foundation. Alpha 1 has no binary release and does not yet contain the planned
> runtime repairs. See the [legacy maintenance documentation](docs/README.md).

![Screenshot of C3 in use.](https://github.com/Julesc013/compact-cassette-catalogue/blob/master/Resources/demonstration-screenshot.png)

## What is it?

*Compact Cassette Catalogue* (hereafter *C3*) allows you to build up a catalogue of your blank cassettes.
It is a **user friendly** and **feature rich** alternative to a database.

## Who is it for?

*C3* is intended for tapeheads who may be finding it **difficult to manage their large collection** of cassettes and need a system for **indexing, sorting and searching**.

I created *C3* for myself because:
1. I prefer *aesthetically minimal j-cards* which are often devoid of useful information...
Which Dolby did I use when I recorded this?
2. I find it handy to be able to open up a list view of all my blank tapes, filter for the brand, type and length I want, and easily *select the perfect tape* for my new mix.

Note: *C3* is not geared towards *pre-recorded* tapes ([Discogs](https://www.discogs.com/ "Discogs - Music Database and Marketplace") does that job exceptionally) though it is fully capable.

## Why is it better?

This program provides many helpful features such as:
- *Search functionality* to assist in navigating large catalogues.
- *List views* to browse catalogued items.
- *Data visualisations* to track changes to the catalogue.

Many pieces of information can be stored about each **tape**, such as:
- Manufacturer/Brand
- Model/Name
- Series year
- Length
- Type
- Region
- Condition

And much more information can be stored about the **recordings** on each side, including:
- Deck model
- Input type
- Peak level
- Noise reduction
- Speed
- Bias and EQ
- Contents

You can even **catalogue your tape decks** (including their specifications).

*Catalogues are stored locally as XML files for ease of use. Treat the catalogues like you would a Word document, for example.*

## How to get started.

Head over to the [releases page](https://github.com/Julesc013/compact-cassette-catalogue/releases "C3 Releases") and download the portable x86 build for the latest version.
(Look for *C3-vX.X.X-win-x86.exe* or *C3-vX.X.X-win-x86-portable.zip*.)

*Note: The x86/32-bit portable build is the compatibility build and **does not** require installing or administrator privileges.*

*Note: The x64 build is for 64-bit Windows only. Windows XP x64 support is unverified unless tested separately.*

*Note: C3 is offline-first. Catalogues are local XML files, update checking is optional, and automatic update checking is disabled by default.*

*Note: Online update checking may fail on some old Windows versions because GitHub HTTPS connections can require newer TLS support than the operating system provides. If that happens, open the releases page in a browser and download updates manually. See [this site](https://mohalogiciels.runasp.net/Tutorials/NetFrameworkTls.xht) for a potential quick fix.*

## Legacy maintenance status

C3 1.3.0 is the final planned release of the original VB.NET/WinForms program.
It is being repaired directly from `v1.2.0b1`; the later project-splitting
refactor is not its production ancestry.

- Alpha 1 freezes the recovery doctrine, compatibility evidence, genome gate,
  and four-lane build scaffolding. It is source-only and unpublished.
- Beta 1 will contain the complete bounded runtime repair set and qualification
  evidence.
- Stable will publish only already-qualified portable packages and will update
  the legacy feed after downloaded assets pass verification.

The existing DataSet catalogue format remains 1.1.0. No 1.3 milestone introduces
a new runtime dependency, production assembly, C# source, project split, or UI
redesign.

The **documentation (wiki)** and **quick-start tutorial** can be read [here](https://github.com/Julesc013/compact-cassette-catalogue/wiki "C3 Wiki");
this will help you get started and explains every function of the software.
If you need to review the documentation, you can open it from the *Help* menu or by pressing *F1*.

## System requirements.

#### Minimum
- Windows XP SP3 or newer (32-bit or 64-bit).
- .NET Framework 4.0
- 32MB of RAM.
- 2MB of hard drive space.
#### Recommended
- 128MB of RAM.
- 100MB of hard drive space.

**Copyright (c) 2019-2026 Jules Carboni**
