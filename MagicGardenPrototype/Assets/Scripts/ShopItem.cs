using UnityEngine;

// Class is not XML'able because of Sprite Property
[System.Serializable]
public class ShopItem {
    public GameItem mainGameItem; // ie Plant
    public GameItem secondaryGameItem; // Pot, when has Plant
    public Sprite mainItemIcon;
    public Sprite secondaryItemIcon;
    public bool sold;
}
