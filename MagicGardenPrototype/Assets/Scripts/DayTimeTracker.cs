﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeTracker : MonoBehaviour {

    public float totalTimeplayed;
    int prevSecond;

	
    public float SecondsRealtime()
    {
        // Flawed. Always 1. Return to fix sometime

        if (prevSecond != DateTime.Now.Second)
        {
            prevSecond = DateTime.Now.Second;
            return 1;
        }
        else
            return 0;
    }

    private void Update()
    {
        //Debug.Log("Second = " + DateTime.Now.Second.ToString() + "Hour = " + DateTime.Now.Hour.ToString());
        // Debug.Log(DayProgression().ToString());
    }
}
