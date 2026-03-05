using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Clean Painter2D icon renderer. Zero emojis, zero textures.
/// Every icon hand-drawn with 1.7px stroke, rounded caps/joins.
/// </summary>
public class SvgIcon : VisualElement
{
    public enum Icon
    {
        AppHub, Music, Map, Gauge, Bolt, Stopwatch, Shield, Sliders,
        Sun, Moon,
        Battery, Range, Thermometer, Tire, Odometer, Trip, Consumption, Power,
        Lock, Home, Briefcase, ChargingPin, Flag, Search, ChevronRight
    }

    public Color  StrokeColor { get; set; } = Color.white;
    public float  StrokeWidth { get; set; } = 1.7f;
    public Icon   IconType    { get; set; } = Icon.Gauge;

    public SvgIcon() { generateVisualContent += Draw; }
    public SvgIcon(Icon icon, Color color, float sw = 1.7f)
    { IconType=icon; StrokeColor=color; StrokeWidth=sw; generateVisualContent+=Draw; }

    void Draw(MeshGenerationContext ctx)
    {
        float w=layout.width, h=layout.height;
        if(w<1||h<1) return;
        var p=ctx.painter2D;
        p.strokeColor=StrokeColor; p.fillColor=Color.clear;
        p.lineWidth=StrokeWidth; p.lineCap=LineCap.Round; p.lineJoin=LineJoin.Round;
        float sx=w/24f, sy=h/24f;
        switch(IconType){
            case Icon.AppHub:       DrawAppHub(p,sx,sy);      break;
            case Icon.Music:        DrawMusic(p,sx,sy);       break;
            case Icon.Map:          DrawMap(p,sx,sy);         break;
            case Icon.Gauge:        DrawGauge(p,sx,sy);       break;
            case Icon.Bolt:         DrawBolt(p,sx,sy);        break;
            case Icon.Stopwatch:    DrawStopwatch(p,sx,sy);   break;
            case Icon.Shield:       DrawShield(p,sx,sy);      break;
            case Icon.Sliders:      DrawSliders(p,sx,sy);     break;
            case Icon.Sun:          DrawSun(p,sx,sy);         break;
            case Icon.Moon:         DrawMoon(p,sx,sy);        break;
            case Icon.Battery:      DrawBattery(p,sx,sy);     break;
            case Icon.Range:        DrawRange(p,sx,sy);       break;
            case Icon.Thermometer:  DrawThermometer(p,sx,sy); break;
            case Icon.Tire:         DrawTire(p,sx,sy);        break;
            case Icon.Odometer:     DrawOdometer(p,sx,sy);    break;
            case Icon.Trip:         DrawTrip(p,sx,sy);        break;
            case Icon.Consumption:  DrawConsumption(p,sx,sy); break;
            case Icon.Power:        DrawPower(p,sx,sy);       break;
            case Icon.Lock:         DrawLock(p,sx,sy);        break;
            case Icon.Home:         DrawHome(p,sx,sy);        break;
            case Icon.Briefcase:    DrawBriefcase(p,sx,sy);   break;
            case Icon.ChargingPin:  DrawChargingPin(p,sx,sy); break;
            case Icon.Flag:         DrawFlag(p,sx,sy);        break;
            case Icon.Search:       DrawSearch(p,sx,sy);      break;
            case Icon.ChevronRight: DrawChevron(p,sx,sy);     break;
        }
    }

    Vector2 S(float x,float y,float sx,float sy)=>new Vector2(x*sx,y*sy);

    // ── SIDEBAR ──────────────────────────────────────────────────
    void DrawAppHub(Painter2D p,float sx,float sy){
        DrawRoundRect(p,3,3,7,7,1.5f,sx,sy);
        DrawRoundRect(p,14,3,7,7,1.5f,sx,sy);
        DrawRoundRect(p,3,14,7,7,1.5f,sx,sy);
        DrawRoundRect(p,14,14,7,7,1.5f,sx,sy);
    }
    void DrawMusic(Painter2D p,float sx,float sy){
        p.BeginPath(); p.MoveTo(S(9,18,sx,sy)); p.LineTo(S(9,5,sx,sy));
        p.LineTo(S(21,3,sx,sy)); p.LineTo(S(21,16,sx,sy)); p.Stroke();
        Circle(p,6,18,3,sx,sy); Circle(p,18,16,3,sx,sy);
    }
    void DrawMap(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(1,6,sx,sy)); p.LineTo(S(1,22,sx,sy)); p.LineTo(S(8,18,sx,sy));
        p.LineTo(S(16,22,sx,sy)); p.LineTo(S(23,18,sx,sy)); p.LineTo(S(23,2,sx,sy));
        p.LineTo(S(16,6,sx,sy)); p.LineTo(S(8,2,sx,sy)); p.ClosePath(); p.Stroke();
        p.BeginPath(); p.MoveTo(S(8,2,sx,sy)); p.LineTo(S(8,18,sx,sy)); p.Stroke();
        p.BeginPath(); p.MoveTo(S(16,6,sx,sy)); p.LineTo(S(16,22,sx,sy)); p.Stroke();
    }
    void DrawGauge(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.Arc(S(12,13,sx,sy),8*sx,200*Mathf.Deg2Rad,340*Mathf.Deg2Rad);
        p.Stroke();
        float[] ta={200f,270f,340f};
        foreach(float t in ta){
            float r=t*Mathf.Deg2Rad;
            p.lineWidth=StrokeWidth*1.1f;
            p.BeginPath();
            p.MoveTo(S(12+Mathf.Cos(r)*6,13+Mathf.Sin(r)*6,sx,sy));
            p.LineTo(S(12+Mathf.Cos(r)*8,13+Mathf.Sin(r)*8,sx,sy));
            p.Stroke();
        }
        p.lineWidth=StrokeWidth;
        float na=265*Mathf.Deg2Rad;
        p.BeginPath(); p.MoveTo(S(12,13,sx,sy));
        p.LineTo(S(12+Mathf.Cos(na)*6.5f,13+Mathf.Sin(na)*6.5f,sx,sy)); p.Stroke();
        p.fillColor=StrokeColor;
        p.BeginPath(); p.Arc(S(12,13,sx,sy),1.5f*sx,0,Mathf.PI*2); p.Fill();
        p.fillColor=Color.clear;
    }
    void DrawBolt(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(13,2,sx,sy)); p.LineTo(S(3,14,sx,sy));
        p.LineTo(S(12,14,sx,sy)); p.LineTo(S(11,22,sx,sy));
        p.LineTo(S(21,10,sx,sy)); p.LineTo(S(12,10,sx,sy));
        p.ClosePath(); p.Stroke();
    }
    void DrawStopwatch(Painter2D p,float sx,float sy){
        Circle(p,12,13.5f,8.5f,sx,sy);
        p.BeginPath(); p.MoveTo(S(9,2,sx,sy)); p.LineTo(S(15,2,sx,sy)); p.Stroke();
        p.BeginPath(); p.MoveTo(S(12,2,sx,sy)); p.LineTo(S(12,5,sx,sy)); p.Stroke();
        p.BeginPath(); p.MoveTo(S(18.5f,5.5f,sx,sy)); p.LineTo(S(20,4,sx,sy)); p.Stroke();
        p.lineWidth=StrokeWidth*1.1f;
        p.BeginPath(); p.MoveTo(S(12,13.5f,sx,sy)); p.LineTo(S(12,8.5f,sx,sy)); p.Stroke();
        p.lineWidth=StrokeWidth;
        p.BeginPath(); p.MoveTo(S(12,13.5f,sx,sy)); p.LineTo(S(15.5f,13.5f,sx,sy)); p.Stroke();
        p.fillColor=StrokeColor;
        p.BeginPath(); p.Arc(S(12,13.5f,sx,sy),1.3f*sx,0,Mathf.PI*2); p.Fill();
        p.fillColor=Color.clear;
    }
    void DrawShield(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(12,2,sx,sy)); p.LineTo(S(22,6,sx,sy)); p.LineTo(S(22,12,sx,sy));
        p.BezierCurveTo(S(22,17,sx,sy),S(17,21,sx,sy),S(12,22,sx,sy));
        p.BezierCurveTo(S(7,21,sx,sy),S(2,17,sx,sy),S(2,12,sx,sy));
        p.LineTo(S(2,6,sx,sy)); p.ClosePath(); p.Stroke();
        p.BeginPath();
        p.MoveTo(S(8.5f,12,sx,sy)); p.LineTo(S(11,14.5f,sx,sy)); p.LineTo(S(15.5f,9.5f,sx,sy));
        p.Stroke();
    }
    void DrawSliders(Painter2D p,float sx,float sy){
        float[] ys={5f,12f,19f}; float[] xs={10f,6f,14f};
        for(int i=0;i<3;i++){
            p.BeginPath(); p.MoveTo(S(3,ys[i],sx,sy)); p.LineTo(S(21,ys[i],sx,sy)); p.Stroke();
            Circle(p,xs[i],ys[i],2.5f,sx,sy);
        }
    }
    void DrawSun(Painter2D p,float sx,float sy){
        Circle(p,12,12,4.5f,sx,sy);
        float[] dirs={0,45,90,135,180,225,270,315};
        foreach(float d in dirs){
            float a=d*Mathf.Deg2Rad;
            p.BeginPath();
            p.MoveTo(S(12+Mathf.Cos(a)*6.5f,12+Mathf.Sin(a)*6.5f,sx,sy));
            p.LineTo(S(12+Mathf.Cos(a)*8.5f,12+Mathf.Sin(a)*8.5f,sx,sy));
            p.Stroke();
        }
    }
    void DrawMoon(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(21,12.79f,sx,sy));
        p.BezierCurveTo(S(21,17.4f,sx,sy),S(17,21,sx,sy),S(12,21,sx,sy));
        p.BezierCurveTo(S(7,21,sx,sy),S(3,17.4f,sx,sy),S(3,12,sx,sy));
        p.BezierCurveTo(S(3,7.6f,sx,sy),S(6.6f,4,sx,sy),S(11.21f,3,sx,sy));
        p.BezierCurveTo(S(7.5f,6,sx,sy),S(7.5f,18,sx,sy),S(12,20,sx,sy));
        p.BezierCurveTo(S(16.5f,18,sx,sy),S(20,14,sx,sy),S(21,12.79f,sx,sy));
        p.ClosePath(); p.Stroke();
    }

    // ── CARD ICONS ───────────────────────────────────────────────
    void DrawBattery(Painter2D p,float sx,float sy){
        DrawRoundRect(p,2,7,17,10,1.5f,sx,sy);
        p.BeginPath(); p.MoveTo(S(21,10,sx,sy)); p.LineTo(S(21,14,sx,sy)); p.Stroke();
        p.fillColor=StrokeColor; DrawRRFill(p,4,9.5f,9,5,1f,sx,sy); p.fillColor=Color.clear;
    }
    void DrawRange(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(13,2,sx,sy)); p.LineTo(S(3,14,sx,sy)); p.LineTo(S(12,14,sx,sy));
        p.LineTo(S(11,22,sx,sy)); p.LineTo(S(21,10,sx,sy)); p.LineTo(S(12,10,sx,sy));
        p.ClosePath(); p.Stroke();
    }
    void DrawThermometer(Painter2D p,float sx,float sy){
        Circle(p,12,18,3.5f,sx,sy);
        p.BeginPath();
        p.MoveTo(S(9.5f,18,sx,sy)); p.LineTo(S(9.5f,8,sx,sy));
        p.Arc(S(12,8,sx,sy),2.5f*sx,Mathf.PI,0);
        p.LineTo(S(14.5f,18,sx,sy)); p.Stroke();
        for(int i=0;i<3;i++){
            float ty=10f+i*2.5f;
            p.BeginPath(); p.MoveTo(S(14.5f,ty,sx,sy)); p.LineTo(S(16f,ty,sx,sy)); p.Stroke();
        }
    }
    void DrawTire(Painter2D p,float sx,float sy){
        Circle(p,12,12,9,sx,sy); Circle(p,12,12,4,sx,sy);
        for(int i=0;i<6;i++){
            float a=i*60f*Mathf.Deg2Rad;
            p.BeginPath();
            p.MoveTo(S(12+Mathf.Cos(a)*5,12+Mathf.Sin(a)*5,sx,sy));
            p.LineTo(S(12+Mathf.Cos(a)*8.5f,12+Mathf.Sin(a)*8.5f,sx,sy));
            p.Stroke();
        }
    }
    void DrawOdometer(Painter2D p,float sx,float sy){
        DrawRoundRect(p,2,6,20,12,2f,sx,sy);
        p.lineWidth=StrokeWidth*0.75f;
        float[] divs={8f,12f,16f};
        foreach(float x in divs){
            p.BeginPath(); p.MoveTo(S(x,6,sx,sy)); p.LineTo(S(x,18,sx,sy)); p.Stroke();
        }
        p.lineWidth=StrokeWidth;
        p.BeginPath(); p.MoveTo(S(6,21,sx,sy)); p.LineTo(S(18,21,sx,sy)); p.Stroke();
    }
    void DrawTrip(Painter2D p,float sx,float sy){
        p.BeginPath(); p.MoveTo(S(5,19,sx,sy)); p.LineTo(S(19,5,sx,sy)); p.Stroke();
        p.BeginPath();
        p.MoveTo(S(6,5,sx,sy)); p.LineTo(S(19,5,sx,sy)); p.LineTo(S(19,18,sx,sy));
        p.Stroke();
    }
    void DrawConsumption(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(12,22,sx,sy));
        p.BezierCurveTo(S(4,22,sx,sy),S(2,13,sx,sy),S(6,7,sx,sy));
        p.BezierCurveTo(S(10,2,sx,sy),S(19,2,sx,sy),S(21,8,sx,sy));
        p.BezierCurveTo(S(22,14,sx,sy),S(18,22,sx,sy),S(12,22,sx,sy));
        p.Stroke();
        p.BeginPath(); p.MoveTo(S(12,22,sx,sy)); p.LineTo(S(12,6,sx,sy)); p.Stroke();
        p.BeginPath(); p.MoveTo(S(7,12,sx,sy)); p.LineTo(S(17,12,sx,sy)); p.Stroke();
    }
    void DrawPower(Painter2D p,float sx,float sy){
        p.BeginPath(); p.MoveTo(S(12,2,sx,sy)); p.LineTo(S(12,12,sx,sy)); p.Stroke();
        p.BeginPath();
        p.Arc(S(12,12,sx,sy),7.5f*sx,-150*Mathf.Deg2Rad,-30*Mathf.Deg2Rad);
        p.Stroke();
    }
    void DrawLock(Painter2D p,float sx,float sy){
        DrawRoundRect(p,3,11,18,11,2f,sx,sy);
        p.BeginPath();
        p.MoveTo(S(7,11,sx,sy)); p.LineTo(S(7,7,sx,sy));
        p.Arc(S(12,7,sx,sy),5*sx,Mathf.PI,0);
        p.LineTo(S(17,11,sx,sy)); p.Stroke();
        p.fillColor=StrokeColor;
        p.BeginPath(); p.Arc(S(12,16,sx,sy),1.8f*sx,0,Mathf.PI*2); p.Fill();
        p.fillColor=Color.clear;
        p.BeginPath();
        p.MoveTo(S(11.2f,17.5f,sx,sy)); p.LineTo(S(12.8f,17.5f,sx,sy));
        p.LineTo(S(12.4f,20f,sx,sy)); p.LineTo(S(11.6f,20f,sx,sy)); p.ClosePath(); p.Stroke();
    }

    // ── NAV PLACE ICONS ──────────────────────────────────────────
    void DrawHome(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(3,10,sx,sy)); p.LineTo(S(12,3,sx,sy)); p.LineTo(S(21,10,sx,sy)); p.Stroke();
        p.BeginPath();
        p.MoveTo(S(5,10,sx,sy)); p.LineTo(S(5,21,sx,sy)); p.LineTo(S(9,21,sx,sy));
        p.LineTo(S(9,15,sx,sy)); p.LineTo(S(15,15,sx,sy)); p.LineTo(S(15,21,sx,sy));
        p.LineTo(S(19,21,sx,sy)); p.LineTo(S(19,10,sx,sy)); p.Stroke();
    }
    void DrawBriefcase(Painter2D p,float sx,float sy){
        DrawRoundRect(p,2,7,20,14,2f,sx,sy);
        p.BeginPath();
        p.MoveTo(S(16,7,sx,sy)); p.LineTo(S(16,4,sx,sy));
        p.LineTo(S(8,4,sx,sy)); p.LineTo(S(8,7,sx,sy)); p.Stroke();
        p.BeginPath(); p.MoveTo(S(2,14,sx,sy)); p.LineTo(S(22,14,sx,sy)); p.Stroke();
    }
    void DrawChargingPin(Painter2D p,float sx,float sy){
        p.BeginPath();
        p.MoveTo(S(12,22,sx,sy));
        p.BezierCurveTo(S(12,22,sx,sy),S(5,15,sx,sy),S(5,10,sx,sy));
        p.Arc(S(12,10,sx,sy),7*sx,Mathf.PI,0);
        p.BezierCurveTo(S(19,15,sx,sy),S(12,22,sx,sy),S(12,22,sx,sy));
        p.Stroke();
        p.BeginPath();
        p.MoveTo(S(13.5f,6.5f,sx,sy)); p.LineTo(S(10,10.5f,sx,sy));
        p.LineTo(S(12.5f,10.5f,sx,sy)); p.LineTo(S(10.5f,13.5f,sx,sy));
        p.LineTo(S(14,9.5f,sx,sy)); p.LineTo(S(11.5f,9.5f,sx,sy));
        p.ClosePath(); p.Stroke();
    }
    void DrawFlag(Painter2D p,float sx,float sy){
        p.BeginPath(); p.MoveTo(S(4,3,sx,sy)); p.LineTo(S(4,22,sx,sy)); p.Stroke();
        p.BeginPath();
        p.MoveTo(S(4,3,sx,sy)); p.LineTo(S(19,3,sx,sy)); p.LineTo(S(15,9,sx,sy));
        p.LineTo(S(19,15,sx,sy)); p.LineTo(S(4,15,sx,sy)); p.Stroke();
    }
    void DrawSearch(Painter2D p,float sx,float sy){
        Circle(p,11,11,7,sx,sy);
        p.BeginPath(); p.MoveTo(S(16.5f,16.5f,sx,sy)); p.LineTo(S(21,21,sx,sy)); p.Stroke();
    }
    void DrawChevron(Painter2D p,float sx,float sy){
        p.BeginPath(); p.MoveTo(S(9,18,sx,sy)); p.LineTo(S(15,12,sx,sy)); p.LineTo(S(9,6,sx,sy)); p.Stroke();
    }

    // ── HELPERS ──────────────────────────────────────────────────
    void Circle(Painter2D p,float cx,float cy,float r,float sx,float sy){
        p.BeginPath(); p.Arc(new Vector2(cx*sx,cy*sy),r*sx,0,Mathf.PI*2); p.Stroke();
    }
    void DrawRoundRect(Painter2D p,float x,float y,float w,float h,float r,float sx,float sy){
        float rx=x*sx,ry=y*sy,rw=w*sx,rh=h*sy,rr=r*sx;
        p.BeginPath();
        p.MoveTo(new Vector2(rx+rr,ry));
        p.LineTo(new Vector2(rx+rw-rr,ry));
        p.Arc(new Vector2(rx+rw-rr,ry+rr),rr,-Mathf.PI/2,0);
        p.LineTo(new Vector2(rx+rw,ry+rh-rr));
        p.Arc(new Vector2(rx+rw-rr,ry+rh-rr),rr,0,Mathf.PI/2);
        p.LineTo(new Vector2(rx+rr,ry+rh));
        p.Arc(new Vector2(rx+rr,ry+rh-rr),rr,Mathf.PI/2,Mathf.PI);
        p.LineTo(new Vector2(rx,ry+rr));
        p.Arc(new Vector2(rx+rr,ry+rr),rr,Mathf.PI,Mathf.PI*3/2);
        p.ClosePath(); p.Stroke();
    }
    void DrawRRFill(Painter2D p,float x,float y,float w,float h,float r,float sx,float sy){
        float rx=x*sx,ry=y*sy,rw=w*sx,rh=h*sy,rr=r*sx;
        p.BeginPath();
        p.MoveTo(new Vector2(rx+rr,ry));
        p.LineTo(new Vector2(rx+rw-rr,ry));
        p.Arc(new Vector2(rx+rw-rr,ry+rr),rr,-Mathf.PI/2,0);
        p.LineTo(new Vector2(rx+rw,ry+rh-rr));
        p.Arc(new Vector2(rx+rw-rr,ry+rh-rr),rr,0,Mathf.PI/2);
        p.LineTo(new Vector2(rx+rr,ry+rh));
        p.Arc(new Vector2(rx+rr,ry+rh-rr),rr,Mathf.PI/2,Mathf.PI);
        p.LineTo(new Vector2(rx,ry+rr));
        p.Arc(new Vector2(rx+rr,ry+rr),rr,Mathf.PI,Mathf.PI*3/2);
        p.ClosePath(); p.Fill();
    }
}
