using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    public void UpdateSlot(Item item) {
        if (item != null)
        {
            // itemIcon.sprite = GetSpriteForItem(item.itemType);
            amountText.text = item.amount > 1 ? item.amount.ToString() : "";
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false); // empty slot
        }
    }
}
