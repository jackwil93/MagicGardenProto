using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicGlobal;

// This class is purely for creating items in Editor, later turned into Game Items to be handled by the game
[CreateAssetMenu]
public class Item : ScriptableObject {
    [Space(100)]
    [SerializeField]
    public Texture referenceIcon; // Only for editor
    [Header ("Item Sprites")]
    [SerializeField] public ItemSprites itemSprites;

    [Space (10)]
    [Header ("Item Properties")]
    [SerializeField] public ItemProperties itemProperties;
}
