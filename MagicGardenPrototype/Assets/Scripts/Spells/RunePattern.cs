using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RunePattern : ScriptableObject {

    public List<IntPair> runeIntPairs = new List<IntPair>();

    public List<int> allIntsInPattern = new List<int>();

}
