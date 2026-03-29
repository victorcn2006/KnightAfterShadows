using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItem(Sprite sprite, int quantity)
    {
        if (itemIcon == null) return;
        
        if (sprite != null)
        {
            itemIcon.sprite = sprite;
            itemIcon.enabled = true;
            
            if (quantityText != null)
            {
                quantityText.text = quantity > 1 ? quantity.ToString() : "";
                quantityText.gameObject.SetActive(quantity > 1);
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        if (itemIcon == null) return;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(false);
        }
    }
}
