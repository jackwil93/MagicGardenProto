using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantCore : MonoBehaviour {

    DayTimeTracker dayTime;
    public float lifetimeSeconds; // Total time alive in seconds
    GameObject child;

    private void Start()
    {
        dayTime = FindObjectOfType<DayTimeTracker>() as DayTimeTracker;
        child = this.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        lifetimeSeconds += dayTime.SecondsRealtime();
        PlantGrow();
    }

    void PlantGrow()
    {
        // Not great but OK for prototype
        //child.transform.localScale = Vector3.Lerp(child.transform.localScale, child.transform.localScale + Vector3.one * lifetimeSeconds / 86400, Time.deltaTime);
    }
}
