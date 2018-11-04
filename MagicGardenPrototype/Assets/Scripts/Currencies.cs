using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGlobal
{
    public static class Currencies
    {
        static int _florets;
        public static int florets
        {
            get { return _florets; }
        }

        static int _crystals;
        public static int crystals
        {
            get { return _crystals; }
        }


        public static bool SubtractFlorets(int value)
        {
            if (florets - value >= 0)
            {
                string s = "Player had " + florets + " florets.";
                _florets -= value;
                Debug.Log(s + value + " florets subtracted. Player now has " + florets + " florets");

                return true;
            }
            else
                Debug.LogWarning("Cannot subtract " + value + " from " + florets + " florets!");
                return false;
        }

        public static void AddFlorets(int value)
        {
            _florets += value;
            Debug.Log(value + " florets added to player total. Player now has " + florets + " florets.");
        }

        public static void OverrideFlorets(int newAmount) // Only use on load
        {
            _florets = newAmount;
            Debug.Log("florets overridden to " + newAmount + " florets. Proof: " + florets);
        }


        public static bool SubtractCrystals(int value)
        {
            if (crystals - value >= 0)
            {
                string s = "Player had " + crystals + " crystals.";

                _crystals -= value;
                Debug.Log(s + value + " crystals subtracted. Player now has " + crystals + " crystals");
                return true;
            }
            else
                Debug.LogWarning("Cannot subtract " + value + " from " + crystals + " crystals!");
            return false;
        }

        public static void AddCrystals(int value)
        {
            _crystals += value;
            Debug.Log(value + " crystals added to player total. Player now has " + crystals + " crystals.");
        }

        public static void OverrideCrystals(int newAmount) // Only use on load
        {
            _crystals = newAmount;
            Debug.Log("crystals overridden to " + newAmount + " crystals. Proof: " + crystals);
        }
    }

}
