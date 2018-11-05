﻿using System.Collections;
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
        public string itemID;
        public string displayedName;
        public enum itemTypes { pot, plant, potWithPlant, seed, potion, decor };
        public itemTypes itemType;
        public Elements.elementTypes baseElement;
        public Elements.elementTypes elementNeeded;
        public string itemDescription;
        public enum itemStage { normal, seed, germ1, germ2, special, sick, dead}
        public itemStage currentStage;
        public int buyPriceFlorets;
        public int sellPriceFlorets;
        public int buyPriceCrystals;
    }
   
}
