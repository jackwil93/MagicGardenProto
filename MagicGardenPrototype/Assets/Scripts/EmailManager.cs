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

    public List<string> conversationsList = new List<string>(); // PURELY for storing how many conversations exist and with who

    List<EmailEntry> targetList; // To make accessing lists a little easier

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
    public EmailEntry GetEmailEntry (string characterID, string emailID)
    {
        switch (characterID)
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

    public List<EmailEntry> GetAllEmails (string characterID)
    {
        switch (characterID)
        {
            case "davinta":
                return emailsDavinta;
                break;
            case "xander":
                return emailsXander;
                break;
        }
        return null;
    }

    /// <summary>
    /// characterName must be lowercase.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns>Returns most recently received of their emails</returns>
    public EmailEntry GetLatestEmailEntry(string characterID)
    {
        switch (characterID)
        {
            case "davinta":
                targetList = emailsDavinta;
                break;
            case "xander":
                targetList = emailsXander;
                break;
        }

        if (targetList.Count > 1)
            return targetList[targetList.Count - 1];
        else
            return targetList[0];
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
                if (!conversationsList.Contains(emailEntry.characterID))
                    conversationsList.Add(emailEntry.characterID);


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
