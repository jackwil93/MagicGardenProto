using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using CurrencyManagement;

[System.Serializable]
[XmlRoot ("PlayerData")]
public class PlayerData
{
    public int playerFlorets;
    public int playerCrystals;
    [XmlArray ("SavedGameItems")]
    [XmlArrayItem ("GameItem")]
    public List<GameItem> allGameItems;
}
