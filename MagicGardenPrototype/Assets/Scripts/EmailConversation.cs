using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailConversation {

    public string conversationID;
    public int stage;
    public int maxStage { get; private set; }
    public List<EmailEntry> receivedEmails = new List<EmailEntry>();

    public void AddNextEmail(EmailEntry incomingEmail) // Called from EmailManager
    {
        // Receive new email
        receivedEmails.Add(incomingEmail);
        incomingEmail.received = true;

        // Update conversation stage
        stage = incomingEmail.stage;

        Debug.Log("New email added to conversation: " + conversationID + ". Conversation Stage = " + stage + "| entryID = " + incomingEmail.entryID);
    }

    public void CheckMaxStage(int emailStage) // Called by Email Manager when setting up all Conversations
    {
        if (emailStage > maxStage)
            maxStage = emailStage;
    }

    public EmailEntry GetLatestEmail () // Called by MenuManager when opening Emails Window
    {
        if (receivedEmails.Count > 1)
            return receivedEmails[receivedEmails.Count - 1];
        else
            return receivedEmails[0];
    }
}
