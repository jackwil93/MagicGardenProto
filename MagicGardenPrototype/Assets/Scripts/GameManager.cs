using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MobileInputManager {

    public enum screens
    {
        mainGame,
        emails,
        shop,
        spellGame
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

    [Header("All Pot Prefabs")]
    public List<GameObject> potTypes = new List<GameObject>();
    [Header("All Plant Prefabs")]
    public List<GameObject> plantTypes = new List<GameObject>(); // This should probably done by just grabbing what is needed from Resources

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
        base.Update();
        // If in the main game scene, run an input raycast

        if (Vector3.Distance(mainCam.position, camMoveToPos) > 0.02f)
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

    public override void SingleTapRelease()
    {
        base.SingleTapRelease();
        Debug.Log("Game Manager registered Single Tap Release!");
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
        heldObject.GetComponent<Collider>().enabled = false;
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
                GameObject newWorldItem = new GameObject();

                WorldItem newItemAttributes = newWorldItem.AddComponent<WorldItem>();

                newItemAttributes.itemID = itemToSpawn.itemID;
                newItemAttributes.displayedName = itemToSpawn.displayedName;
                newItemAttributes.potID = itemToSpawn.potID;
                newItemAttributes.plantID = itemToSpawn.plantID;
                newItemAttributes.ageTime = itemToSpawn.ageTime;
                newItemAttributes.invSlotNumber = itemToSpawn.invSlotNumber;
                newItemAttributes.inWorld = itemToSpawn.inWorld;
                newItemAttributes.placedPointName = itemToSpawn.placedPointName;
                newItemAttributes.placedPointX = itemToSpawn.placedPointX;
                newItemAttributes.placedPointY = itemToSpawn.placedPointY;
                newItemAttributes.placedPointZ = itemToSpawn.placedPointZ;


                newItemAttributes.SpawnSelf(potTypes[itemToSpawn.potID], plantTypes[itemToSpawn.plantID]);

                GameObject.Find(itemToSpawn.placedPointName).GetComponent<PlacePoint>().empty = false;

                // Finally, add it back to the Inventory's WorldItems list
                mainInv.worldItems.Add(newItemAttributes);
            }
        }

        }
    }
}
