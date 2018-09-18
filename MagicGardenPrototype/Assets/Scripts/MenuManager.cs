﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject emailCanvasGroup;
    public Transform emailListScrollContent;
    public Transform emailConvoScrollContent;
    public GameObject inventoryCanvasGroup;

    [Header("Prefabs")]
    public GameObject emailPrefab;
    public GameObject emailContentPrefab;
    public GameObject emailConvoWindowPrefab;

    bool emailsLoaded;

    List<GameObject> conversationsLoaded = new List<GameObject>();

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

    void PopulateEmailList()
    {
        foreach (string convo in EM.conversationsList)
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
        if (!conversationsLoaded.Contains(GameObject.Find(characterID)))
        {
            // Create the Conversation Window
            Transform emailUIGroup = GameObject.Find("Emails UI").transform;
            GameObject newConvoWindow = GameObject.Instantiate(emailConvoWindowPrefab, emailUIGroup);
            newConvoWindow.name = characterID;

            // Create each Email Entry 
            emailConvoScrollContent = newConvoWindow.transform.Find("Content");

            foreach (EmailEntry email in EM.GetAllEmails(characterID))
            {
                Debug.Log(characterID + " email added");
                GameObject newEmailEntry = GameObject.Instantiate(emailContentPrefab, emailConvoScrollContent);
                newEmailEntry.transform.Find("Text_Content").GetComponent<Text>().text = email.bodyText;


                // Needs to cater for showing the player's past response
            }

            // Set up the Close Window button
            newConvoWindow.transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(
                delegate
            {
                newConvoWindow.SetActive(false);
            }
                );

            // Store the Conversation Window somewhere (dont load it every time)
            conversationsLoaded.Add(newConvoWindow);
        }
        else
        {
            foreach (GameObject convoWindow in conversationsLoaded)
            {
                if (convoWindow.name == characterID)
                {
                    convoWindow.SetActive(true);
                    break;
                }
            }
        }

    }

    void AddNewEntryToConversation()
    {

    }

    void ClearEmailConversation()
    {

    }
}
