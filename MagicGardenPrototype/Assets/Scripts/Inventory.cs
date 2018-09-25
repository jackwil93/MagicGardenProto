using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryData data;
    [Header("ALL items")]
    public List<InventoryItem> allItemsList = new List<InventoryItem>();
    [Header("Inventory Items Only")]
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [Header("World Items Only. Checked at Start and Save")]
    public List<WorldItem> worldItems = new List<WorldItem>(); // Loaded in by GameManager initially

    /// <summary>
    /// For saving ALL Items, in Inventory and World
    /// </summary>
    public void CheckInAllItems() // Called from XMLSaveLoad
    {
        allItemsList.Clear();
        allItemsList.AddRange(inventoryItems);

        worldItems.Clear();
        worldItems.AddRange(WorldItem.FindObjectsOfType<WorldItem>() as WorldItem[]);

        foreach (WorldItem worldItem in worldItems)
        {
            InventoryItem itemToSave = ScriptableObject.CreateInstance<InventoryItem>();

            itemToSave.itemType =         worldItem.itemType;
            itemToSave.displayedName =  worldItem.displayedName;
            itemToSave.potID =          worldItem.potID;
            itemToSave.plantID =        worldItem.plantID;
            itemToSave.ageTime =        worldItem.ageTime;
            itemToSave.invSlotNumber =  worldItem.invSlotNumber;
            itemToSave.inWorld =        worldItem.inWorld;

            itemToSave.placedPointName =    worldItem.placedPointName;
            itemToSave.placedPointX =       worldItem.placedPointX;
            itemToSave.placedPointY =       worldItem.placedPointY;
            itemToSave.placedPointZ =       worldItem.placedPointZ;

            allItemsList.Add(itemToSave);
        }
    }
}