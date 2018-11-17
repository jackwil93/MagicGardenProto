using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputManager : MonoBehaviour {
    // This script doubles for mouse input for development purposes

    public bool userIsTouching;
    float touchTime;
    public Vector3 screenTouchPos;
    Vector3 prevScreenTouchPos; // Recorded at end of Update to check change between frames
    float touchMoveDistance;
    float storedTouchMoveDist; // For accessing from other scripts
    Vector3 swipeDirection;
    bool holdDown;

    // For Graphics UI interactions
    public GraphicRaycaster m_GUIRaycaster;
    public PointerEventData m_GUIPointerEventData;
    public EventSystem m_EventSystem;


    // Update is called once per frame
    public void FixedUpdate () {

        if (Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // Could cause issues for 2 finger input later
        {

            Vector3 scaledFingerTouchPos = Vector3.zero;

            if (Input.touchCount > 0)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                Vector2 viewportTouch = Camera.main.ScreenToViewportPoint(touchPos);

                scaledFingerTouchPos = new Vector3(Screen.width * viewportTouch.x, Screen.height * viewportTouch.y, 0);
            }

            //screenTouchPos = Input.mousePosition + scaledFingerTouchPos;
            screenTouchPos = scaledFingerTouchPos;
            //Debug.Log("Screen touch pos = " + screenTouchPos);
            // Stop bug on first input
            if (prevScreenTouchPos == Vector3.zero)
                prevScreenTouchPos = screenTouchPos;

            touchTime += Time.deltaTime;
            userIsTouching = true;


            // Get Distance of drag, if player is moving finger while touching
            touchMoveDistance = Vector3.Distance(screenTouchPos, prevScreenTouchPos);

            // To register when being 'Held Down'
            if (touchTime > 0.2f && touchMoveDistance < 0.2f)
            {
                holdDown = true;
                HoldDown();
            }

            // To register 'Hold and Drag'
            if (holdDown && touchMoveDistance > 0.5f)
                Drag();

            // To calculate swiping. Note, only finalises on touch off
            if (touchMoveDistance > 50f)
            { // Fast enough to be a swipe
                swipeDirection = Vector3.Normalize(screenTouchPos - prevScreenTouchPos);
            } else
            {
                swipeDirection = Vector3.zero;
            }

            storedTouchMoveDist = touchMoveDistance;
            prevScreenTouchPos = screenTouchPos;
        }
        else 
        {
            // The following is registered when the User releases their touch
            // Register swipe direction
            GetSwipeDirection();

            if (holdDown)
                HoldRelease();
            else if (userIsTouching && !holdDown)
                SingleTapRelease();


            // Clean up and reset variables
            userIsTouching = false;
            holdDown = false;
            touchTime = 0;
            touchMoveDistance = 0;
            storedTouchMoveDist = 0;
            screenTouchPos = Vector3.zero;
            prevScreenTouchPos = Vector3.zero;
        }
		
	}

    public Transform GetSelectedObject() 
    {

        Vector2 inputPos = new Vector2();

        if (Input.touchCount > 0)
            inputPos += Input.GetTouch(0).position;


        Ray r = Camera.main.ScreenPointToRay(inputPos);
        Debug.DrawRay(r.origin, r.direction * 100, Color.cyan);

        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
            return hit.transform;
        else
            return null;
    }

    public Transform GetSelectedGUIObject()
    {
        m_GUIPointerEventData = new PointerEventData(m_EventSystem);
        m_GUIPointerEventData.position = Input.GetTouch(0).position;

        List<RaycastResult> guiResults = new List<RaycastResult>();
        m_GUIRaycaster.Raycast(m_GUIPointerEventData, guiResults);


        if (guiResults.Count > 0)
        {
            Debug.Log("GUI Selected Object = " + guiResults[0].gameObject.transform.name);
            return guiResults[0].gameObject.transform;
        }
        else
            return null;
    }


    public Vector3 GetRaycastHitPoint()
    {
        Ray r = Camera.main.ViewportPointToRay(screenTouchPos);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
            return hit.point;
        else
            return Vector3.zero;
    }

    public float GetTouchMoveDistance()
    {
        Debug.Log("touchMoveDistance = " + storedTouchMoveDist);

        return storedTouchMoveDist;
    }

    public virtual void SingleTapRelease()
    {
        // Override for Single Tap input. Registered on release
        Debug.Log("Touch Input: Single Tap Release");
    }


    public virtual void SwipeUp()
    {
        Debug.Log("Touch Input: Swipe Up");
    }
    public virtual void SwipeDown()
    {
        Debug.Log("Touch Input: Swipe Down");

    }
    public virtual void SwipeLeft()
    {
        Debug.Log("Touch Input: Swipe Left");

    }
    public virtual void SwipeRight()
    {
        Debug.Log("Touch Input: Swipe Right");

    }
    public virtual void HoldDown()
    {
        // Override for input where the user is holding their finger in one spot
        Debug.Log("Touch Input: Hold Down()");
        
    }

    public virtual void HoldRelease()
    {
        // Override for input release after being held down. Registered on release
        Debug.Log("Touch Input: Hold Release");
    }

    public virtual void Drag()
    {
        // Override for input where the user has been holding their finger down and is now dragging across screen slowly
        Debug.Log("Touch Input: Drag()");
    }

    private void GetSwipeDirection()
    {
        if (swipeDirection.x < -0.5)
            SwipeLeft();
        if (swipeDirection.x > 0.5)
            SwipeRight();
        if (swipeDirection.y > 0.5)
            SwipeUp();
        if (swipeDirection.y < -0.5)
            SwipeDown();

        swipeDirection = Vector3.zero;
    }
}
