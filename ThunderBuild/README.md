<p align="center">
  <img src="https://i.imgur.com/g4131Z3.png" alt="Alternative text for image">
</p>

#
**A CUSTOMIZABLE hardcore mod that randomizes events ranging between insanity and normal.**
- This will be updated constantly with the intent of adding more unique and mind-twisting events.

## Features
- Over 20+ Unique Events and Counting.
- Custom Quota Start Values & increased progression rates.
- Increased Spawn Rates.
- Any Moon ALL Enemies.
- Increased Loot Drop Rates.

## Brutal Company Plus: Configuration Options

### Gameplay Adjustments
- **Event Activation**: Choose which events to activate or disable.
- **Starting Quotas**:
  - `DeadlineDaysAmount`: Set the number of days before the deadline.
  - `StartingCredits`: Determine the starting credit value for a new game.
  - `StartingQuota`: Set the initial quota value.
  - `BaseIncrease`: Define the quota increase after each deadline.

### Scrap Management
- Individual scrap value settings for each moon.
- To retain vanilla values, input `-1`.
- Format: `(Min Scrap Pieces / Max Scrap Pieces / Min Total Scrap Value / Max Total Scrap Value)`
  - Example: `(6, 25, 400, 1500)`

### Enemy Dynamics
- **Enemy Enablement**: Option to enable all enemies on every moon.
- **Enemy Rarity & Spawn Chances**:
  - Default: `-1`
  - Disable Spawn: `0`
  - Max Spawn Chance: `100`
  - Customize spawn rates (0-100) for each moon.
  - The higher the chance the more will spawn.
  - Option to disable Rarity altogether to allow cohesion with other mods if needed or if you just want vanilla rates!

### Economic Features
- **Free Money**:
  - Toggleable option for monetary rewards after surviving a moon.
  - Configurable reward amount.

### Environmental Factors
- **Moon Heat Values**:
  - Rate of decrease when not visiting a planet.
  - Rate of increase upon returning to a planet.

### Defense Mechanisms
- **Turret/Landmine Spawn Rates**:
  - Enable/disable modifications to turret/landmine spawn rates.
  - Turret Spawn Rate (Default: `8` turrets per moon).
  - Landmine Spawn Rate (Default: `30` landmines per moon).

### Instructions
- The config file is named `BrutalCompanyPlus.cfg`.
- Find this config in your Thunderstore Mod Manager Profile Directory:
  - Go to the profile containing "Brutal Company Plus".
  - Click Settings in the left panel, then the "Locations" tab.
  - Click "Browse profile folder".
  - Navigate to `BepInEx\config\` to find `BrutalCompanyPlus.cfg`.
  - Open and adjust the config to your liking!

### IMPORTANT
- This mod will now be REQUIRED on all clients to allow events to properly play out as intended.

- Avoid typos in the config; errors can lead to total mod failure.
- Experiencing an issue where only the event "None" occurs could be due to a config issue.

## Notes
- Continuously adding new and exciting events.

## Common Mod Conflicts
- Game Master ( this mod wipes the chat )
- Other event related mods ( they conflict and overwrite the chat )
- NoFriendlyFire ( this conflicts with a few events that use the PlayerDamageServerRPC )

## Coming Soon
**Events**
- Solar Malfunction (Unpredictable teleportation... "Wait where am I?")
- More innovative ideas underway.

## Credit
- Based off "Brutal Company" by "2018".
- Logo in header designed by `lilboi__` on Discord.
- Logo in avatar designed by `sadamazon` on Discord.
- Special thanks for their incredible artwork!

## Contact
**Discord**: `_nips`
- Feedback, bug reports, new event ideas? Feel free to message me (response not guaranteed).