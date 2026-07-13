# Photo Organizer

A Windows desktop app (WinUI 3 / .NET) for tidying up photos on disk: find photos across
multiple source folders, remove exact duplicates, and file everything into a single organized
archive — without ever deleting anything on its own.

> Status: early stage. This repository currently contains only the UI prototype under
> [`Layout/`](Layout/). The WinUI 3 application (`PhotoOrganizer`) has not been scaffolded yet.

## What it does

- **Scan** one or more user-selected **source** folders (JPEG, PNG, HEIC, TIFF, WebP, RAW).
- **Find exact duplicates** — byte-for-byte identical files, matched by content hash.
- **Organize** unique photos into a single **destination** archive, foldered by capture date
  (`Year/Year-Month/`), read from EXIF (falling back to the file date).
- **Never deletes automatically.** It only *marks* files for deletion; the actual removal happens
  only after the user reviews and confirms, and always goes to the Recycle Bin (with an undo log).

## Core model

- **Destination** — a single folder, treated as the reference archive.
- **Sources** — one or more folders to pull photos from.
- Per source file: if it already exists in the destination (same hash) it is marked for deletion;
  duplicates within sources are collapsed to one kept copy; unique files are copied into the
  destination and the original is marked for deletion only after the copy is verified.
- A **persistent SQLite index** stores file paths and hashes between runs, so a re-scan only
  hashes new or changed files.

## Interface

Explorer-style two-pane window:

- **Left** — two independent trees: the destination (pick one folder) on top, the sources
  (checkboxes, pick several) on the bottom, with a draggable splitter between them.
- **Right** — thumbnails of the folder selected by the last click, each showing its status
  (unique / duplicate / already in archive / marked for deletion).

See the interactive prototype in [`Layout/Photo Organizer (standalone).html`](Layout/) — open it
directly in any modern browser (it self-unpacks; no server needed).

## Tech stack

- .NET 10, WinUI 3 (Windows App SDK), packaged desktop app.
- MVVM via CommunityToolkit.Mvvm, dependency injection via Microsoft.Extensions.DependencyInjection.
- SQLite (`Microsoft.Data.Sqlite`) for the persistent index.

## Conventions

All product text — identifiers, comments, string literals, UI labels, logs, file names, error
messages, and commit messages — is **English only**.
