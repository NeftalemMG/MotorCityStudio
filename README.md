# MCS Dashboard

Interactive sports car infotainment UI built in Unity using UI Toolkit (UIElements).

---

## What's in the project

**Assets/UI/UXML/**
- `InstrumentCluster.uxml` — main driving view: speedometer, RPM, battery, tire pressure, gear selector
- `AppHub.uxml`: home grid with 6 mode cards
- `MusicScreen.uxml`: now playing, queue, album art panel
- `NavigationScreen.uxml`: map, route card, recent places
- `TrackMode.uxml`: telemetry, lap times
- `ValetMode.uxml`: speed limiter, geo-fence options, valet ring display
- `ShowtimeMode.uxml`: car show spec display, Motor City Apex screen
- `SettingsScreen.uxml`: display, sound, lighting, profiles, privacy

**Assets/UI/USS/**
- `dark-theme.uss`: night driving theme (default)
- `light-theme.uss`: day driving theme

**Assets/UI/Scripts/**
- `DashboardController.cs`: manages theme switching and screen navigation
- `SpeedometerElement.cs`: custom drawn speedometer arc
- `SvgIcon.cs`: sidebar icon helper

---

## Setup

### 1. Panel Settings

Open your UI Document component in the Inspector.

Set the following under **Panel Settings**:

- **Scale Mode**: Scale With Screen Size
- **Reference Resolution**: 1920 x 720
- **Match**: 0.5
- **Screen Match Mode**: Match Width Or Height

Create a Panel Settings asset if you don't have one yet:
`Assets > Create > UI Toolkit > Panel Settings`

Assign it to your UIDocument component in the scene.

---

### 2. Import the assets

Copy the `Assets/UI` folder into your Unity project under `Assets/`. Unity will auto-import the UXML and USS files.

If assets don't appear:
- Right-click the folder in Project > Reimport
- Make sure you're on Unity 2021.2 or later (UI Toolkit required)

---

### 3. Wire up the UI Document

1. Create an empty GameObject in your scene, name it `Dashboard`
2. Add a `UI Document` component to it
3. Set **Source Asset** to `InstrumentCluster.uxml` (or `AppHub.uxml` for the home screen)
4. Set **Panel Settings** to your configured Panel Settings asset
5. Drag `dark-theme.uss` or `light-theme.uss` into the **Theme Style Sheet** field on the Panel Settings asset

---

### 4. Theme switching

`DashboardController.cs` handles switching between themes at runtime. To toggle:

```csharp
dashboardController.SetTheme(ThemeMode.Light);
dashboardController.SetTheme(ThemeMode.Dark);
```

You can also call this from a UI button or tie it to an ambient light sensor value.