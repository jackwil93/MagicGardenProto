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
        itemName.text = myShopItem.gameItem.itemProperties.displayedName;

        if (buyOrSell == saleType.buy)
            itemPrice.text = myShopItem.gameItem.itemProperties.buyPriceFlorets.ToString();
        else
            itemPrice.text = myShopItem.gameItem.itemProperties.sellPriceFlorets.ToString();

        itemImage.sprite = myShopItem.itemIcon;
    }
    

    public void OnClickCustom()
    {
        Shop shop = Shop.FindObjectOfType<Shop>();
        shop.InspectShopItem(this, myShopItem);

        Debug.Log("Inspecting Shop Item");
    }

    
}
