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

    [Header ("ShopFront Elements")]
    public Transform shopWindowContent;
    public GameObject saleItemButtonPrefab;
    public GameObject salesSummaryWindow;


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


    // For switching shop windows
    RectTransform focusedWindow;

    // Get a Total List of all GameItems Here. Makes Life Easier when Handling Selling
    //List<GameItem> itemsToSell = new List<GameItem>();

    // The actual List of items that appear in the Selling Window
    [SerializeField]
    public List<ShopItem> itemsForSale = new List<ShopItem>();

    public int saleChance = 10; // + 10 for every 1 in PlayerData ShopRating

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();
        inv = FindObjectOfType<Inventory>();
        CreateBuyButtons();
        FocusWindow(seedShopContent.GetComponent<RectTransform>());

        Invoke("UpdateCurrenciesUI", 0.1f);

        saleChance += (GM.playerData.shopRating * 10);

        SalesOverInactiveTime();
        InvokeRepeating("RandomSale", 60, 60);
    }

    void CreateBuyButtons()
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
        tempItem.mainGameItem = new GameItem();
        tempItem.mainGameItem.itemProperties = sourceItem.itemProperties;

        Sprite displaySprite;
        // Seed sprites for plant types
        if (sourceItem.itemProperties.itemType == ItemProperties.itemTypes.plant)
            displaySprite = sourceItem.itemSprites.seedSprite;
        else // If Pot or Decor. No other options here.
            displaySprite = sourceItem.itemSprites.normalSprites[0];

        tempItem.mainItemIcon = displaySprite;

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
        if (shopButton.buyOrSell == ShopItemButton.saleType.buy)
            OpenBuyWindow(item);
            
            // TODO - Ability to cancel sale and put item back in world
    }

    void OpenBuyWindow(ShopItem itemToBuy) 
    {
        buyWindow.SetActive(true);
        buyWindowItemImage.sprite =     GM.GetSpriteSet(itemToBuy.mainGameItem.itemProperties.itemID).normalSprites[0];
        buyWindowItemName.text =        itemToBuy.mainGameItem.itemProperties.displayedName;
        buyWindowItemPrice.text =       itemToBuy.mainGameItem.itemProperties.buyPriceFlorets.ToString();
        buyWindowItemDescription.text = itemToBuy.mainGameItem.itemProperties.itemDescription;

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
        Debug.Log("Buying item, type = " + currentShopItem.mainGameItem.itemProperties.itemType.ToString());
        // Make a new item, get the values, otherwise causes a bug where all purchases copy each others properties!
        GameItem newGameItem = new GameItem();
        newGameItem.itemProperties = currentShopItem.mainGameItem.itemProperties;

        // If buying a plant, force it's type to Seed
        newGameItem.itemProperties.currentStage = ItemProperties.itemStage.seed;

        if (inv.CheckIfFreeSlot(newGameItem))
        {
            if (Currencies.SubtractFlorets(currentShopItem.mainGameItem.itemProperties.buyPriceFlorets) 
                && Currencies.SubtractCrystals(currentShopItem.mainGameItem.itemProperties.buyPriceCrystals))
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


    public void AddItemToSales() // Runs when the 'Sell' Button is Pressed in the Sell Item Window
    {
        ShopItem newShopItem = new ShopItem();
        newShopItem.mainGameItem = selectedWorldItem.myGameItem;

        if (selectedWorldItem.myGameItem.itemProperties.itemType == ItemProperties.itemTypes.potWithPlant)
            newShopItem.secondaryGameItem = selectedWorldItem.secondaryGameItem;

        itemsForSale.Add(newShopItem);

        // Destroy World Object
        GameObject.Find(selectedWorldItem.myGameItem.placedPointName).GetComponent<PlacePoint>().empty = true;
        GM.allWorldItemsInScene.Remove(selectedWorldItem);
        Destroy(selectedWorldItem.gameObject);
    }

    public void RefreshShopWindow() // Called whenever the player visits their shop
    {
        foreach (Transform child in shopWindowContent)
            Destroy(child.gameObject);

        foreach (ShopItem itemSelling in itemsForSale)
        {
            GameObject saleItemPrefab = Instantiate(saleItemButtonPrefab, shopWindowContent);
            ShopItemButton shopButton = saleItemPrefab.GetComponent<ShopItemButton>();

            shopButton.myShopItem = itemSelling;
            shopButton.buyOrSell = ShopItemButton.saleType.sell;

            shopButton.UpdateShopItemInfo();

            if (itemSelling.sold)
                saleItemButtonPrefab.transform.Find("Text_Sold").gameObject.SetActive(true);
            else
                saleItemButtonPrefab.transform.Find("Text_Sold").gameObject.SetActive(false);

        }

    }

    void SalesOverInactiveTime()
    {
        for (int i = GameDateTime.realTimeSinceLastPlay; i > 0; i --)
        {
            RandomSale();
        }
    }

    void RandomSale()
    {
        Debug.Log("Random Sale, pass or fail...");

        // for each minute passed, there is a 10% chance somebody buys an item. This % (fame) increases with more sales
        int itemIndex = Random.Range(0, itemsForSale.Count);

        int roll = Random.Range(0, 100);

        if (roll <= saleChance)
            ItemIsSold(itemsForSale[itemIndex]);
        Debug.Log("Random Sale failed");
        
    }

    public void ItemIsSold(ShopItem gameItem)
    {
        gameItem.sold = true;
        Debug.Log(gameItem.mainGameItem.itemProperties.itemID + " has been sold!");
    }

    public void CollectSales() // Called from Collect Sales button
    {
        List<ShopItem> itemsToProcess = new List<ShopItem>();

        foreach (ShopItem saleItem in itemsForSale)
            if (saleItem.sold)
            {
                itemsToProcess.Add(saleItem);
            }

        ProcessSales(itemsToProcess);
    }

    void ProcessSales(List<ShopItem> itemsSold)
    {
        // Open Window
        salesSummaryWindow.gameObject.SetActive(true);

        int earnings = 0;
        int numberSold = itemsSold.Count;

        foreach (ShopItem item in itemsSold)
        {
            earnings += item.mainGameItem.itemProperties.sellPriceFlorets;

            if (item.secondaryGameItem != null)
                earnings += item.secondaryGameItem.itemProperties.sellPriceFlorets;

            // Remove it from the list of items
            itemsForSale.Remove(item);
        }

        salesSummaryWindow.transform.Find("Text_Earnings").GetComponent<Text>().text = "+" + earnings.ToString() + " florets";
        salesSummaryWindow.transform.Find("Text_Summary").GetComponent<Text>().text = "You sold " + numberSold + " items!";

        Currencies.AddFlorets(earnings);

    }

    //public void SellItem() // 
    //{
    //    Currencies.AddFlorets(selectedWorldItem.myGameItem.itemProperties.sellPriceFlorets);

    

    //    UpdateCurrenciesUI();

    //    sellWindow.SetActive(false);
    //}


    


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


    /// <summary>
    /// Runs on Game Startup. Not the same as displaying the items in the window. Try UpdateItemsForSale in MenuManager
    /// </summary>
    /// <param name="itemsLoaded"></param>
    public void LoadItemsForSale(List<GameItem> itemsLoaded) // Called From GM (TODO)
    {
        GM = FindObjectOfType<GameManager>();

        foreach (GameItem loadedGI in itemsLoaded)
        {
            ShopItem newShopItem = new ShopItem();
            newShopItem.mainGameItem = loadedGI;

            // If pot with plant, add the pot as secondary GameItem
            if (loadedGI.itemProperties.itemType == ItemProperties.itemTypes.potWithPlant)
            {
                ItemProperties originalPot = GM.GetPotOriginal(loadedGI.basePotID);
                ItemProperties cloneOfPotScrObj = MagicTools.DeepCopy<ItemProperties>(originalPot);
                GameItem potGI = new GameItem();
                potGI.itemProperties = cloneOfPotScrObj;

                newShopItem.secondaryGameItem = potGI;
            }

            // Add the created ShopItem to the list of items for sale
            itemsForSale.Add(newShopItem); 
        }

        Debug.Log(itemsForSale.Count + " Items loaded to Items For Sale");
    }

    public List<GameItem> SaveItemsForSale() // For Saving. Called from XML SaveLoad (TODO)
    {
        // Turn all ShopItems in itemsForSale into Game Items (just grab the main Game Item, the secondary can be ref'd via basePotID)

        List<GameItem> tempList = new List<GameItem>();

        foreach (ShopItem si in itemsForSale)
            tempList.Add(si.mainGameItem);

        return tempList;
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
