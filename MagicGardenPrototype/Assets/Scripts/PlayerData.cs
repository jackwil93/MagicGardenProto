using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using MagicGlobal;

[System.Serializable]
[XmlRoot ("PlayerData")]
public class PlayerData
{
    [SerializeField] public int playerFlorets;
    [SerializeField] public int playerCrystals;
    [XmlArray ("SavedGameItems")]
    [XmlArrayItem ("GameItem")]
    public List<GameItem> allGameItems;
    public List<DelayedOrder> delayedOrdersUndelivered;
    public int savedMinuteOfYear;
    public int savedDayOfYear;
}
