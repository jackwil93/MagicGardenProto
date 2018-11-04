using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

//Base class for every kind of item in game that can be in world or in inventory
[System.Serializable]
public class GameItem {
    public ItemProperties itemProperties;
    // If you think about it, only a WorldItem would need both pot and plant ID... could be cleaned up
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
