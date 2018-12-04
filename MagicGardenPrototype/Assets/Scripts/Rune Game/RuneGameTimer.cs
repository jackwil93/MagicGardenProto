using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneGameTimer : MonoBehaviour {

    public RectTransform timerBar;
    public RectTransform timerGroup; // Gets 'pop scaled'
    public Text timerText;
    public float maxTime = 10; // seconds
    float timeRemaining;

    public bool gamePlaying = true;

	// Use this for initialization
	void Start () {
        timeRemaining = maxTime;
	}
	
	// Update is called once per frame
	void Update () {

        if (gamePlaying)
        {
            timeRemaining -= Time.deltaTime;
            timerBar.localScale = new Vector3(timeRemaining / maxTime, 1, 1);
            timerText.text = timeRemaining.ToString("n2") + "s";
        }
        if (timeRemaining < 0)
        {
            gamePlaying = false;
            timerText.text = "0.00s";
        }

        timerGroup.localScale = Vector3.Lerp(timerGroup.localScale, Vector3.one, Time.deltaTime * 5);
	}

    public void AddOneSecond()
    {
        AddTime(1);
    }

    public void AddTime(float f)
    {
        timeRemaining = Mathf.Clamp(timeRemaining + f, 0, maxTime);

        timerGroup.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }
}
