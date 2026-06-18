# Arkania Survival Project Plan

## Scope and guardrails

This plan keeps the Arkania survival concept close to SWLOR's existing patterns instead of introducing a large custom rules engine. The goal is to make the cold dangerous and memorable while reusing familiar systems:

- **Planet setup**: add Arkania as a normal `PlanetType` entry, with area names following the existing `"PlanetName - AreaName"` convention.
- **Weather**: extend the current `Weather` service and `WeatherClimate` data instead of creating a separate survival tick system.
- **Quests**: define content through a normal `Feature/QuestDefinition/ArkaniaQuestDefinition.cs` using `QuestBuilder` states, kill objectives, collect objectives, repeatable quests, and object-use progression.
- **Generator resources**: store one city-level entity in Redis and expose it through a single NPC/dialog or NUI, avoiding area-by-area persistence or per-player global state.
- **Theme fit**: use KOTOR-era Arkania, Republic/Old Sith tension, Arkanian scientists, offshoot miners, droids, mercenaries, and ancient machinery. Avoid direct Frostpunk names, factions, laws, or modern steampunk references.

## Existing systems to build on

| Need | Existing code pattern | Recommended Arkania use |
| --- | --- | --- |
| Planet identity | `PlanetType` entries define planet name, area prefix, orbit waypoint, landing waypoint, fee, and active flag. | Add `Arkania = 256` with `"Arkania - "`, `Arkania_Orbit`, and `ARKANIA_LANDING`. |
| Climate by planet name | `Weather.GetAreaClimate` parses the area name before the hyphen and looks up `WeatherPlanetDefinitions`. | Arkania outdoor areas named `Arkania - Frozen Causeway` automatically inherit Arkania climate. |
| Snowstorm damage | `Weather.DoWeatherEffects` already checks `SNOW_STORM` and applies recurring cold damage through `ApplySnowstorm`. | Use existing snowstorms as the first playable hazard before adding new freeze-depth mechanics. |
| Quest structure | Planet quest files use `IQuestListDefinition` and `QuestBuilder` with states, rewards, prerequisites, repeatables, and callbacks. | Keep Arkania quests in one planet quest definition until content volume justifies splitting. |
| Persistent server state | `DB.Get<T>` / `DB.Set` already persist entities such as module cache, players, properties, and accounts. | Add one `ArkaniaGeneratorState` entity keyed to `ARKANIA_GENERATOR` for heat, coal, parts, and storm state. |

## Arkania quest package

The first release should be a compact vertical slice: one settlement hub, one mine/cave, one exterior route, one generator chamber, and one storm-safe interior. The quests below are designed to share props and objective locations so the build does not balloon.

### 1. The City Must Survive

- **Type**: repeatable community turn-in / generator contribution.
- **NPC**: Generator Steward Veyra Tann, placed near the generator console.
- **Objectives**:
  - Turn in `ark_coal`, `ark_heat_core`, or `ark_machine_part`.
  - Update the shared generator stockpile.
  - Reward credits, XP, and a small survival token/city scrip item.
- **Implementation fit**:
  - Use a collect-item quest for the first iteration.
  - Later, move the turn-in to a dialog/NUI that directly calls `DB.Get<ArkaniaGeneratorState>` and `DB.Set`.
- **Flavor**: players are not saving the world; they are buying the city another day.

### 2. Children in the Ice

- **Type**: rescue/object-use chain.
- **Location**: collapsed frozen mining tunnel.
- **Objectives**:
  1. Speak to a foreman at the settlement.
  2. Activate three blocked tunnel markers to clear ice or stabilize beams.
  3. Escort or locate trapped children/survivors.
  4. Return to the foreman.
- **Implementation fit**:
  - Use quest states with object-use advancement like existing object-based quests.
  - Use visibility toggles for trapped survivor NPCs if needed.
- **Weather tie-in**: tunnel mouth can be an exterior area with snowstorm risk; inner tunnel is safe from weather but contains hostile creatures or failing machinery.

### 3. Coal Run: Whiteout Route

- **Type**: repeatable collection route.
- **Location**: exterior road from city to old mining depot.
- **Objectives**:
  - Gather coal crates from exposed waypoints or placeables.
  - Return before or during a storm.
- **Implementation fit**:
  - Start as `AddCollectItemObjective("ark_coal", quantity)`.
  - Add random crate respawns later through existing spawn/placeable scripts.
- **Reward hook**: higher reward if the generator heat is critically low.

### 4. The Generator Coughs

- **Type**: generator repair quest.
- **Prerequisites**: one or more basic city quests.
- **Objectives**:
  1. Inspect pressure gauges.
  2. Recover machine parts from a ruined Arkanian facility.
  3. Defeat sabotage droids or scavengers.
  4. Install the parts at the generator console.
- **Implementation fit**:
  - Combine object-use quest states with a kill objective and collect objective.
  - On completion, add a fixed amount of generator integrity.
- **KOTOR-era flavor**: the generator is old Republic/Arkanian thermal infrastructure, not a Frostpunk steam core.

### 5. Heat for the Wards

- **Type**: moral/community support quest.
- **NPCs**: med ward doctor, refugee quartermaster.
- **Objectives**:
  - Deliver insulated blankets, medpacs, and food packs.
  - Optional: redirect limited generator reserve to the ward for a temporary morale/medical reward.
- **Implementation fit**:
  - Keep the first pass as a normal collect quest.
  - Later, tie optional choices to generator state by increasing `Morale` or decreasing `HeatReserve`.
- **Tone**: survival choices should be grim, but not copy Frostpunk law systems.

### 6. Black Ice Signal

- **Type**: exploration and combat.
- **Location**: ancient transmitter half-buried in ice.
- **Objectives**:
  - Track distress pings during storm windows.
  - Clear wildlife or old security droids.
  - Restore the signal to improve city forecasts.
- **Implementation fit**:
  - Completion can unlock stronger storm warnings from the generator NPC.
  - This gives a lore reason for better player-facing weather messages.

### 7. Saboteurs in the White

- **Type**: escalating repeatable/event quest.
- **Enemies**: mercenaries, Sith-aligned agents, or industrial rivals trying to seize Arkanian tech.
- **Objectives**:
  - Defend fuel sleds or generator maintenance crews.
  - Kill attackers and recover stolen parts.
- **Implementation fit**:
  - Use kill objectives and stolen-part collect objectives.
  - Use as a later-phase quest after the simple generator loop is stable.

## Generator resource concept

### Name

**The Arkanian Thermal Generator**

This should feel like a KOTOR-era planetary survival system: geothermal taps, ionized heat exchangers, Republic-era regulators, Arkanian cold-weather engineering, and fragile ancient infrastructure.

### Player-facing resources

| Resource | Item resref suggestion | Use |
| --- | --- | --- |
| Coal / thermal ore | `ark_coal` | Basic fuel; common from mines and exterior routes. |
| Machine parts | `ark_machine_part` | Repairs integrity decay and storm damage. |
| Heat cores | `ark_heat_core` | Rare high-value fuel from facilities, bosses, or hard quests. |
| Medical supplies | `ark_med_supplies` | Optional city morale/ward support. |
| Forecast data | `ark_fcst_data` | Optional unlock for improved warning text or reduced surprise storms. |

### Server state entity sketch

Add this only when the first quest loop works. Until then, use collect quests and rewards to validate fun.

```csharp
public class ArkaniaGeneratorState : EntityBase
{
    public ArkaniaGeneratorState()
    {
        Id = "ARKANIA_GENERATOR";
    }

    public int HeatReserve { get; set; }
    public int Integrity { get; set; } = 100;
    public int Coal { get; set; }
    public int MachineParts { get; set; }
    public int HeatCores { get; set; }
    public int Morale { get; set; } = 50;
    public DateTime LastProcessed { get; set; } = DateTime.UtcNow;
}
```

### Generator behavior

- **HeatReserve** is the primary score players see.
- **Integrity** controls how efficiently fuel converts into heat.
- **Coal** is consumed first.
- **Heat cores** are emergency fuel.
- **Machine parts** repair integrity.
- **Morale** is optional and should not block core functionality in the first pass.

Recommended first-pass thresholds:

| Generator condition | HeatReserve | Gameplay effect |
| --- | ---: | --- |
| Stable | 70+ | Normal city, standard exterior weather. |
| Strained | 40-69 | More warning text, no extra damage yet. |
| Failing | 15-39 | Exterior storm damage can be slightly more frequent. |
| Critical | 0-14 | City NPC warnings, emergency quests highlighted, storm damage allowed near city outskirts. |

Keep the first implementation passive: the generator changes messages, rewards, and storm severity. Do not immediately make the city unusable or kill players in safe hub interiors.

## Weather coding plan

### Step 1: planet climate only

Add an Arkania climate entry patterned after Hutlar, but tune it as its own world:

```csharp
[PlanetType.Arkania] = new WeatherClimate
{
    HeatModifier = -9,
    HumidityModifier = -2,
    WindModifier = +2,
    HasSnowStorms = true,
    FreezingText = "The Arkanian cold bites through exposed seams in your gear.",
    SnowText = "Fine ice falls like dust, whitening every exposed surface.",
    WindyText = "A hard polar wind drags loose snow across the ground.",
    ColdWindyText = "The wind cuts like a vibroblade, numbing exposed flesh.",
    StormText = "A whiteout swallows the horizon; only the generator lights feel real."
};
```

This uses current climate modifiers and `HasSnowStorms`, keeping Arkania inside the existing weather framework.

### Step 2: use existing snowstorm damage

The current system already damages PCs during `SNOW_STORM` with `d6(2)` cold damage every 6 seconds via `ApplySnowstorm`. For the initial Arkania test, prefer that existing behavior over a new survival meter.

Recommended rule:

- **Interior or underground areas**: no weather damage.
- **Outdoor hub outskirts**: snowstorm visuals and warning text, but no damage until players leave safe boundaries.
- **Wilderness and mine exterior**: normal `SNOW_STORM` damage.
- **Deep storm expedition areas**: optional stronger damage after the base system is accepted.

### Step 3: small Arkania-specific cold helper

Only add this if Zunath is comfortable with a planet-specific branch. The function should remain tiny and reuse NWScript effects.

```csharp
private static void ApplyArkaniaCold(uint target, uint area, int severity)
{
    if (GetArea(target) != area) return;
    if (GetIsDead(target)) return;
    if (GetIsPC(target) && GetIsPC(GetMaster(target)) == false) return;

    var damage = severity <= 1 ? d4() : d6(severity);
    var effect = EffectLinkEffects(
        EffectVisualEffect(VisualEffect.Vfx_Dur_Iceskin),
        EffectDamage(damage, DamageType.Cold));

    ApplyEffectToObject(DurationType.Instant, effect, target);
    DelayCommand(6.0f, () => ApplyArkaniaCold(target, area, severity));
}
```

Suggested safety checks before applying it:

- Area planet must be Arkania.
- Area local int `ARKANIA_SAFE_FROM_COLD` must not be `1`.
- Player should be above ground and outside.
- Severity should be capped to `1` or `2` for the first release.

### Step 4: generator influences weather severity

When `ArkaniaGeneratorState.HeatReserve` exists, use it lightly:

```csharp
var generator = DB.Get<ArkaniaGeneratorState>("ARKANIA_GENERATOR") ?? new ArkaniaGeneratorState();
var severity = generator.HeatReserve < 15 ? 2 : 1;
```

Do not make the generator directly drive the whole planet weather table at first. Let it adjust local severity and warning messages only.

## NPC/dialog plan

### Generator Steward Veyra Tann

Dialogue options:

1. **"How is the Generator holding?"**
   - Shows HeatReserve, Integrity, Coal, MachineParts, and HeatCores in simple text.
2. **"I brought fuel."**
   - Consumes `ark_coal` or `ark_heat_core` and updates stockpiles.
3. **"I brought repair parts."**
   - Consumes `ark_machine_part` and improves Integrity.
4. **"Where can I help?"**
   - Points to currently relevant repeatable quests.
5. **"What happens if the Generator fails?"**
   - Lore explanation and warning, not a hard fail-state in the first version.

Keep the NPC on Viscara only if this is a cross-planet test console. For the real feature, place Veyra in Arkania's settlement so the system feels diegetic.

## Suggested implementation phases

### Phase 1: vertical slice

- Add `PlanetType.Arkania`.
- Add Arkania climate in `WeatherPlanetDefinitions`.
- Build one exterior area and one safe interior.
- Add two quests:
  - `ark_city_survive`
  - `ark_children_ice`
- Use current snowstorm weather damage.

### Phase 2: resource loop

- Add `ArkaniaGeneratorState`.
- Add Steward Veyra dialog for status and item contribution.
- Add coal and part item resrefs.
- Add `ark_coal_run` repeatable.

### Phase 3: consequences and escalation

- Generator condition changes storm warnings and optional severity.
- Add `ark_generator_coughs` and `ark_saboteurs_white`.
- Add rewards for community contributors.
- Add improved forecasts after `ark_black_ice_signal`.

## Zunath concern checklist

Before implementation, confirm these items:

- Does the project want Arkania active immediately, or hidden until areas/waypoints are ready?
- Should generator resources be global across the server or reset weekly/event-style?
- What are acceptable damage numbers for low-level players in weather areas?
- Should city hub interiors always be safe from cold damage?
- Should the first version avoid new GUI and rely on dialog text?
- Which existing item resrefs should be reused for coal/parts, if any, before adding new item blueprints?

## Recommended minimum code changes

For the smallest code footprint, start with only these:

1. Add `Arkania = 256` to `PlanetType`.
2. Add `[PlanetType.Arkania]` to `WeatherPlanetDefinitions`.
3. Add `ArkaniaQuestDefinition.cs` with two quests.
4. Add a simple dialog later for generator status.
5. Reuse `SNOW_STORM` and `ApplySnowstorm` until player feedback proves a custom Arkania cold tick is needed.
