using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class ColorChanger : MonoBehaviour, ISelectHandler, IDeselectHandler{
    private TMP_Text targetText;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    private void Awake() {
        targetText = GetComponentInChildren<TMP_Text>();
    }
    public void OnSelect(BaseEventData eventData) {
        targetText.color = selectedColor;
    }
    public void OnDeselect(BaseEventData eventData) {
        targetText.color = normalColor;
    }
}
