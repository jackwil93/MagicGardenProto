using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

// This class is purely for creating items in Editor, later turned into Game Items to be handled by the game
[CreateAssetMenu]
public class Item : ScriptableObject {
    ItemProperties itemProperties;
}
