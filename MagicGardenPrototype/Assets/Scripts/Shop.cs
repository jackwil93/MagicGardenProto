using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class Shop : MonoBehaviour {
    Inventory inv;
    GameManager GM;

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
    public ShopItem currentShopItem; // For Buying
    WorldItem selectedWorldItem; // For Selling

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

    // Get a Total List of all GameItems Here. Makes Life Easier when Handling Selling
    List<GameItem> itemsToSell = new List<GameItem>();

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();
        inv = FindObjectOfType<Inventory>();
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
        focusedWindow.anchoredPosition = new Vector2(-27.7f, focusedWindow.anchoredPosition.y);



    }

    public void InspectShopItem(ShopItemButton shopButton, ShopItem item) // Called by ShopItemButton. Delegate Applied on ShopItemButton Start
    {
        // After Recent Changes (Nov 18) Shop Items ONLY exist in Buy Menu
            OpenBuyWindow(item);
    }

    void OpenBuyWindow(ShopItem itemToBuy) 
    {
        buyWindow.SetActive(true);
        buyWindowItemImage.sprite =     GM.GetSpriteSet(itemToBuy.gameItem.itemProperties.itemID).normalSprites[0];
        buyWindowItemName.text =        itemToBuy.gameItem.itemProperties.displayedName;
        buyWindowItemPrice.text =       itemToBuy.gameItem.itemProperties.buyPriceFlorets.ToString();
        buyWindowItemDescription.text = itemToBuy.gameItem.itemProperties.itemDescription;

        currentShopItem = itemToBuy;
        Debug.Log("Current item = " + itemToBuy.ToString());
    }

    public void OpenSellWindow(WorldItem itemToSell) // Called from GM. The window that pops up: "Do you want to sell this item"?
    {
        sellWindow.SetActive(true);
        sellWindowItemImage.sprite = itemToSell.GetWorldItemSprites().normalSprites[0];
        sellWindowItemName.text =        itemToSell.myGameItem.itemProperties.displayedName;
        sellWindowItemPrice.text =       itemToSell.myGameItem.itemProperties.sellPriceFlorets.ToString();
        sellWindowItemDescription.text = itemToSell.myGameItem.itemProperties.itemDescription;

        selectedWorldItem = itemToSell;

        //instancesOfCurrentItem = GM.GetInstancesOfGameItemOwnedWorldAndInv(itemToSell.gameItem.itemProperties.itemID);
        //currentItem = itemToSell[0];
    }

    public void BuyItem()
    {
        Debug.Log("Buying item, type = " + currentShopItem.gameItem.itemProperties.itemType.ToString());
        // Make a new item, get the values, otherwise causes a bug where all purchases copy each others properties!
        GameItem newGameItem = new GameItem();
        newGameItem.itemProperties = currentShopItem.gameItem.itemProperties;

        if (inv.CheckIfFreeSlot(newGameItem))
        {
            if (Currencies.SubtractFlorets(currentShopItem.gameItem.itemProperties.buyPriceFlorets) 
                && Currencies.SubtractCrystals(currentShopItem.gameItem.itemProperties.buyPriceCrystals))
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

    public void SellItem() // Runs when the 'Sell' Button is Pressed in the Sell Item Window
    {
        Currencies.AddFlorets(selectedWorldItem.myGameItem.itemProperties.sellPriceFlorets);

        GameObject.Find(selectedWorldItem.myGameItem.placedPointName).GetComponent<PlacePoint>().empty = true;
        GM.allWorldItemsInScene.Remove(selectedWorldItem);
        Destroy(selectedWorldItem.gameObject);

        UpdateCurrenciesUI();

        sellWindow.SetActive(false);
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


    public void EnterSellMode() // When "Start Selling" Button is clicked
    {
        GM.SetScreen(GameStates.gameScreens.selling);
        GameObject.Find("Laptop UI").GetComponent<UIMovement>().MoveOffScreen();

        // All sellable items should animate
    }

    public void ExitSellMode() // When user swipes down in Sell Mode. Called from GM
    {
        GM.SetScreen(GameStates.gameScreens.laptop);
        GameObject.Find("Laptop UI").GetComponent<UIMovement>().MoveOnScreen();

    }

    //public void UpdateSellButtons() // Called when the Sell tab is open in the Shop Window. Lists all items the player has to Sell
    //{
    //    GameManager GM = FindObjectOfType<GameManager>();
    //    itemsToSell.Clear();
    //    itemsToSell = GM.RefreshAndGetAllGameItemsWorldAndInventory();

    //    // Clear existing buttons
    //    foreach (Transform t in sellContent)
    //        Destroy(t.gameObject);

    //    // Set up new buttons
    //    foreach (GameItem gi in itemsToSell)
    //    {
    //        GameObject newButton = Instantiate(shopItemButtonPrefab, sellContent);
    //        ShopItem itemToSell = new ShopItem();
    //        itemToSell.gameItem = gi;

    //        ShopItemButton newShopButton = newButton.GetComponent<ShopItemButton>();

    //        newShopButton.myShopItem = itemToSell;
    //        newShopButton.buyOrSell = ShopItemButton.saleType.sell;
    //        newShopButton.UpdateShopItemInfo();
    //    }
    //}

}
