using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MagicGlobal;


public class XMLSaveLoad : MonoBehaviour
{
    // For debug
    public bool loadPlayerDataOnStart;
    public bool resetEmailData;

    // For future Ref: 
    // Inventory is for run time only, lives on the GAMECORE
    // Inventory Item (a Scriptable Obj) is to make my life easier creating new items for this game
    // InventoryData is the data structure used for saving the inventory to an XML
    // InventoryItemXML is what each Item is translated to for saving to XML (Scriptables cant be loaded via XML)

    public string saveFileName = "ma";
    public string JSONfileName = "emaildata.json";

    string dir;

    private void Awake()
    {
        dir = Application.persistentDataPath.ToString() + "/data/";
    }

    public void SaveGame() // Called from Game Manager
    {
        Debug.Log("Saving...");
        GameManager GM = GetComponent<GameManager>();

        SaveCurrencies(GM.playerData);
        SaveItems(GM, GM.playerData);
        SaveDelayedOrders(GM.playerData);
        SaveTime(GM.playerData);
        SaveXML(GetComponent<GameManager>().playerData, saveFileName);

        SaveEmailsToJson();
    }

    private void SaveCurrencies(PlayerData pd)
    {
        pd.playerFlorets = Currencies.florets;
        pd.playerCrystals = Currencies.crystals;
        Debug.Log("PlayerData Currency Saved");
    }
    
    private void SaveItems(GameManager GM, PlayerData pd)
    {
        Inventory inv = GetComponent<Inventory>();

        // Refresh Player Data
        pd.allGameItems.Clear();
        
        // Add all World Items to Player Data
        foreach (WorldItem wItem in GM.allWorldItemsInScene)
        {
            pd.allGameItems.Add(wItem.myGameItem);
        }

        // Add all Inventory Items to Player Data
        pd.allGameItems.AddRange(inv.CheckInAllItems());

        Debug.Log("PlayerData GameItems Saved");
    }

    private void SaveDelayedOrders(PlayerData pd)
    {
        pd.delayedOrdersUndelivered = GetComponent<DelayedOrderManager>().GetAllDelayedOrders();
        Debug.Log("PlayerData Undelivered Orders Saved");
    }


    private void SaveTime(PlayerData pd)
    {
        pd.savedMinuteOfYear = (int)GameDateTime.LogCurrentDateTime().x;
        pd.savedDayOfYear = (int)GameDateTime.LogCurrentDateTime().y;
        
    }

    void SaveXML(object dataPackage, string fileName)
    {
        Debug.Log("Saving out XML...");
        if (File.Exists(dir + fileName + ".gic"))
        {
            FileStream stream = new FileStream(dir + fileName + ".gic", FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(dataPackage.GetType());
            serializer.Serialize(stream, dataPackage);
            Debug.Log("Saved XML to " + dir + fileName + ".gic");
            stream.Close();
        }
        else
        {
            Debug.Log("File does not exist");
            Directory.CreateDirectory(dir);
            FileStream stream = new FileStream(dir + fileName + ".xml", FileMode.Create);
            stream.Close();
            
            Debug.Log("Created new XML file");
            SaveXML(dataPackage, fileName);
        }
    }

    public void LoadGame() // Called from Game Manager on Start
    {
        Debug.Log("Loading Emails...");
        LoadEmailsFromJSON();
        Debug.Log("Loaded Emails");


        if (loadPlayerDataOnStart)
        {
            Debug.Log("Loading Player Data...");
            LoadPlayerDataXML(typeof(PlayerData), "ma");
            Debug.Log("Loaded");
        }
    }

    private void LoadPlayerDataXML(System.Type type, string fileName)
    {
        XmlSerializer loadXML = new XmlSerializer(type);

        if (!File.Exists(dir + fileName + ".gic"))
        {
            File.Create(dir + fileName + ".gic");
            Debug.Log("New Save File created");
            LoadPlayerDataXML(typeof(PlayerData), fileName);
        }

        FileStream stream = new FileStream(dir + fileName + ".gic", FileMode.Open);
        GetComponent<GameManager>().LoadPlayerData((PlayerData)loadXML.Deserialize(stream));
    }


    /// JSON SERIALISATION STUFF

    void SaveEmailsToJson()
    {
        Debug.Log("Saving over old Email JSON File...");
        string data = JsonUtility.ToJson(GetComponent<EmailManager>().CheckInAllEmails(), true);

        

        Debug.Log("email Data being saved = " + data);
        File.WriteAllText(Application.persistentDataPath + "/data/" + "testJson.json", data);
        Debug.Log("New Email JSON Saved");
    }


    void LoadEmailsFromJSON()
    {
        if (File.Exists(dir + JSONfileName))
        {
            string dataAsJson;
            if (resetEmailData)
                dataAsJson = File.ReadAllText(dir + "emailData.json");
            else
                dataAsJson = File.ReadAllText(dir + "testJson.json");

            string JsonString = fixJson(dataAsJson);

            EmailListJSON newJSONList = JsonUtility.FromJson<EmailListJSON>(dataAsJson);

            foreach (EmailEntry email in newJSONList.emailEntries)
                email.loadedToGame = false;


            GetComponent<EmailManager>().SetUpAllEmails(newJSONList);
        }
        else
            Debug.LogWarning("Json file not found");
    }

    

    // The following was learned from: 
    // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.EmailEntry;
    }

    private class Wrapper<T>
    {
        public T[] EmailEntry;
    }

    string fixJson(string value)
    {
        value = "{\"EmailEntry\":" + value + "}";
        return value;
    }


    //void LoadInventoryData(InventoryData loadedInventoryXML)
    //{
    //    Inventory mainInventory = GetComponent<Inventory>();

    //    foreach (InventoryItemXML itemXML in loadedInventoryXML.inventoryList)
    //    {
    //        InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
    //        item.itemType = itemXML.itemType;
    //        item.displayedName = itemXML.displayedName;
    //        item.ageTime = itemXML.ageTime;
    //        item.invSlotNumber = itemXML.invSlotNumber;
    //        item.plantID = itemXML.plantID;
    //        item.potID = itemXML.potID;
    //        item.inWorld = itemXML.inWorld;
    //        item.placedPointName = itemXML.placedPointName;
    //        item.placedPointX = itemXML.placedPointX;
    //        item.placedPointY = itemXML.placedPointY;
    //        item.placedPointZ = itemXML.placedPointZ;

    //        mainInventory.allItemsList.Add(item);
    //    }
    //}

}
