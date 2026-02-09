using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextColorChanger : MonoBehaviour {
    [Header("Text Color configuration")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    private TextMeshProUGUI textToChange;

    private void Awake() {
        textToChange = GetComponentInChildren<TextMeshProUGUI>();
        if (textToChange != null) return;
    }
    private void Update() {
        if (textToChange == null) return;
        //Selected color
        if (EventSystem.current.currentSelectedGameObject == this.gameObject) {
            textToChange.color = selectedColor;
        }
        //Default color
        else textToChange.color = normalColor;
    }
}
