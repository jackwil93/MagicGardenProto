using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneGameController : MonoBehaviour {

    public int puzzlesComplete;
    public int lives;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPuzzleComplete()
    {
        puzzlesComplete++;
    }

    public void AddLife()
    {
        AddLives(1);
    }

    public void AddLives(int i)
    {
        lives += i;
    }
}
