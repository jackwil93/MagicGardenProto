using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineDraw : MonoBehaviour {

    public LineRenderer myLine;
    public LineRenderer playbackLine;
    public Text textDisplay;
    Vector3 mousePos;

    bool firstNodeMoved = false;

    [Header("Tick before play to record")]
    public bool recordMode;
    public string spellID;
    public int spellNo;
    [Header("Tick and Play to Delete All")]
    public bool deleteAllKeys;

    bool playback;
    List<Vector3> recordedLinePoints = new List<Vector3>();
    int savedPoints;

    bool playersTurn;
    bool lineLoaded;
    // For framerate optimisation
    int tick;
    public int tickMax = 6;

    // For checking line accuracy
    int missedPoints;
    int bigNumber;
    int difference;
    bool passes;


    int turnNumber;

    public void StartSpellGame()
    {
        if (!recordMode)
        {
            turnNumber = 1;
            DrawRecordedLine();
        }

        if (deleteAllKeys)
            PlayerPrefs.DeleteAll();
    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (playersTurn || recordMode)
            PlayerDrawsLine();
        }

        if (Input.GetMouseButtonUp(0))
        {
            playersTurn = false;

            if (recordMode)
                SaveRecordedLine();

            if (lineLoaded)
                LineComparison();
        }
    }

    void PlayerDrawsLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.transform)
            {
                mousePos = hit.point + (Vector3.up * 0.1f);
                if (!firstNodeMoved)
                {
                    myLine.SetPosition(0, mousePos);
                    firstNodeMoved = true;
                }



                if (tick > tickMax) // Runs once every 5 frames
                {
                    AddLineNode();
                    tick = 0;
                }
                tick++;
            }
        }
    }


    void AddLineNode()
    {
        myLine.positionCount++;
        myLine.SetPosition(myLine.positionCount - 1, mousePos);

        // Add to Recorded Line Points List
        if (recordMode && myLine.positionCount > 2)
        {
            Debug.Log("Myline position count = " + myLine.positionCount);

            recordedLinePoints.Add(myLine.GetPosition(myLine.positionCount - 1));

            Debug.Log("saved point " + recordedLinePoints.Count.ToString());
        }
    }

    void DrawRecordedLine()
    {
        //Debug.Log(PlayerPrefs.GetFloat("point" + savedPoints.ToString() + "x"));

        if (PlayerPrefs.HasKey(spellID + turnNumber + "point" + savedPoints.ToString() + "x"))
            StartCoroutine(DrawNextPoint());
        else
        {
            // Line finished loading
            Debug.Log("Finished loading line with " + savedPoints + " points");
            playersTurn = true;
            lineLoaded = true;
        }
    }

    IEnumerator DrawNextPoint()
    {
        yield return new WaitForSeconds(0.05f);
        playbackLine.SetPosition(savedPoints, new Vector3(
            PlayerPrefs.GetFloat(spellID + turnNumber + "point" + savedPoints.ToString() + "x"),
            PlayerPrefs.GetFloat(spellID + turnNumber + "point" + savedPoints.ToString() + "y"),
            PlayerPrefs.GetFloat(spellID + turnNumber + "point" + savedPoints.ToString() + "z")));
        savedPoints++;
        playbackLine.positionCount++;
        DrawRecordedLine();
    }

    void SaveRecordedLine()
    {
        for (int i = 0; i < recordedLinePoints.Count - 1; i ++)
        {
            PlayerPrefs.SetFloat(spellID + spellNo + "point" + i.ToString() + "x", recordedLinePoints[i].x);
            PlayerPrefs.SetFloat(spellID + spellNo + "point" + i.ToString() + "y", recordedLinePoints[i].y);
            PlayerPrefs.SetFloat(spellID + spellNo + "point" + i.ToString() + "z", recordedLinePoints[i].z);
        }

        Debug.Log("Line saved with " + recordedLinePoints.Count.ToString() + " points!");
    }
   

    void LineComparison()
    {
        for (int i = 0; i < playbackLine.positionCount; i++) // Check each playback point
        {
            passes = false; // Set to false until proven as passed

            if (i < playbackLine.positionCount)
            {
                for (int ii = 0; ii < myLine.positionCount; ii++) // check against each player drawn point
                {
                    float dist = Vector3.Distance(playbackLine.GetPosition(i), myLine.GetPosition(ii));

                    if (dist < 0.1f)
                        passes = true;
                }
                // If this point is too far from any other Saved Point, it did not pass
                if (passes == false)
                {
                    missedPoints++;
                }
            }
        }


        if (myLine.positionCount < playbackLine.positionCount)
            difference = (playbackLine.positionCount - myLine.positionCount);

        int totalPoints = playbackLine.positionCount + difference;
        float correctPoints = totalPoints - missedPoints - difference;

        Debug.Log("Line pos count " + totalPoints + " | difference = " + difference + "| missed Points = " + missedPoints);
        float hitPercent = (correctPoints / totalPoints) * 100f;
        Debug.Log(correctPoints + " / " + totalPoints + " = " + hitPercent + "%");

        Debug.Log("Total accuracy = "+ correctPoints
            + " correct points / " + totalPoints + " total points, " +
             hitPercent.ToString() + "%");

        // Show Accuracy text
        textDisplay.gameObject.SetActive(true);
        textDisplay.text = Mathf.Round(hitPercent).ToString() + "%";
        textDisplay.transform.GetComponent<Animator>().SetTrigger("PlayAnim");


        StartCoroutine(BeginNextTurn());
    }

    IEnumerator BeginNextTurn()
    {
        yield return new WaitForSecondsRealtime(2);
        myLine.positionCount = 0;
        playbackLine.positionCount = 1;
        savedPoints = 0;
        difference = 0;
        missedPoints = 0;
        turnNumber++;
        playersTurn = false;
        textDisplay.gameObject.SetActive(false);


        Debug.Log("Next turn | turn Number = " + turnNumber);

        DrawRecordedLine();
    }
}
