using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMovement : MonoBehaviour {

    public Vector3 onScreenPos;
    public Vector3 offScreenPos;

    private RectTransform myRect;
    private Vector3 targetPos;

    public bool startOffScreen;

    float m_speed = 10;

    private void Start()
    {
        myRect = this.GetComponent<RectTransform>();
        if (startOffScreen)
            targetPos = offScreenPos;
    }

    // Update is called once per frame
    void Update () {
        if (targetPos != myRect.localPosition)
            myRect.localPosition = Vector2.Lerp(myRect.localPosition, targetPos, Time.deltaTime * m_speed);
	}

    public void MoveOffScreen()
    {
        targetPos = offScreenPos;
    }

    public void MoveOnScreen()
    {
        targetPos = onScreenPos;
    }
}
