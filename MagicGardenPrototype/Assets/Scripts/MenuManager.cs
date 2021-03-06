﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class MenuManager : MonoBehaviour {

    GameManager GM;

    public Transform emailListScrollContent;
    public Transform emailConvoScrollContent;
    public GameObject emailReplyWindow;
    public Button replyButton1;
    public Button replyButton2;
    public Button replyButton3;

    public List<Button> replyButtons = new List<Button>();


    [Header("Laptop")]
    public GameObject laptopUI;
    [Header("Canvas Groups")]
    public GameObject grapevineHomeCanvasGroup;
    public GameObject emailCanvasGroup;
    public GameObject dailyMarketCanvasGroup;
    public GameObject shopCanvasGroup;
    public GameObject transactionsCanvasGroup;
    public GameObject inventoryCanvasGroup;


    [Header("Prefabs")]
    public GameObject emailPrefab;
    public GameObject emailContentPrefab;
    public GameObject emailConvoWindowPrefab;
    public GameObject emailPlayerReplyPrefab;

    EmailEntry currentEmail; // The email of what you're replying to

    public List<string> conversationsLoaded = new List<string>();
    Vector3 emailWindowPos;

    EmailManager EM;

    [Header("Inventory")]
    public UIMovement inventoryPanelUI;
    InventoryUI inventoryScreensUI;
    

    private void Start()
    {
        GM = GetComponent<GameManager>();
        EM = GetComponent<EmailManager>();
        inventoryScreensUI = inventoryPanelUI.GetComponent<InventoryUI>();
    }

    public void OpenLaptop()
    {
        laptopUI.SetActive(true);
    }

    public void CloseLaptop()
    {
        laptopUI.SetActive(false);
        GM.currentScreen = MagicGlobal.GameStates.gameScreens.mainGame;

    }

    public void OpenGrapevineHome()
    {
        grapevineHomeCanvasGroup.SetActive(true);

        emailCanvasGroup.SetActive(false);
        shopCanvasGroup.SetActive(false);
        transactionsCanvasGroup.SetActive(false);
        dailyMarketCanvasGroup.SetActive(false);
    }

    public void OpenEmails()
    {
        emailCanvasGroup.SetActive(true);

        shopCanvasGroup.SetActive(false);
        transactionsCanvasGroup.SetActive(false);
        dailyMarketCanvasGroup.SetActive(false);
        grapevineHomeCanvasGroup.SetActive(false);


        PopulateConversationsList();
    }

    public void OpenDailyMarket()
    {
        dailyMarketCanvasGroup.SetActive(true);

        emailCanvasGroup.SetActive(false);
        shopCanvasGroup.SetActive(false);
        transactionsCanvasGroup.SetActive(false);
        grapevineHomeCanvasGroup.SetActive(false);

    }

    public void OpenShop()
    {
        shopCanvasGroup.SetActive(true);

        emailCanvasGroup.SetActive(false);
        transactionsCanvasGroup.SetActive(false);
        dailyMarketCanvasGroup.SetActive(false);
        grapevineHomeCanvasGroup.SetActive(false);

    }

    public void OpenTransactions()
    {
        transactionsCanvasGroup.SetActive(true);

        emailCanvasGroup.SetActive(false);
        shopCanvasGroup.SetActive(false);
        dailyMarketCanvasGroup.SetActive(false);
        grapevineHomeCanvasGroup.SetActive(false);

    }


    public void OpenInventory() // Called from GM SwipeUp
    {
       inventoryPanelUI.MoveOnScreen();
    }

    public void InventoryLeft() // Called from GM SwipeLeft
    {
        Debug.Log("MM Inventory Left");

        inventoryScreensUI.MoveScreenLeft();
    }

    public void InventoryRight() // Called from GM SwipeRight
    {
        Debug.Log("MM Inventory Right");
        inventoryScreensUI.MoveScreenRight();
    }

    public void CloseInventory() // Called from GM SwipeDown & Inventory Panel Close Button
    {
        inventoryPanelUI.MoveOffScreen();
        GM.currentScreen = MagicGlobal.GameStates.gameScreens.mainGame;
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
        CloseLaptop();
        GM.SetScreen(0);
    }


    #region Emails

    void PopulateConversationsList() // The Screen where it shows all conversations and the latest from each
    {
        foreach (KeyValuePair<string, EmailConversation> eConvo in EM.emailConversationsDictionary)
        {
            EmailConversation conversation = eConvo.Value;
            GameObject emailConversationHeader;

            if (!eConvo.Value.uiHeaderExists)
            {
                // Set up Email Conversation Prefab and Text, etc
                emailConversationHeader = GameObject.Instantiate(emailPrefab, emailListScrollContent);
                conversation.inGameHeader = emailConversationHeader;
                conversation.uiHeaderExists = true;
            }
            EmailEntry latestEmail = eConvo.Value.GetLatestEmail();

            conversation.inGameHeader.transform.Find("Text_Name").GetComponent<Text>().text = latestEmail.characterName;
            conversation.inGameHeader.transform.Find("Text_Subject").GetComponent<Text>().text = latestEmail.bodyText;


            Transform readButton = conversation.inGameHeader.transform.Find("Button_Open");

            // Display "NEW" if has an unread email
            if (conversation.unreadEmail)
            {
                readButton.GetComponent<Image>().color = Color.yellow;
                readButton.GetComponentInChildren<Text>().text = "New Message!";
            }
            else
            {
                readButton.GetComponent<Image>().color = Color.white;
                readButton.GetComponentInChildren<Text>().text = "Open";
            }


            // Set up button
            readButton.GetComponent<Button>().onClick.AddListener(delegate { CreateEmailConversation(latestEmail.conversationID); });


        }
    }

    public void CreateEmailConversation(string convoID) // for Buttons. This runs when a Convo Header 'Open' Button is clicked.
    {

        // This is for optimisation. Rather than loading every email conversation on start, only load them once when opened
        // And then keep them open but off screen for duration of play
        // Check if has not been loaded, else open

        string window = convoID + "_convoWindow";
        var convoWindow = conversationsLoaded.FirstOrDefault(conversationsLoaded => conversationsLoaded.Contains(window));

        if (convoWindow == null || convoWindow == "")
        {
            // Create the Conversation Window
            Transform emailUIGroup = GameObject.Find("Emails UI").transform;
            GameObject newConvoWindow = GameObject.Instantiate(emailConvoWindowPrefab, emailUIGroup);
            newConvoWindow.name = convoID + "_convoWindow";

            emailWindowPos = newConvoWindow.transform.localPosition;


            foreach (Transform t in newConvoWindow.GetComponentsInChildren<Transform>())
            {
                if (t.name == "Content_Log")
                    emailConvoScrollContent = t;
            }
            

            // Set up the Close Window button
            newConvoWindow.transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(
                delegate
            {
                newConvoWindow.transform.localPosition = Vector3.right * 2000;
                PopulateConversationsList();
            }
                );

            // Store the Conversation Window somewhere (dont load it every time)
            conversationsLoaded.Add(newConvoWindow.name);
        }
        else 
        {
            GameObject.Find(window).transform.localPosition = emailWindowPos;

        }

        // Create each Email Entry that has NOT been loaded already
        foreach (EmailEntry email in EM.emailConversationsDictionary[convoID].receivedEmails)
        {
            if (email.loadedToGame == false)
                CreateEmailConversationEntry(email);

            if (email.opened == false)
            {
                email.opened = true;
            }
                EM.CheckNotifications();
        }

        // Mark this Convo as 'Read'
        EM.emailConversationsDictionary[convoID].MarkEmailsRead();
    }

    // Actually put the EmailEntry content into the email as a new panel in the Scroll View. Only if not yet loaded
    void CreateEmailConversationEntry(EmailEntry email)
    {

        GameObject prefabToUse;

        // Check if this email is player or character
        if (email.characterName == "Player")
        {
            prefabToUse = emailPlayerReplyPrefab;
            email.opened = true;
            email.received = true;
        }
        else
            prefabToUse = emailContentPrefab;

        GameObject newEmailEntry = GameObject.Instantiate(prefabToUse, emailConvoScrollContent);
        newEmailEntry.name = "EmailEntry_" + email.entryID;
        newEmailEntry.transform.FindDeepChild("Text_Content").GetComponent<Text>().text = email.bodyText;

        if (email.characterName == "Player")
            newEmailEntry.transform.Find("Text_Label_Header").GetComponent<Text>().text = "Your Response [" + email.dateTime + "]:";
        else // If an NPC Email
        {
            // Check if GM Sprite Dictionary Contains Reference, if so, pull sprites
            if (email.itemID != null && email.itemID != "" && email.itemID != " ")
            {
                Debug.Log("Attempting to Get Sprites for " + email.itemID);
                newEmailEntry.transform.FindDeepChild("Image_ItemOrder").GetComponent<Image>().sprite = GM.GetSpriteSet(email.itemID).normalSprites[0];
                Debug.Log("Retrived sprites");
            }
            else
                newEmailEntry.transform.FindDeepChild("Image_ItemOrder").gameObject.SetActive(false);


            // Set up the Respond button
            Transform respondButton = newEmailEntry.transform.FindDeepChild("Button_Respond");

            if (email.playerCanReply)
            {
                respondButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        OpenEmailReplyWindow(email);
                    });
            }
            else
                respondButton.gameObject.SetActive(false);
        
        }


        // Is now considered loaded and opened. NOTE: All emails set loaded to false when game loads for first time. 
        // Important so they can appear next time
        email.opened = true;
        email.loadedToGame = true;
    }

    void OpenEmailReplyWindow(EmailEntry email)
    {
        currentEmail = email;
        emailReplyWindow.SetActive(true);
        emailReplyWindow.transform.SetAsLastSibling();

        // Set up Images
        if (email.itemID != null)
            emailReplyWindow.transform.FindDeepChild("Image_Item").GetComponent<Image>().sprite = GM.GetSpriteSet(email.itemID).normalSprites[0];
        else
            emailReplyWindow.transform.FindDeepChild("Image_Item").gameObject.SetActive(false);

        // Reset Buttons
        foreach(Button b in replyButtons)
        {
            b.onClick.RemoveAllListeners();
        }


        replyButtons.Clear();
        replyButtons.Add(replyButton1);
        replyButtons.Add(replyButton2);
        replyButtons.Add(replyButton3);


        // set up button text and listeners

        // Assign random button as good response, then remove it. Assign bad response from the left over, and then neutral response
        int randomGood = AssignRandom(0, replyButtons.Count);
     
        replyButtons[randomGood].transform.GetComponentInChildren<Text>().text = currentEmail.playerReplyGood;
        replyButtons[randomGood].onClick.AddListener(delegate { SubmitReply("g", currentEmail.playerReplyGood); });
        replyButtons.Remove(replyButtons[randomGood]);

        // Assign Bad
        int randomBad = AssignRandom(0, replyButtons.Count);
        replyButtons[randomBad].transform.GetComponentInChildren<Text>().text = currentEmail.playerReplyBad;
        replyButtons[randomBad].onClick.AddListener(delegate { SubmitReply("b", currentEmail.playerReplyBad); });
        replyButtons.Remove(replyButtons[randomBad]);


        // Assign Neutral
        replyButtons[0].transform.GetComponentInChildren<Text>().text = currentEmail.playerReplyNeutral;
        replyButtons[0].onClick.AddListener(delegate { SubmitReply("n", currentEmail.playerReplyNeutral); });
    }

    int AssignRandom(int min, int maxExclusive)
    {
        int i = Random.Range(min, maxExclusive);

        return i;
    }

    public void CloseReplyWindow() // Assigned in Inspector
    {
        emailReplyWindow.SetActive(false);
    }


    void SubmitReply(string gbn, string replyText)
    {
        Debug.Log("Submit reply");
        if (currentEmail.replied == false)
        {
            currentEmail.replied = true;
            
            currentEmail.playerReplyGBN = gbn;
            CloseReplyWindow();

            GameObject.Find("EmailEntry_" + currentEmail.entryID).transform.FindDeepChild("Button_Respond").gameObject.SetActive(false);

            EM.RecordPlayerReplyAndQueueNextEmail(currentEmail, gbn, replyText);

            // Because the player email has been recorded and added, we can just get it as the latest email in the conversation
            CreateEmailConversationEntry(EM.emailConversationsDictionary[currentEmail.conversationID].GetLatestEmail());

            // Now don't show the player's reply as an unread emaik
            EM.emailConversationsDictionary[currentEmail.conversationID].MarkEmailsRead();
        }
    }

    #endregion


}
