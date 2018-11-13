using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class SpellTracker : MonoBehaviour {

    public GraphicRaycaster graphicRaycaster;
    public LineRenderer playerLine;
    public Text headerText;
    public List<Spell> knownSpells = new List<Spell>();

    [Space(20)]
    public RunePattern currentRunePattern;
    int currentPatternInList;
    public List<RunePattern> allRunePatterns;
    public Material standardLineMaterial;
    public Material highlightLineMaterial;


    [Header ("Auto fills")]
    public List<int> recordedRuneIndexes = new List<int>();

    PointerEventData pressPoint;

    public List<SpellRuneNode> allRuneNodes = new List<SpellRuneNode>();

    SpellRuneNode currentNode;
    Vector3 newLinePos;
    public List<IntPair> validRunePairings = new List<IntPair>();

    bool resetLineOnTouch;

    private void Start()
    {
        Debug.Log("Screen: " + Screen.width + " x " + Screen.height);
        Debug.Log("Camera: " + Camera.main.pixelWidth + " x " + Camera.main.pixelHeight);
        Debug.Log("Camera (Scaled): " + Camera.main.scaledPixelWidth + " x " + Camera.main.scaledPixelHeight);

        pressPoint = new PointerEventData(EventSystem.current);

        allRuneNodes.AddRange(FindObjectsOfType<SpellRuneNode>());

        // Load and Display the Rune Pattern
        ShowRunePattern();

    }


    private void Update()
    {
        // DEBUG ONLY
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            SaveRunePattern();


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
                        
                        // Add to valid pairings
                        
                        IntPair newValidPair = new IntPair();
                        newValidPair.int1 = currentNode.spellRuneIndex;
                        newValidPair.int2 = selectedNode.spellRuneIndex;

                        // Weird bug adding number duplicates, this is the workaround. Stickytape.
                        if (newValidPair.int1 != newValidPair.int2)
                            validRunePairings.Add(newValidPair);

                        // Update Current Node and Draw the line
                        currentNode = selectedNode;
                        AddNewLinePos(currentNode.transform.position);

                        // Record number for spell code
                        int runeCode = currentNode.spellRuneIndex;

                        // If not the first entry, Stop it from adding the same one twice in a row / every frame
                        if (recordedRuneIndexes.Count == 0 || recordedRuneIndexes.Count > 0 && recordedRuneIndexes[recordedRuneIndexes.Count - 1] != runeCode)
                        {
                            recordedRuneIndexes.Add(runeCode);
                            selectedNode.HitRune();
                        }
                    }

                   // Debug.Log("Current Node = " + currentNode.gameObject.name);

                }
            }


            DrawLine();
           


            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                // Remove any overhanging line
                if (playerLine.positionCount > 0)
                    playerLine.positionCount -= 1;


                // If there has actually been some attempt
                if (recordedRuneIndexes.Count > 2)
                {
                    //Debug.Log("Check Pattern Complete");
                    CheckPatternComplete();
                }
                
                //CalculateSpell();
                ResetRuneNodes();

                resetLineOnTouch = true;
            }
        }


       
    }



    void ShowRunePattern() // Called at Start after other setup
    {
        // Get all Rune Lines
        List<RuneLine> allRuneLines = new List<RuneLine>();
        allRuneLines.AddRange(FindObjectsOfType<RuneLine>());

        // Set materials blank (for new pattern)
        foreach (RuneLine line in allRuneLines)
        {
        line.transform.GetComponent<LineRenderer>().material = standardLineMaterial;
        }



        // Compare Pattern to find matching Rune Lines
        foreach (IntPair patternIntPair in currentRunePattern.runeIntPairs)
        {
            foreach (RuneLine compareLine in allRuneLines)
            {
                // if Int 1 matches...
                if (patternIntPair.int1 == compareLine.runePair.int1 || patternIntPair.int1 == compareLine.runePair.int2)
                {
                    // if Int 2 matches
                    if (patternIntPair.int2 == compareLine.runePair.int1 || patternIntPair.int2 == compareLine.runePair.int2)
                    {
                        // It's a match!
                        compareLine.transform.GetComponent<LineRenderer>().material = highlightLineMaterial;
                    }

                }
            }
        }

    }


    void CheckPatternComplete() // Called on Finger Lift
    {
        //Debug.Log("Checking " + recordedRuneIndexes.Count + " recorded rune indexes against " + 
            //currentRunePattern.allIntsInPattern.Count + " Runes needed");

        int correctMatches = 0;
        
        // Reverse for loop to check and remove matching numbers
        for (int i = recordedRuneIndexes.Count - 1; i > -1; i --)
        {
            foreach (int patternRuneInt in currentRunePattern.allIntsInPattern)
                if (recordedRuneIndexes[i] == patternRuneInt)
                {
                    //Debug.Log("Matched " + patternRuneInt + " with " + recordedRuneIndexes[i]);
                    correctMatches++;
                    recordedRuneIndexes.Remove((recordedRuneIndexes[i]));
                    break;
                }
            else
                {
                    // nothing
                }
        }

        //Debug.Log((currentRunePattern.allIntsInPattern.Count - recordedRuneIndexes.Count) + " matches left");

        //Debug.Log(correctMatches + "Correct Matches. " + currentRunePattern.allIntsInPattern.Count + " Matches needed");

        // Pattern Complete!
        //if (correctMatches == currentRunePattern.allIntsInPattern.Count)
        if (currentRunePattern.allIntsInPattern.Count - correctMatches == 0)
        {
           // Debug.Log("Pattern Complete");

            if (currentPatternInList < allRunePatterns.Count - 1)
                currentPatternInList++;
            else
                currentPatternInList = 0;

            currentRunePattern = allRunePatterns[currentPatternInList];
            headerText.text = "New Pattern!";

            //Reset!
            ResetLine();
            ShowRunePattern();
        }
        else // Pattern Failed
        {
            headerText.text = "Try Again!";
            ResetLine();
        }

    }




    // DEBUG ONLY
    [ContextMenu ("SaveRunePattern")]
    void SaveRunePattern()
    {
       // Debug.Log("Saving Rune Pattern...");
        RunePattern newPattern = ScriptableObject.CreateInstance(typeof(RunePattern)) as RunePattern;
        // Add all of the int pairs before sorting out the individual ints
        newPattern.runeIntPairs.AddRange(validRunePairings);

        foreach (IntPair intPair in newPattern.runeIntPairs)
        {
            // Stop double up between int 1 and previous int 2
            if (newPattern.allIntsInPattern.Count == 0 ||
                intPair.int1 != newPattern.allIntsInPattern[newPattern.allIntsInPattern.Count - 1])
            {
                newPattern.allIntsInPattern.Add(intPair.int1);
            }

            newPattern.allIntsInPattern.Add(intPair.int2);
        }

       // string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/RunePatterns" + "/New " + typeof(RunePattern).ToString() + ".asset");
        //AssetDatabase.CreateAsset(newPattern, assetPathAndName);

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
        recordedRuneIndexes.Clear();
        validRunePairings.Clear();

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

               // Debug.Log("Connected node " + currentNode + " with " + targetNode);
                return true;
            }
            // If there is a match but they are already connected, return false
            else if (runeConnection.subNode == currentNode && runeConnection.connected)
            {
                //Debug.Log("Nodes match but are already connected");
                return false;
            }
        }

       // Debug.Log("Nodes do not share a link");
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
