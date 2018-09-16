using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class EmailManager : MonoBehaviour {

    public string JSONfileName = "emaildata.json";
    string filePath;

    public List<EmailEntry> emailsDavinta = new List<EmailEntry>();
    public List<EmailEntry> emailsXander = new List<EmailEntry>();


    private void Start()
    {
        filePath = Application.persistentDataPath + "/data/" + JSONfileName;
        //DataToJson();
       GetDataFromJSON();
    }

    /// <summary>
    /// characterName must be in lowercase
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="emailID"></param>
    /// <returns></returns>
    public EmailEntry GetEmailEntry (string characterName, string emailID)
    {
        switch (characterName)
        {
            case "davinta":
                return emailsDavinta.Where(EmailEntry => EmailEntry.entryID == emailID).SingleOrDefault();
                break;
            case "xander":
                return emailsXander.Where(EmailEntry => EmailEntry.entryID == emailID).SingleOrDefault();
                break;
        }
        return null;
    }

    void DataToJson()
    {
        string data = JsonUtility.ToJson(emailsDavinta[0]);
        File.AppendAllText(Application.persistentDataPath + "/data/" + "testJson.json", data);
    }

    void GetDataFromJSON()
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            string JsonString = fixJson(dataAsJson);
            EmailEntry[] emails = FromJson<EmailEntry>(JsonString);
            
            //EmailEntry emailEntry = JsonUtility.FromJson<EmailEntry>(dataAsJson);
            
            foreach (EmailEntry emailEntry in emails)
            {
            if (emailEntry.characterID == "davinta")
                emailsDavinta.Add(emailEntry);
            if (emailEntry.characterID == "xander")
                emailsXander.Add(emailEntry);
            }
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
}
