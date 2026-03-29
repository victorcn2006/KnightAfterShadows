using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    private ItemDefinition _item;
    private Action<int> _onBuy;
    private Action<int> _onSell;

    public void Setup(ItemDefinition item, Sprite sprite, Action<int> onBuy, Action<int> onSell)
    {
        _item = item;
        _onBuy = onBuy;
        _onSell = onSell;

        itemIcon.sprite = sprite;
        priceText.text = item.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => _onBuy?.Invoke(_item.id));

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => _onSell?.Invoke(_item.id));
    }
}
