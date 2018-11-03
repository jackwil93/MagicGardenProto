using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CurrencyManagement;

public class Shop : MonoBehaviour {
    Inventory inv;

    [Header("Screen UI")]
    public Text floretsUI;
    public Text crystalsUI;

    public GameObject shopItemButtonPrefab;
    public ShopItem currentItem;

    public List<ShopItem> seedShopItems = new List<ShopItem>();
    public List<ShopItem> potShopItems = new List<ShopItem>();
    public List<ShopItem> decorShopItems = new List<ShopItem>();

    [Space(20)]
    public Transform seedShopContent;
    public Transform potShopContent;
    public Transform decorShopContent;


    private void Start()
    {
        inv = Inventory.FindObjectOfType<Inventory>();
        CreateButtons();

        Invoke("UpdateCurrenciesUI", 0.1f);
    }

    void CreateButtons()
    {
        foreach (ShopItem seedItem in seedShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, seedShopContent);
            newItem.GetComponent<ShopItemButton>().myShopItem = seedItem;
        }

        foreach (ShopItem potItem in potShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, potShopContent);
            newItem.GetComponent<ShopItemButton>().myShopItem = potItem;
        }

        foreach (ShopItem decorItem in decorShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, decorShopContent);
            newItem.GetComponent<ShopItemButton>().myShopItem = decorItem;
        }
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
