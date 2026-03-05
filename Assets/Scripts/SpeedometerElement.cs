using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Speedometer arc rendered via Painter2D.
/// Colors match ApexDashboard_V5 color palette exactly:
///   Dark:  arc = #7DE9EB (rgb 125,233,235), glow = rgba(125,233,235,0.12)
///   Light: arc = #088f92 (rgb 8,143,146),   glow = rgba(8,143,146,0.12)
/// Arc shape: 220° sweep (start 205°, end 335° going clockwise — matching the HTML SVG)
/// </summary>
public class SpeedometerElement : VisualElement
{
    private float _speed    = 67f;
    private bool  _isDark   = true;
    private const float MAX_SPEED   = 220f;
    private const float START_DEG   = 215f;   // bottom-left
    private const float TOTAL_SWEEP = 290f;   // degrees of arc

    public float Speed  { get => _speed;  set { _speed  = Mathf.Clamp(value, 0, MAX_SPEED); MarkDirtyRepaint(); } }
    public bool  IsDark { get => _isDark; set { _isDark = value; MarkDirtyRepaint(); } }

    public SpeedometerElement()
    {
        generateVisualContent += OnDraw;
    }

    void OnDraw(MeshGenerationContext ctx)
    {
        float w = layout.width;
        float h = layout.height;
        if (w < 2 || h < 2) return;

        var p  = ctx.painter2D;
        float cx = w * 0.5f;
        float cy = h * 0.52f;
        float R  = Mathf.Min(w, h) * 0.40f;

        // ─ Colors from HTML palette ─
        Color arcColor, glowColor, trackColor, tickFaint, tickBright;
        if (_isDark)
        {
            arcColor   = new Color(125/255f, 233/255f, 235/255f, 1f);
            glowColor  = new Color(125/255f, 233/255f, 235/255f, 0.12f);
            trackColor = new Color(1f,1f,1f,0.04f);
            tickFaint  = new Color(1f,1f,1f,0.12f);
            tickBright = new Color(1f,1f,1f,0.20f);
        }
        else
        {
            arcColor   = new Color(8/255f,  143/255f, 146/255f, 1f);
            glowColor  = new Color(8/255f,  143/255f, 146/255f, 0.12f);
            trackColor = new Color(0f,0f,0f,0.08f);
            tickFaint  = new Color(0f,0f,0f,0.08f);
            tickBright = new Color(0f,0f,0f,0.18f);
        }

        float norm        = Mathf.Clamp01(_speed / MAX_SPEED);
        float activeSweep = norm * TOTAL_SWEEP;

        // ── 1. Wide glow halo behind track ──
        PaintArc(p, cx, cy, R, 28f, new Color(glowColor.r, glowColor.g, glowColor.b, 0.06f),
            START_DEG, START_DEG + TOTAL_SWEEP, 90);

        // ── 2. Background track ──
        PaintArc(p, cx, cy, R, 3f, trackColor, START_DEG, START_DEG + TOTAL_SWEEP, 90);

        // ── 3. Active arc glow ──
        if (activeSweep > 1f)
        {
            PaintArc(p, cx, cy, R, 20f,
                new Color(arcColor.r, arcColor.g, arcColor.b, 0.10f),
                START_DEG, START_DEG + activeSweep, 90);

            // ── 4. Active arc solid ──
            PaintArc(p, cx, cy, R, 3f, arcColor, START_DEG, START_DEG + activeSweep, 90);

            // ── 5. Leading dot ──
            float tipRad = (START_DEG + activeSweep) * Mathf.Deg2Rad;
            float tx = cx + Mathf.Cos(tipRad) * R;
            float ty = cy + Mathf.Sin(tipRad) * R;
            // Glow
            p.fillColor = new Color(arcColor.r, arcColor.g, arcColor.b, 0.3f);
            p.BeginPath(); p.Arc(new Vector2(tx, ty), 10f, 0, Mathf.PI * 2); p.Fill();
            // Dot
            p.fillColor = arcColor;
            p.BeginPath(); p.Arc(new Vector2(tx, ty), 5f, 0, Mathf.PI * 2); p.Fill();
            // White core
            p.fillColor = Color.white;
            p.BeginPath(); p.Arc(new Vector2(tx, ty), 2.5f, 0, Mathf.PI * 2); p.Fill();
        }

        // ── 6. Major tick marks ──
        int majors = 11;
        for (int i = 0; i <= majors; i++)
        {
            float t   = (float)i / majors;
            float deg = START_DEG + t * TOTAL_SWEEP;
            float rad = deg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // Major tick
            p.strokeColor = tickBright;
            p.lineWidth   = 1.5f;
            p.BeginPath();
            p.MoveTo(new Vector2(cx + cos * (R + 10f), cy + sin * (R + 10f)));
            p.LineTo(new Vector2(cx + cos * (R + 24f), cy + sin * (R + 24f)));
            p.Stroke();

            // 4 minor ticks between majors
            if (i < majors)
            {
                for (int m = 1; m <= 4; m++)
                {
                    float md  = deg + (TOTAL_SWEEP / majors) * m / 5f;
                    float mr  = md * Mathf.Deg2Rad;
                    p.strokeColor = tickFaint;
                    p.lineWidth   = 0.75f;
                    p.BeginPath();
                    p.MoveTo(new Vector2(cx + Mathf.Cos(mr)*(R+10f), cy + Mathf.Sin(mr)*(R+10f)));
                    p.LineTo(new Vector2(cx + Mathf.Cos(mr)*(R+16f), cy + Mathf.Sin(mr)*(R+16f)));
                    p.Stroke();
                }
            }
        }
    }

    /// <summary>Fills an arc ring (thick stroke) between innerR and outerR.</summary>
    static void PaintArc(Painter2D p, float cx, float cy, float radius,
                         float thickness, Color color,
                         float startDeg, float endDeg, int steps)
    {
        float half  = thickness * 0.5f;
        float outer = radius + half;
        float inner = radius - half;

        p.fillColor = color;
        p.BeginPath();

        for (int i = 0; i <= steps; i++)
        {
            float a  = (startDeg + (float)i / steps * (endDeg - startDeg)) * Mathf.Deg2Rad;
            var   pt = new Vector2(cx + Mathf.Cos(a) * outer, cy + Mathf.Sin(a) * outer);
            if (i == 0) p.MoveTo(pt); else p.LineTo(pt);
        }
        for (int i = steps; i >= 0; i--)
        {
            float a = (startDeg + (float)i / steps * (endDeg - startDeg)) * Mathf.Deg2Rad;
            p.LineTo(new Vector2(cx + Mathf.Cos(a) * inner, cy + Mathf.Sin(a) * inner));
        }
        p.ClosePath();
        p.Fill();
    }
}
