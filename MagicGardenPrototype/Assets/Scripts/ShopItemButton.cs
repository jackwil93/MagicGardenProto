using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class ShopItemButton : MonoBehaviour {

    
    public ShopItem myShopItem;
    public Text itemName;
    public Text itemPrice;
    public Image itemImage;

    public enum saleType { buy, sell}
    public saleType buyOrSell; // Set to 'sell' by Shop when creating buttons in Sale window. Defaults to Buy.

    private void Start()
    {
        if (myShopItem != null)
            UpdateShopItemInfo();

        GetComponent<Button>().onClick.AddListener(OnClickCustom);

    }

    public void UpdateShopItemInfo()
    {
        itemName.text = myShopItem.mainGameItem.itemProperties.displayedName;
        int price = 0;

        if (buyOrSell == saleType.buy)
            price = myShopItem.mainGameItem.itemProperties.buyPriceFlorets;
        else if (myShopItem.mainGameItem.itemProperties.itemType == ItemProperties.itemTypes.potWithPlant)
        {
            // If Plant is at full stage, sells for full price. Otherwise only base price
            if (myShopItem.mainGameItem.itemProperties.currentStage == ItemProperties.itemStage.normal)
                price += myShopItem.mainGameItem.itemProperties.sellPriceFlorets;

            // Now add the pot
            price += myShopItem.secondaryGameItem.itemProperties.sellPriceFlorets;
        }
        else // Only selling Pot or Decor
            price = myShopItem.mainGameItem.itemProperties.sellPriceFlorets;

        itemPrice.text = price.ToString();

        itemImage.sprite = myShopItem.mainItemIcon;
    }
    

    public void OnClickCustom()
    {
        Shop shop = Shop.FindObjectOfType<Shop>();
        shop.InspectShopItem(this, myShopItem);

        Debug.Log("Inspecting Shop Item");
    }

    
}
