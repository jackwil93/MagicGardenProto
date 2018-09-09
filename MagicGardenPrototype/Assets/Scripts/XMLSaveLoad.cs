using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLSaveLoad : MonoBehaviour
{
    // For future Ref: 
    // Inventory is for run time only, lives on the GAMECORE
    // Inventory Item (a Scriptable Obj) is to make my life easier creating new items for this game
    // InventoryData is the data structure used for saving the inventory to an XML
    // InventoryItemXML is what each Item is translated to for saving to XML (Scriptables cant be loaded via XML)


    InventoryData tempInv;
    string dir;

    private void Awake()
    {
        dir = Application.persistentDataPath.ToString() + "/data/";
    }

    public void SaveGame() // Called from Game Manager
    {
        Debug.Log("Saving...");
        SaveInventory();
        SaveEmails();
    }
    
    private void SaveInventory()
    {
        Inventory mainInv = GetComponent<Inventory>();
        tempInv = mainInv.data;

        foreach (InventoryItem item in mainInv.itemsList)
        {
            InventoryItemXML itemXML = new InventoryItemXML();
            itemXML.itemID = item.itemID;
            itemXML.ageTime = item.ageTime;
            itemXML.displayedName = item.displayedName;
            itemXML.invSlotNumber = item.invSlotNumber;
            itemXML.plantID = item.plantID;
            itemXML.potID = item.potID;

            tempInv.inventoryList.Add(itemXML);
        }
        SaveXML((object)tempInv, "inv");

        tempInv.inventoryList.Clear();
    }

    private void SaveEmails()
    {

    }

    void SaveXML(object dataPackage, string fileName)
    {
        if (File.Exists(dir + fileName + ".xml"))
        {
            FileStream stream = new FileStream(dir + fileName + ".xml", FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(dataPackage.GetType());
            serializer.Serialize(stream, dataPackage);
            Debug.Log("Saved XML to " + dir + fileName + ".xml");
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

    public void LoadGame() // Called from Game Manager
    {
        LoadXML(typeof(InventoryData), "inv");
    }

    private void LoadXML(System.Type type, string fileName)
    {
            XmlSerializer loadXML = new XmlSerializer(type);
            FileStream stream = new FileStream(dir + fileName + ".xml", FileMode.Open);

        if (type == typeof(InventoryData))
        {
            InventoryData obj = (InventoryData)loadXML.Deserialize(stream);
            LoadInventoryData(obj);
        }
    }

    void LoadInventoryData(InventoryData loadedInventoryXML)
    {
        Inventory mainInventory = GetComponent<Inventory>();

        foreach (InventoryItemXML itemXML in loadedInventoryXML.inventoryList)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemID = itemXML.itemID;
            item.ageTime = itemXML.ageTime;
            item.displayedName = itemXML.displayedName;
            item.invSlotNumber = itemXML.invSlotNumber;
            item.plantID = itemXML.plantID;
            item.potID = itemXML.potID;

            mainInventory.itemsList.Add(item);
        }
    }
        
}
