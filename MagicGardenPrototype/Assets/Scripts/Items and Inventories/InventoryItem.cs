using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {
    public GameItem myGameItem;
    public WorldItem myWorldItem;
    RawImage myRawImage;

    // Potted plants cannot be in the Inventory
    private void Start()
    {
        myRawImage = GetComponentInChildren<RawImage>();

        GameManager GM = GameManager.FindObjectOfType(typeof(GameManager)) as GameManager;

        if (myGameItem.itemType == GameItem.itemTypes.pot)
        {
            myRawImage.texture = GM.allPotSprites[myGameItem.potID].texture;
        }
    }
}
