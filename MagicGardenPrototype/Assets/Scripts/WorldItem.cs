using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour { // Must be MonoBehaviour so it can exist in the scene

    public string itemID;
    public string displayedName;
    public int potID;
    public int plantID;
    public float ageTime;
    public int invSlotNumber = 999;
    public bool inWorld = true;
    public string placedPointName;
    public float placedPointX;
    public float placedPointY;
    public float placedPointZ;

    public void SpawnSelf(GameObject pot, GameObject plant) // Called from GameManager
    {
        GameObject myPot = Instantiate(pot, this.transform);
        GameObject myPlant = Instantiate(plant, myPot.transform.Find("node_flower_base"));

        BoxCollider boxCol = this.gameObject.AddComponent<BoxCollider>();
        boxCol.center = new Vector3(0, 0.44f, 0);

        transform.tag = "moveable";
        transform.position = new Vector3(placedPointX, placedPointY, placedPointZ);

        if (itemID != null)
            this.name = itemID;
        else
            this.name = "Plant Item";
    }
}
