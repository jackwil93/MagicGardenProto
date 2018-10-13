using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public int BPM;
    [HideInInspector]
    public float BPMinSec;

    private void Awake()
    {
        CalculateBPMinSec();
        InvokeRepeating("CalculateBPMinSec", 0, 1);
    }

    public void CalculateBPMinSec()
    {
        BPMinSec = 60 / BPM;

    }
}
