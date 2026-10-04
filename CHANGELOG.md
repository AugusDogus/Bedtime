# Changelog

## 1.0.0

- Announce the current bed count in compact top-left notifications to all players, including vanilla clients.
- List awake players alphabetically below the count, with the count positioned below the hotbar.
- Allow server admins to disable the list with `Announcements.ShowAwakePlayers`.
- Update the count when players leave bed, join, or disconnect.
- Update the list when awake players change, even if the count stays the same.
- Preserve vanilla sleep requirements. No sitting votes or partial-player sleep.
