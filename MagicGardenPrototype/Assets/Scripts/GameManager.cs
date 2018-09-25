using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MobileInputManager {

    public enum screens
    {
        mainGame,
        emails,
        shop,
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
    Transform heldObject;
    Vector3 heldObjectInitialPos; // Where the object was picked up from

    public Inventory currentInventory;
    public GameObject worldItemPrefab;
    [Header("All Pot Prefabs")]
    public List<Sprite> potSprites = new List<Sprite>();
    [Header("All Plant Prefabs")]
    public List<Sprite> plantTypes = new List<Sprite>(); // This should probably done by just grabbing what is needed from Resources

    private void Start()
    {
        currentScreen = screens.mainGame;
        currentCamPos = 0;
        mainCam = Camera.main.transform;
        camMoveToPos = cameraPosList[0].position;

        GetComponent<XMLSaveLoad>().LoadGame();


        // Turn off PlacePoint Icons
        placePoints.AddRange(GameObject.FindObjectsOfType<PlacePoint>());
        foreach (PlacePoint p in placePoints)
            p.HidePointer();

        // Spawn World Items
        SpawnWorldItems();
    }

    private void Update()
    {
        if (currentScreen == screens.mainGame) // ONLY register custom touch input if not in a Menu. Otherwise let Unity do its Event things
            base.Update();

        if (Vector3.Distance(mainCam.position, camMoveToPos) > 0.02f && !userIsTouching)
        {
            mainCam.position = Vector3.Lerp(mainCam.position, camMoveToPos, Time.deltaTime * 5);
            mainCam.rotation = Quaternion.Lerp(mainCam.rotation, cameraPosList[currentCamPos].rotation, Time.deltaTime * 5);
        }

        if (holdingMoveable)
        {
            heldObject.transform.position = GetRaycastHitPoint();
        }


        // FOR DEV TESTING PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            this.GetComponent<XMLSaveLoad>().SaveGame();
        }
    }

    public void SetScreen(int enumValue) // Called when click on Interactive
    {
        currentScreen = (screens)enumValue;
        MenuManager MM = GetComponent<MenuManager>();

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
    }

    public override void HoldDown()
    {
        if (GetSelectedObject().CompareTag("moveable") && !holdingMoveable)
            PickUpObject(GetSelectedObject());


        // Move cam a little
        //if (!holdingMoveable)
        //{
        //    mainCam.Rotate(mainCam.rotation.x,
        //        cameraPosList[currentCamPos].rotation.y + Mathf.Clamp(Screen.width / base.screenTouchPos.x, -1, 1),
        //        mainCam.rotation.z);
        //}
                
    }

    public override void HoldRelease()
    {
        if (holdingMoveable)
            PlaceObject();
    }

    void PickUpObject(Transform obj)
    {
        heldObject = obj;
        heldObjectInitialPos = obj.position;
        heldObject.GetComponent<BoxCollider>().enabled = false;
        holdingMoveable = true;

        GameObject.Find(heldObject.GetComponent<WorldItem>().placedPointName).GetComponent<PlacePoint>().empty = true;

        // Show pointer UI
        foreach (PlacePoint p in placePoints)
            p.ShowPointer();
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

                heldWorldItem.placedPointName = selectedObject.name;
                Vector3 placePointPos = selectedObject.position;
                heldWorldItem.placedPointX = placePointPos.x;
                heldWorldItem.placedPointY = placePointPos.y;
                heldWorldItem.placedPointZ = placePointPos.z;

                selectedObject.GetComponent<PlacePoint>().empty = false;
            }
        }
        else
            heldObject.position = heldObjectInitialPos;

        heldObject.GetComponent<Collider>().enabled = true;
        heldObject = null;
        holdingMoveable = false;

        // Hide pointer UI
        foreach (PlacePoint p in placePoints)
            p.HidePointer();
    }


    public void SpawnWorldItems() 
    {
        Inventory mainInv = GetComponent<Inventory>();

        foreach (InventoryItem itemToSpawn in mainInv.allItemsList)
        {
            if (itemToSpawn.inWorld)
            {
                GameObject newWorldItem = Instantiate(worldItemPrefab);
                newWorldItem.GetComponent<WorldItem>().SetupSelf(this, newWorldItem, itemToSpawn);

                // Make sure the matching placedpoint is considered NOT empty
                GameObject.Find(itemToSpawn.placedPointName).GetComponent<PlacePoint>().empty = false;

                // Finally, add it back to the Inventory's WorldItems list
                mainInv.worldItems.Add(newWorldItem.GetComponent<WorldItem>());
            }
        }
    }
}
