using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class CreateScreens
{
    static readonly string[] screenNames =
    {
        "Screen_Start",
        "Screen_KitSelect",
        "Screen_PickupDay",
        "Screen_DropDay",
        "Screen_HowMuchTime",
        "Screen_YourSelections",
        "Screen_Confirm",
        "Screen_ThanksLoan",
        "Popup_KitInUse_Popup",
        "Popup_DatesUnavailable_Popup"
    };

    [MenuItem("Tools/QuestKitLoan/Crear Canvas y Pantallas")]
    static void Create()
    {
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2048, 2732);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        Undo.RegisterCreatedObjectUndo(canvasGO, "Crear Canvas");

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

        var created = new GameObject[screenNames.Length];
        for (int i = 0; i < screenNames.Length; i++)
        {
            created[i] = CreatePanel(screenNames[i], canvasGO.transform);
        }

        var manager = canvasGO.AddComponent<ScreenManager>();
        manager.screens = created;

        for (int i = 0; i < created.Length; i++)
            created[i].SetActive(screenNames[i] == "Screen_Start");

        Selection.activeGameObject = canvasGO;
        EditorUtility.SetDirty(canvasGO);
        Debug.Log("Canvas y pantallas creados correctamente.");
    }

    static GameObject CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var img = go.GetComponent<Image>();
        bool isPopup = name.EndsWith("Popup");
        img.color = isPopup ? new Color(0f, 0f, 0f, 0.6f) : Color.white;

        var textGO = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(go.transform, false);
        var trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0, 1);
        trt.anchorMax = new Vector2(1, 1);
        trt.pivot = new Vector2(0.5f, 1);
        trt.anchoredPosition = new Vector2(0, -80);
        trt.sizeDelta = new Vector2(0, 200);

        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.fontSize = 80;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = isPopup ? Color.white : Color.black;

        return go;
    }
}
