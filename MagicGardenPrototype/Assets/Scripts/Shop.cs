using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CurrencyManagement;

public class Shop : MonoBehaviour {
    [Header("Screen UI")]
    public Text floretsUI;
    public Text crystalsUI;

    public ShopItem currentItem;
    Inventory inv;

    private void Start()
    {
        inv = Inventory.FindObjectOfType<Inventory>();
        Invoke("UpdateCurrenciesUI", 0.1f);
    }

    public void BuyItemFlorets()
    {
        if (inv.CheckIfFreeSlot(currentItem.gameItem))
        {
            if (Currencies.SubtractFlorets(currentItem.buyPriceFlorets))
            {
                inv.AddItemToInventory(currentItem.gameItem);
                UpdateCurrenciesUI();
            }
        }
        else
            Debug.LogWarning("No Free Slots to purchase this item");
    }


    public void UpdateCurrenciesUI()
    {
        floretsUI.text = Currencies.florets + "f";
        crystalsUI.text = Currencies.crystals + " Crystals ";
    }

}
