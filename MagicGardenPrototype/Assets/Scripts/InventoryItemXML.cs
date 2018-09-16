using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class InventoryItemXML {
    [XmlAttribute("itemID")]
    public string itemID;
    public string displayedName;
    public string potID;
    public string plantID;
    public float ageTime;
    public int invSlotNumber;
}
