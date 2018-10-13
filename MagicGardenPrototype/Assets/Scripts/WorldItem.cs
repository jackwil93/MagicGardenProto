using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour { // Must be MonoBehaviour so it can exist in the scene

    public string itemType; // ie "potplant" for all pots with or without plants, "decor" etc
    public string displayedName;
    public int potID;
    public int plantID;
    public float ageTime;
    public int invSlotNumber = 999;
    public bool inWorld = true;
    public string placedPointName;
    public float placedPointX;
    public float placedPointY;
    public float placedPointZ;

    public void SetupSelf(GameManager GM, GameObject myGameObject, InventoryItem data) // Called from GameManager.
    {
        // "GM" is Game Manager. Passing it through as a parameter makes it easier to pull info from its public Lists
        // "myGameObject" is the object in scene which has been spawned by GameManager, this script is customising it
        // "data" refers to the data incoming from the XML SaveLoad, passed thru the GameManager

        // Get data...
        itemType =          data.itemType;
        displayedName =     data.displayedName;
        potID =             data.potID;
        plantID =           data.plantID;
        ageTime =           data.ageTime;
        invSlotNumber =     data.invSlotNumber;
        inWorld =           data.inWorld;
        placedPointName =   data.placedPointName;
        placedPointX =      data.placedPointX;
        placedPointY =      data.placedPointY;
        placedPointZ =      data.placedPointZ;

        // Set Object name
        if (displayedName == null && itemType == "potplant")
            displayedName = "PotPlant Item";

        // Set position
        this.transform.position = new Vector3(placedPointX, placedPointY, placedPointZ);

       // If a pot plant, do the following
        if (itemType == "potplant")
        {
            SpriteRenderer potSprite = transform.Find("node_pot_base").GetComponent<SpriteRenderer>();
            SpriteRenderer plantSprite = transform.Find("node_flower_base").GetComponent<SpriteRenderer>(); // TODO change to plant base

            potSprite.sprite = GM.allPotSprites[potID];

            PlantCore myPlant = plantSprite.gameObject.AddComponent<PlantCore>();
            //Get Sprites based on plantid
            myPlant.frame1 = GM.allPlantTypes[plantID].frame1;
            myPlant.frame2 = GM.allPlantTypes[plantID].frame2;
        }
    }
}
