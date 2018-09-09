using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MobileInputManager {

    public enum screens
    {
        mainGame,
        emails,
        shop,
        spellGame
    }
    public screens currentScreen;

    Transform mainCam;
    public List<Transform> cameraPosList = new List<Transform>();
    int currentCamPos;
    Vector3 camMoveToPos;

    private void Start()
    {
        currentScreen = screens.mainGame;
        currentCamPos = 0;
        mainCam = Camera.main.transform;
        camMoveToPos = cameraPosList[0].position;
    }

    private void Update()
    {
        base.Update();
        // If in the main game scene, run an input raycast

        if (Vector3.Distance(mainCam.position, camMoveToPos) > 0.02f)
        {
            mainCam.position = Vector3.Lerp(mainCam.position, camMoveToPos, Time.deltaTime * 5);
            mainCam.rotation = Quaternion.Lerp(mainCam.rotation, cameraPosList[currentCamPos].rotation, Time.deltaTime * 5);
        }
            
    }

    public override void SingleTapRelease()
    {
        base.SingleTapRelease();
        Debug.Log("Game Manager registered Single Tap Release!");
    }

    public override void SwipeLeft()
    {
        base.SwipeLeft();
        if (currentScreen == screens.mainGame)
            if (currentCamPos - 1 < 0)
                currentCamPos = cameraPosList.Count - 1;
            else
                currentCamPos--;

        camMoveToPos = cameraPosList[currentCamPos].position;
            
    }

    public override void SwipeRight()
    {
        base.SwipeRight();
        if (currentScreen == screens.mainGame)
            if (currentCamPos + 1 > cameraPosList.Count - 1)
                currentCamPos = 0;
            else
                currentCamPos++;

        camMoveToPos = cameraPosList[currentCamPos].position;
    }
}
