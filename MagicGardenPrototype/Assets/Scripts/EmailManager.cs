using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class EmailManager : MonoBehaviour
{

    public string JSONfileName = "emaildata.json";
    string filePath;


    List<EmailEntry> allEmailsInData = new List<EmailEntry>(); // Refers to ALL emails in the entire game
    List<EmailEntry> allReceivedEmails = new List<EmailEntry>(); // The emails the player has received based on their choices. Saved to PD

    /// <summary>
    /// string = conversationID. The character's name in lowercase ie "davinta"
    /// </summary>
    public Dictionary<string, EmailConversation> emailConversationsDictionary;

    public void SetUpAllEmails(EmailEntry[] loadedEmails) // Called from XMLSaveLoad
    {
        foreach (EmailEntry email in loadedEmails)
        {
            // Make new EmailConversations if not already made
            if (emailConversationsDictionary.ContainsKey(email.conversationID) == false)
            {
                EmailConversation newConversation = new EmailConversation();
                newConversation.conversationID = email.conversationID;

                emailConversationsDictionary.Add(newConversation.conversationID, newConversation);
            }

            // Check if player has received this email before. If so, add it to the conversation
            if (email.received)
            {
                emailConversationsDictionary[email.conversationID].AddNextEmail(email);
            }

            // Find Max stage for each conversation
            emailConversationsDictionary[email.conversationID].CheckMaxStage(email.stage);

        }
    }

        /// <summary>
        /// currentEmail is the EmailEntry the player is replying to.
        /// playerReplyGBN is a single-character string g,b,or n
        /// </summary>
        /// <param name="currentEmail"></param>
    public void RecordPlayerReplyAndQueueNextEmail(EmailEntry currentEmail, string playerReplyGBN, string playerReplyFull) 
    // Will run when player has replied to an email. Finds next Email, Packs it, sends to DeliveryMgr
    // NOTE: Has to queue TWO emails: The initial reply *and* the next item order (if there is one)
    {
        EmailConversation currentConversation = emailConversationsDictionary[currentEmail.conversationID];


        // -------------------- RECORD PLAYER RESPONSE --------------
        // Before Anything else, record the player's response. Important for saving and loading emails later
        currentEmail.playerReplyGBN = playerReplyGBN;

        EmailEntry playerReplyEmail = new EmailEntry();
        playerReplyEmail.conversationID = currentEmail.conversationID;
        playerReplyEmail.characterName = "player";
        playerReplyEmail.stage = currentEmail.stage;
        playerReplyEmail.entryID = currentEmail.conversationID + "_" + currentEmail.stage + "_" + "player";
        playerReplyEmail.bodyText = playerReplyFull;
        playerReplyEmail.received = true;

        // Add player reply to Conversation
        currentConversation.AddNextEmail(playerReplyEmail);

        
        
        // ----------------- FIND NEXT EMAIL ----------------
        // Find next emailID
        // Examples: davinta_0_normal_initial_n
        //davinta_0_normal_reply_g
        //davinta_0_normal_reply_b
        //davinta_0_normal_reply_n
      

        // Create the string to find the next emailID (The immediate reply to the player).
        string replyEmailID = currentEmail.conversationID + "_" + currentEmail.stage + "_" +
            currentEmail.normalOrLove + "_" + "reply" + "_" + playerReplyGBN;

        // Create the email on disc, we will send it to Delivery Manager shortly
        EmailEntry replyEmail = allEmailsInData.Where(EmailEntry => EmailEntry.entryID == replyEmailID).SingleOrDefault();



        // ---------------- QUEUE NEXT TIME EMAIL ---------------

        EmailEntry nextTimeEmail = new EmailEntry();

        if (currentConversation.stage < currentConversation.maxStage)
        {
            nextTimeEmail = allEmailsInData.Where(EmailEntry => EmailEntry.conversationID == currentEmail.conversationID &&
                EmailEntry.stage == currentEmail.stage + 1).SingleOrDefault();
        }


        // ---------------- SEND NEXT EMAILS TO DELIVERY MANAGER -----------
        
        // Immediate NPC Response Order
        Order emailOrder = new Order();
        emailOrder.myOrderType = Order.orderType.email;
        emailOrder.orderAmount = 1;
        emailOrder.orderID = replyEmail.entryID;
        GetComponent<DelayedOrderManager>().AddNewOrder(emailOrder, 1); // 1 minute

        // Next time NPC Initial Email Order
        if (nextTimeEmail.entryID != null)
        {
            Order nextTimeEmailOrder = new Order();
            nextTimeEmailOrder.myOrderType = Order.orderType.email;
            nextTimeEmailOrder.orderAmount = 1;
            nextTimeEmailOrder.orderID = nextTimeEmail.entryID;
            GetComponent<DelayedOrderManager>().AddNewOrder(nextTimeEmailOrder, 360); // Six hours
        }

    }



    public void PutNextEmailToConversation(string receivedEmailID) // Called from Delivery Manager when EmailOrder is Received
    {
        // Find the relevant email, then use the property conversationID to find where to send it

       
        EmailEntry nextEmail = allEmailsInData.Where(EmailEntry => EmailEntry.entryID == receivedEmailID).SingleOrDefault();
        nextEmail.received = true;

        // Send to EmailConversation
        emailConversationsDictionary[nextEmail.conversationID].AddNextEmail(nextEmail);

    }




















    //    private void Start()
    //    {

    //        filePath = Application.persistentDataPath + "/data/" + JSONfileName;
    //        //DataToJson();
    //       GetDataFromJSON();
    //    }

    //    /// <summary>
    //    /// characterName must be in lowercase
    //    /// </summary>
    //    /// <param name="characterName"></param>
    //    /// <param name="emailID"></param>
    //    /// <returns></returns>
    //    public EmailEntry GetEmailEntry (string characterID, string emailID)
    //    {
    //        switch (characterID)
    //        {
    //            case "davinta":
    //                return allEmailsDavinta.Where(EmailEntry => EmailEntry.entryID == emailID).SingleOrDefault();
    //            case "xander":
    //                return allEmailsXander.Where(EmailEntry => EmailEntry.entryID == emailID).SingleOrDefault();
    //            case "mrstew":
    //                return allEmailsMrsTew.Where(EmailEntry => EmailEntry.entryID == emailID).SingleOrDefault();
    //        }
    //        return null;
    //    }

    //    public List<EmailEntry> GetAllEmails (string characterID)
    //    {
    //        switch (characterID)
    //        {
    //            case "davinta":
    //                return allEmailsDavinta;
    //            case "xander":
    //                return allEmailsXander;
    //            case "mrstew":
    //                return allEmailsMrsTew;
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// characterName must be lowercase.
    //    /// </summary>
    //    /// <param name="characterName"></param>
    //    /// <returns>Returns most recently received of their emails</returns>
    //    public EmailEntry GetLatestEmailEntry(string characterID)
    //    {
    //        return GetConversation(characterID).latestEmail;
    //    }




    //    /// JSON SERIALISATION STUFF

    //    void DataToJson()
    //    {
    //        string data = JsonUtility.ToJson(allEmailsDavinta[0]);
    //        File.AppendAllText(Application.persistentDataPath + "/data/" + "testJson.json", data);
    //    }

    //    void GetDataFromJSON()
    //    {
    //        if (File.Exists(filePath))
    //        {
    //            string dataAsJson = File.ReadAllText(filePath);

    //            string JsonString = fixJson(dataAsJson);
    //            EmailEntry[] emails = FromJson<EmailEntry>(JsonString);


    //            foreach (EmailEntry emailEntry in emails)
    //            {
    //                // If this character hasn't appeared yet, add their name in list
    //                if (!conversationsByNameList.Contains(emailEntry.characterID))
    //                    conversationsByNameList.Add(emailEntry.characterID);


    //                if (emailEntry.characterID == "davinta")
    //                {
    //                    allEmailsDavinta.Add(emailEntry);
    //                    Debug.Log("New email added to Davinta");
    //                }
    //                if (emailEntry.characterID == "xander")
    //                {
    //                    allEmailsXander.Add(emailEntry);
    //                    Debug.Log("New email added to Xander");

    //                }
    //                if (emailEntry.characterID == "mrstew")
    //                {
    //                    allEmailsMrsTew.Add(emailEntry);
    //                    Debug.Log("New email added to Mrs Tew");

    //                }
    //            }

    //            CreateActiveConversatons();
    //        }
    //        else
    //            Debug.LogWarning("Json file not found");
    //    }

    //    void CreateActiveConversatons() // MUST only run once
    //    {
    //        foreach (string charName in conversationsByNameList)
    //        {
    //            EmailConversation newConvo = this.gameObject.AddComponent<EmailConversation>();
    //            newConvo.characterID = charName;
    //            activeConversations.Add(newConvo);


    //            switch (charName)
    //            {
    //                case "davinta":
    //                    SortConversationEmails(allEmailsDavinta, newConvo);
    //                    break;
    //                case "xander":
    //                    SortConversationEmails(allEmailsXander, newConvo);
    //                    break;
    //                case "mrstew":
    //                    SortConversationEmails(allEmailsMrsTew, newConvo);
    //                    break;
    //            }
    //        }
    //    }

    //    public List<EmailEntry> GetConversationEmails(string characterID) // Called from Menu Manager to populate the conversation window
    //    {
    //       return activeConversations.Where(EmailConversation => EmailConversation.characterID == characterID).SingleOrDefault().emailConvoList;
    //    }

    //    public EmailConversation GetConversation(string characterID)
    //    {
    //        return activeConversations.Where(EmailConversation => EmailConversation.characterID == characterID).SingleOrDefault();
    //    }

    //    /// <summary>
    //    /// Get the character's Response email to the Player's Reply. This completes the Stage
    //    /// </summary>
    //    /// <param name="referenceEmail"></param>
    //    /// <returns></returns>
    //    public EmailEntry GetReturnEmail(EmailEntry referenceEmail)
    //    {
    //        targetList.Clear();
    //        targetList = GetAllEmails(referenceEmail.characterID);

    //        for (int i = 0; i < targetList.Count; i++)
    //        {
    //            // If email is the next in this stage and matches with the players reply (g/b/n) then it is the reply email
    //            if (targetList[i].initialOrReply == "reply" && targetList[i].stage == referenceEmail.stage && targetList[i].characterReplyGBN == referenceEmail.playerReplyGBN)
    //            {
    //                EmailEntry e = targetList[i];
    //                e.received = true;
    //                return e;
    //            }
    //        }
    //        return null;
    //    }

    //    public void ReplyToEmailConversation(EmailEntry repliedEmail) // Called from Menu Manager after Player replies
    //    {
    //        // Save reply to the current email, then fetch the next one
    //        repliedEmail.replied = true;

    //        EmailConversation targetConversation = GetConversation(repliedEmail.characterID);
    //        targetConversation.latestEmail.playerReplyGBN = repliedEmail.playerReplyGBN;

    //        EmailEntry returnEmail = GetReturnEmail(repliedEmail);
    //        targetConversation.emailConvoList.Add(returnEmail);

    //        // Set the new latest email
    //        targetConversation.latestEmail = returnEmail;
    //    }

    //    // Used at Set Up to get all conversation emails in and find the latest email
    //    void SortConversationEmails(List<EmailEntry> listToSort, EmailConversation targetConversation)
    //    {
    //        foreach (EmailEntry email in listToSort)
    //        {
    //            if (email.initialOrReply == "initial")
    //            {
    //                targetConversation.emailConvoList.Add(email);
    //                targetConversation.latestEmail = email;
    //                return;
    //            }
    //            else if (email.received && email.initialOrReply == "reply")
    //            {
    //                targetConversation.emailConvoList.Add(email);
    //            }
    //        }

    //        // Find latest email
    //        foreach (EmailEntry email in targetConversation.emailConvoList)
    //        {
    //            if (email.stage > targetConversation.stage)
    //            {
    //                targetConversation.stage = email.stage;
    //                if (!email.replied)
    //                    targetConversation.latestEmail = email;
    //            }
    //            else if (email.replied == false && email.stage == 0 && targetConversation.stage == 0)
    //            {
    //                targetConversation.latestEmail = email;
    //            }
    //        }

    //       // Not sure if needed? GetNextEmail(targetConversation.emailConvoList[targetConversation.emailConvoList.Count -1]);
    //    }


    //    // The following was learned from: 
    //    // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111

    //    public static T[] FromJson<T>(string json)
    //    {
    //        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
    //        return wrapper.EmailEntry;
    //    }

    //    private class Wrapper<T>
    //    {
    //        public T[] EmailEntry;
    //    }

    //    string fixJson(string value)
    //    {
    //        value = "{\"EmailEntry\":" + value + "}";
    //        return value;
    //    }
    //}
}