using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpellTest : ScriptableObject {
    public Spell spellLearned;

    [Space (20)]
    public float timeForTest;
    public List<RunePress> runeCombo1 = new List<RunePress>();
    public List<RunePress> runeCombo2 = new List<RunePress>();
    public List<RunePress> runeCombo3 = new List<RunePress>();
    public List<RunePress> runeCombo4 = new List<RunePress>();
    public List<RunePress> runeCombo5 = new List<RunePress>();
}

[System.Serializable]
public class RunePress
{
    public int runeNumber;
    public int presses;
}
