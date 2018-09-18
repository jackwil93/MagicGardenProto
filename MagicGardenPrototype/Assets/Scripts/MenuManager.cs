using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject emailCanvasGroup;
    public Transform emailListScrollContent;
    public Transform emailConvoScrollContent;
    public GameObject inventoryCanvasGroup;
    public GameObject emailReplyWindow;
    public Button replyButton1;
    public Button replyButton2;
    public Button replyButton3;

    public List<Button> replyButtons = new List<Button>();

    [Header("Prefabs")]
    public GameObject emailPrefab;
    public GameObject emailContentPrefab;
    public GameObject emailConvoWindowPrefab;

    bool emailsLoaded;
    EmailEntry currentEmail; // The email of what you're replying to

    public List<string> conversationsLoaded = new List<string>();
    Vector3 emailWindowPos;

    EmailManager EM;

    private void Start()
    {
        EM = GetComponent<EmailManager>();
        
    }

    public void OpenEmails()
    {
        emailCanvasGroup.SetActive(true);
        if (!emailsLoaded)
            PopulateEmailList();
    }

    public void OpenInventory()
    {

    }

    public void OpenShop()
    {

    }

    public void OpenSpellGame()
    {

    }

    public void OpenSettings()
    {

    }

    public void OpenMicroTransactions()
    {

    }

    public void CloseMenu() // Called from Buttons. Do not use GameManager SetScreen on Buttons
    {
        emailCanvasGroup.SetActive(false);
        inventoryCanvasGroup.SetActive(false);
        GetComponent<GameManager>().SetScreen(0);
    }

    void PopulateEmailList() // The Screen where it shows the latest email from each convo
    {
        foreach (string convo in EM.conversationsByNameList)
        {
            GameObject newEmail = GameObject.Instantiate(emailPrefab, emailListScrollContent);
            EmailEntry emailInfo = EM.GetLatestEmailEntry(convo);
            newEmail.transform.Find("Text_Name").GetComponent<Text>().text = emailInfo.characterName;
            newEmail.transform.Find("Text_Subject").GetComponent<Text>().text = emailInfo.bodyText;

            // Set up button
            newEmail.transform.Find("Button_Open").GetComponent<Button>().onClick.AddListener(delegate { CreateEmailConversation(convo); });
        
        }

        emailsLoaded = true;
    }

    public void CreateEmailConversation(string characterID) // for Buttons
    {
        // Check if has not been loaded, else open
        if (!conversationsLoaded.Contains(characterID + "_emailConvo"))
        {
            // Create the Conversation Window
            Transform emailUIGroup = GameObject.Find("Emails UI").transform;
            GameObject newConvoWindow = GameObject.Instantiate(emailConvoWindowPrefab, emailUIGroup);
            newConvoWindow.name = characterID + "_emailConvo";

            emailWindowPos = newConvoWindow.transform.localPosition;


            foreach (Transform t in newConvoWindow.GetComponentsInChildren<Transform>())
            {
                if (t.name == "Content_Log")
                    emailConvoScrollContent = t;
            }
            

            // Create each Email Entry 
            foreach (EmailEntry email in EM.GetConversationEmails(characterID))
            {
                CreateEmailConversationEntry(email);
            }



            // Set up the Close Window button
            newConvoWindow.transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(
                delegate
            {
                newConvoWindow.transform.localPosition = Vector3.right * 2000;
            }
                );

            // Store the Conversation Window somewhere (dont load it every time)
            conversationsLoaded.Add(newConvoWindow.name);
        }
        else if (conversationsLoaded.Contains(characterID + "_emailConvo"))
        {
            GameObject.Find(characterID + "_emailConvo").transform.localPosition = emailWindowPos;
        }
    }

    void CreateEmailConversationEntry(EmailEntry email)
    {
        // NOTE BUG: Currently does a full load on open. Needs to check if there are any new emails in the Conversation List. More work but better system
        GameObject newEmailEntry = GameObject.Instantiate(emailContentPrefab, emailConvoScrollContent); 
        newEmailEntry.transform.Find("Text_Content").GetComponent<Text>().text = email.bodyText;

        // Set up the Respond button
        newEmailEntry.transform.Find("Button_Respond").GetComponent<Button>().onClick.AddListener(
            delegate
            {
                OpenEmailReplyWindow(EM.GetLatestEmailEntry(email.characterID));
            });

        // Needs to cater for showing the player's past response
    }

    void OpenEmailReplyWindow(EmailEntry email)
    {
        currentEmail = email;
        emailReplyWindow.SetActive(true);
        emailReplyWindow.transform.SetAsLastSibling();
        replyButtons.Clear();
        replyButtons.Add(replyButton1);
        replyButtons.Add(replyButton2);
        replyButtons.Add(replyButton3);

        // Set up Images

        // set up button text and listeners
        int randomGood = AssignRandom(0, 3, 99);
        int randomBad = AssignRandom(0, 3, randomGood);

        replyButtons[randomGood].transform.GetComponentInChildren<Text>().text = email.playerReplyGood;
        replyButtons[randomBad].transform.GetComponentInChildren<Text>().text = email.playerReplyBad;


        replyButtons[randomGood].onClick.AddListener(delegate { SubmitReply("g"); });
        replyButtons[randomBad].onClick.AddListener(delegate { SubmitReply("b"); });

        replyButtons.Remove(replyButtons[randomGood]);
        replyButtons.Remove(replyButtons[randomBad]);
        replyButtons.Sort();

        replyButtons[0].transform.GetComponentInChildren<Text>().text = email.playerReplyNeutral;
        replyButtons[0].onClick.AddListener(delegate { SubmitReply("n"); });
    }

    int AssignRandom(int min, int maxExclusive, int taken)
    {
        int i = Random.Range(min, maxExclusive);

        if (i == taken)
            i = AssignRandom(min, maxExclusive, taken);

        return Random.Range(min, maxExclusive);
    }

    public void CloseReplyWindow() // Assigned in Inspector
    {
        emailReplyWindow.SetActive(false);
    }

    

    void SubmitReply(string gbn)
    {
        CloseReplyWindow();
        currentEmail.playerReplyGBN = gbn;
        EM.ReplyToEmailConversation(currentEmail);

        AddNewEntryToConversation();
    }

    void AddNewEntryToConversation()
    {
        CreateEmailConversationEntry(EM.GetLatestEmailEntry(currentEmail.characterID));
    }

    void ClearEmailConversation()
    {

    }
}
