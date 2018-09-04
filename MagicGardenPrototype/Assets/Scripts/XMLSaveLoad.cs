using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLSaveLoad : MonoBehaviour
{
    [Header("Will auto save a file on play")]
    public int i;

    public EmailCharacter testChar;
    public string docToLoad;

    private void Start()
    {
        SaveXML();
        LoadXML();
    }

    public void SaveXML()
    {
        //var serializer = new XmlSerializer(typeof(EmailCharacter));

        if (File.Exists(Path.Combine(Application.dataPath, "XMLs/character1.xml")))
        {
            FileStream stream = new FileStream(Path.Combine(Application.dataPath, "XMLs/character1.xml"), FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(EmailCharacter));

            serializer.Serialize(stream, testChar);
            Debug.Log("Saved XML to " + Path.Combine(Application.dataPath, "XMLs/character1.xml"));
        }
        else
        {
            Debug.Log("File does not exist");
            File.CreateText(Path.Combine(Application.dataPath, "XMLs/character1.xml"));
            Debug.Log("Created new XML file");
            SaveXML();
        }
    }

    public void LoadXML()
    {
        XmlSerializer loadXML = new XmlSerializer(typeof(EmailCharacter));
        FileStream stream = new FileStream(Path.Combine(Application.dataPath, "XMLs/" + docToLoad + ".xml"), FileMode.Open);
        EmailCharacter charNew = loadXML.Deserialize(stream) as EmailCharacter;

        Debug.Log("Read " + docToLoad + ":\nName = " + charNew.name + "\nEmails = " + charNew.emailList.Count);
    }
        
}
