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

    private void Start()
    {
        if (myShopItem != null)
            UpdateShopItemInfo();
    }

    public void UpdateShopItemInfo()
    {
        itemName.text = myShopItem.gameItem.itemProperties.displayedName;
        itemPrice.text = myShopItem.buyPriceFlorets.ToString();

        GetImage();
    }

    void GetImage()
    {
        GameManager GM = GameManager.FindObjectOfType<GameManager>();

        switch (myShopItem.gameItem.itemProperties.itemType)
        {
            case ItemProperties.itemTypes.seed:
                // No seed sprites yet
                break;
            case ItemProperties.itemTypes.pot:
                itemImage.sprite = GM.allPotSprites[myShopItem.gameItem.potID];
                break;
            case ItemProperties.itemTypes.potion:
                break;
            case ItemProperties.itemTypes.decor:
                break;
        }
    }
}
