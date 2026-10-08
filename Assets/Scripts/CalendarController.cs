using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public enum Mode { Pickup, Drop }

    public Mode mode;
    public string nextScreen;
    public Sprite dotEndpoint;
    public Sprite dotRange;
    public Sprite confirmSprite;

    const float W = 1024f;
    const float H = 1366f;
    static readonly float[] CellX = { 216, 330, 444, 558, 672, 786 };
    static readonly float[] CellY = { 571, 771, 971 };
    const float CellHalf = 45f;

    class Cell
    {
        public DateTime date;
        public Image sel;
        public TextMeshProUGUI label;
    }

    readonly List<Cell> cells = new List<Cell>();
    TextMeshProUGUI status;
    GameObject confirm;
    ScreenManager manager;
    bool built;

    void Awake()
    {
        Build();
    }

    void OnEnable()
    {
        DateSelection.Changed += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        DateSelection.Changed -= Refresh;
    }

    static void SetRect(RectTransform rt, float x0, float y0, float x1, float y1)
    {
        rt.anchorMin = new Vector2(x0 / W, 1f - y1 / H);
        rt.anchorMax = new Vector2(x1 / W, 1f - y0 / H);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void Build()
    {
        if (built) return;
        built = true;
        manager = GetComponentInParent<ScreenManager>();

        DateTime today = DateTime.Today;
        DateTime monday = today.DayOfWeek == DayOfWeek.Sunday
            ? today.AddDays(1)
            : today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 6; c++)
            {
                var cell = new Cell { date = monday.AddDays(r * 7 + c) };
                float cx = CellX[c];
                float cy = CellY[r];

                var go = new GameObject("Day_" + cell.date.ToString("MMdd"),
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                go.transform.SetParent(transform, false);
                SetRect(go.GetComponent<RectTransform>(), cx - CellHalf, cy - CellHalf, cx + CellHalf, cy + CellHalf);
                go.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                var btn = go.GetComponent<Button>();
                btn.transition = Selectable.Transition.None;
                int index = cells.Count;
                btn.onClick.AddListener(() => OnCell(index));

                var selGO = new GameObject("Sel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                selGO.transform.SetParent(go.transform, false);
                var srt = selGO.GetComponent<RectTransform>();
                srt.anchorMin = new Vector2(0.5f - 37.5f / 90f, 0.5f - 33.5f / 90f);
                srt.anchorMax = new Vector2(0.5f + 37.5f / 90f, 0.5f + 33.5f / 90f);
                srt.offsetMin = Vector2.zero;
                srt.offsetMax = Vector2.zero;
                cell.sel = selGO.GetComponent<Image>();
                cell.sel.raycastTarget = false;
                cell.sel.enabled = false;

                var lblGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                lblGO.transform.SetParent(go.transform, false);
                var lrt = lblGO.GetComponent<RectTransform>();
                lrt.anchorMin = Vector2.zero;
                lrt.anchorMax = Vector2.one;
                lrt.offsetMin = Vector2.zero;
                lrt.offsetMax = Vector2.zero;
                cell.label = lblGO.GetComponent<TextMeshProUGUI>();
                cell.label.text = cell.date.Day.ToString();
                cell.label.fontSize = 40;
                cell.label.alignment = TextAlignmentOptions.Center;
                cell.label.raycastTarget = false;

                cells.Add(cell);
            }
        }

        var sgo = new GameObject("Status", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        sgo.transform.SetParent(transform, false);
        SetRect(sgo.GetComponent<RectTransform>(), 176, 292, 833, 401);
        status = sgo.GetComponent<TextMeshProUGUI>();
        status.alignment = TextAlignmentOptions.Center;
        status.color = Color.white;
        status.enableAutoSizing = true;
        status.fontSizeMin = 20;
        status.fontSizeMax = 60;
        status.raycastTarget = false;

        confirm = new GameObject("ConfirmDate",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        confirm.transform.SetParent(transform, false);
        SetRect(confirm.GetComponent<RectTransform>(), 264, 1192, 760, 1342);
        var cimg = confirm.GetComponent<Image>();
        if (confirmSprite != null) cimg.sprite = confirmSprite;
        else cimg.color = new Color(0.57f, 1f, 0.72f);
        var cbtn = confirm.GetComponent<Button>();
        cbtn.transition = Selectable.Transition.None;
        cbtn.onClick.AddListener(() =>
        {
            if (manager != null && !string.IsNullOrEmpty(nextScreen)) manager.Show(nextScreen);
        });
    }

    void OnCell(int i)
    {
        DateTime date = cells[i].date;
        if (date < DateTime.Today) return;

        if (mode == Mode.Pickup)
        {
            DateSelection.SetPickup(date);
        }
        else
        {
            var p = DateSelection.Pickup;
            if (p.HasValue && date <= p.Value)
            {
                status.text = "Drop day must be after pickup";
                return;
            }
            DateSelection.SetDrop(date);
        }
    }

    void Refresh()
    {
        if (!built) return;

        DateTime? p = DateSelection.Pickup;
        DateTime? d = DateSelection.Drop;
        DateTime today = DateTime.Today;

        foreach (var cell in cells)
        {
            bool isPickup = p.HasValue && cell.date == p.Value;
            bool isDrop = mode == Mode.Drop && d.HasValue && cell.date == d.Value;
            bool inRange = mode == Mode.Drop && p.HasValue && d.HasValue && cell.date > p.Value && cell.date < d.Value;
            bool endpoint = isPickup || isDrop;

            bool disabledDay = cell.date < today || (mode == Mode.Drop && p.HasValue && cell.date <= p.Value && !isPickup);

            cell.sel.enabled = endpoint || inRange;
            cell.sel.sprite = endpoint ? dotEndpoint : dotRange;

            if (endpoint) cell.label.color = Color.white;
            else if (disabledDay) cell.label.color = new Color(0.62f, 0.58f, 0.58f);
            else cell.label.color = new Color(0.22f, 0.2f, 0.2f);
        }

        if (mode == Mode.Pickup)
        {
            status.text = p.HasValue
                ? "Pickup: " + DateSelection.Short(p.Value)
                : "Choose your pickup day";
            confirm.SetActive(p.HasValue);
        }
        else
        {
            string left = p.HasValue ? "Pickup " + DateSelection.Short(p.Value) : "Pickup -";
            status.text = d.HasValue
                ? left + "  |  Drop " + DateSelection.Short(d.Value)
                : left + "  |  Choose your drop day";
            confirm.SetActive(d.HasValue);
        }
    }
}
