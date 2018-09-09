﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputManager : MonoBehaviour {
    // This script doubles for mouse input for development purposes


    public bool userIsTouching;
    float touchTime;
    public Vector3 screenTouchPos;
    Vector3 prevScreenTouchPos; // Recorded at end of Update to check change between frames
    float touchMoveDistance;
    Vector3 swipeDirection;
    bool holdDown;

	// Update is called once per frame
	public void Update () {

        if (Input.GetMouseButton(0) || Input.touchCount > 0) // Could cause issues for 2 finger input later
        {
            Vector3 fingerTouchPos = Vector3.zero;
            if (Input.touchCount > 0)
                fingerTouchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);

            screenTouchPos = Input.mousePosition + fingerTouchPos;

            // Stop bug on first input
            if (prevScreenTouchPos == Vector3.zero)
                prevScreenTouchPos = screenTouchPos;

            touchTime += Time.deltaTime;
            userIsTouching = true;

            // Get Distance of drag, if player is moving finger while touching
            touchMoveDistance = Vector3.Distance(screenTouchPos, prevScreenTouchPos);
            Debug.Log("touchMoveDistance = " + touchMoveDistance);

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
            if (touchMoveDistance > 3f)
            { // Fast enough to be a swipe
                swipeDirection = Vector3.Normalize(screenTouchPos - prevScreenTouchPos);
            } else
            {
                swipeDirection = Vector3.zero;
            }


            prevScreenTouchPos = screenTouchPos;
        }
        else
        {
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
            screenTouchPos = Vector3.zero;
            prevScreenTouchPos = Vector3.zero;
        }
		
	}

    public virtual void SingleTapRelease()
    {
        // Override for Single Tap input. Registered on release
        Debug.Log("Touch Input: Single Tap Release");
    }

    public virtual void HoldRelease()
    {
        // Override for input release after being held down. Registered on release
        Debug.Log("Touch Input: Hold Release");
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