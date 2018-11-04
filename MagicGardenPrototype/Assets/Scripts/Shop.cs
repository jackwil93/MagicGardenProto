using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class Shop : MonoBehaviour {
    Inventory inv;

    [Header("Screen UI")]
    public Text floretsUI;
    public Text crystalsUI;

    [Header ("Buy Window")]
    public GameObject buyWindow;
    public Image    buyWindowItemImage;
    public Text     buyWindowItemName;
    public Text     buyWindowItemDescription;
    public Text     buyWindowItemPrice;

    [Header ("Sell Window")]
    public GameObject sellWindow;
    public Image    sellWindowItemImage;
    public Text     sellWindowItemName;
    public Text     sellWindowItemDescription;
    public Text     sellWindowItemPrice;


    public GameObject shopItemButtonPrefab;
    public ShopItem currentItem;

    public List<ShopItem> seedShopItems = new List<ShopItem>();
    public List<ShopItem> potShopItems = new List<ShopItem>();
    public List<ShopItem> decorShopItems = new List<ShopItem>();

    [Space(20)]
    public Transform seedShopContent;
    public Transform potShopContent;
    public Transform decorShopContent;

    [Space(20)]
    public Transform sellContent;

    // For switching shop windows
    RectTransform focusedWindow;


    private void Start()
    {
        inv = Inventory.FindObjectOfType<Inventory>();
        CreateButtons();
        FocusWindow(seedShopContent.GetComponent<RectTransform>());

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

    public void FocusWindow(RectTransform panelTransform)
    {
        // Prev focused window
        if (focusedWindow != null)
            focusedWindow.anchoredPosition = new Vector2(1000, focusedWindow.anchoredPosition.y);

        focusedWindow = panelTransform;
        focusedWindow.anchoredPosition = new Vector2(-32, focusedWindow.anchoredPosition.y);



    }

    public void InspectShopItem(ShopItemButton shopButton, ShopItem item) // Called by ShopItemButton. Delegate Applied on ShopItemButton Start
    {
        if (shopButton.buyOrSell == ShopItemButton.saleType.sell)
            OpenSellWindow(item);
        else
            OpenBuyWindow(item);
    }

    void OpenBuyWindow(ShopItem itemToBuy) 
    {
        buyWindow.SetActive(true);
        //buyWindowItemImage.sprite =     itemToBuy.gameItem.itemProperties.itemSprite;
        buyWindowItemName.text =        itemToBuy.gameItem.itemProperties.displayedName;
        buyWindowItemPrice.text =       itemToBuy.buyPriceFlorets.ToString();
        buyWindowItemDescription.text = itemToBuy.gameItem.itemProperties.itemDescription;

        currentItem = itemToBuy;
        Debug.Log("Current item = " + itemToBuy.ToString());
    }

    void OpenSellWindow(ShopItem itemToSell) 
    {
        sellWindow.SetActive(true);
        //sellWindowItemImage.sprite =     itemToSell.gameItem.itemProperties.itemSprite;
        sellWindowItemName.text =        itemToSell.gameItem.itemProperties.displayedName;
        sellWindowItemPrice.text =       itemToSell.buyPriceFlorets.ToString();
        sellWindowItemDescription.text = itemToSell.gameItem.itemProperties.itemDescription;

        currentItem = itemToSell;
    }

    public void BuyItem()
    {
        Debug.Log("Buying item, type = " + currentItem.gameItem.itemProperties.itemType.ToString());
        // Make a new item, get the values, otherwise causes a bug where all purchases copy each others properties!
        GameItem newGameItem = new GameItem();
        newGameItem.itemProperties = currentItem.gameItem.itemProperties;

        if (inv.CheckIfFreeSlot(newGameItem))
        {
            if (Currencies.SubtractFlorets(currentItem.buyPriceFlorets) && Currencies.SubtractCrystals(currentItem.buyPriceCrystals))
            {

                inv.AddItemToInventory(newGameItem);
                UpdateCurrenciesUI();
            }
            else
                Debug.LogWarning("The Player cannot afford this item");
        }
        else
            Debug.LogWarning("No Free Slots to purchase this item");
    }


    // Thank you http://technico.qnownow.com/how-copy-properties-one-object-another-c/
    public void CopyProperties(object objSource, object objDestination)
    {
        //get the list of all properties in the destination object
        var destProps = objDestination.GetType().GetProperties();

        Debug.Log("CopyProperties()");
        //get the list of all properties in the source object
        foreach (var sourceProp in objSource.GetType().GetProperties())
        {
            Debug.Log("Copying property...");
            foreach (var destProperty in destProps)
            {
                //if we find match between source & destination properties name, set
                //the value to the destination property
                if (destProperty.Name == sourceProp.Name &&
                        destProperty.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    destProperty.SetValue(destProps, sourceProp.GetValue(
                        sourceProp, new object[] { }), new object[] { });
                    Debug.Log("Copied property " + destProperty.Name);
                }
            }
        }
    }


    public void UpdateCurrenciesUI()
    {
        floretsUI.text = Currencies.florets + "f";
        crystalsUI.text = Currencies.crystals + " Crystals ";
    }

    public void UpdateSellButtons() // Called when the Sell tab is open in the Shop Window
    {
        GameManager GM = GameManager.FindObjectOfType<GameManager>();
        List<GameItem> allGameItems = GM.RefreshAndGetAllGameItemsWorldAndInventory();

        // Clear existing buttons
        foreach (Transform t in sellContent)
            Destroy(t.gameObject);

        // Set up new buttons
        foreach (GameItem gi in allGameItems)
        {
            GameObject newButton = Instantiate(shopItemButtonPrefab, sellContent);
            ShopItem itemToSell = new ShopItem();
            itemToSell.gameItem = gi;
            

            ShopItemButton newShopButton = newButton.GetComponent<ShopItemButton>();

            newShopButton.myShopItem = itemToSell;
            newShopButton.buyOrSell = ShopItemButton.saleType.sell;
            newShopButton.UpdateShopItemInfo();
        }
    }

}
