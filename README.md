<p align="center">
  <img src="package/banner.png" alt="Bedtime: sleep announcements for Valheim" width="900">
</p>

A dedicated-server Valheim mod that shows how many players are in bed. Players can use vanilla clients, including consoles.

![Top-left notification: 1 out of 3 players are in bed. Not Sleeping: Bob and Charlie.](package/screenshots/not-sleeping.webp)

*Staged local example with fictional player names, captured in Valheim's actual HUD.*

Valheim still decides when everyone can sleep. Sitting does not count, there is
no voting, and the mod never starts sleep or skips time.

<details>
<summary>Full in-game view and everyone-in-bed example</summary>

![Bedtime's multiline notification in the top-left corner of the game.](package/screenshots/in-game.webp)

When everyone is in bed, the list disappears:

![Top-left notification: 3 out of 3 players are in bed.](package/screenshots/all-in-bed.webp)

These are staged UI examples, not a live multiplayer session.
[Capture details](docs/SCREENSHOTS.md).

</details>

## Behavior

- Shows a top-left notification with the bed count and a bulleted list of players
  not in bed. Names appear in alphabetical order; the list disappears when everyone is in bed.
- Updates when the count or awake-player list changes, including bed swaps,
  joins, and disconnects.
- Uses Valheim's existing sleep-update pass, currently every two seconds.
  Bedtime adds no timer or per-frame update loop.
- Counts the same active player characters as vanilla's sleep check. Characters
  still loading or respawning may not be counted until the game registers them.
- Does not repeat unchanged announcements, and stays quiet during sleep.
  The list uses the normal notification fade time, so large groups may be hard to read.
- No client installation or configuration is required.

## Installation

1. Install BepInExPack_Valheim 5.4.2350 or newer on the dedicated server.
2. Extract `plugins/Bedtime.dll` from the release ZIP into
   `BepInEx/plugins/Bedtime/` on the server, then restart it.
3. Remove or disable JustSleep or any other mod that changes sleep requirements
   if you want vanilla sleeping. Bedtime does not undo other mods' gameplay changes.

For the community-valheim-tools container, place the DLL in the persistent
`/config/bepinex/plugins/Bedtime/` directory so it survives updates.

Installing Bedtime on a client has no effect. To uninstall, remove its DLL and
restart the server. It writes no world or player data.

## Build and verification

```sh
bun install --frozen-lockfile
dotnet build src/Bedtime/Bedtime.csproj -c Release -t:Package \
  -p:GameDir="/path/to/Valheim" \
  -p:BepInExDir="/path/to/profile/BepInEx"
MANAGED_DIR="/path/to/Valheim/valheim_Data/Managed" \
BEPINEX_DIR="/path/to/profile/BepInEx" bash scripts/check.sh
bun run typecheck
bun test tests/
```

Requires .NET SDK 8 and Bun 1.4.1+. Output: `artifacts/Bedtime-1.0.0.zip`.
Tests cover announcement transitions and the installed game's bed/HUD contracts.
See [manual multiplayer checks](docs/TESTING.md) for verification with real clients.

Built from [ValheimModTemplate](https://github.com/AugusDogus/ValheimModTemplate).
Inspired by the server announcements in
[ServersideQoL](https://github.com/ArgusMagnus/ValheimServersideQoL), with an
independent implementation and no dependency on its code or plugins.

[Development and releases](docs/DEVELOPMENT.md) · [Repository layout](docs/REPOSITORY.md)

## License

[MIT](LICENSE.md). Copyright (c) 2026 AugusDogus.
Banner font notices are retained in [FONT-LICENSE.txt](assets/artwork/FONT-LICENSE.txt).
