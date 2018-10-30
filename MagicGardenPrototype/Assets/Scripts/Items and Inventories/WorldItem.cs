using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour { // Must be MonoBehaviour so it can exist in the scene

    GameManager GM;
    public GameItem myGameItem;
    SpriteRenderer mainSprite; // The central sprite. Pot, potions, decor, etc
    SpriteRenderer topSprite; // A raised sprite for plants or things that sit on top

    public void SetupSelf(GameManager gm, GameObject myGameObject, GameItem data) // Called from GameManager.
    {
        GM = gm;
        myGameItem = data;
        mainSprite = transform.Find("node_pot_base").GetComponent<SpriteRenderer>();
        topSprite = transform.Find("node_flower_base").GetComponent<SpriteRenderer>();

        // Set Object name
        transform.name = myGameItem.displayedName;
        if (transform.name == "")
            transform.name = "World Item";

        // Set position
        this.transform.position = new Vector3(myGameItem.placedPointX, myGameItem.placedPointY, myGameItem.placedPointZ);

        // If just a pot, do the following
        if (myGameItem.itemType == GameItem.itemTypes.pot)
            SetPotSprite();

        // If a pot plant, do the following
        if (myGameItem.itemType == GameItem.itemTypes.potWithPlant)
        {
            SetPotSprite();
            //Get Plant Sprites based on plantid
            SetPlantSprites();
        }

        if (myGameItem.itemType == GameItem.itemTypes.potion)
            SetPotionSprite();


        // Finally, tick bool
        myGameItem.inWorld = true;
    }
    void SetPotSprite()
    {
        mainSprite.sprite = GM.allPotSprites[myGameItem.potID];
    }

    void SetPlantSprites()
    {
        PlantCore myPlant = topSprite.gameObject.AddComponent<PlantCore>();
        myPlant.frame1 = GM.allPlantTypes[myGameItem.plantID].frame1;
        myPlant.frame2 = GM.allPlantTypes[myGameItem.plantID].frame2;
    }

    void SetPotionSprite()
    { }

    void SetDecorSprite()
    { }
}
