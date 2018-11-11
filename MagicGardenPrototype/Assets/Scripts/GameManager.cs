using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class GameManager : MobileInputManager {

    [Header("For Debug")]
    public Text debugText;

    [Header ("Player Data")]
    public PlayerData playerData;

    MenuManager MM;
    InventoryUI InvUI;

    public GameStates.gameScreens currentScreen;

    Transform mainCam;
    public List<Transform> cameraPosList = new List<Transform>();
    int currentCamPos;
    Vector3 camMoveToPos;
    bool cameraMoving;

    [Space(20)]
    public List<PlacePoint> placePoints = new List<PlacePoint>();
    bool holdingMoveable;
    bool holdingInventoryItem;
    public Transform heldObject;
    Vector3 heldObjectInitialPos; // Where the object was picked up from

    public GameObject worldItemPrefab;
    public List<WorldItem> worldItemsPool = new List<WorldItem>(); // Keeps inventory items hidden off screen. NOTE: Is this being used?
    public List<WorldItem> allWorldItemsInScene = new List<WorldItem>();


    [Header("All Pot Sprites")]
    public List<Sprite> allPotSprites = new List<Sprite>();

    [Header("All Pot Types (Scriptables")]
    public List<Item> allPotTypes = new List<Item>();
    [Header("All Plant Types (Scriptables)")]
    public List<Item> allPlantTypes = new List<Item>();


    // int = itemID
    Dictionary<string, ItemSprites> spriteDictionary = new Dictionary<string, ItemSprites>();

    private void Start()
    {
        MM = GetComponent<MenuManager>();
        InvUI = InventoryUI.FindObjectOfType(typeof(InventoryUI)) as InventoryUI;
        currentScreen = GameStates.gameScreens.mainGame;
        currentCamPos = 0;
        mainCam = Camera.main.transform;
        camMoveToPos = cameraPosList[0].position;

        // Update Sprite Dictionary before loading the player data
        foreach (Item itemPot in allPotTypes)
            spriteDictionary.Add(itemPot.itemProperties.itemID, itemPot.itemSprites);

        foreach (Item itemPlant in allPlantTypes)
            spriteDictionary.Add(itemPlant.itemProperties.itemID, itemPlant.itemSprites);

        //Start Loading Player Data
        GetComponent<XMLSaveLoad>().LoadGame();
        Debug.Log("Loading going to the next stage...");

        // If New Game, Do stuff...
        if (playerData.newGame)
            NewGame();

        // Update Currencies
        Currencies.OverrideFlorets(playerData.playerFlorets);
        Currencies.OverrideCrystals(playerData.playerCrystals);

        // Turn off PlacePoint Icons
        placePoints.AddRange(GameObject.FindObjectsOfType<PlacePoint>());
        foreach (PlacePoint p in placePoints)
            p.HidePointer();


        // TESTING DateTime stuff
        GameDateTime.LogCurrentDateTime();
        Debug.Log("Finished Updating DateTime");

    }

    void NewGame()
    {
        Debug.Log("Setting up New Game stats...");
        playerData.playerFlorets = 1000;
        playerData.playerCrystals = 100;
        playerData.newGame = false;
        Debug.Log("Set up New Game Currency amounts. Set New Game to False");
    }

    private void FixedUpdate()
    {
        //if (currentScreen == GameStates.gameScreens.mainGame) // ONLY register custom touch input if not in a Menu. Otherwise let Unity do its Event things
            base.FixedUpdate();

        if (mainCam.position != camMoveToPos)
        {
            if (!userIsTouching || cameraMoving)
            {
                if (Vector3.Distance(mainCam.position, camMoveToPos) > 0.02f)
                {
                    mainCam.position = Vector3.Lerp(mainCam.position, camMoveToPos, Time.deltaTime * 5);
                    mainCam.rotation = Quaternion.Lerp(mainCam.rotation, cameraPosList[currentCamPos].rotation, Time.deltaTime * 5);
                }
                else
                    cameraMoving = false;
            }
        }

        if (holdingMoveable && heldObject != null)
            heldObject.transform.position = GetRaycastHitPoint();

        if (holdingInventoryItem && heldObject != null)
            heldObject.transform.position = screenTouchPos;


        // FOR DEV TESTING PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GameDateTime.LogCurrentDateTime();
            this.GetComponent<XMLSaveLoad>().SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Order newEmailOrder = new Order();
            newEmailOrder.orderID = "davinta_0_normal_reply_b";
            newEmailOrder.orderAmount = 1;
            newEmailOrder.myOrderType = Order.orderType.email;

            Debug.Log("New order created... " + newEmailOrder.orderID);


            GetComponent<DelayedOrderManager>().AddNewOrder(newEmailOrder, 1);
        }
    }

    public ItemSprites GetSpriteSet(string id)
    {
        return spriteDictionary[id];
    }

    public void SetScreen(int enumValue) // Called when click on Interactive
    {
        currentScreen = (GameStates.gameScreens)enumValue;

        switch (currentScreen)
        {
            case GameStates.gameScreens.laptop:
                MM.OpenLaptop();
                break;
        }

        return;
    }


    

    public override void SingleTapRelease()
    {
        base.SingleTapRelease();
        Debug.Log("Game Manager registered Single Tap Release!");

        if (GetSelectedObject() != null && GetSelectedObject().CompareTag("interactive") && currentScreen == GameStates.gameScreens.mainGame)
            SetScreen((int)GetSelectedObject().GetComponent<Interactive>().screenToOpen);
    }

    public override void SwipeLeft()
    {
        base.SwipeLeft();

        cameraMoving = true;

        if (currentScreen == GameStates.gameScreens.mainGame)
            if (currentCamPos + 1 > cameraPosList.Count - 1)
                currentCamPos = 0;
            else
                currentCamPos++;

        camMoveToPos = cameraPosList[currentCamPos].position;


        if (currentScreen == GameStates.gameScreens.inventory)
            MM.InventoryRight();

    }

    public override void SwipeRight()
    {
        base.SwipeRight();

        
        cameraMoving = true;

        if (currentScreen == GameStates.gameScreens.mainGame)
            if (currentCamPos - 1 < 0)
                currentCamPos = cameraPosList.Count - 1;
            else
                currentCamPos--;

        camMoveToPos = cameraPosList[currentCamPos].position;

        if (currentScreen == GameStates.gameScreens.inventory)
            MM.InventoryLeft();
    }

    public override void SwipeUp()
    {
        base.SwipeUp();

        // Open Inventory if in Main
        if (currentScreen == GameStates.gameScreens.mainGame)
        {
            currentScreen = GameStates.gameScreens.inventory;
            MM.OpenInventory();
        }
    }

    public override void SwipeDown()
    {
        base.SwipeDown();

        // Close Inventory if in Inventory
        if (currentScreen == GameStates.gameScreens.inventory)
        {
            currentScreen = GameStates.gameScreens.mainGame;
            MM.CloseInventory();
        }
    }

    public override void HoldDown()
    {
        if (currentScreen == GameStates.gameScreens.mainGame)
        {
            if (heldObject == null)
            {
                Transform heldItem = GetSelectedObject();

                //FOR DEBUG ONLY
                if (heldItem != null)
                debugText.text = "Held item: " + heldItem.name;

                if (heldItem != null && heldItem.CompareTag("moveable") && !holdingMoveable)
                    PickUpObject(heldItem);
            }

            // If held in lower third of screen, move world item to inventory
            if (holdingMoveable && screenTouchPos.y < Screen.height / 3)
                MoveWorldItemToInventory();
            // If player takes inv item out of inv but wants to return it without placing in world...
            else if (holdingInventoryItem && screenTouchPos.y < Screen.height / 3)
            {
                MM.OpenInventory();
                currentScreen = GameStates.gameScreens.inventory;
            }

            else if (holdingMoveable || holdingInventoryItem)
            {
                // if item held on side of screen, move
                if (!cameraMoving && screenTouchPos.x < 20)
                {
                    SwipeLeft();
                }
                if (!cameraMoving && screenTouchPos.x > (Screen.width -20))
                {
                    SwipeRight();
                }
            }

        }

        else if (currentScreen == GameStates.gameScreens.inventory)
        {
            if (heldObject == null && GetSelectedGUIObject() != null)
            {
                if (GetSelectedGUIObject().GetComponent<InventoryUISlot>())
                {
                InventoryUISlot targetUISlot = GetSelectedGUIObject().GetComponent<InventoryUISlot>();
                InvUI.lastUsedSlot = targetUISlot; // To record where to send the item if dropped somewhere invalid

                heldObject = targetUISlot.TakeItemFromSlot();
                holdingInventoryItem = true;
                }
            }

            // For taking Inv Item into the world
            if (holdingInventoryItem && screenTouchPos.y > Screen.height / 2)
            {
                MM.CloseInventory();
                currentScreen = GameStates.gameScreens.mainGame;

                ShowPlacePointMarkers();
            }
           
        }

                
    }

    public override void HoldRelease()
    {
        // For moving World Item to world spot
        if (currentScreen == GameStates.gameScreens.mainGame && holdingMoveable)
            PlaceObject();

        // For moving Inventory Item to world spot
        if (currentScreen == GameStates.gameScreens.mainGame && holdingInventoryItem)
            PlaceInventoryItemInWorld();

        // For moving Inventory Item to inventory slot
        else if (currentScreen == GameStates.gameScreens.inventory && heldObject != null && heldObject.GetComponent<InventoryItem>())
        {
            Transform target = GetSelectedGUIObject();
            if (target != null && target.GetComponent<InventoryUISlot>() != null)
                PlaceInventoryObjectInSlot(target.GetComponent<InventoryUISlot>());
            else if (target.name == "Tab_Close")
            {
                // Delete object, it has been dragged onto the bin
                Destroy(heldObject.gameObject);
                ClearHeldObject();
            }
            else
                PlaceInventoryObjectInSlot(InvUI.lastUsedSlot);
        }

        // For moving World Item to inventory slot
        else if (currentScreen == GameStates.gameScreens.mainGame && heldObject != null && heldObject.GetComponent<WorldItem>() != null)
        {
            MoveWorldItemToInventory();
        }

        
    }

    void PickUpObject(Transform obj)
    {
        heldObject = obj;
        heldObjectInitialPos = obj.position;
        heldObject.GetComponent<BoxCollider>().enabled = false;
        holdingMoveable = true;

        if (heldObject.GetComponent<WorldItem>().myGameItem.placedPointName != "")
            GameObject.Find(heldObject.GetComponent<WorldItem>().myGameItem.placedPointName).GetComponent<PlacePoint>().empty = true;

        ShowPlacePointMarkers();
    }


    void PlaceObject()
    {
        Transform selectedObject = GetSelectedObject();

        // Check if being placed correctly
        if (selectedObject.CompareTag("placePoint") )
        {
            if (selectedObject.GetComponent<PlacePoint>().empty)
            {
                heldObject.position = selectedObject.position;
                WorldItem heldWorldItem = heldObject.GetComponent<WorldItem>();

                heldWorldItem.myGameItem.placedPointName = selectedObject.name;
                Vector3 placePointPos = selectedObject.position;
                heldWorldItem.myGameItem.placedPointX = placePointPos.x;
                heldWorldItem.myGameItem.placedPointY = placePointPos.y;
                heldWorldItem.myGameItem.placedPointZ = placePointPos.z;

                selectedObject.GetComponent<PlacePoint>().empty = false;
            }
        }
        else
            heldObject.position = heldObjectInitialPos;

        heldObject.GetComponent<Collider>().enabled = true;
        ClearHeldObject();
        HidePlacePointMarkers();
    }

    void ClearHeldObject()
    {
        heldObject = null;
        holdingMoveable = false;
        holdingInventoryItem = false;
    }

    void ShowPlacePointMarkers()
    {
        // Show pointer UI
        foreach (PlacePoint p in placePoints)
            p.ShowPointer();
    }

    void HidePlacePointMarkers()
    {
        // Hide pointer UI
        foreach (PlacePoint p in placePoints)
            p.HidePointer();
    }


    void PlaceInventoryObjectInSlot(InventoryUISlot selectedSlot)
    {
        // If slot is not free, return to previous slot
        if (selectedSlot.TryPlaceItem(heldObject))
        {
            selectedSlot.PlaceItemInSlot(heldObject);
            ClearHeldObject();
            HidePlacePointMarkers(); // Runs even if just doing things in inventory. Could be cleaner.
        }
        else PlaceInventoryObjectInSlot(InvUI.lastUsedSlot); // Return to previous slot
    }


    void PlaceInventoryItemInWorld()
    {
        // Get Holding Position
        Transform selectedObject = GetSelectedObject();
        // Check if being placed correctly
        if (selectedObject != null && selectedObject.CompareTag("placePoint"))
        {
            if (selectedObject.GetComponent<PlacePoint>().empty)
            {
                GameItem currentHeldGameItem = heldObject.GetComponent<InventoryItem>().myGameItem;

                currentHeldGameItem.placedPointName = selectedObject.name;
                Vector3 placePointPos = selectedObject.position;
                currentHeldGameItem.placedPointX = placePointPos.x;
                currentHeldGameItem.placedPointY = placePointPos.y;
                currentHeldGameItem.placedPointZ = placePointPos.z;

                LoadItemToWorld(currentHeldGameItem);

                heldObject.gameObject.SetActive(false);

                // Remove the item from Inventory Item lists, as it is now in the World
                GetComponent<Inventory>().RemoveItemFromInventory(currentHeldGameItem);
                
                ClearHeldObject();
                HidePlacePointMarkers();
            }
        }
        else
        {
            PlaceInventoryObjectInSlot(InvUI.lastUsedSlot);
            currentScreen = GameStates.gameScreens.inventory;
            MM.OpenInventory();
        }

    }

    void MoveWorldItemToInventory()
    {
        Debug.Log("World Item To Inventory = " + heldObject.name);
        if (heldObject.GetComponent<WorldItem>().myGameItem.itemProperties.itemType != ItemProperties.itemTypes.potWithPlant)
        {
            MM.OpenInventory();
            currentScreen = GameStates.gameScreens.inventory;
            holdingInventoryItem = true;
            Inventory inv = GetComponent<Inventory>();


            // Create new Inventory UI object
            GameItem gi = heldObject.GetComponent<WorldItem>().myGameItem;
            GameObject newInvItem = Instantiate(inv.inventoryItemPrefab);
            newInvItem.GetComponent<InventoryItem>().myGameItem = gi;
            Debug.Log("World Item to Inv, new Inv Item Created");
            
            // Open correct Inventory tab and child new Inventory item
            switch (gi.itemProperties.itemType)
            {
                case ItemProperties.itemTypes.seed:
                    InvUI.GoToScreen(0);
                    newInvItem.transform.SetParent(inv.panelSeeds);
                    break;
                case ItemProperties.itemTypes.pot:
                    InvUI.GoToScreen(1);
                    newInvItem.transform.SetParent(inv.panelPots);
                    break;
                case ItemProperties.itemTypes.potion:
                    InvUI.GoToScreen(1);
                    newInvItem.transform.SetParent(inv.panelPots);
                    break;
                case ItemProperties.itemTypes.decor:
                    InvUI.GoToScreen(2);
                    newInvItem.transform.SetParent(inv.panelDecor);
                    break;
            }

            // Set held object to Inv Item UI, destroy old world object
            GameObject temp = heldObject.gameObject;
            heldObject = newInvItem.transform;
            Destroy(temp);
            Debug.Log("Old World Item destroyed");
        }
    }

    public void LoadPlayerData(PlayerData data) // Called from XMLSaveLoad
    {
        Debug.Log("Game Manager getting info from PlayerData");
        playerData = data;

        if (playerData.newGame == false)
        {
            Debug.Log("Checking saved inventory...");
            // Sort World Items from Inventory Items
            foreach (GameItem item in playerData.allGameItems)
            {
                if (item.inWorld)
                    LoadItemToWorld(item);
                else
                    LoadItemToInventory(item);
            }
            Debug.Log("Inventory and World Items Loaded");
        }


        // Get Time Since Last Play
        GameDateTime.SetRealTimeSinceLastPlay(playerData.newGame, playerData.savedMinuteOfYear, playerData.savedDayOfYear);

        // Load and Check Delayed Orders AFTER Realtime Since Last Play ONLY if this is NOT a new game
        if (playerData.newGame == false)
        {
            Debug.Log("Updating Undelivered Orders...");
            GetComponent<DelayedOrderManager>().AddListOfDelayedOrders(playerData.delayedOrdersUndelivered, GameDateTime.realTimeSinceLastPlay);
            Debug.Log("Undelivered Orders have been updated");

        }

        Debug.Log("Finished LoadPlayerData()");
    }

    

     void LoadItemToWorld(GameItem newGameItem)
    {
        GameObject newWorldItem = Instantiate(worldItemPrefab);
        newWorldItem.GetComponent<WorldItem>().SetupSelf(this, newWorldItem, newGameItem);

        if (CheckForWorldDuplicate(newWorldItem.GetComponent<WorldItem>()))
        {
             // This was a duplicate and has been deleted
        }
        else
        {
            // Make sure the matching placedpoint is considered NOT empty
            GameObject.Find(newGameItem.placedPointName).GetComponent<PlacePoint>().empty = false;

            // Finally, add it back to the WorldItems list
            allWorldItemsInScene.Add(newWorldItem.GetComponent<WorldItem>());

            // This was not a duplicate. A success!
        }

    }


    bool CheckForWorldDuplicate(WorldItem loadedItem) // Called at load to stop world items duplicating
    {
        PlacePoint point = GameObject.Find(loadedItem.myGameItem.placedPointName).GetComponent<PlacePoint>();
        if (point.empty == false)
        {
            Destroy(loadedItem.gameObject);
            return true;
        }
        else
            return false;
    }


    void LoadItemToInventory(GameItem newGameItem)
    {
        GetComponent<Inventory>().AddItemToInventory(newGameItem);
    }

    public List<GameItem> RefreshAndGetAllGameItemsWorldAndInventory()
    {
        List<GameItem> allGameItems = new List<GameItem>();

        // Refresh and get all world items
        allWorldItemsInScene.Clear();
        allWorldItemsInScene.AddRange(WorldItem.FindObjectsOfType<WorldItem>());

        // Add all world items to AllGameItems
        foreach (WorldItem wItem in allWorldItemsInScene)
            allGameItems.Add(wItem.myGameItem);

        // Refresh and get all inventory items, add to AllGameItems
        Inventory inv = GetComponent<Inventory>();
        allGameItems.AddRange(inv.CheckInAllItems());

        // Send back a list of all World and Inventory items
        return allGameItems;
    }

}
