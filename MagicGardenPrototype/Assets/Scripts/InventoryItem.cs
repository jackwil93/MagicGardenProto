using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu]
public class InventoryItem : ScriptableObject {

    public string itemType;
    public string displayedName;
    public int potID;
    public int plantID;
    public float ageTime;
    public int invSlotNumber;
    public bool inWorld;
    public string placedPointName;
    public float placedPointX;
    public float placedPointY;
    public float placedPointZ;
}
