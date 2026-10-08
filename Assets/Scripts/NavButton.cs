using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NavButton : MonoBehaviour
{
    public string target;

    void Awake()
    {
        var manager = GetComponentInParent<ScreenManager>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (manager == null || string.IsNullOrEmpty(target)) return;

            if (target == "01_Start") DateSelection.Clear();

            manager.Show(target);
        });
    }
}
