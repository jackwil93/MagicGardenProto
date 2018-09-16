using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

[XmlRoot("Inventory")]
[System.Serializable]
public class InventoryData {

    [XmlArray("InventoryList")]
    [XmlArrayItem("InventoryItem")]
    public List<InventoryItemXML> inventoryList;
}
