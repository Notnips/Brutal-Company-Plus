## Version 3.5.1

### At this point on teh mod is now required to be on all clients to ensure full compatibility with outside enemies.

#### Scrap Settings Update:
- Individual scrap value settings for each moon are now available.
- To retain vanilla values, input `-1`.
- Format: `(Min Scrap Pieces / Max Scrap Pieces / Min Total Scrap Value / Max Total Scrap Value)`
  - Example: `(6, 25, 400, 1500)`

### Bug Fixes and Gameplay Adjustments:
- Resolved an issue where enemies spawned outside wouldn't attack players.
- Event rarity now accurately reflects the defined percentages. The restriction on the reoccurrence of the last event has been removed.
- Temporarily disabled weather effects related to moon heat until the release of v4.0.0, which will introduce API interrogation.


<details>
  <summary>Version 3.5.0 (click to expand)</summary>

## Version 3.5.0

### New Features

- **Added New Events:**
<details>
  <summary>Outside the Box</summary>
	Spawns Jesters Outside
</details>

<details>
  <summary>Shadow Realm</summary>
	Spawns several Bracken outside ( 50% chance to add fog for fun c: )
</details>

<details>
  <summary>Guards</summary>
	Nutcracker will spawn outside near the ship
</details>

  - *Note: Removed `Bad Planet` & `Chaos Company` due to their overpowering and non-enjoyable nature.*

### Event Modifications

- **Changes to Existing Events:**
  - `They have Evolved`: Now spawns an additional blob outside.
  - `Hoarder Town`: Now spawns Hoarder bugs both inside and outside.
  - `They are Shy`: Now spawns Spring and Bracken inside and outside.
  - Several events have been renamed.
  - Events no longer modify rarity or the enemy list, meaning:
    - Events that previously spawned only a certain enemy type will now spawn various enemies.
    - This change promotes better integration with additional mods featuring custom enemies.

### Gameplay Adjustments

- **Moon Heat Mechanics:**
  - Moon Heat will now reset when you wipe the save file or get fired.
  - Moon Heat no longer affects spawn rates of enemies.
  - Moon Heat now influences weather type as follows:
    - `>=20 & <40`: Rainy
    - `>=40 & <60`: Foggy
    - `>=60 & <80`: Flooded
    - `>=80 & <100`: Stormy
    - `>=100`: Eclipsed
    - *Configurable options for this feature will be added in a future update.*

- **Enemy Spawn Rates:**
  - Adjusted outside enemy spawn rates for a more balanced experience throughout the day. (Note: Current lack of outside enemies will be addressed soon.)
  - Inside enemy spawn rates have been fine-tuned for a smoother experience. (The reintroduction of configurable spawn rates is under consideration.)

### Configuration Updates

- **Deprecated Options:**
  - Temporarily removed `FactorySpawnChance` and `OutsideSpawnChance` config settings.

- **New Config Option:**
  - Added an option to disable rarity modifications, enhancing compatibility with other mods or for those preferring vanilla spawn rarity. 
    - Note: This setting can be overridden by specific events.
</details>

<details>
  <summary>Version 3.4.0 (click to expand)</summary>
  
## Version 3.4.0

### Added Features

- **Event Chance Rarity**
  - Events now have a weighted chance of occurring, allowing you to adjust their rarity.
  - Chance values range from `0` (never) to `100` (highest chance).
  - Events are no longer simple "True" or "False" toggles.

### New Configurable Options

#### Spawn Chance Values
  - **FactoryStartOfDaySpawnChance**: Adjust the Factory enemy spawn chance at the start of the day. Set to `-1` to use Brutal's default value. (Vanilla default is around 2-5, depending on the moon).
  - **FactoryMidDaySpawnChance**: Adjust the Factory enemy spawn chance at midday. Set to `-1` for Brutal's default. (Vanilla default is around 5-10, depending on the moon).
  - **FactoryEndOfDaySpawnChance**: Adjust the Factory enemy spawn chance at the end of the day. Set to `-1` for Brutal's default. (Vanilla default is around 10-15, depending on the moon).
  - **OutsideStartOfDaySpawnChance**: Adjust the Outside enemy spawn chance at the start of the day. Set to `-1` for the default value. (Vanilla is `0`).
  - **OutsideMidDaySpawnChance**: Adjust the Outside enemy spawn chance at midday. Set to `-1` for the default value. (Vanilla is `0.5`).
  - **OutsideEndOfDaySpawnChance**: Adjust the Outside enemy spawn chance at the end of the day. Set to `-1` for the default value. (Vanilla is `5`).

### Adjustments and Improvements

- Refined the Enemy Spawn Rate Config to function as intended.
- Updated the **ScrapSettings** config:
  - The values set now add onto the existing game values.
  - Setting these values to `0` will use the Vanilla default rates.
  - Note: Setting values higher than `0` adds to the existing Vanilla value depending on the moon, so adjust accordingly.
  - Default values have been adjusted. You may need to update these if you launched the mod before this update.
</details>

<details>
  <summary>Version 3.3.1 (click to expand)</summary>
  
 ## Version 3.3.1
  - Fixed a bug with Event Rarity always being None c:
</details>

<details>
  <summary>Version 3.3.0 (click to expand)</summary>
  
## Version 3.3.0
- New Config Options

- **Moon Heat Value Changes**
  - **Decrease Rate**: Adjust how much the moon heat decreases when not visiting a planet.
  - **Increase Rate**: Modify how much the moon heat increases upon returning to the same planet.

- **Enemy Rarity Spawn Chance Adjustment**
  - **Default Game Value**: Set to `-1` to use the game's default settings.
  - **Remove Spawn Chance**: Set to `0` to prevent spawning.
  - **Max Spawn Chance**: Set up to `100` for the highest spawn probability.
  - **Customize per Moon**: Tailor spawn chances (0-100) individually for each enemy on each moon.

- **Turret/Landmine Spawn Rate Customization**
  - **Toggle Options**: Enable or disable modifications to Turret/Landmine spawn rates.
  - **Turret Spawn Rate**: Default set to `8` - spawns 8 Turrets on every moon.
  - **Landmine Spawn Rate**: Default set to `30` - spawns 30 Landmines on every moon.
	
- **Added New V45 Enemies**
  - Now included in the "All Enemy Spawn Chance" variable.
</details>

<details>
  <summary>Version 3.2.0 (click to expand)</summary>
  
##  Version 3.2.0
- Added new config options
	- EnableAllEnemy ( This will add every enemy type to each moon as a spawn chance )
		- True = Any enemy can spawn on any map
		- False = Enemy spawn types will be defaulted to normal game logic
	- EnableFreeMoney ( This will give free money everytime survive and escape the planet )
	- FreeMoneyValue ( This will control the amount of money you get when EnableFreeMoney is true )
	
- Rewrote a massive chunk of enemy spawn logic to adhere to the EnableAllEnemy config
	- Eventually this will allow me to implement way more customization with the config on what can spawn and where
	
- Chenged spawn rates on Rumbling and Beast Inside events

- Added new Logo to Readme credit to (Discord ```lilboi__```)
</details>

<details>
  <summary>Version 3.1.0 (click to expand)</summary>
  
##  Version 3.1.0
- Redesigned config function using Bepinex Config Manager
	- This will allow better syncing and easier updates with addition config options in the future
- Removed LC_API for the required dependencies list
- Added FixCentipedeLag to the required dependencies list
	- This will help prevent lag spikes when centipedes spawn especially on events like "Internecivus Raptus"
- Adjusted spawn count for "The Rumbling" depending on the moon

**Config is now in the Bepinex Config Folder of your Profile**
</details>

<details>
  <summary>Version 3.0.0 (click to expand)</summary>
  
##  Version 3.0.0
Add New Event:
- Inside Out ( You will find out c: )
	- This took alot of effort to code and tweak to play out just as I wanted c:

**CONFIG RELEASED** ```BrutalCompanyPlus.cfg```
- You can now fully customize several elements of Brutal Company Plus
	- Change which events are active and disabled
	- Change the Starting Quota Values
		- DeadlineDaysAmount
		- StartingCredits
		- StartingQuota
		- BaseIncrease
	- Change Total Scrap Values on Every moon
		- MinScrap
		- MaxScrap
		- MinTotalScrapValue
		- MaxTotalScrapValue
</details>

<details>
  <summary>Version 2.3.0 (click to expand)</summary>
  
##  Version 2.3.0
Add New Event:
- The Hunger Games (You don't want to participate)

- Fixed a bug causing wrong enemies to spawn for Events like "The Rumbling" and "The Beasts Inside" on Paid Moons
	- Adjusted spawn rates on those events as well
</details>

<details>
  <summary>Version 2.2.2 (click to expand)</summary>
  
##  Version 2.2.2
- Fixed a bug causing some events to not trigger properly
</details>

<details>
  <summary>Version 2.2.1 (click to expand)</summary>
  
##  Version 2.2.1
- Minor update to Initialize Component on Start() instead of onDestroy()
	- This will align with new updates coming to bepinex
</details>

<details>
  <summary>Version 2.2.0 (click to expand)</summary>	
  
##  Version 2.2.0
Add New Events:
- The Rumbling (We are gunna need a Levi!)
	- Spawns a ton of giants that have lower chase speed
- The Beast Inside (Be vewy vewy quiet)
	- Spawns dogs inside the Factory (this hurt to code and sync with others without sharing the mod to all clients, but it works c:)
</details>

<details>
  <summary>Version 2.1.0 (click to expand)</summary>
  
##  Version 2.1.0
- Added a stable randomized rarity on Factory enemy spawns based on Heat level
	- This will fix any previous issues where past events lingered into new events
	- This will SOON be a fully customizable option via Config when added (You choose what can spawn during heat %)
	- Small tweaks with old code that was useless
</details>

<details>
  <summary>Version 2.0.1 (click to expand)</summary>
  
##  Version 2.0.1
- Fixed Event Randomizer bug ( Sorry :p )	
</details>

<details>
  <summary>Version 2.0.0 (click to expand)</summary>
  
##  Version 2.0.0
- Massive code cleanup, optimization.
	- This was done to allow me to eventually allow a config option to customize your own experience!
- Redesigned several events and how they playout.
	- Example: Spontanious Combustion will now only spawn landmines when your standing on the planets surface.
	- Many more changes
- Redesigned Level event names, print outs, rarity chances.
- Adjust moon heat dissipation when no longer on moon
- Removed usless events, codes, and other things causing minor lag or unwanted resource loss.
</details>

<details>
  <summary>Version 1.2.2 (click to expand)</summary>
  
##  Version 1.2.2
- 	Fix an issue where not all AI spawned on each level
-	Fix an issue where Blobs never opened doors
-	Now when a turret spawns on the ship, it will disable itself when you pull the lever
	- This was implemented to prevent the turret spamming shots in orbit mode.
</details>

<details>
  <summary>Version 1.2.1 (click to expand)</summary>
  
##  Version 1.2.1
- 	Fixed an issue with Quota Settings not applying!
</details>

<details>
  <summary>Version 1.2.0 (click to expand)</summary>
  
##  Version 1.2.0
- 	New Event Added: Pop Goes The... HOLY FUC- (Do I really need to add a description)
</details>

<details>
  <summary>Version 1.1.0 (click to expand)</summary>
  
## Version 1.1.0
- 	New Event Added: They have EVOLVED?!? (Blobs have evolved to open doors and be more aggresive)
</details>

<details>
  <summary>Version 1.0.0 (click to expand)</summary>

  ## Version 1.0.0
- Adjusted Quota Settings"
	Deadline is now set to 4 Days
	Starting Quota is now 400$

- Fixed "Spontanious Combustion Event" landmines will now disable when flipping the lever to take the ship into orbit.
- This should allow players to play the event out and also have a chance of leaving with the ship and not explode during the takeoff.
</details>
