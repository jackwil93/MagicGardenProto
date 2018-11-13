using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellTracker : MonoBehaviour {

    public GraphicRaycaster graphicRaycaster;
    public LineRenderer playerLine;
    public Text headerText;
    public List<Spell> knownSpells = new List<Spell>();

    [Header ("Auto fills")]
    public List<int> recordedRuneIndexes = new List<int>();

    PointerEventData pressPoint;

    private List<SpellRuneNode> allRuneNodes = new List<SpellRuneNode>();

    SpellRuneNode currentNode;
    Vector3 newLinePos;

    bool resetLineOnTouch;

    private void Start()
    {
        Debug.Log("Screen: " + Screen.width + " x " + Screen.height);
        Debug.Log("Camera: " + Camera.main.pixelWidth + " x " + Camera.main.pixelHeight);
        Debug.Log("Camera (Scaled): " + Camera.main.scaledPixelWidth + " x " + Camera.main.scaledPixelHeight);

        pressPoint = new PointerEventData(EventSystem.current);

        allRuneNodes.AddRange(FindObjectsOfType<SpellRuneNode>());

    }


    private void Update()
    {
        if (Input.touchCount > 0)
        {
           
            pressPoint.position = Input.GetTouch(0).position;

            List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pressPoint, objectsHit);

            if (resetLineOnTouch)
                ResetLine();


            if (objectsHit.Count > 0)
            {
                GameObject hitObject = objectsHit[objectsHit.Count - 1].gameObject;
                if (hitObject.name.Contains("SpellRune"))
                {
                    SpellRuneNode selectedNode = hitObject.GetComponent<SpellRuneNode>();

                    // Update target node before setting as current node (must pass connection check first!)
                    if (selectedNode != currentNode && CheckRuneConnection(selectedNode))
                        {
                        // Passed the check. The nodes are now connected.

                        // Update Current Node and Draw the line
                        currentNode = selectedNode;
                        AddNewLinePos(currentNode.transform.position);
                        
                        // Record number for spell code
                        int runeCode = hitObject.transform.GetSiblingIndex() + 1;

                        // If not the first entry, Stop it from adding the same one twice in a row / every frame
                        if (recordedRuneIndexes.Count == 0 || recordedRuneIndexes.Count > 0 && recordedRuneIndexes[recordedRuneIndexes.Count - 1] != runeCode)
                        {
                            recordedRuneIndexes.Add(runeCode);
                            selectedNode.HitRune();
                        }
                    }

                    Debug.Log("Current Node = " + currentNode.gameObject.name);

                }
            }


            DrawLine();
           


            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                // Remove any overhanging line
                if (playerLine.positionCount > 0)
                    playerLine.positionCount -= 1;

                CalculateSpell();
                ResetRuneNodes();

                resetLineOnTouch = true;
            }
        }
    }

    void DrawLine()
    {
        
        Vector2 touchPos = Input.GetTouch(0).position;
        Vector2 viewportTouch = Camera.main.ScreenToViewportPoint(touchPos);
        Vector3 scaledScreenPos = new Vector3(Screen.width * viewportTouch.x, Screen.height * viewportTouch.y, 0);

        playerLine.SetPosition(playerLine.positionCount - 1, scaledScreenPos);

    }

    void ResetLine()
    {
        playerLine.positionCount = 1;
        playerLine.SetPosition(0, Vector3.zero);

        resetLineOnTouch = false;
    }

    void AddNewLinePos(Vector3 hitPos)
    {
        //Vector2 touchPos = Input.GetTouch(0).position;
        Vector2 viewportTouch = Camera.main.ScreenToViewportPoint(hitPos);
        Vector3 scaledScreenPos = new Vector3(Screen.width * viewportTouch.x, Screen.height * viewportTouch.y, 0);

        playerLine.SetPosition(playerLine.positionCount - 1, scaledScreenPos);
        playerLine.positionCount++;

    }

    bool CheckRuneConnection(SpellRuneNode targetNode)
    {
        if (currentNode == null) // First node selected. Nothing to do here.
        {
            currentNode = targetNode;
            return true;
        }

        foreach (SpellRuneConnection runeConnection in targetNode.connectingNodes)
        {
            // Find current Node in target node's connection list, are they already connected?
            if (runeConnection.subNode == currentNode && !runeConnection.connected)
            {
                // Tick connection in target node
                runeConnection.connected = true;


                // Find and tick matching connection in current node
                foreach (SpellRuneConnection currentRuneConnection in currentNode.connectingNodes)
                    if (currentRuneConnection.subNode == targetNode)
                        currentRuneConnection.connected = true;

                Debug.Log("Connected node " + currentNode + " with " + targetNode);
                return true;
            }
            // If there is a match but they are already connected, return false
            else if (runeConnection.subNode == currentNode && runeConnection.connected)
            {
                Debug.Log("Nodes match but are already connected");
                return false;
            }
        }

        Debug.Log("Nodes do not share a link");
        return false;
    }

    void ResetRuneNodes()
    {
        currentNode = null;

        foreach (SpellRuneNode node in allRuneNodes) 
            node.ResetRune();
    }


    void CalculateSpell() // Called after finger lifted
    {
        // Form the comparison string
        string spellCode = "";

        for (int i = 0; i<recordedRuneIndexes.Count; i++)
        {
            if (i != 0)
            {
                spellCode += ",";
            }
            spellCode += recordedRuneIndexes[i];
        }



        Debug.Log("Spell Code = " + spellCode);

        foreach (Spell spell in knownSpells)
        {
            if (spell.spellCode == spellCode)
            {
                headerText.text = "You Performed the " + spell.spellName + " spell!";
                break;
            }
            else
                headerText.text = "Sorry, try again.";

        }

        recordedRuneIndexes.Clear();
    }
    
}
