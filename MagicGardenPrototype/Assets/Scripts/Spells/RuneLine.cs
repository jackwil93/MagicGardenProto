using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class RuneLine : MonoBehaviour {

    public IntPair runePair = new IntPair();

    public RectTransform pointA;
    public RectTransform pointB;

    public LineRenderer lineRender;


    // If I ever need to manually move them
    //private void OnGUI()
    //{
    //    if (pointA && pointB && lineRender)
    //        lineRender.SetPosition(0, pointA.position);
    //        lineRender.SetPosition(1, pointB.position);
    //}
    
    public void UpdateLinePositions()
    {
                lineRender.SetPosition(0, pointA.position);
                lineRender.SetPosition(1, pointB.position);
    }

    [ContextMenu("UpdateLineName")]
    public void UpdateLineName()
    {
        this.transform.name = "Line " + runePair.int1 + "-" + runePair.int2;
    }


    [ContextMenu("SortOrder")]
    public void SortOrder()
    {
        if (runePair.int1 > transform.parent.transform.GetChild(transform.GetSiblingIndex() + 1).GetComponent<RuneLine>().runePair.int1)
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        else if (transform.GetSiblingIndex() - 1 >= 0)
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

    }
}
