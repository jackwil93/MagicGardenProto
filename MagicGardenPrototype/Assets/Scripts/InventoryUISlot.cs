using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour {

    public GameObject myItem;
    public int slotNumber;
    public Image itemImage;

    private void Start()
    {
        slotNumber = transform.GetSiblingIndex();
        itemImage = transform.GetComponentInChildren<Image>();
    }

    public void AssignNewItem() { }
}
