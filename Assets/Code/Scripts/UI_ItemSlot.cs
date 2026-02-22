using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    private Item currentItem;
    private Button slotButton;
    private Image slotBackground;
    private int slotIndex;

    private void Awake() {
        slotButton = GetComponent<Button>();
        slotBackground = GetComponent<Image>();

        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    public void UpdateSlot(Item item) {
        currentItem = item;

        if (item != null)
        {
            // Show item icon
            if (itemIcon != null)
            {
                itemIcon.sprite = item.GetIcon();
                itemIcon.enabled = true;
            }

            // Show amount if more than 1
            if (amountText != null)
            {
                amountText.text = item.amount > 1 ? item.amount.ToString() : "";
            }
        }
        else
        {
            // Empty slot
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
            if (amountText != null)
            {
                amountText.text = "";
            }
        }
    }

    public void SetSelected(bool isSelected) {
        if (slotBackground == null) return;
        slotBackground.color = isSelected ? selectedColor : normalColor;
    }

    public Item GetItem() {
        return currentItem;
    }

    private void OnSlotClicked() {
        // Notify parent inventory that this slot was clicked
        var uiInventory = GetComponentInParent<UI_Inventory>();
        if (uiInventory != null)
        {
            uiInventory.OnSlotClicked(this);
        }
    }
}
