using UnityEngine;

public class ShopItemHandler : MonoBehaviour
{
    [Header("Item Config")]
    public int itemId;
    public int buyPrice;
    public int sellPrice;

    public void Buy()
    {
        ExecutePurchase(false);
    }

    public void BuyWithFailError()
    {
        ExecutePurchase(true);
    }

    private void ExecutePurchase(bool forceError)
    {
        string username = Systems.instance.CurrentUsername;
        if (string.IsNullOrEmpty(username)) username = "testuser";

        var user = SQLiteReader.instance.GetAllUsers().Find(u => u.username == username);
        if (user == null) return;

        var player = SQLiteReader.instance.GetPlayerByUserId(user.id);
        
        if (SQLiteReader.instance.BuyItem(player.id, itemId, 1, buyPrice, forceError))
        {
            if (ShopInventoryController.instance != null)
                ShopInventoryController.instance.ShowMessage("Purchase Successful!");
        }
        else
        {
            if (ShopInventoryController.instance != null)
                ShopInventoryController.instance.ShowMessage("Rollback: Data Protected!", true);
        }

        if (ShopInventoryController.instance != null)
            ShopInventoryController.instance.RefreshUI();
    }

    public void Sell()
    {
        string username = Systems.instance.CurrentUsername;
        if (string.IsNullOrEmpty(username)) username = "testuser";

        var user = SQLiteReader.instance.GetAllUsers().Find(u => u.username == username);
        if (user == null) return;

        var player = SQLiteReader.instance.GetPlayerByUserId(user.id);
        
        if (SQLiteReader.instance.SellItem(player.id, itemId, 1, sellPrice))
        {
            if (ShopInventoryController.instance != null)
            {
                ShopInventoryController.instance.ShowMessage("Item Sold!");
                ShopInventoryController.instance.RefreshUI();
            }
        }
    }
}
