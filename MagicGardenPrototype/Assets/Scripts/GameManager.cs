using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurrencyManagement;

public class GameManager : MobileInputManager {
    [Header ("Player Data")]
    public PlayerData playerData;

    public int currentFlorets;

    MenuManager MM;
    InventoryUI InvUI;

    public enum screens
    {
        mainGame,
        emails,
        shop,
        inventory,
        spellGame,
        settings,
        transactions
    }
    public screens currentScreen;

    Transform mainCam;
    public List<Transform> cameraPosList = new List<Transform>();
    int currentCamPos;
    Vector3 camMoveToPos;
    [Space(20)]
    public List<PlacePoint> placePoints = new List<PlacePoint>();
    bool holdingMoveable;
    bool holdingInventoryItem;
    public Transform heldObject;
    Vector3 heldObjectInitialPos; // Where the object was picked up from

    public GameObject worldItemPrefab;
    public List<WorldItem> worldItemsPool = new List<WorldItem>(); // Keeps inventory items hidden off screen
    public List<WorldItem> allWorldItemsInScene = new List<WorldItem>();

    [Header("All Pot Sprites")]
    public List<Sprite> allPotSprites = new List<Sprite>();
    [Header("All Plant Types (Scriptables)")]
    public List<PlantType> allPlantTypes = new List<PlantType>();

    private void Start()
    {
        MM = GetComponent<MenuManager>();
        InvUI = InventoryUI.FindObjectOfType(typeof(InventoryUI)) as InventoryUI;
        currentScreen = screens.mainGame;
        currentCamPos = 0;
        mainCam = Camera.main.transform;
        camMoveToPos = cameraPosList[0].position;

        GetComponent<XMLSaveLoad>().LoadGame();

        // Turn off PlacePoint Icons
        placePoints.AddRange(GameObject.FindObjectsOfType<PlacePoint>());
        foreach (PlacePoint p in placePoints)
            p.HidePointer();
    }

    private void Update()
    {
        //if (currentScreen == screens.mainGame) // ONLY register custom touch input if not in a Menu. Otherwise let Unity do its Event things
            base.Update();

        if (Vector3.Distance(mainCam.position, camMoveToPos) > 0.02f && !userIsTouching)
        {
            mainCam.position = Vector3.Lerp(mainCam.position, camMoveToPos, Time.deltaTime * 5);
            mainCam.rotation = Quaternion.Lerp(mainCam.rotation, cameraPosList[currentCamPos].rotation, Time.deltaTime * 5);
        }

        if (holdingMoveable && heldObject != null)
            heldObject.transform.position = GetRaycastHitPoint();

        if (holdingInventoryItem && heldObject != null)
            heldObject.transform.position = screenTouchPos;


        // FOR DEV TESTING PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            this.GetComponent<XMLSaveLoad>().SaveGame();
        }
    }


    public void SetScreen(int enumValue) // Called when click on Interactive
    {
        currentScreen = (screens)enumValue;

        switch (currentScreen)
        {
            case screens.emails:
                MM.OpenEmails();
                break;
        }

        return;
    }

    

    public override void SingleTapRelease()
    {
        base.SingleTapRelease();
        Debug.Log("Game Manager registered Single Tap Release!");

        if (GetSelectedObject() != null && GetSelectedObject().CompareTag("interactive") && currentScreen == screens.mainGame)
            SetScreen((int)GetSelectedObject().GetComponent<Interactive>().screenToOpen);
    }

    public override void SwipeLeft()
    {
        base.SwipeLeft();
        if (currentScreen == screens.mainGame)
            if (currentCamPos - 1 < 0)
                currentCamPos = cameraPosList.Count - 1;
            else
                currentCamPos--;

        camMoveToPos = cameraPosList[currentCamPos].position;

        if (currentScreen == screens.inventory)
            MM.InventoryLeft();
    }

    public override void SwipeRight()
    {
        base.SwipeRight();
        if (currentScreen == screens.mainGame)
            if (currentCamPos + 1 > cameraPosList.Count - 1)
                currentCamPos = 0;
            else
                currentCamPos++;

        camMoveToPos = cameraPosList[currentCamPos].position;


        if (currentScreen == screens.inventory)
            MM.InventoryRight();
    }

    public override void SwipeUp()
    {
        base.SwipeUp();

        // Open Inventory if in Main
        if (currentScreen == screens.mainGame)
        {
            currentScreen = screens.inventory;
            MM.OpenInventory();
        }
    }

    public override void SwipeDown()
    {
        base.SwipeDown();

        // Close Inventory if in Inventory
        if (currentScreen == screens.inventory)
        {
            currentScreen = screens.mainGame;
            MM.CloseInventory();
        }
    }

    public override void HoldDown()
    {
        if (currentScreen == screens.mainGame)
        {
            if (heldObject == null)
            {
                Transform heldItem = GetSelectedObject();

                if (heldItem.CompareTag("moveable") && !holdingMoveable)
                    PickUpObject(heldItem);
            }

            // If held in lower third of screen, move world item to inventory
            if (holdingMoveable && screenTouchPos.y < Screen.height / 3)
                MoveWorldItemToInventory();
            // If player takes inv item out of inv but wants to return it without placing in world...
            else if (holdingInventoryItem && screenTouchPos.y < Screen.height / 3)
            {
                MM.OpenInventory();
                currentScreen = screens.inventory;
            }

        }

        else if (currentScreen == screens.inventory)
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
                currentScreen = screens.mainGame;

                ShowPlacePointMarkers();
            }
           
        }

                
    }

    public override void HoldRelease()
    {
        // For moving World Item to world spot
        if (currentScreen == screens.mainGame && holdingMoveable)
            PlaceObject();

        // For moving Inventory Item to world spot
        if (currentScreen == screens.mainGame && holdingInventoryItem)
            PlaceInventoryItemInWorld();

        // For moving Inventory Item to inventory slot
        else if (currentScreen == screens.inventory && heldObject != null && heldObject.GetComponent<InventoryItem>())
        {
            Transform target = GetSelectedGUIObject();
            if (target != null && target.GetComponent<InventoryUISlot>() != null)
                PlaceInventoryObjectInSlot(target.GetComponent<InventoryUISlot>());
            else
                PlaceInventoryObjectInSlot(InvUI.lastUsedSlot);
        }

        // For moving World Item to inventory slot
        else if (currentScreen == screens.mainGame && heldObject != null && heldObject.GetComponent<WorldItem>() != null)
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
            currentScreen = screens.inventory;
            MM.OpenInventory();
        }

    }

    void MoveWorldItemToInventory()
    {
        Debug.Log("World Item To Inventory = " + heldObject.name);
        if (heldObject.GetComponent<WorldItem>().myGameItem.itemType != GameItem.itemTypes.potWithPlant)
        {
            MM.OpenInventory();
            currentScreen = screens.inventory;
            holdingInventoryItem = true;
            Inventory inv = GetComponent<Inventory>();


            // Create new Inventory UI object
            GameItem gi = heldObject.GetComponent<WorldItem>().myGameItem;
            GameObject newInvItem = Instantiate(inv.inventoryItemPrefab);
            newInvItem.GetComponent<InventoryItem>().myGameItem = gi;
            Debug.Log("World Item to Inv, new Inv Item Created");
            
            // Open correct Inventory tab and child new Inventory item
            switch (gi.itemType)
            {
                case GameItem.itemTypes.seed:
                    InvUI.GoToScreen(0);
                    newInvItem.transform.SetParent(inv.panelSeeds);
                    break;
                case GameItem.itemTypes.pot:
                    InvUI.GoToScreen(1);
                    newInvItem.transform.SetParent(inv.panelPots);
                    break;
                case GameItem.itemTypes.potion:
                    InvUI.GoToScreen(1);
                    newInvItem.transform.SetParent(inv.panelPots);
                    break;
                case GameItem.itemTypes.decor:
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
        playerData = data;

        // Sort World Items from Inventory Items
        foreach (GameItem item in playerData.allGameItems)
        {
            if (item.inWorld)
                LoadItemToWorld(item);
            else
                LoadItemToInventory(item);
        }
    }

     void LoadItemToWorld(GameItem newGameItem)
    {
        GameObject newWorldItem = Instantiate(worldItemPrefab);
        newWorldItem.GetComponent<WorldItem>().SetupSelf(this, newWorldItem, newGameItem);

        // Make sure the matching placedpoint is considered NOT empty
        GameObject.Find(newGameItem.placedPointName).GetComponent<PlacePoint>().empty = false;

        // Finally, add it back to the WorldItems list
        allWorldItemsInScene.Add(newWorldItem.GetComponent<WorldItem>());
    }

    void LoadItemToInventory(GameItem newGameItem)
    {
        GetComponent<Inventory>().AddItemToInventory(newGameItem);
    }
}
