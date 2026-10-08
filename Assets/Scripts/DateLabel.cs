using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DateLabel : MonoBehaviour
{
    public enum Kind { FromTo, ReturnDay }
    public Kind kind;

    TextMeshProUGUI tmp;

    void OnEnable()
    {
        DateSelection.Changed += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        DateSelection.Changed -= Refresh;
    }

    void Refresh()
    {
        if (tmp == null) tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = kind == Kind.FromTo
            ? DateSelection.FromToText()
            : "Building H, " + DateSelection.ReturnText();
    }
}
