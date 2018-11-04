using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGlobal
{
    public static class Elements
    {
        public enum elementTypes { noElement, fire, ice, air, force, water, energy, summer, autumn, winter, spring, life, death};
    }

    public static class GameStates
    {
        public enum gameScreens
        { mainGame, emails, inventory, laptop, spellGame, settings, transactions };
    }

    [System.Serializable]
    public class ItemProperties
    {
        public int itemID;
        public string displayedName;
        public enum itemTypes { pot, plant, potWithPlant, seed, potion, decor };
        public itemTypes itemType;
        public Elements.elementTypes baseElement;
        public Elements.elementTypes elementNeeded;
        public string itemDescription;
    }
   
}
