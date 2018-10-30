using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
[System.Serializable]
[XmlRoot ("PlayerData")]
public class PlayerData
{
    [XmlArray ("SavedGameItems")]
    [XmlArrayItem ("GameItem")]
    public List<GameItem> allGameItems;
}
