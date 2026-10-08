using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildScreensFromImages
{
    const float W = 1024f;
    const float H = 1366f;
    const string SpritesFolder = "Assets/Sprites";

    class Btn { public float x0, y0, x1, y1; public string target; }
    class Txt { public float x0, y0, x1, y1; public int kind; }   // 0 = From/To, 1 = dia de devolucion
    class Def
    {
        public string name;
        public List<Btn> btns = new List<Btn>();
        public List<Txt> txts = new List<Txt>();
        public int calMode = -1;      // -1 = sin calendario, 0 = recogida, 1 = devolucion
        public string calNext;
    }

    static List<Def> defs;

    static Def D(string name)
    {
        var d = new Def { name = name };
        defs.Add(d);
        return d;
    }

    static void B(Def d, float x0, float y0, float x1, float y1, string target)
    {
        d.btns.Add(new Btn { x0 = x0, y0 = y0, x1 = x1, y1 = y1, target = target });
    }

    static void Back(Def d, string target)
    {
        B(d, 15, 25, 100, 105, target);
    }

    static void T(Def d, float x0, float y0, float x1, float y1, int kind)
    {
        d.txts.Add(new Txt { x0 = x0, y0 = y0, x1 = x1, y1 = y1, kind = kind });
    }

    static void Cal(Def d, int mode, string next)
    {
        d.calMode = mode;
        d.calNext = next;
    }

    static void DefineFlow()
    {
        defs = new List<Def>();
        Def d;

        d = D("01_Start");
        B(d, 140, 930, 882, 1097, "02_KitGrid");

        d = D("02_KitGrid");
        Back(d, "01_Start");
        B(d, 47, 66, 1003, 319, "03_KitSelected");
        B(d, 47, 404, 1003, 964, "04_KitLoanedSelected");
        B(d, 47, 1047, 1003, 1300, "04_KitLoanedSelected");

        d = D("03_KitSelected");
        Back(d, "02_KitGrid");
        B(d, 112, 1091, 911, 1224, "06_PickupDay");
        B(d, 73, 444, 950, 1070, "04_KitLoanedSelected");

        d = D("04_KitLoanedSelected");
        Back(d, "02_KitGrid");
        B(d, 73, 66, 950, 349, "03_KitSelected");
        B(d, 112, 1091, 911, 1224, "05_PopupKitInUse");

        d = D("05_PopupKitInUse");
        B(d, 785, 427, 868, 482, "04_KitLoanedSelected");
        Back(d, "04_KitLoanedSelected");

        d = D("06_PickupDay");
        Back(d, "03_KitSelected");
        Cal(d, 0, "07_DropDay");

        d = D("07_DropDay");
        Back(d, "06_PickupDay");
        Cal(d, 1, "09_Selections_Dates");

        d = D("09_Selections_Dates");
        Back(d, "07_DropDay");
        B(d, 438, 1095, 612, 1250, "11_HowMuchTime");
        T(d, 560, 856, 990, 922, 0);

        d = D("10_Selections_Dates_Confirm");
        Back(d, "07_DropDay");
        B(d, 136, 1095, 851, 1252, "11_HowMuchTime");
        T(d, 560, 856, 990, 922, 0);

        d = D("11_HowMuchTime");
        Back(d, "09_Selections_Dates");
        B(d, 212, 302, 782, 438, "12_HowMuchTime_90");
        B(d, 212, 531, 782, 667, "12_HowMuchTime_90");
        B(d, 212, 745, 782, 881, "12_HowMuchTime_90");
        B(d, 212, 959, 782, 1095, "12_HowMuchTime_90");

        d = D("12_HowMuchTime_90");
        Back(d, "09_Selections_Dates");
        B(d, 136, 1194, 872, 1308, "13_Selections_Time");

        d = D("13_Selections_Time");
        Back(d, "12_HowMuchTime_90");
        B(d, 438, 1095, 612, 1250, "15_Selections_Items");

        d = D("14_Selections_Time_Confirm");
        Back(d, "12_HowMuchTime_90");
        B(d, 136, 1095, 851, 1252, "15_Selections_Items");

        d = D("15_Selections_Items");
        Back(d, "13_Selections_Time");
        B(d, 383, 1095, 557, 1250, "16_Selections_Loading");

        d = D("16_Selections_Loading");
        Back(d, "15_Selections_Items");
        B(d, 425, 1180, 598, 1335, "17_Summary");

        d = D("17_Summary");
        Back(d, "16_Selections_Loading");
        B(d, 58, 1178, 231, 1332, "01_Start");
        B(d, 824, 1178, 998, 1332, "18_Thanks");
        T(d, 470, 626, 890, 708, 0);

        d = D("18_Thanks");
        B(d, 75, 1177, 998, 1332, "01_Start");
        T(d, 190, 838, 830, 906, 1);
    }

    [MenuItem("Tools/QuestKitLoan/Construir pantallas desde imagenes")]
    static void Build()
    {
        if (!AssetDatabase.IsValidFolder(SpritesFolder))
        {
            EditorUtility.DisplayDialog("Falta la carpeta",
                "No encuentro la carpeta Assets/Sprites. Extrae el Sprites.zip dentro de Assets.", "OK");
            return;
        }

        ConfigureSprites();
        DefineFlow();

        var old = GameObject.Find("Canvas");
        if (old != null) Undo.DestroyObjectImmediate(old);

        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2048, 2732);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        Undo.RegisterCreatedObjectUndo(canvasGO, "Crear Canvas");

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

        var screens = new List<GameObject>();
        foreach (var def in defs)
            screens.Add(CreateScreen(def, canvasGO.transform));

        var manager = canvasGO.AddComponent<ScreenManager>();
        manager.screens = screens.ToArray();
        manager.startScreen = "01_Start";

        foreach (var s in screens)
            s.SetActive(s.name == "01_Start");

        Selection.activeGameObject = canvasGO;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Pantallas creadas: " + screens.Count);
    }

    static Sprite LoadSprite(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(SpritesFolder + "/" + name + ".png");
    }

    static GameObject CreateScreen(Def def, Transform parent)
    {
        var go = new GameObject(def.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var img = go.GetComponent<Image>();
        var sprite = LoadSprite(def.name);
        if (sprite != null) img.sprite = sprite;
        else
        {
            img.color = Color.gray;
            Debug.LogWarning("No encontre la imagen: " + def.name + ".png en " + SpritesFolder);
        }

        int i = 0;
        foreach (var b in def.btns)
            CreateButton(go.transform, b, def.name + "_Btn" + (i++));

        i = 0;
        foreach (var t in def.txts)
            CreateText(go.transform, t, def.name + "_Text" + (i++));

        if (def.calMode >= 0)
        {
            var cc = go.AddComponent<CalendarController>();
            cc.mode = def.calMode == 0 ? CalendarController.Mode.Pickup : CalendarController.Mode.Drop;
            cc.nextScreen = def.calNext;
            cc.dotEndpoint = LoadSprite("Cal_DotEndpoint");
            cc.dotRange = LoadSprite("Cal_DotRange");
            cc.confirmSprite = LoadSprite("Cal_ConfirmDate");
        }

        return go;
    }

    static void SetAnchors(RectTransform rt, float x0, float y0, float x1, float y1)
    {
        rt.anchorMin = new Vector2(x0 / W, 1f - y1 / H);
        rt.anchorMax = new Vector2(x1 / W, 1f - y0 / H);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void CreateButton(Transform parent, Btn b, string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(NavButton));
        go.transform.SetParent(parent, false);
        SetAnchors(go.GetComponent<RectTransform>(), b.x0, b.y0, b.x1, b.y1);

        go.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        go.GetComponent<Button>().transition = Selectable.Transition.None;
        go.GetComponent<NavButton>().target = b.target;
    }

    static void CreateText(Transform parent, Txt t, string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(DateLabel));
        go.transform.SetParent(parent, false);
        SetAnchors(go.GetComponent<RectTransform>(), t.x0, t.y0, t.x1, t.y1);

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = t.kind == 0 ? "Fri 9 Oct to Wed 14 Oct" : "Building H, Wednesday 14 Oct";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 24;
        tmp.fontSizeMax = 64;
        tmp.raycastTarget = false;

        go.GetComponent<DateLabel>().kind = t.kind == 0 ? DateLabel.Kind.FromTo : DateLabel.Kind.ReturnDay;
    }

    static void ConfigureSprites()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { SpritesFolder }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) continue;
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.alphaIsTransparency = true;
            imp.mipmapEnabled = false;
            imp.maxTextureSize = 4096;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.SaveAndReimport();
        }
    }
}
