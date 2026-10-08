using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject[] screens;
    public string startScreen = "01_Start";

    void Start()
    {
        Show(startScreen);
    }

    public void Show(string screenName)
    {
        foreach (var s in screens)
        {
            if (s == null) continue;
            s.SetActive(s.name == screenName);
        }
    }

    public void ShowPopup(string popupName)
    {
        foreach (var s in screens)
        {
            if (s != null && s.name == popupName) s.SetActive(true);
        }
    }

    public void ClosePopup(string popupName)
    {
        foreach (var s in screens)
        {
            if (s != null && s.name == popupName) s.SetActive(false);
        }
    }
}
