using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicGlobal;

public class AutoDrawLines : MonoBehaviour {

    public GameObject lineContainer;

    public List<RuneLine> allLines = new List<RuneLine>();

    int currentCount;

    private void Start()
    {
        allLines.AddRange(FindObjectsOfType<RuneLine>());
        // Delete double ups
    }

    private void Update()
    {

        DeleteMatchingLines();


        //DEBUG
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            GetLines();
            //Do
    }
    
    void DeleteMatchingLines()
    {

        if (currentCount < allLines.Count)
        {
            RuneLine currentLine = allLines[currentCount];

            foreach (RuneLine comparedLine in allLines)
            {
                if (comparedLine != currentLine)
                {

                    // if int 1 matches
                    if (currentLine.runePair.int1 == comparedLine.runePair.int1 || currentLine.runePair.int1 == comparedLine.runePair.int2)
                    {
                        Debug.Log("Found match on int 1");

                        // if int 2 matches
                        if (currentLine.runePair.int2 == comparedLine.runePair.int1 || currentLine.runePair.int2 == comparedLine.runePair.int2)
                        {
                            Debug.Log("Found match on int 2");
                            allLines.Remove(comparedLine);
                            Destroy(comparedLine.gameObject);
                            Debug.Log("Match deleted");

                            return;
                        }

                    }
                }
            }
            currentCount++;
        }

        allLines.Clear();
        allLines.AddRange(FindObjectsOfType<RuneLine>());
    }


    void GetLines()
    {
        SpellTracker spelltrk = SpellTracker.FindObjectOfType<SpellTracker>();

        foreach (SpellRuneNode node in spelltrk.allRuneNodes)
        {
            foreach (SpellRuneConnection connection in node.connectingNodes)
            {
                GameObject newLine = new GameObject();
                newLine.transform.SetParent(lineContainer.transform);
                newLine.transform.position = Vector3.zero;

                RuneLine lines = newLine.AddComponent<RuneLine>();
                lines.lineRender = newLine.GetComponent<LineRenderer>();

                // Get the PARTICLE Positions
                lines.pointA = node.particleEffect.GetComponent<RectTransform>();
                lines.pointB = connection.subNode.particleEffect.GetComponent<RectTransform>();

                Debug.Log(node.spellRuneIndex);
                Debug.Log(connection.subNode.spellRuneIndex);

                lines.runePair.int1 = node.spellRuneIndex;
                lines.runePair.int2 = connection.subNode.spellRuneIndex;

                lines.UpdateLinePositions();

                // Record Line Ints
                //lines.runePair.int1 = node.spellRuneIndex;
                //lines.runePair.int2 = connection.subNode.spellRuneIndex;
            }
        }


    }

}
