# Murder Mystery
## Default Configuration:

| Config | Default Value | Description |
|---|---|---|
| is_enabled | true | Enables the murder mystery plugin. |
| debug | false | Enables debug logging for release versions. |
| murderer_percentage | 1/6 | Sets the percentage of murderers. |
| murderer_offset | 0 | Sets the murderer offset, simulates more players in the playercount when multiplying the percentage. |
| detective_percentage | 1/12 | Sets the percentage of detectives. |
| detective_offset | 6 | Sets the detective offset, simulates more players in the playercount when multiplying the percentage. |
| equipment_delay | 45 | The delay from the start of the round (in seconds) before equipment is given to players. |
| round_time | 720 | The delay from the start of the round (in seconds) before the round ends by default. Set to 0 to disable. |
| murderers939_vision_time | 90 | The amount of time (in seconds) that must be remaining before murderers are given 939 vision. Set to 0 to disable. (Round timer must be enabled) |
| generator_unlock_time | 360 | The amount of time (in seconds) that must be remaining before generators are unlocked. Set to 0 to disable. (Round timer must be enabled) |

## Commands:
### **Note: Debug commands are prefixed with mmd, otherwise mm.**

| Command | Permission | Description | Debug? |
|---|---|---|---|
| enable | murdermystery.enable | Enables the murder mystery gamemode. | No |
| disable | murdermystery.enable | Disables the murder mystery gamemode. | No |
| showroles | murdermystery.showroles | Shows all the players and their roles. | No |
| ensureff | murdermystery.debug | Ensures friendly fire is enabled during the event. | Yes |
| giveequipment | murdermystery.debug | Forcefully calls the give equipment method on the specified player. | Yes |
| info | None | Shows info about the current version of murder mystery. | Yes |
| setrole | murdermystery.setroles | Set a players role during murder mystery. | Yes |
| status | None | Shows the current status of the gamemode. | Yes |
| killcoroutine | Debug | Kills a coroutine at the specified index of the main gamemode coroutines array. | Yes |
| removeequipment | Debug | Removes equipment from a specified players inventory. | Yes |
| validateinventory | Debug | Removes invalid items from a specified players inventory. | Yes |