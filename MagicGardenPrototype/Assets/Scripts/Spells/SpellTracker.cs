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
            Vector2 touchPos = Input.GetTouch(0).position;
            Vector2 viewportTouch = Camera.main.ScreenToViewportPoint(touchPos);
            pressPoint.position = touchPos;
            Debug.Log("TouchPos = " + touchPos);
            Debug.Log("ViewportPos = " + viewportTouch);


            List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pressPoint, objectsHit);
            

            if (objectsHit.Count > 0)
            {
                GameObject hitObject = objectsHit[objectsHit.Count - 1].gameObject;
                if (hitObject.name.Contains("SpellRune"))
                {
                    int runeCode = hitObject.transform.GetSiblingIndex() + 1;

                    // If not the first entry, Stop it from adding the same one twice in a row / every frame
                    if (recordedRuneIndexes.Count == 0 || recordedRuneIndexes.Count > 0 && recordedRuneIndexes[recordedRuneIndexes.Count - 1] != runeCode)
                    {
                    recordedRuneIndexes.Add(runeCode);
                    hitObject.GetComponent<SpellRuneNode>().HitRune();
                    }
                }
            }
            


            if (playerLine.positionCount < 2 ||
                Vector3.Distance(playerLine.GetPosition(playerLine.positionCount - 2),
                playerLine.GetPosition(playerLine.positionCount - 1)) > 50)
            {
                playerLine.positionCount++;
            }

            Vector3 scaledScreenPos = new Vector3(Screen.width * viewportTouch.x, Screen.height * viewportTouch.y, 0);
            Debug.Log("Scaled Screen Touch Pos = " + scaledScreenPos);

            playerLine.SetPosition(playerLine.positionCount - 1, scaledScreenPos);
            //playerLine.SetPosition(playerLine.positionCount - 1, Camera.main.ViewportToWorldPoint(viewportTouch));



            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                playerLine.positionCount = 0;
                CalculateSpell();
                ResetRuneNodes();
            }
        }


        
    }

    void ResetRuneNodes()
    {
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
