using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour {

    public ShopItem myShopItem;
    public Text itemName;
    public Text itemPrice;
    public Image itemImage;

    private void Start()
    {
        itemName.text = myShopItem.gameItem.displayedName;
        itemPrice.text = myShopItem.buyPriceFlorets.ToString();

        GetImage();
    }

    void GetImage()
    {
        GameManager GM = GameManager.FindObjectOfType<GameManager>();

        switch (myShopItem.gameItem.itemType)
        {
            case GameItem.itemTypes.seed:
                // No seed sprites yet
                break;
            case GameItem.itemTypes.pot:
                itemImage.sprite = GM.allPotSprites[myShopItem.gameItem.potID];
                break;
            case GameItem.itemTypes.potion:
                break;
            case GameItem.itemTypes.decor:
                break;
        }
    }
}
