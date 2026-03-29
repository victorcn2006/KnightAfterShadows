using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopInventoryController : MonoBehaviour
{
    public static ShopInventoryController instance;

    [Header("Inventory Display")]
    [SerializeField] private InventorySlot[] slots; // The 6-20 slots in the middle
    [SerializeField] private TextMeshProUGUI moneyText; // The text showing "0" or "1000"

    [Header("Assets")]
    [SerializeField] private SpriteLibrary spriteLibrary;

    [Header("UI Feedback")]
    [SerializeField] private TextMeshProUGUI statusText; // A text at the bottom or top of the screen
    [SerializeField] private float messageDuration = 2f;

    private Coroutine _messageCoroutine;

    private void Awake()
    {
        instance = this;
        if (statusText != null) statusText.text = "";
    }

    public void ShowMessage(string message, bool isError = false)
    {
        if (statusText == null) return;

        if (_messageCoroutine != null) StopCoroutine(_messageCoroutine);
        _messageCoroutine = StartCoroutine(DisplayMessage(message, isError ? Color.red : Color.green));
    }

    private System.Collections.IEnumerator DisplayMessage(string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
        yield return new WaitForSeconds(messageDuration);
        statusText.text = "";
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        string username = Systems.instance.CurrentUsername;
        if (string.IsNullOrEmpty(username)) username = "testuser"; // Fallback for testing

        // Fetch user/player
        var user = SQLiteReader.instance.GetAllUsers().FirstOrDefault(u => u.username == username);
        if (user == null) return;

        var player = SQLiteReader.instance.GetPlayerByUserId(user.id);
        if (moneyText != null) moneyText.text = player.diners.ToString();

        // Refresh Inventory Slots
        var inventory = SQLiteReader.instance.GetPlayerInventory(player.id);
        var itemDefinitions = SQLiteReader.instance.GetAllItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (i < inventory.Count)
            {
                var invItem = inventory[i];
                var def = itemDefinitions.FirstOrDefault(d => d.id == invItem.item_id);
                if (def != null)
                {
                    slots[i].SetItem(spriteLibrary.GetSprite(def.sprite_name), invItem.quantity);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
