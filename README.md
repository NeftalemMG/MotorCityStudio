# MCS Dashboard

Interactive sports car infotainment UI built in Unity using UI Toolkit (UIElements).

---

## What's in the project

**Assets/UI/UXML/**
- `InstrumentCluster.uxml` — main driving view: speedometer, RPM, battery, tire pressure, gear selector
- `AppHub.uxml` — home grid with 6 mode cards
- `MusicScreen.uxml` — now playing, queue, album art panel
- `NavigationScreen.uxml` — map, route card, recent places
- `TrackMode.uxml` — telemetry, lap times, sector data, G-force
- `ValetMode.uxml` — speed limiter, geo-fence options, valet ring display
- `ShowtimeMode.uxml` — car show spec display, Motor City Apex screen
- `SettingsScreen.uxml` — display, sound, lighting, profiles, privacy

**Assets/UI/USS/**
- `dark-theme.uss` — night driving theme (default)
- `light-theme.uss` — day driving theme

**Assets/UI/Scripts/**
- `DashboardController.cs` — manages theme switching and screen navigation
- `SpeedometerElement.cs` — custom drawn speedometer arc
- `SvgIcon.cs` — sidebar icon helper

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

---

## Resolution scaling issue at 1.3x

When you scale the panel above 1.0, Unity re-rasterizes the UI at the display resolution, not the reference resolution. This causes blur because the text and elements were laid out at 1920x720 and are being stretched.

Things to check and try:

**Check your Game view resolution**
Make sure your Game view is set to exactly 1920x720 or a clean multiple. Odd resolutions (like 2496x936 at 1.3x) cause subpixel rendering artifacts.

**DPI / Physical resolution mismatch**
Go to `Edit > Project Settings > Player > Resolution and Presentation`. If you are targeting a specific screen, set the default resolution to match it exactly rather than relying on a scale factor.

**Panel Settings — Dynamic Atlas Settings**
On your Panel Settings asset, expand **Dynamic Atlas Settings** and increase:
- `Min Atlas Size`: 1024
- `Max Atlas Size`: 4096
This prevents UI textures from being downsampled in the atlas.

**Text rendering**
In `Project Settings > UI Toolkit`, check **Text Settings**. If you're using runtime font assets, make sure the atlas size on each font asset (`Font Asset > Atlas > Width/Height`) is at least 1024x1024. Small atlas sizes cause blurry text at higher scales.

**Scale Mode alternative**
If you need the UI to render crisp at a larger physical size, instead of scaling the panel try setting your Game view resolution to the actual physical pixel dimensions and keeping scale at 1.0. For a 1.3x display (2496x936), set that as your reference resolution instead of 1920x720.

**Constant Pixel Size**
As a diagnostic test, switch Scale Mode to `Constant Pixel Size` temporarily. If things look sharp, the issue is definitely in how Scale With Screen Size is interpolating. Switch back and adjust Match value — moving Match closer to 0 (width-dominant) or 1 (height-dominant) can reduce blur depending on your aspect ratio.