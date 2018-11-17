using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour {

    public int slotNumber;
    [Space(20)]
    public InventoryItem myItem; // if has item, is occupied. 

    private void Start()
    {
        slotNumber = transform.GetSiblingIndex();
        if (transform.childCount > 0)
            myItem = transform.GetComponentInChildren<InventoryItem>();
    }

    public bool TryPlaceItem(Transform itemToPlace) // Called from GM
    {
        if (transform.childCount == 0)
        {
            return true;
        }
        else
            return false;
    }

    public void PlaceItemInSlot(Transform newItem) // Called from GM and Inventory on Load
    {
        if (newItem == null)
            return;


        newItem.SetParent(this.transform);
        newItem.localPosition = Vector3.zero;
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        myItem = newItem.GetComponent<InventoryItem>();
        myItem.myGameItem.invSlotNumber = slotNumber;
        myItem.myGameItem.inWorld = false;
    }

    public Transform TakeItemFromSlot()
    {
        if (myItem != null)
        {
            Debug.Log("Take item from slot " + slotNumber);
            Transform itemToGive = myItem.transform;
            itemToGive.SetParent(transform.parent);
            myItem = null;
            return itemToGive;
        }
        else
            return null;
    }
}
