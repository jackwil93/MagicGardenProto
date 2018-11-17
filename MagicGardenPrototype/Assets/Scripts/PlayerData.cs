using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
[XmlRoot ("PlayerData")]
public class PlayerData
{
    [SerializeField] public int playerFlorets;
    [SerializeField] public int playerCrystals;
    [XmlArray("SavedGameItems")]
    [XmlArrayItem("GameItem")]
    public List<GameItem> allGameItems = new List<GameItem>();
    [XmlArray("DelayedOrdersUndelivered")]
    [XmlArrayItem("DelayedOrder")]
    public List<DelayedOrder> delayedOrdersUndelivered = new List<DelayedOrder>();
    public int savedMinuteOfYear;
    public int savedDayOfYear;
    public bool newGame;

}
