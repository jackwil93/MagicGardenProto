using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

// This Class is attached to GAMECORE object 
// Only manages items stored in the inventory
// For World Items, see the GameManager
public class Inventory : MonoBehaviour
{
    // Only items in Inventory. Not in world!
    public GameObject inventoryItemPrefab;
    public List<GameItem> itemsInInventory = new List<GameItem>();
    List<InventoryUISlot> itemSlots = new List<InventoryUISlot>();

    InventoryUISlot targetSlot; // For putting item to Inventory on Purchase

    [Header ("Auto-Assigned")]
    public Transform panelPots;
    public Transform panelSeeds;
    public Transform panelDecor;

    private void Start()
    {
        itemSlots.AddRange(InventoryUISlot.FindObjectsOfType<InventoryUISlot>());
        panelPots = GameObject.Find("Panel_Pots").transform;
        panelSeeds = GameObject.Find("Panel_Seeds").transform;
        panelDecor = GameObject.Find("Panel_Decor").transform;
    }

    public void AddItemToInventory(GameItem newItem) // Called from GameManager on Load and TODO: when purchasing an item
    {
        itemsInInventory.Add(newItem);
        // Sort to the correct Inventory Tab and instantiate

        CreateUIItem();

    }

    Transform InventoryTab(GameItem item)
    {
        switch (item.itemProperties.itemType)
        {
            case ItemProperties.itemTypes.pot:
                return panelPots;
            case ItemProperties.itemTypes.seed:
                return panelSeeds;
            case ItemProperties.itemTypes.decor:
                return panelDecor;
            case ItemProperties.itemTypes.potion:
                return panelPots;
        }
        return null;
    }

    public void RemoveItemFromInventory(GameItem item)
    {
        if (itemsInInventory.Contains(item))
            itemsInInventory.Remove(item);
    }

    void CreateUIItem()
    {
        GameObject newInvItem = Instantiate(inventoryItemPrefab);
        // Fix weird scale bug
        // Assign GameItem (Latest added)
        GameItem gi = itemsInInventory[itemsInInventory.Count - 1];
        newInvItem.GetComponent<InventoryItem>().myGameItem = gi;

        Transform panelTab = InventoryTab(gi);

        // Put to correct position
        InventoryUISlot invSlot = panelTab.GetChild(gi.invSlotNumber).GetComponent<InventoryUISlot>();
        if (invSlot.TryPlaceItem(newInvItem.transform))
        {
            invSlot.PlaceItemInSlot(newInvItem.transform);
            newInvItem.transform.localScale = Vector3.one;
            newInvItem.GetComponent<RectTransform>().localScale = Vector3.one;
        }
        else if (gi.invSlotNumber < panelTab.childCount)
        {
            itemsInInventory.Remove(gi);
            Debug.Log("Inventory item loading shares occupied slot. Deleting item...");
            Destroy(newInvItem);
        }
        else 

        Debug.Log("Inventory Item Loaded: " + gi.itemProperties.itemType + " " + gi.itemProperties.displayedName + " at " + panelTab.name + " slot " + gi.invSlotNumber);
    }



    // Specifically to check if there is a free slot to allow purchase of a new item
    public bool CheckIfFreeSlot(GameItem gameItem)
    {
        Transform targetTab = InventoryTab(gameItem);

        List<InventoryUISlot> slots = new List<InventoryUISlot>();
        slots.AddRange(targetTab.GetComponentsInChildren<InventoryUISlot>());

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].myItem == null)
            {
                gameItem.invSlotNumber = i;
                targetSlot = slots[i];
                Debug.Log("Found free slot at slot " + i);
                return true;
            }
            else if (i == slots.Count)
                return false;
        }
        return false;
    }
    


    /// <summary>
    /// For saving all items in Inventory
    /// </summary>
    public List<GameItem> CheckInAllItems() // Called from XMLSaveLoad
    {
        itemsInInventory.Clear();
        foreach (InventoryUISlot slot in itemSlots)
        {
            if (slot.myItem != null)
                itemsInInventory.Add(slot.myItem.myGameItem);
        }
        Debug.Log(itemsInInventory);
        return itemsInInventory;
    }
}