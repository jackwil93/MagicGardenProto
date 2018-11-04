using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class InventoryItem : MonoBehaviour {
    public GameItem myGameItem;
    public WorldItem myWorldItem;
    RawImage myRawImage;

    // Potted plants cannot be in the Inventory
    private void Start()
    {
        myRawImage = GetComponentInChildren<RawImage>();

        GameManager GM = GameManager.FindObjectOfType(typeof(GameManager)) as GameManager;

        if (myGameItem.itemProperties.itemType == ItemProperties.itemTypes.pot)
        {
            myRawImage.texture = GM.allPotSprites[myGameItem.potID].texture;
        }
    }
}
