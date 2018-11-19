using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class EmailManager : MonoBehaviour
{
    public GameObject laptopScreenNotification;

    [SerializeField]
    public EmailListJSON emailsForJSON = new EmailListJSON();

    public List<EmailEntry> allEmailsInData = new List<EmailEntry>(); // Refers to ALL emails in the entire game
    //List<EmailEntry> allReceivedEmails = new List<EmailEntry>(); // The emails the player has received based on their choices. Saved to PD

    /// <summary>
    /// string = conversationID. The character's name in lowercase ie "davinta"
    /// </summary>
    [SerializeField]
    public Dictionary<string, EmailConversation> emailConversationsDictionary = new Dictionary<string, EmailConversation>();




    public void SetUpAllEmails(EmailListJSON loadedEmails) // Called from XMLSaveLoad
    {
        foreach (EmailEntry email in loadedEmails.emailEntries)
        {
            allEmailsInData.Add(email);
            Debug.Log("loaded email entryID = " + email.entryID + " | conversationID = " + email.conversationID);

            // Make new EmailConversations if not already made
            if (emailConversationsDictionary.ContainsKey (email.conversationID) == false)
            {
                EmailConversation newConversation = new EmailConversation();
                newConversation.conversationID = email.conversationID;

                emailConversationsDictionary.Add(newConversation.conversationID, newConversation);
                Debug.Log("Created New Email Conversation: " + newConversation.conversationID);

                // Add the initial email 
                newConversation.AddNextEmail(email);
            }

            // Check if player has received this email before. If so, add it to the conversation
            if (email.received)
            {
                emailConversationsDictionary[email.conversationID].AddNextEmail(email);
            }

            // Find Max stage for each conversation
            emailConversationsDictionary[email.conversationID].CheckMaxStage(email.stage);

        }

        CheckNotifications();
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
        currentEmail.replied = true;

        EmailEntry playerReplyEmail = new EmailEntry();
        playerReplyEmail.conversationID = currentEmail.conversationID;
        playerReplyEmail.characterName = "Player";
        playerReplyEmail.stage = currentEmail.stage;
        playerReplyEmail.entryID = currentEmail.conversationID + "_" + currentEmail.stage + "_" + "player";
        playerReplyEmail.bodyText = playerReplyFull;
        playerReplyEmail.received = true;
        playerReplyEmail.dateTime = System.DateTime.Now.ToString("dddd MMM dd h:mm:ss tt");

        // Put player reply to allEmails List in the right spot
        int targetIndex = allEmailsInData.IndexOf(currentEmail) + 1;
        allEmailsInData.Insert(targetIndex, playerReplyEmail);

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
            currentEmail.state + "_" + "reply" + "_" + playerReplyGBN;
        Debug.Log("Next NPC Reply EmailID = " + replyEmailID);

        // Create the email on disc, we will send it to Delivery Manager shortly
        EmailEntry replyEmail = GetEmailByID(replyEmailID);
        Debug.Log("Found reply email: " + replyEmail.entryID);


        // ---------------- QUEUE NEXT TIME EMAIL ---------------

        EmailEntry nextTimeEmail = new EmailEntry();
        string nextEmailID = currentEmail.conversationID + "_" + (currentEmail.stage + 1) + "_" +
            "normal" + "_" + "initial" + "_" + "n";

        if (currentConversation.stage < currentConversation.maxStage)
        {
            nextTimeEmail = GetEmailByID(nextEmailID);
        }


        // ---------------- SEND NEXT EMAILS TO DELIVERY MANAGER -----------
        
        // Immediate NPC Response Order
        Order emailOrder = new Order();
        emailOrder.myOrderType = Order.orderType.email;
        emailOrder.orderAmount = 1;
        emailOrder.orderID = replyEmail.entryID;
        GetComponent<DelayedOrderManager>().AddNewOrder
            (emailOrder, 1, "You have a reply email from " + replyEmail.characterName); // 1 minute

        // Next time NPC Initial Email Order
        if (nextTimeEmail.entryID != null)
        {
            Order nextTimeEmailOrder = new Order();
            nextTimeEmailOrder.myOrderType = Order.orderType.email;
            nextTimeEmailOrder.orderAmount = 1;
            nextTimeEmailOrder.orderID = nextTimeEmail.entryID;

            // Randomise delivery time between 1 - 6 hours (adjusted to 1 hour for development)
            int deliveryTime = 60;

            GetComponent<DelayedOrderManager>().AddNewOrder
                (nextTimeEmailOrder, deliveryTime, "You have a new email from " + replyEmail.characterName + "!"); // Six hours
        }

    }

    EmailEntry GetEmailByID(string ID)
    {
        Debug.Log("Attempting to search " + allEmailsInData.Count + " items in allEmailsInData");
        foreach (EmailEntry email in allEmailsInData)
        {
            Debug.Log("Searching for: " + ID + " | " + email.entryID);
            if (email.entryID == ID)
            {
                Debug.Log("Found Matching Email by ID");
                return email;
            }
        }
        Debug.Log("Found No Match by ID: " + ID);
        return null;
    }

    public void PutNextEmailToConversation(string receivedEmailID) // Called from Delivery Manager when EmailOrder is Received
    {
        // Find the relevant email, then use the property conversationID to find where to send it

       
        EmailEntry nextEmail = allEmailsInData.Where(EmailEntry => EmailEntry.entryID == receivedEmailID).SingleOrDefault();
        nextEmail.received = true;

        // Send to EmailConversation
        emailConversationsDictionary[nextEmail.conversationID].AddNextEmail(nextEmail);

        // Show 'New' Notification
        laptopScreenNotification.SetActive(true);

    }

    public EmailListJSON CheckInAllEmails()
    {
        foreach (KeyValuePair<string, EmailConversation> eConvo in emailConversationsDictionary)
            foreach (EmailEntry emailEntry in eConvo.Value.receivedEmails)
                if (!allEmailsInData.Contains(emailEntry))
                {
                    allEmailsInData.Add(emailEntry);
                    Debug.Log(emailEntry.entryID + " added to emailsInData");
                }

        emailsForJSON.emailEntries.Clear();
        emailsForJSON.emailEntries.AddRange(allEmailsInData);


        return emailsForJSON;
    }

    public void CheckNotifications()
    {
        Debug.Log("Checking Notifications");

        laptopScreenNotification.SetActive(false);

        foreach (EmailEntry receivedEmail in allEmailsInData)
        {
            if (receivedEmail.received && receivedEmail.opened == false)
            {
                laptopScreenNotification.SetActive(true);
                break;
            }
        }
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




    //    

    //    
    //    
    //    
    //    
    //    

    //    
    //    
    //    
    //    
    //    

    //    
    //    


    //    
    //    
    //    
    //    
    //    


    //    
    //    
    //    
    //    
    //    
    //    
    //    
    //    
    //    

    //    
    //    
    //    
    //    
    //    

    //    
    //    

    //    
    //    
    //    
    //    
    //    

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


    //   
    //   

    //   
    //   
    //   
    //   
    //   

    //   
    //   
    //   
    //   

    //   
    //   
    //   
    //   
    //   
    //}
}