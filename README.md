<p align="center">
  <img src="package/banner.png" alt="Bedtime: sleep announcements for Valheim" width="900">
</p>

A dedicated-server Valheim mod that shows how many players are in bed. Players can use vanilla clients, including consoles.

![Top-left notification showing the bed count and Not Sleeping list.](package/screenshots/not-sleeping.webp)

Valheim still decides when everyone can sleep. Sitting does not count, there is
no voting, and the mod never starts sleep or skips time.

## Behavior

- Shows `X of Y players asleep` in a top-left notification, with an optional
  alphabetical list of players not in bed.
- Refreshes the notification every two seconds while someone is waiting in bed,
  including the latest count and awake-player list.
- Shows `Everyone went to sleep. Sweet dreams!` once when everyone is in bed.
- Stays silent with only one active player, when nobody is in bed, and during sleep.
- Uses Valheim's existing sleep-update pass, currently every two seconds.
  Bedtime adds no timer or per-frame update loop.
- Counts the same active player characters as vanilla's sleep check. Characters
  still loading or respawning may not be counted until the game registers them.
- Uses the normal notification queue and fade. Other game messages can interrupt
  the display, and repeated sleep messages also appear in the game's message log.
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

## Configuration

After the first start, edit `BepInEx/config/augusdogus.mods.Bedtime.cfg` on the server:

```ini
[Announcements]
ShowAwakePlayers = true
```

Set `ShowAwakePlayers = false` to hide the `Not Sleeping:` heading and player list.
Only the count remains, for example `1 of 3 players asleep`.
The list is enabled by default. Restart the server after changing the setting.

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
