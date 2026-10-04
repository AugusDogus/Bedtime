# Multiplayer checks

Use a test dedicated server with BepInEx and Bedtime, without another sleep mod.
Connect two players with vanilla clients. These checks require a running game;
the automated tests do not establish end-to-end console or crossplay behavior.

1. At night, have Alice enter a valid bed while Bob remains standing.
   Both clients should see `1 out of 2 players are in bed.` in the top-left notification area.
   The night must continue, with Alice waiting in bed.
2. Have Bob use the sit emote. The count must remain one and the night must continue.
3. Have Alice leave bed. Both clients should see `0 out of 2 players are in bed.`
4. Have Alice reenter bed. The count should appear again, then remain quiet
   while the bed state stays unchanged.
5. Have Bob enter another valid bed. The game should perform its normal sleep
   transition, with a final count announcement as the transition starts.
6. With one of two players waiting in bed, connect a third player. Once their
   character is active, the count should change to `1 out of 3 players are in bed.`
   Disconnect the third player and verify it returns to `1 out of 2 players are in bed.`
7. Disconnect the waiting sleeper while another player remains awake. The
   remaining client should see `0 out of 1 player is in bed.`
8. Restart the server and verify announcements resume. Remove Bedtime and restart
   to verify that ordinary sleeping still works and no missing-mod prompt appears.

Allow one vanilla sleep-update pass (about two seconds), network delay, and any
queued top-left messages for each announcement. Use an unmodded console client for at least one player when
validating crossplay. Confirm the server log contains `Bedtime loaded` and no
Bedtime errors.
