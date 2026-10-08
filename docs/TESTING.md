# Multiplayer checks

Use a test dedicated server with BepInEx and Bedtime, without another sleep mod.
Connect two players with vanilla clients. These checks require a running game;
the automated tests do not establish end-to-end console or crossplay behavior.

1. At night, have Alice enter a valid bed while Bob remains standing.
   Both clients should see `1 of 2 players asleep` in the top-left notification area.
   Under it, verify `Not Sleeping:` and a separate `• Bob` line are visible.
   The night must continue, with Alice waiting in bed.
2. Have Bob use the sit emote. The count must remain one and the night must continue.
3. Have Alice leave bed. The last notification should fade and no zero-sleeper message should appear.
4. Have Alice reenter bed. The count should appear again once. Leave the bed
   state unchanged for at least ten seconds and verify there are no repeats.
5. Have Bob enter another valid bed. The game should perform its normal sleep
   transition, with `Everyone went to sleep. Sweet dreams!` appearing once.
   The final announcement should have no `Not Sleeping:` list and should fade normally.
6. With one of two players waiting in bed, connect a third player. Once their
   character is active, the count should change to `1 of 3 players asleep`.
   Disconnect the third player and verify it returns to `1 of 2 players asleep`.
7. Disconnect the waiting sleeper while another player remains awake. The
   notifications should stop. Have the remaining player enter bed alone and
   verify that neither the count nor the final message appears.
8. Restart the server and verify announcements resume. Remove Bedtime and restart
   to verify that ordinary sleeping still works and no missing-mod prompt appears.

While someone waits in bed, have an awake player use a portal. After loading,
type `zzz` in normal chat. Every client should receive the current top-left
status on the next sleep check. The requester should see their own local command
echo; other clients should not see it in chat. No virtual Bedtime roster entry
should exist. Repeat with the requester far from other players and with `/s zzz`.

Request again immediately and from a second player: there should be no extra
broadcast during the five-second cooldown. After five seconds, a new request
should work. Test with three clients so per-recipient packet copies cannot cause
duplicate announcements. Ordinary normal, whisper, and shout chat must still
work. `/zzz` should remain an unrecognized vanilla command. Requests with only
one active player, nobody in bed, or sleep already underway should stay silent.
Test that `zzz please` is ordinary chat, not a command.
Verify `zzz`, `zzzz`, and mixed-case `ZzZZz` all trigger a replay after the cooldown.
Messages with fewer than three Zs, internal spaces, or punctuation must remain ordinary chat.

Allow one vanilla sleep-update pass (about two seconds), network delay, and any
queued top-left messages for each announcement. Use an unmodded console client for at least one player when
validating crossplay. Confirm the server log contains `Bedtime loaded` and no
Bedtime errors.

Also check three or more players: every awake player should appear on a separate
bullet line, in alphabetical order. Swap a sleeping and awake player within one
sleep-update pass: the count should stay the same but the names should update.
Verify duplicate character names appear once per player, long names remain
readable, and multiline notifications remain visible at the client's UI scale.
Check that the count stays below the hotbar as the awake-player list grows or shrinks.
Names containing markup or line breaks should display as plain, single-line names.

Set `ShowAwakePlayers = false` in the server's
`BepInEx/config/augusdogus.mods.Bedtime.cfg`, restart the test server, and repeat
the bed-entry, bed-exit, join, and disconnect checks. Only the count should appear,
without a trailing period, list heading, bullets, or leading blank lines. Swapping
sleepers without changing the count should stay silent. `zzz` should still repeat
the count without the list. Restore
`ShowAwakePlayers = true` and restart to verify the list returns.
