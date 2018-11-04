using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

public class WorldItem : MonoBehaviour { // Must be MonoBehaviour so it can exist in the scene

    GameManager GM;
    public GameItem myGameItem;
    GameItem secondaryGameItem; // For pots with plants. 'myGameItem' would be the pot, secondaryGameItem would be the plant
    SpriteRenderer mainSprite; // The central sprite. Pot, potions, decor, etc
    SpriteRenderer topSprite; // A raised sprite for plants or things that sit on top

    ItemSprites spriteSet;

    public void SetupSelf(GameManager gm, GameObject myGameObject, GameItem data) // Called from GameManager.
    {
        GM = gm;
        myGameItem = data;
        mainSprite = transform.Find("node_pot_base").GetComponent<SpriteRenderer>();
        topSprite = transform.Find("node_flower_base").GetComponent<SpriteRenderer>();

        // Set Object name
        transform.name = myGameItem.itemProperties.displayedName;
        if (transform.name == "")
            transform.name = "World Item";

        // Set position
        this.transform.position = new Vector3(myGameItem.placedPointX, myGameItem.placedPointY, myGameItem.placedPointZ);

        // Get Sprites from GM Sprite Dictionary
        SetUpSprites();
        // TODO: Add getting sprites for plants. Long way off yet. Will need seed planting etc first


        // Finally, tick bool
        myGameItem.inWorld = true;
    }

    void SetUpSprites()
    {
        spriteSet = GM.GetSpriteSet(myGameItem.itemProperties.itemID);
        mainSprite.sprite = spriteSet.normalSprites[0];
    }

}
