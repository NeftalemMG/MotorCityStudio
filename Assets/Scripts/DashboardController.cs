using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Motor City Studio — Dashboard Controller v5.1
/// Fixes: RPM crit/warn only on lit segments, gear single-highlight,
/// speedo centered, light-theme label color forcing.
/// </summary>
public class DashboardController : MonoBehaviour
{
    [Header("UXML Screens")]
    public VisualTreeAsset clusterUXML;
    public VisualTreeAsset appHubUXML;
    public VisualTreeAsset musicUXML;
    public VisualTreeAsset navigationUXML;
    public VisualTreeAsset trackUXML;
    public VisualTreeAsset valetUXML;
    public VisualTreeAsset showtimeUXML;
    public VisualTreeAsset settingsUXML;

    [Header("Themes")]
    public StyleSheet darkThemeUSS;
    public StyleSheet lightThemeUSS;

    [Header("Live Data")]
    [Range(0,200)]  public float Speed   = 0f;
    [Range(0,8400)] public float Rpm     = 0f;
    [Range(0,100)]  public float Battery = 85f;

    enum Screen { Cluster, AppHub, Music, Navigation, Track, Valet, Showtime, Settings }

    UIDocument         _doc;
    VisualElement      _root;
    Screen             _current = Screen.Cluster;
    bool               _isDark  = true;
    SpeedometerElement _speedoEl;

    static readonly Color BG_DARK  = new Color(0f,     0f,     0f,     1f);
    static readonly Color BG_LIGHT = new Color(0.941f, 0.941f, 0.953f, 1f);

    void Start()
    {
        _doc  = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;
        LoadScreen(Screen.Cluster);
        StartCoroutine(SimData());
        InvokeRepeating(nameof(TickClock), 0f, 1f);
    }

    void Update()
    {
        if (!_isDark) ForceAllLabelColors();
    }

    // ── Screen loader ──────────────────────────────────────────────────
    void LoadScreen(Screen screen)
    {
        _current  = screen;
        _speedoEl = null;

        VisualTreeAsset asset = GetAsset(screen);
        if (asset == null)
        {
            Debug.LogWarning($"[MCS] UXML not assigned for {screen}, fallback to Cluster.");
            if (clusterUXML == null) { Debug.LogError("[MCS] clusterUXML is null!"); return; }
            asset    = clusterUXML;
            _current = Screen.Cluster;
        }

        StyleSheet theme = _isDark ? darkThemeUSS : lightThemeUSS;
        if (theme == null) { Debug.LogError("[MCS] USS theme not assigned!"); return; }

        _root.Clear();
        _root.styleSheets.Clear();
        _root.styleSheets.Add(theme);
        _root.style.backgroundColor = _isDark ? BG_DARK : BG_LIGHT;
        _root.style.width  = 1920;
        _root.style.height = 720;
        _root.style.overflow = Overflow.Hidden;

        asset.CloneTree(_root);
        WireSidebar();
        InjectSidebarIcons();

        switch (_current)
        {
            case Screen.Cluster:    SetupCluster();    break;
            case Screen.AppHub:     SetupAppHub();     break;
            case Screen.Music:      SetupMusic();      break;
            case Screen.Navigation: SetupNavigation(); break;
            case Screen.Track:      SetupTrack();      break;
            case Screen.Valet:      SetupValet();      break;
            case Screen.Showtime:   SetupShowtime();   break;
            case Screen.Settings:   SetupSettings();   break;
        }

        // Force all label colors after layout
        _root.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);
    }

    void OnLayoutReady(GeometryChangedEvent evt)
    {
        _root.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
        ForceAllLabelColors();
        StartCoroutine(ColorRetry());
    }

    IEnumerator ColorRetry()
    {
        yield return null;          ForceAllLabelColors();
        yield return null;          ForceAllLabelColors();
        yield return new WaitForSeconds(0.2f); ForceAllLabelColors();
    }

    void ForceAllLabelColors()
    {
        if (_isDark) return;
        _root.Query<Label>().ForEach(lbl =>
        {
            lbl.style.color = new Color(0f, 0f, 0f, 1f);
        });
    }

    VisualTreeAsset GetAsset(Screen s) => s switch
    {
        Screen.Cluster    => clusterUXML,
        Screen.AppHub     => appHubUXML,
        Screen.Music      => musicUXML,
        Screen.Navigation => navigationUXML,
        Screen.Track      => trackUXML,
        Screen.Valet      => valetUXML,
        Screen.Showtime   => showtimeUXML,
        Screen.Settings   => settingsUXML,
        _ => null
    };

    // ── Sidebar ────────────────────────────────────────────────────────
    void WireSidebar()
    {
        SafeClick("nav-apphub",   () => LoadScreen(Screen.AppHub));
        SafeClick("nav-music",    () => LoadScreen(Screen.Music));
        SafeClick("nav-nav",      () => LoadScreen(Screen.Navigation));
        SafeClick("nav-cluster",  () => LoadScreen(Screen.Cluster));
        SafeClick("nav-showtime", () => LoadScreen(Screen.Showtime));
        SafeClick("nav-track",    () => LoadScreen(Screen.Track));
        SafeClick("nav-valet",    () => LoadScreen(Screen.Valet));
        SafeClick("nav-settings", () => LoadScreen(Screen.Settings));
        SafeClick("nav-theme",    () => { _isDark = !_isDark; LoadScreen(_current); });
    }

    void InjectSidebarIcons()
    {
        Color ac = _isDark ? new Color(0.49f,0.91f,0.92f,1f) : new Color(0.03f,0.56f,0.57f,1f);
        Color dm = _isDark ? new Color(1f,1f,1f,0.35f)       : new Color(0f,0f,0f,0.40f);

        InjectIcon("nav-apphub",   SvgIcon.Icon.AppHub,    _current==Screen.AppHub    ?ac:dm);
        InjectIcon("nav-music",    SvgIcon.Icon.Music,     _current==Screen.Music     ?ac:dm);
        InjectIcon("nav-nav",      SvgIcon.Icon.Map,       _current==Screen.Navigation?ac:dm);
        InjectIcon("nav-cluster",  SvgIcon.Icon.Gauge,     _current==Screen.Cluster   ?ac:dm);
        InjectIcon("nav-showtime", SvgIcon.Icon.Bolt,      _current==Screen.Showtime  ?ac:dm);
        InjectIcon("nav-track",    SvgIcon.Icon.Stopwatch, _current==Screen.Track     ?ac:dm);
        InjectIcon("nav-valet",    SvgIcon.Icon.Shield,    _current==Screen.Valet     ?ac:dm);
        InjectIcon("nav-settings", SvgIcon.Icon.Sliders,   _current==Screen.Settings  ?ac:dm);
        InjectIcon("nav-theme",    _isDark ? SvgIcon.Icon.Sun : SvgIcon.Icon.Moon, dm);
    }

    void InjectIcon(string name, SvgIcon.Icon icon, Color color)
    {
        var el = _root.Q<VisualElement>(name);
        if (el == null) return;
        el.Clear();
        var ic = new SvgIcon(icon, color, 1.8f);
        ic.style.width = ic.style.height = 22;
        el.Add(ic);
    }

    // ── Cluster ────────────────────────────────────────────────────────
    void SetupCluster()
    {
        InjectSpeedometer();
        BuildRpmBar();
        BuildMiniChart();
        InjectCardIcons();
        UpdateCluster();
    }

    void InjectSpeedometer()
    {
        var area = _root.Q<VisualElement>("speedo-area");
        if (area == null) return;
        _speedoEl = new SpeedometerElement();
        // Fill the entire speedo-area absolutely so arc is full size
        _speedoEl.style.position = Position.Absolute;
        _speedoEl.style.left = _speedoEl.style.top    = 0;
        _speedoEl.style.right= _speedoEl.style.bottom = 0;
        _speedoEl.IsDark = _isDark;
        _speedoEl.Speed  = Speed;
        // Insert BEHIND the text labels
        area.Insert(0, _speedoEl);
    }

    void BuildRpmBar()
    {
        var seg = _root.Q<VisualElement>("rpm-segments");
        if (seg == null) return;
        seg.Clear();
        for (int i = 0; i < 40; i++)
        {
            var s = new VisualElement();
            s.AddToClassList("rpm-segment");
            // Mark zone membership (for color when lit) — NOT lit by default
            if      (i >= 32) s.AddToClassList("rpm-crit-zone");
            else if (i >= 26) s.AddToClassList("rpm-warn-zone");
            seg.Add(s);
        }
    }

    void BuildMiniChart()
    {
        var chart = _root.Q<VisualElement>("mini-chart");
        if (chart == null) return;
        chart.Clear();
        float[] vals = {0.3f,0.4f,0.35f,0.5f,0.6f,0.45f,0.7f,0.65f,0.8f,0.6f,
                        0.5f,0.55f,0.4f,0.6f,0.7f,0.65f,0.5f,0.45f,0.55f,0.4f};
        foreach (float v in vals)
        {
            var b = new VisualElement();
            b.AddToClassList("mini-bar");
            b.style.height = v * 26;
            chart.Add(b);
        }
    }

    void InjectCardIcons()
    {
        Color dark = new Color(0f, 0f, 0f, 0.85f);
        (string slot, SvgIcon.Icon icon)[] cards =
        {
            ("icon-slot-range",       SvgIcon.Icon.Range),
            ("icon-slot-battery",     SvgIcon.Icon.Battery),
            ("icon-slot-temperature", SvgIcon.Icon.Thermometer),
            ("icon-slot-tire",        SvgIcon.Icon.Tire),
            ("icon-slot-odometer",    SvgIcon.Icon.Odometer),
            ("icon-slot-trip",        SvgIcon.Icon.Trip),
            ("icon-slot-consumption", SvgIcon.Icon.Consumption),
            ("icon-slot-power",       SvgIcon.Icon.Power),
        };
        foreach (var (slot, icon) in cards)
        {
            var el = _root.Q<VisualElement>(slot);
            if (el == null) continue;
            el.Clear();
            var ic = new SvgIcon(icon, dark, 1.5f);
            ic.style.width = ic.style.height = 18;
            el.Add(ic);
        }
    }

    void UpdateCluster()
    {
        // Speed text
        SetText("speedo-speed", Mathf.RoundToInt(Speed).ToString());
        SetText("speedo-unit-label", "MPH");

        // Card values
        float range = Battery * 2.4f;
        SetText("card-range-value",   Mathf.RoundToInt(range).ToString());
        SetText("card-battery-value", Mathf.RoundToInt(Battery).ToString());
        SetWidth("bar-range",   range / 240f);
        SetWidth("bar-battery", Battery / 100f);

        // RPM bar — lit class only controls visibility,
        // rpm-crit-zone / rpm-warn-zone only apply color WHEN lit
        var seg = _root.Q<VisualElement>("rpm-segments");
        if (seg != null)
        {
            int lit = Mathf.RoundToInt((Rpm / 8400f) * 40);
            int idx = 0;
            foreach (var s in seg.Children())
            {
                bool on = idx < lit;
                if  (on && !s.ClassListContains("lit")) s.AddToClassList("lit");
                if (!on &&  s.ClassListContains("lit")) s.RemoveFromClassList("lit");
                idx++;
            }
        }
        SetText("rpm-value", Mathf.RoundToInt(Rpm).ToString("N0"));

        UpdateGears();
        if (_speedoEl != null) { _speedoEl.Speed = Speed; _speedoEl.MarkDirtyRepaint(); }
    }

    void UpdateGears()
    {
        // Determine active gear purely from speed — D is always the mode indicator
        int gear = Speed < 12  ? 0 :
                   Speed < 28  ? 1 :
                   Speed < 50  ? 2 :
                   Speed < 80  ? 3 :
                   Speed < 115 ? 4 :
                   Speed < 160 ? 5 : 6;

        string[] ids = {"gear-R","gear-N","gear-D",
                        "gear-1","gear-2","gear-3","gear-4","gear-5","gear-6","gear-7"};
        // Clear all
        foreach (string id in ids)
            _root.Q<VisualElement>(id)?.RemoveFromClassList("selected");

        // Only highlight the numbered gear (the moving one).
        // D is shown as mode but styled differently — add "gear-mode" class to it
        var gD = _root.Q<VisualElement>("gear-D");
        if (gD != null) gD.AddToClassList("gear-mode");

        // Active numbered gear
        if (gear > 0)
            _root.Q<VisualElement>($"gear-{gear}")?.AddToClassList("selected");
        else
            _root.Q<VisualElement>("gear-N")?.AddToClassList("selected");
    }

    // ── AppHub ─────────────────────────────────────────────────────────
    void SetupAppHub()
    {
        SafeClick("hub-music",    () => LoadScreen(Screen.Music));
        SafeClick("hub-nav",      () => LoadScreen(Screen.Navigation));
        SafeClick("hub-showtime", () => LoadScreen(Screen.Showtime));
        SafeClick("hub-track",    () => LoadScreen(Screen.Track));
        SafeClick("hub-valet",    () => LoadScreen(Screen.Valet));
        SafeClick("hub-settings", () => LoadScreen(Screen.Settings));
    }

    // ── Music ──────────────────────────────────────────────────────────
    void SetupMusic()
    {
        string[] tabs = {"tab-stream","tab-fm","tab-am","tab-usb"};
        foreach (string t in tabs)
        {
            string captured = t;
            SafeClick(captured, () => {
                foreach (string tt in tabs)
                    _root.Q<VisualElement>(tt)?.RemoveFromClassList("active");
                _root.Q<VisualElement>(captured)?.AddToClassList("active");
            });
        }
    }

    // ── Navigation ─────────────────────────────────────────────────────
    void SetupNavigation()
    {
        InjectNavIcon("nav-place-icon-home",    SvgIcon.Icon.Home);
        InjectNavIcon("nav-place-icon-office",  SvgIcon.Icon.Briefcase);
        InjectNavIcon("nav-place-icon-charger", SvgIcon.Icon.ChargingPin);
        InjectNavIcon("nav-place-icon-track",   SvgIcon.Icon.Flag);
        SafeClick("btn-go",      () => Debug.Log("[MCS] Route started"));
        SafeClick("btn-preview", () => Debug.Log("[MCS] Route preview"));
    }

    void InjectNavIcon(string slot, SvgIcon.Icon icon)
    {
        var el = _root.Q<VisualElement>(slot);
        if (el == null) return;
        el.Clear();
        Color c = _isDark ? new Color(1f,1f,1f,0.55f) : new Color(0f,0f,0f,0.55f);
        var ic = new SvgIcon(icon, c, 1.5f);
        ic.style.width = ic.style.height = 18;
        el.Add(ic);
    }

    // ── Track ──────────────────────────────────────────────────────────
    void SetupTrack()
    {
        SafeClick("btn-export", () => Debug.Log("[MCS] Export"));
        SafeClick("btn-share",  () => Debug.Log("[MCS] Share"));
        SafeClick("btn-reset",  () => Debug.Log("[MCS] Reset"));
    }

    // ── Valet ──────────────────────────────────────────────────────────
    void SetupValet()
    {
        var ring = _root.Q<VisualElement>("valet-ring-icon-slot");
        if (ring != null)
        {
            ring.Clear();
            Color c = _isDark ? new Color(0.78f,1f,0.20f,1f) : new Color(0.36f,0.52f,0f,1f);
            var ic = new SvgIcon(SvgIcon.Icon.Lock, c, 2.2f);
            ic.style.width = ic.style.height = 48;
            ring.Add(ic);
        }
        InjectNavIcon("valet-icon-speed",  SvgIcon.Icon.Stopwatch);
        InjectNavIcon("valet-icon-geo",    SvgIcon.Icon.Map);
        InjectNavIcon("valet-icon-camera", SvgIcon.Icon.Sliders);
        InjectNavIcon("valet-icon-alerts", SvgIcon.Icon.Bolt);
        SafeClick("btn-deactivate", () => LoadScreen(Screen.AppHub));
    }

    // ── Showtime ───────────────────────────────────────────────────────
    void SetupShowtime()
    {
        SafeClick("btn-launch-control", () => Debug.Log("[MCS] Launch Control"));
        SafeClick("btn-heritage",       () => Debug.Log("[MCS] Heritage mode"));
    }

    // ── Settings ───────────────────────────────────────────────────────
    void SetupSettings()
    {
        string[] panels = {"display","sound","lighting","profiles","privacy","about"};
        string[] navIds = {"snav-display","snav-sound","snav-lighting",
                           "snav-profiles","snav-privacy","snav-about"};

        void ShowPanel(string name)
        {
            foreach (string p in panels)
            {
                var panel = _root.Q<VisualElement>($"panel-{p}");
                if (panel != null) panel.style.display = DisplayStyle.None;
            }
            foreach (string n in navIds)
                _root.Q<VisualElement>(n)?.RemoveFromClassList("active");
            var showPanel = _root.Q<VisualElement>($"panel-{name}");
            if (showPanel != null) showPanel.style.display = DisplayStyle.Flex;
            _root.Q<VisualElement>($"snav-{name}")?.AddToClassList("active");
        }

        ShowPanel("display");
        for (int i = 0; i < panels.Length; i++)
        {
            string captured = panels[i];
            SafeClick(navIds[i], () => ShowPanel(captured));
        }

        string[] toggleIds = {"toggle-darkmode","toggle-autobright","toggle-ambient",
                              "toggle-animations","toggle-haptics","toggle-siri",
                              "toggle-location","toggle-crashdetect"};
        foreach (string tid in toggleIds)
        {
            string captured = tid;
            BindToggle(captured, captured == "toggle-darkmode" ? () => {
                _isDark = !_isDark; LoadScreen(Screen.Settings);
            } : (System.Action)null);
        }
    }

    void BindToggle(string id, System.Action onToggle)
    {
        var el = _root.Q<VisualElement>(id);
        if (el == null) return;
        el.RegisterCallback<ClickEvent>(_ => {
            bool isOn = el.ClassListContains("toggle-on");
            el.RemoveFromClassList(isOn ? "toggle-on" : "toggle-off");
            el.AddToClassList(isOn ? "toggle-off" : "toggle-on");
            var thumb = el.Q<VisualElement>(className:"tgl-thumb");
            if (thumb != null)
            {
                thumb.RemoveFromClassList(isOn ? "thumb-on" : "thumb-off");
                thumb.AddToClassList(isOn ? "thumb-off" : "thumb-on");
            }
            onToggle?.Invoke();
        });
    }

    // ── Simulation ─────────────────────────────────────────────────────
    IEnumerator SimData()
    {
        while (true)
        {
            Speed   = Mathf.Clamp(Speed + Random.Range(-3f, 3f), 0, 200);
            Rpm     = Speed * 56f + Random.Range(-200f, 200f);
            Battery = Mathf.Clamp(Battery - 0.001f, 0, 100);
            if (_current == Screen.Cluster) UpdateCluster();
            yield return new WaitForSeconds(0.3f);
        }
    }

    void TickClock()
    {
        var t = System.DateTime.Now;
        SetText("status-time", t.ToString("h:mm"));
        SetText("status-date", t.ToString("ddd MMM d").ToUpper());
    }

    // ── Helpers ────────────────────────────────────────────────────────
    void SafeClick(string name, System.Action action)
    {
        _root.Q<VisualElement>(name)?.RegisterCallback<ClickEvent>(_ => action?.Invoke());
    }

    void SetText(string name, string text)
    {
        var el = _root.Q<Label>(name);
        if (el != null) el.text = text;
    }

    void SetWidth(string name, float pct)
    {
        var el = _root.Q<VisualElement>(name);
        if (el != null) el.style.width = Length.Percent(Mathf.Clamp01(pct) * 100f);
    }
}
