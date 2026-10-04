# README screenshots

Captured on October 4, 2026, using the installed Linux Valheim client and
Bedtime's announcement formatter from commit `3a4df25`, with no period after the
bed count and leading blank lines that keep it below the hotbar.

The game ran on a hidden Gamescope display at 1920 × 1080. A Bubblewrap sandbox
hid the normal home directory, used disposable local character/world saves,
and blocked external network access. A local-only bridge allowed authentication
with the already-running Steam client. Cloud storage was disabled in the
temporary capture harness. The real server and player saves were not used.

A temporary plugin supplied fictional bed counts and names to the native
`MessageHud` top-left notification. It held the notification visible for capture,
without changing its font, size, alignment, or layout. No text was composited
onto the images afterward. The capture plugin is not part of Bedtime or its
release package.

- `package/screenshots/in-game.webp`: full in-game view, encoded as WebP.
- `package/screenshots/not-sleeping.webp`: native-resolution, lossless crop of
  the same frame, showing the count and awake-player list.
- `package/screenshots/all-in-bed.webp`: native-resolution, lossless crop of
  the staged all-in-bed notification.

The count's first glyph started at approximately y=131 with both two and nine
awake players, matching the all-in-bed notification. Before the padding, the
two-player list started at y=95 and overlapped the hotbar. These positions were
measured from the native text mesh at the captured resolution and UI scale.

These images demonstrate appearance at this resolution and UI scale. They do
not establish multiplayer behavior, console compatibility, or actual sleep
transitions. Valheim's game artwork remains its respective owners' material.
