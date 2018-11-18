using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

public class WorldItem : MonoBehaviour { // Must be MonoBehaviour so it can exist in the scene

    GameManager GM;
    public GameItem myGameItem; // Overwritten when has a seed planted.

    GameItem secondaryGameItem; // For pots with plants. 'secondary' is the pot as plant becomes main item

    SpriteRenderer mainSprite; // The central sprite. Pot, potions, decor, etc
    SpriteRenderer topSprite; // A raised sprite for plants or things that sit on top

    // NOTE: Don't need to check if the Pot GI is empty. The Game Item is switched to Plant when it has a Seed.
    
    // Might be worth having an enum for single or double sprite (ie pot vs pot with plant)

    ItemSprites spriteSet;

    public void UpdateWorldItemStats() // Called from GM after DateTime stuff updated
    {
        // If a Plant type, add to age and check age
        if (myGameItem.itemProperties.itemType == ItemProperties.itemTypes.potWithPlant)
        {
            myGameItem.ageTimeMins += GameDateTime.realTimeSinceLastPlay;

            CalculateAgeStage();

            InvokeRepeating("OneMinuteOlder", 60, 60);
        }
    }

    void OneMinuteOlder()
    {
        myGameItem.ageTimeMins++;
    }

    void CalculateAgeStage()
    {
        switch (myGameItem.itemProperties.currentStage)
        {
            case ItemProperties.itemStage.seed:
                if (myGameItem.ageTimeMins < myGameItem.itemProperties.ageStartGerm1Stage) // Stay Seed
                {
                    StagePlanted();
                    return;
                }
                else
                {
                    myGameItem.itemProperties.currentStage = ItemProperties.itemStage.germ1;
                    CalculateAgeStage();
                }
                break;
            case ItemProperties.itemStage.germ1:
                if (myGameItem.ageTimeMins < myGameItem.itemProperties.ageStartGerm2Stage) // Stay Germ1
                {
                    StageGermination1();
                    return;
                }
                else
                {
                    myGameItem.itemProperties.currentStage = ItemProperties.itemStage.germ2;
                    CalculateAgeStage();
                }
                break;
            case ItemProperties.itemStage.germ2:
                if (myGameItem.ageTimeMins < myGameItem.itemProperties.ageStartBloomStage) // Stay Germ2
                {
                    StageGermination2();
                    return;
                }
                else
                {
                    myGameItem.itemProperties.currentStage = ItemProperties.itemStage.normal; // Bloom / 'Normal'
                    StageBloomed();
                }
                break;
        }
    }

    public void SetupSelf(GameManager gm, GameObject myGameObject, GameItem data) // Called from GameManager on Load
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
        // Check for Sprites before getting
        if (GM.GetSpriteSet(myGameItem.itemProperties.itemID) != null)
        {
            spriteSet = GM.GetSpriteSet(myGameItem.itemProperties.itemID);
        }

        if (myGameItem.itemProperties.itemType == ItemProperties.itemTypes.pot)
            mainSprite.sprite = spriteSet.normalSprites[0];
        else // If a pot with a plant, Pot is now secondary object. Use that to get pot sprite
            mainSprite.sprite = GM.GetSpriteSet(myGameItem.basePotID).normalSprites[0];

    }

    /// <summary>
    /// Overrides this item with a new Plant Game Item, keeping the pot sprite
    /// </summary>
    /// <param name="incomingPlantItem"></param>
    public void AddPlantToWorldPotItem(GameItem incomingPlantScriptable) // Called from GM
    {
        GameItem incomingPlantItem = new GameItem();
        incomingPlantItem.itemProperties = incomingPlantScriptable.itemProperties;

        incomingPlantItem.placedPointName = myGameItem.placedPointName;
        incomingPlantItem.placedPointX = myGameItem.placedPointX;
        incomingPlantItem.placedPointY = myGameItem.placedPointY;
        incomingPlantItem.placedPointZ = myGameItem.placedPointZ;
        incomingPlantItem.basePotID = myGameItem.itemProperties.itemID; // IMPORTANT. Allows loading of Pot Sprite on Game Re-Open

        secondaryGameItem = myGameItem;
        myGameItem = incomingPlantItem;

        myGameItem.itemProperties.itemType = ItemProperties.itemTypes.potWithPlant;
        myGameItem.inWorld = true;
        myGameItem.ageTimeMins = 0;

        // New Sprite Set for Plants
        spriteSet = GM.GetSpriteSet(myGameItem.itemProperties.itemID);

        StagePlanted();
    }

    void StagePlanted()
    {
        topSprite.sprite = spriteSet.plantedSprite;
    }

    void StageGermination1()
    {
        topSprite.sprite = spriteSet.germ1Sprites[0];
    }

    void StageGermination2()
    {
        topSprite.sprite = spriteSet.germ2Sprites[0];
    }

    void StageBloomed() // 'Normal'
    {
        topSprite.sprite = spriteSet.normalSprites[0];
    }

    public ItemSprites GetWorldItemSprites() // Called from Shop when Sell Window Opened
    {
        return spriteSet;
    }

}
