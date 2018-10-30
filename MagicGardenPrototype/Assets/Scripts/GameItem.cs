using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

//Base class for every kind of item in game that can be in world or in inventory
[System.Serializable]
public class GameItem {
    public enum itemTypes { pot,plant,potWithPlant,seed,potion,decor};
    public itemTypes itemType;
    public string displayedName;
    public int potID;
    public int plantID;
    public float ageTime;
    public int invSlotNumber = 999;
    public bool inWorld;
    public string placedPointName;
    public float placedPointX;
    public float placedPointY;
    public float placedPointZ;
}
