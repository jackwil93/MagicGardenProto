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
    [XmlArray("SavedShopItems")]
    [XmlArrayItem("ItemForSale")]
    public List<GameItem> itemsInShop = new List<GameItem>();
    [XmlArray("DelayedOrdersUndelivered")]
    [XmlArrayItem("DelayedOrder")]
    public List<DelayedOrder> delayedOrdersUndelivered = new List<DelayedOrder>();
    public int savedMinuteOfYear;
    public int savedDayOfYear;
    public int shopRating; // 0 - 5, 5 Star Rating
    public bool newGame;

}
