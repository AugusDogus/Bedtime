# README screenshots

Captured on October 5, 2026, using the installed Linux Valheim client and
Bedtime's persistent announcement formatter, with `X of Y players asleep`
and the final `Everyone went to sleep. Sweet dreams!` message.

The game ran on a hidden Gamescope display at 1920 × 1080. A Bubblewrap sandbox
hid the normal home directory, used disposable local character/world saves,
and blocked external network access. A local-only bridge allowed authentication
with the already-running Steam client. Cloud storage was disabled in the
temporary capture harness. The real server and player saves were not used.

A temporary plugin supplied fictional bed counts and names to the native
`MessageHud` top-left notification on the game's existing two-second sleep check.
The waiting-player screenshot was taken after five refreshes, with the normal
notification fade and no changes to its font, size, alignment, or layout. The
all-in-bed message was sent once and allowed to fade. No text was composited
onto the images afterward. The capture plugin is not part of Bedtime or its
release package.

- `package/screenshots/in-game.webp`: full in-game view, encoded as WebP.
- `package/screenshots/not-sleeping.webp`: native-resolution, lossless crop of
  the same frame, showing the count and awake-player list.
- `package/screenshots/all-in-bed.webp`: native-resolution, lossless crop of
  the staged all-in-bed notification.

These images still match the current top-left text and layout. Current announcements
are sent on status changes or `zzz` requests instead of periodic refreshes.
These images demonstrate appearance at this resolution and UI scale. They do
not establish multiplayer behavior, console compatibility, or actual sleep
transitions. Valheim's game artwork remains its respective owners' material.
