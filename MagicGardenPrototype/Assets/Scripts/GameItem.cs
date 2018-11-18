using System.Xml.Serialization;
using MagicGlobal;

//Base class for every kind of item in game that can be in world or in inventory
[System.Serializable]
[XmlType("GameItem")]
public class GameItem {
    public ItemProperties itemProperties;
    // If you think about it, only a WorldItem would need both pot and plant ID... could be cleaned up
    public int ageTimeMins;
    public int invSlotNumber;
    public bool inWorld;
    public string placedPointName;
    public float placedPointX;
    public float placedPointY;
    public float placedPointZ;
    public string basePotID;
}
