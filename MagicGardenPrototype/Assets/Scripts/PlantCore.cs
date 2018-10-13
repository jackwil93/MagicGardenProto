using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantCore : MonoBehaviour {

    DayTimeTracker dayTime;
    public float lifetimeSeconds; // Total time alive in seconds
    GameObject child;

    MusicManager musicManager;
    public SpriteRenderer spriteRend;
    public Sprite frame1;
    public Sprite frame2;
    float beatChange;
    int spriteFrame = 1;
    bool offBeat; // Randomly selected

    private void Start()
    {
        dayTime = FindObjectOfType<DayTimeTracker>() as DayTimeTracker;
        musicManager = MusicManager.FindObjectOfType<MusicManager>();
        spriteRend = GetComponent<SpriteRenderer>();

        // Randomise starting frame. 3 is excluded
        spriteFrame = Random.Range(1, 3);
        int i = Random.Range(0, 5);
        offBeat = (i == 3) ? true : false;
        Debug.Log("i = " + i);

        beatChange = musicManager.BPMinSec;
        if (offBeat)
            beatChange += (musicManager.BPMinSec / 2); // add a half beat
    }

    private void Update()
    {
        lifetimeSeconds += dayTime.SecondsRealtime();

        beatChange -= Time.deltaTime;

        if (beatChange < 0)
            ChangeFrame();
    }

    void ChangeFrame()
    {
        beatChange = musicManager.BPMinSec;
        

        switch (spriteFrame)
        {
            case 1:
                spriteRend.sprite = frame2;
                spriteFrame = 2;
                break;
            case 2:
                spriteRend.sprite = frame1;
                spriteFrame = 1;
                break;
        }
        return;
    }
        

    void PlantGrow()
    {
        // Not great but OK for prototype
        //child.transform.localScale = Vector3.Lerp(child.transform.localScale, child.transform.localScale + Vector3.one * lifetimeSeconds / 86400, Time.deltaTime);
    }
}
