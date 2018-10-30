using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Class is attached to GAMECORE object 
// Only manages items stored in the inventory
// For World Items, see the GameManager
public class Inventory : MonoBehaviour
{
    // Only items in Inventory. Not in world!
    public List<GameItem> itemsInInventory = new List<GameItem>();

    public void AddItemToInventory(GameItem newItem)
    {
        itemsInInventory.Add(newItem);
    }


    /// <summary>
    /// For saving ALL Items, in Inventory and World
    /// </summary>
    public void CheckInAllItems() // Called from XMLSaveLoad
    {
      
    }
}