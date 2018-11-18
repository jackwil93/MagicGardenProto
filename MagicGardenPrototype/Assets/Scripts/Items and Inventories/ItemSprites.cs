using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSprites {
    public Sprite seedSprite;
    public Sprite plantedSprite;
    public Sprite deadSprite;
    [Space(10)]
    public List<Sprite> germ1Sprites;
    public List<Sprite> germ2Sprites;
    public List<Sprite> normalSprites;
    public List<Sprite> sickSprites;
}
