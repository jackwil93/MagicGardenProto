using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailConversation : MonoBehaviour {

    public string characterID;
    public int stage;
    public EmailEntry latestEmail;
    public EmailEntry queuedEmail;
    [Space (20)]
    public List<EmailEntry> emailConvoList = new List<EmailEntry>();
}
