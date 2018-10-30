using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public Transform invScreensTransform;

    public List<Vector3> invScreenLocalPosList = new List<Vector3>();
    Vector3 targetPos;

    public int invScreenInt;
    float m_speed = 10;
	
	void Update () {
        if (targetPos != invScreensTransform.localPosition)
            invScreensTransform.localPosition = Vector2.Lerp(invScreensTransform.localPosition, targetPos, Time.deltaTime * m_speed);
    }

    public void MoveScreenLeft()
    {
        if (invScreenInt + 1 < invScreenLocalPosList.Count) invScreenInt++;
        targetPos = invScreenLocalPosList[invScreenInt];
    }

    public void MoveScreenRight()
    {
        if (invScreenInt - 1 >= 0) invScreenInt--;
        targetPos = invScreenLocalPosList[invScreenInt];
    }

    public void GoToScreen(int screen)
    {
        invScreenInt = screen;
        targetPos = invScreenLocalPosList[invScreenInt];
    }
}
