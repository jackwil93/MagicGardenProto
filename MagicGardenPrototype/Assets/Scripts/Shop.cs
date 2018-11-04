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

    public List<Item> seedShopItems = new List<Item>();
    public List<Item> potShopItems = new List<Item>();
    public List<Item> decorShopItems = new List<Item>();

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
        foreach (Item seedItem in seedShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, seedShopContent);
            AssignProperties(newItem, seedItem);
        }
        Debug.Log("Created Seed Shop Buttons");
        foreach (Item potItem in potShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, potShopContent);
            AssignProperties(newItem, potItem);
        }
        Debug.Log("Created Pot Shop Buttons");

        foreach (Item decorItem in decorShopItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemButtonPrefab, decorShopContent);
            AssignProperties(newItem, decorItem);
        }
        Debug.Log("Created Decor Shop Buttons");

    }

    void AssignProperties(GameObject newItem, Item sourceItem)
    {

        ShopItem tempItem = new ShopItem();
        tempItem.gameItem = new GameItem();
        tempItem.gameItem.itemProperties = sourceItem.itemProperties;

        tempItem.itemIcon = sourceItem.itemSprites.normalSprites[0];

        newItem.GetComponent<ShopItemButton>().myShopItem = tempItem;
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
        buyWindowItemPrice.text =       itemToBuy.gameItem.itemProperties.buyPriceFlorets.ToString();
        buyWindowItemDescription.text = itemToBuy.gameItem.itemProperties.itemDescription;

        currentItem = itemToBuy;
        Debug.Log("Current item = " + itemToBuy.ToString());
    }

    void OpenSellWindow(ShopItem itemToSell) 
    {
        sellWindow.SetActive(true);
        //sellWindowItemImage.sprite =     itemToSell.gameItem.itemProperties.itemSprite;
        sellWindowItemName.text =        itemToSell.gameItem.itemProperties.displayedName;
        sellWindowItemPrice.text =       itemToSell.gameItem.itemProperties.sellPriceFlorets.ToString();
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
            if (Currencies.SubtractFlorets(currentItem.gameItem.itemProperties.buyPriceFlorets) 
                && Currencies.SubtractCrystals(currentItem.gameItem.itemProperties.buyPriceCrystals))
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
